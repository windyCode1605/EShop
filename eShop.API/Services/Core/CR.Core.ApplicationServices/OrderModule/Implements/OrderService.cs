using CR.ApplicationBase.Common;
using CR.Constants;
using CR.Constants.Discount;
using CR.Constants.ErrorCodes;
using CR.Constants.Payment;
using CR.Constants.Shipment;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.OrderModule.Abstracts;
using CR.Core.ApplicationServices.OrderModule.Dtos;
using CR.Core.Domain.Coupons;
using CR.Core.Domain.Logistics;
using CR.Core.Domain.Orders;
using CR.Core.Domain.Payment;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;
using CR.Core.Dtos.Shipment;
using CR.Constants.Orders;

namespace CR.Core.ApplicationServices.OrderModule.Implements;

public class OrderService : CoreServiceBase, IOrderService
{
    public OrderService(ILogger<OrderService> logger, IHttpContextAccessor httpContext)
    : base(logger, httpContext)
    {
    }

    public async Task<Result<OrderDto>> CreateOrder(CreateOrderDto input)
    {
        _logger.LogInformation("{method} called with input: {@input}", nameof(CreateOrder), input);
        var userId = _httpContext.GetCurrentUserId();

        var cart = await _dbContext.Carts.Include(c => c.Items)
                                         .ThenInclude(ci => ci.ProductVariant)
                                         .ThenInclude(pv => pv.Product)
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null || cart.Items.Count == 0)
        {
            return Result<OrderDto>.Failure(ErrorCode.CartEmpty, this.GetCurrentMethodInfo());
        }

        var stockErrors = new List<string>();
        foreach (var item in cart.Items)
        {
            var variant = item.ProductVariant;
            if (variant == null || variant.Deleted || variant.Product.Deleted)
            {
                stockErrors.Add($"Sản phẩm '{variant?.Product?.Name ?? "Không xác định"}' không tồn tại hoặc đã ngừng kinh doanh.");
                continue;
            }

            if (variant.StockQuantity < item.Quantity)
            {
                stockErrors.Add($"'{variant.Product.Name} - Size/Màu: {variant.SKU}': Chỉ còn {variant.StockQuantity} sản phẩm.");
            }
        }

        if (stockErrors.Any())
        {
            return Result<OrderDto>.Failure(ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(), stockErrors);
        }

        string shippingAddressSnapshot;
        string receiverName;
        string receiverPhone;

        if (input.AddressId.HasValue)
        {
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == input.AddressId.Value && a.UserId == userId && !a.IsDeleted);
            if (address == null)
                return Result<OrderDto>.Failure(ErrorCode.AddressNotFound, this.GetCurrentMethodInfo(), $"Địa chỉ với ID {input.AddressId.Value} không tồn tại.");

            var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
            receiverName = address.ReceiverName ?? userProfile?.FullName ?? "Khách hàng";
            receiverPhone = address.ReceiverPhone ?? userProfile?.PhoneNumber ?? string.Empty;
            shippingAddressSnapshot = $"{address.Street}, {address.City}, {address.Province}";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(input.ReceiverName) || string.IsNullOrWhiteSpace(input.ReceiverPhone) ||
                string.IsNullOrWhiteSpace(input.Province) || string.IsNullOrWhiteSpace(input.City) || string.IsNullOrWhiteSpace(input.Street))
            {
                return Result<OrderDto>.Failure(ErrorCode.AddressRequired, this.GetCurrentMethodInfo(), "Vui lòng cung cấp đầy đủ thông tin địa chỉ giao hàng.");
            }
            receiverName = input.ReceiverName;
            receiverPhone = input.ReceiverPhone;
            shippingAddressSnapshot = $"{input.Street}, {input.City}, {input.Province}";
        }

        var tempSubtotal = cart.Items.Sum(ci => (ci.ProductVariant.Product.BasePrice + ci.ProductVariant.PriceAdjustment) * ci.Quantity);

        decimal discountAmount = 0;
        Coupons? coupons = null;

        if (!string.IsNullOrWhiteSpace(input.CouponCode))
        {
            var now = DateTime.UtcNow;
            coupons = await _dbContext.Coupons
                .FirstOrDefaultAsync(c => c.Code == input.CouponCode && c.IsActive && c.StartDate <= now && c.ExpiryDate >= now);

            if (coupons == null)
                return Result<OrderDto>.Failure(ErrorCode.CouponNotFound, this.GetCurrentMethodInfo(), $"Mã giảm giá '{input.CouponCode}' không tồn tại hoặc hết hạn.");

            if (coupons.UsageLimit.HasValue && coupons.UsedCount >= coupons.UsageLimit.Value)
                return Result<OrderDto>.Failure(ErrorCode.CouponUsageLimitReached, this.GetCurrentMethodInfo(), $"Mã giảm giá '{input.CouponCode}' đã hết lượt.");

            if (coupons.UsageLimitPerUser.HasValue)
            {
                var userUsageCount = await _dbContext.CouponUsages.CountAsync(cu => cu.CouponId == coupons.Id && cu.UserId == userId);
                if (userUsageCount >= coupons.UsageLimitPerUser.Value)
                    return Result<OrderDto>.Failure(ErrorCode.CouponUsageLimitReached, this.GetCurrentMethodInfo(), $"Bạn đã sử dụng mã này đủ số lần cho phép.");
            }

            if (coupons.MinOrderValue.HasValue && tempSubtotal < coupons.MinOrderValue.Value)
                return Result<OrderDto>.Failure(ErrorCode.CouponMinOrderNotMet, this.GetCurrentMethodInfo(), $"Chưa đạt giá trị tối thiểu {coupons.MinOrderValue.Value:N0}đ.");

            if (coupons.DiscountType == DiscountType.Percentage)
            {
                discountAmount = tempSubtotal * (coupons.DiscountValue / 100);
                if (coupons.MaxDiscountValue.HasValue)
                    discountAmount = Math.Min(discountAmount, coupons.MaxDiscountValue.Value);
            }
            else
            {
                discountAmount = coupons.DiscountValue;
            }
            discountAmount = Math.Min(discountAmount, tempSubtotal);
        }

        const decimal shippingFee = 30000; // TODO: Make this configurable

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var totalAmount = tempSubtotal - discountAmount + shippingFee;
            var order = _dbContext.Orders.Add(new Order
            {
                OrderCode = GenerateOrderCode(),
                UserId = userId,
                ShippingAddress = shippingAddressSnapshot,
                Subtotal = tempSubtotal,
                DiscountAmount = discountAmount,
                ShippingFee = shippingFee,
                AddressesId = input.AddressId,
                TotalAmount = totalAmount,
                Status = OrderStatusConst.Pending.ToString(),
                PaymentMethod = input.PaymentMethod,
                CreatedDate = DateTimeUtils.GetDate(),
            }).Entity;

            await _dbContext.SaveChangesAsync();

            var orderItems = cart.Items.Select(ci => new OrderItem
            {
                OrderId = order.Id,
                ProductVariantId = ci.ProductVariantId,
                Quantity = ci.Quantity,
                ProductName = ci.ProductVariant.Product.Name,
                VariantSKU = ci.ProductVariant.SKU,
                UnitPrice = ci.ProductVariant.Product.BasePrice + ci.ProductVariant.PriceAdjustment
            }).ToList();

            _dbContext.OrderItems.AddRange(orderItems);

            foreach (var ci in cart.Items)
            {
                var variant = ci.ProductVariant;
                if (variant.StockQuantity < ci.Quantity)
                {
                    await transaction.RollbackAsync();
                    return Result<OrderDto>.Failure(ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(), $"Sản phẩm '{variant.Product.Name}' vừa hết hàng.");
                }
                variant.StockQuantity -= ci.Quantity;
            }

            if (coupons != null)
            {
                coupons.UsedCount += 1;
                _dbContext.CouponUsages.Add(new CouponUsage
                {
                    CouponId = coupons.Id,
                    UserId = userId,
                    OrderId = order.Id,
                    DiscountAmount = discountAmount,
                    UsedAt = DateTimeUtils.GetDate()
                });
            }

            _dbContext.Payments.Add(new Payments
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending.ToString(),
                Method = input.PaymentMethod,
                PaidAt = null
            });

            _dbContext.Shipments.Add(new Shipment
            {
                OrderId = order.Id,
                ShippingProvider = input.ShippingProvider ?? "Standard",
                ShippingFee = shippingFee,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddressSnapshot,
                Status = ShipmentStatus.Pending.ToString(),
                CreatedDate = DateTimeUtils.GetDate(),
            });

            _dbContext.CartItems.RemoveRange(cart.Items);
            cart.LastUpdatedAt = DateTimeUtils.GetDate();

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Đơn hàng {OrderCode} tạo thành công cho User {UserId}", order.OrderCode, userId);

            return Result<OrderDto>.Success(new OrderDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedDate = order.CreatedDate
            });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            _logger.LogWarning(ex, "Conflict RowVersion (Tồn kho) khi chốt đơn cho User: {UserId}", userId);
            return Result<OrderDto>.Failure(ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(), "Sản phẩm bạn chọn vừa có sự thay đổi. Vui lòng thử lại.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi Database/System khi tạo đơn cho User: {UserId}", userId);
            return Result<OrderDto>.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), "Hệ thống gặp sự cố. Vui lòng thử lại sau.");
        }
    }

    public async Task<Result> CancelOrder(int orderId, string? reason = null)
    {
        _logger.LogInformation("{method} : OrderId={OrderId} with reason: {Reason}", nameof(CancelOrder), orderId, reason);
        var userId = _httpContext.GetCurrentUserId();

        // Fix cảnh báo Cartesian Explosion bằng AsSplitQuery
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
            .Include(o => o.Payments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && !o.Deleted);

        if (order == null)
            return Result.Failure(ErrorCode.OrderNotFound, this.GetCurrentMethodInfo());

        if (!OrderStatusConst.CancellableStatuses.Contains(order.Status))
            return Result.Failure(ErrorCode.BadRequest, this.GetCurrentMethodInfo(), $"Đơn hàng hiện tại ở trạng thái '{order.Status}' nên không thể hủy.");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            order.Status = OrderStatusConst.Cancelled;
            order.ModifiedDate = DateTimeUtils.GetDate();

            foreach (var item in order.OrderItems)
            {
                var variant = item.ProductVariant;
                if (variant != null)
                {
                    variant.StockQuantity += item.Quantity;
                }
            }

            var couponUsage = await _dbContext.CouponUsages.Include(cu => cu.Coupon).FirstOrDefaultAsync(cu => cu.OrderId == orderId);
            if (couponUsage != null)
            {
                couponUsage.Coupon.UsedCount = Math.Max(0, couponUsage.Coupon.UsedCount - 1);
                _dbContext.CouponUsages.Remove(couponUsage);
            }

            var payment = order.Payments.FirstOrDefault();
            if (payment != null)
            {
                payment.Status = payment.Status == PaymentStatus.Success ? PaymentStatus.Refunded : PaymentStatus.Failed;
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi Database/System nội bộ khi hủy đơn hàng {OrderCode}", order.OrderCode);
            return Result.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), "Hệ thống gặp sự cố khi hủy đơn hàng.");
        }
    }

    public async Task<Result<OrderDto>> GetById(int orderId)
    {
        var userId = _httpContext.GetCurrentUserId();
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .AsSplitQuery() // Tránh lỗi Cartesian Explosion khi Include nhiều collections
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && !o.Deleted);

        if (order == null)
            return Result<OrderDto>.Failure(ErrorCode.OrderNotFound, this.GetCurrentMethodInfo(), $"Không tìm thấy đơn hàng với ID {orderId}.");

        return Result<OrderDto>.Success(MapToDto(order));
    }

    private string GenerateOrderCode()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        return $"ORD-{datePart}-{randomPart}";
    }

    public async Task<Result<PageResult<OrderDto>>> GetMyOrders(FilterOrderPagingDto input)
    {
        var userID = _httpContext.GetCurrentUserId();
        var query = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .AsSplitQuery()
            .Where(o => o.UserId == userID && !o.Deleted && (string.IsNullOrEmpty(input.Status) || o.Status == input.Status))
            .OrderByDescending(o => o.CreatedDate);

        var totalItems = await query.CountAsync();

        // EF Core lấy data về bộ nhớ trước (ToListAsync)
        var orders = await query.Paging(input).ToListAsync();

        // Map DTO trên RAM thay vì bắt EF dịch hàm MapToDto sang SQL
        var itemsDto = orders.Select(MapToDto).ToList();

        return Result<PageResult<OrderDto>>.Success(new PageResult<OrderDto>
        {
            TotalItems = totalItems,
            Items = itemsDto
        });
    }

    public async Task<Result<PageResult<OrderDto>>> GetAllOrders(FilterOrderPagingDto input)
    {
        var query = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .AsSplitQuery()
            .Where(o =>
                !o.Deleted &&
                (string.IsNullOrEmpty(input.Status) || o.Status == input.Status) &&
                (string.IsNullOrEmpty(input.Keyword) || o.OrderCode.Contains(input.Keyword) || o.ShippingAddress.Contains(input.Keyword)))
            .OrderByDescending(o => o.CreatedDate);

        var totalItems = await query.CountAsync();

        // Fetch data trước
        var orders = await query.Paging(input).ToListAsync();

        // Map sang Dto trong RAM
        var itemsDto = orders.Select(MapToDto).ToList();

        return Result<PageResult<OrderDto>>.Success(new PageResult<OrderDto>
        {
            TotalItems = totalItems,
            Items = itemsDto
        });
    }

    public async Task<Result> UpdateOrderStatus(int orderId, string newStatus)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && !o.Deleted);
        if (order == null)
            return Result.Failure(ErrorCode.OrderNotFound, this.GetCurrentMethodInfo());

        if (OrderStatusConst.TerminalStatuses.Contains(order.Status))
            return Result.Failure(ErrorCode.OrderCannotBeCancelled, this.GetCurrentMethodInfo(), $"Đơn hàng hiện tại ở trạng thái '{order.Status}' nên không thể cập nhật nữa.");

        order.Status = newStatus;
        order.ModifiedDate = DateTimeUtils.GetDate();

        if (newStatus == OrderStatusConst.Cancelled)
        {
            // Explicitly load navigation properties needed for cancellation
            await _dbContext.Entry(order).Collection(o => o.OrderItems).Query().Include(oi => oi.ProductVariant).LoadAsync();
            await _dbContext.Entry(order).Collection(o => o.Payments).LoadAsync();

            foreach (var item in order.OrderItems)
            {
                var variant = item.ProductVariant;
                if (variant != null)
                {
                    variant.StockQuantity += item.Quantity;
                }
            }

            var couponUsage = await _dbContext.CouponUsages.Include(cu => cu.Coupon).FirstOrDefaultAsync(cu => cu.OrderId == orderId);
            if (couponUsage != null)
            {
                couponUsage.Coupon.UsedCount = Math.Max(0, couponUsage.Coupon.UsedCount - 1);
                _dbContext.CouponUsages.Remove(couponUsage);
            }

            var payment = order.Payments.FirstOrDefault();
            if (payment != null)
            {
                payment.Status = payment.Status == PaymentStatus.Success ? PaymentStatus.Refunded : PaymentStatus.Failed;
            }
        }
        else if (newStatus == OrderStatusConst.Shipping)
        {
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment != null) shipment.Status = ShipmentStatus.InTransit.ToString();
        }
        else if (newStatus == OrderStatusConst.Delivered)
        {
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment != null)
            {
                shipment.Status = ShipmentStatus.Delivered.ToString();
                shipment.ActualDelivery = DateTimeUtils.GetDate();
            }
        }
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }

    // ── MAPPER ─────────────────────────────────────────────────────────────────
    private static OrderDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        OrderCode = order.OrderCode,
        Status = order.Status,
        PaymentMethod = order.PaymentMethod,
        Subtotal = order.Subtotal,
        DiscountAmount = order.DiscountAmount,
        ShippingFee = order.ShippingFee,
        TotalAmount = order.TotalAmount,
        ShippingAddress = order.ShippingAddress,
        CreatedDate = order.CreatedDate,
        Items = order.OrderItems?.Select(i => new OrderItemDto
        {
            Id = i.Id,
            ProductName = i.ProductName,
            VariantSKU = i.VariantSKU,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            LineTotal = i.UnitPrice * i.Quantity,
        }) ?? Array.Empty<OrderItemDto>(),
        Payment = order.Payments?.FirstOrDefault() is { } p ? new PaymentDto
        {
            Id = p.Id,
            OrderId = p.OrderId,
            Method = p.Method,
            Status = p.Status,
            Amount = p.Amount,
            PaidAt = p.PaidAt,
            TransactionId = p.TransactionId,
            GatewayResponseCode = p.GatewayResponseCode,
            PaymentUrl = p.PaymentUrl,
            CreatedAt = p.CreatedAt,
            RefundedAmount = p.RefundedAmount,
            RefundedAt = p.RefundedAt,
            RefundReason = p.RefundReason
        } : null,
        Shipment = order.Shipments?.FirstOrDefault() is { } s ? new Dtos.ShipmentDto
        {
            Id = s.Id,
            OrderId = s.OrderId,
            ShippingProvider = s.ShippingProvider,
            TrackingNumber = s.TrackingNumber,
            ShippingFee = s.ShippingFee,
            ReceiverName = s.ReceiverName,
            ReceiverPhone = s.ReceiverPhone,
            ShippingAddress = s.ShippingAddress,
            Status = s.Status,
            EstimatedDelivery = s.EstimatedDelivery,
            ActualDelivery = s.ActualDelivery,
            CreatedDate = s.CreatedDate,
        } : null,
    };
}