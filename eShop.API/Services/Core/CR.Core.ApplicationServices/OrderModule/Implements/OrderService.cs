using CR.ApplicationBase.Common;
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
using CR.Constants.Orders;
using CR.Core.ApplicationServices.CartModule.Abstracts;
using CR.Core.ApplicationServices.AddressModule.Abstracts;
using CR.Core.ApplicationServices.PaymentModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.ShipmentModule.Abstracts;


namespace CR.Core.ApplicationServices.OrderModule.Implements;

public class OrderService : CoreServiceBase, IOrderService
{
    private readonly ICartService _cartService;
    private readonly IAddressService _addressService;
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    private readonly IShipmentService _shipmentService;
    public OrderService(
        ILogger<OrderService> logger,
        IHttpContextAccessor httpContext,
        ICartService cartService,
        IAddressService addressService,
        IPaymentService paymentService,
        IUserService userService,
        IShipmentService shipmentService)
    : base(logger, httpContext)
    {
        _cartService = cartService;
        _addressService = addressService;
        _paymentService = paymentService;
        _userService = userService;
        _shipmentService = shipmentService;
    }

    public async Task<Result<OrderDto>> CreateOrder(CreateOrderDto input)
    {
        _logger.LogInformation("{method} called with input: {@input}", nameof(CreateOrder), input);
        var userId = _httpContext.GetCurrentUserId();

        var cartResult = await _cartService.GetCartAsync();
        if (cartResult.IsFailure)
            return Result<OrderDto>.Failure(cartResult.ErrorCode, cartResult.GetCurrentMethodInfo());
        var cart = cartResult.Value;
        if (cart == null || cart.Items.Count == 0)
        {
            return Result<OrderDto>.Failure(ErrorCode.CartEmpty, this.GetCurrentMethodInfo());
        }

        var stockErrors = new List<string>();
        foreach (var item in cart.Items)
        {
            if (!item.IsAvailable)
            {
                stockErrors.Add($"Sản phẩm '{item.ProductName ?? "Không xác định"}' không tồn tại hoặc đã ngừng kinh doanh.");
                continue;
            }

            if (item.MaxQuantity < item.Quantity)
            {
                stockErrors.Add($"'{item.ProductName} - Size/Màu: {item.SKU}': Chỉ còn {item.MaxQuantity} sản phẩm.");
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
            var addressResult = await _addressService.GetAddressesByUserIdAsync();
            if (addressResult.IsFailure)
                return Result<OrderDto>.Failure(addressResult.ErrorCode, addressResult.GetCurrentMethodInfo());

            var address = addressResult.Value.FirstOrDefault(a => a.Id == input.AddressId.Value);
            if (address == null)
                return Result<OrderDto>.Failure(ErrorCode.AddressNotFound, this.GetCurrentMethodInfo(), $"Địa chỉ với ID {input.AddressId.Value} không tồn tại.");

            var userResult = await _userService.GetUserByIdAsync(userId);
            var userProfile = userResult.IsSuccess ? userResult.Value : null;

            receiverName = !string.IsNullOrWhiteSpace(address.ReceiverName) ? address.ReceiverName : (userProfile?.FullName ?? "Khách hàng");
            receiverPhone = !string.IsNullOrWhiteSpace(address.ReceiverPhone) ? address.ReceiverPhone : (userProfile?.Phone ?? string.Empty);
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

        var tempSubtotal = cart.Subtotal;

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
                CreatedDate = DateTime.UtcNow,
            }).Entity;

            await _dbContext.SaveChangesAsync();

            var orderItems = cart.Items.Select(ci => new OrderItem
            {
                OrderId = order.Id,
                ProductVariantId = ci.ProductVariantId,
                Quantity = ci.Quantity,
                ProductName = ci.ProductName,
                VariantSKU = ci.SKU,
                UnitPrice = ci.UnitPrice
            }).ToList();

            _dbContext.OrderItems.AddRange(orderItems);

            var variantIds = cart.Items.Select(i => i.ProductVariantId).ToList();
            var variants = await _dbContext.ProductVariants
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();

            foreach (var ci in cart.Items)
            {
                var variant = variants.FirstOrDefault(v => v.Id == ci.ProductVariantId);
                if (variant == null || variant.StockQuantity < ci.Quantity)
                {
                    await transaction.RollbackAsync();
                    return Result<OrderDto>.Failure(ErrorCode.InsufficientStock, this.GetCurrentMethodInfo(), $"Sản phẩm '{ci.ProductName}' vừa hết hàng.");
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

            var paymentResult = await _paymentService.CreateInitialPaymentAsync(order.Id, order.TotalAmount, input.PaymentMethod);
            if (!paymentResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return Result<OrderDto>.Failure(paymentResult.ErrorCode, this.GetCurrentMethodInfo());
            }

            var shipmentResult = await _shipmentService.CreateInitialShipmentAsync(order.Id, receiverName, receiverPhone, shippingAddressSnapshot, input.ShippingProvider ?? "Standard", shippingFee);
            if (!shipmentResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return Result<OrderDto>.Failure(shipmentResult.ErrorCode, this.GetCurrentMethodInfo());
            }

            var cartItemsToRemoveResult = await _cartService.ClearCart();
            if (!cartItemsToRemoveResult.IsSuccess)
                return Result<OrderDto>.Failure(cartItemsToRemoveResult.ErrorCode, this.GetCurrentMethodInfo());

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
            return Result<OrderDto>.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), $"[DEV ONLY] Lỗi hệ thống: {ex.InnerException?.Message ?? ex.Message}");
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
                if (payment.Status == PaymentStatus.Success.ToString())
                {
                    payment.Status = PaymentStatus.Refunded.ToString();
                    payment.RefundedAmount = payment.Amount;
                    payment.RefundedAt = DateTimeUtils.GetDate();
                    payment.RefundReason = reason ?? "Khách hàng hủy đơn";

                    // Ghi vào bảng OrderRefunds
                    foreach (var item in order.OrderItems)
                    {
                        var orderRefund = new OrderRefund
                        {
                            OrderItemId = item.Id,
                            RefundQuantity = item.Quantity,
                            Reason = reason ?? "Khách hàng hủy đơn",
                            Status = "APPROVED"
                        };
                        _dbContext.OrderRefunds.Add(orderRefund);
                    }
                }
                else if (payment.Status == PaymentStatus.Pending.ToString())
                {
                    payment.Status = PaymentStatus.Failed.ToString();
                }
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


        var baseQuery = _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userID && !o.Deleted && (string.IsNullOrEmpty(input.Status)
            || o.Status == input.Status));


        var totalItems = await baseQuery.CountAsync();

        var orders = await baseQuery
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .AsSplitQuery()
            .OrderByDescending(o => o.CreatedDate)
            .Paging(input)
            .ToListAsync();

        var itemsDto = orders.Select(MapToDto).ToList();

        return Result<PageResult<OrderDto>>.Success(new PageResult<OrderDto>
        {
            TotalItems = totalItems,
            Items = itemsDto
        });
    }

    public async Task<Result<PageResult<OrderDto>>> GetAllOrders(FilterOrderPagingDto input)
    {
        // 1. Base Query chỉ dùng để lọc
        var baseQuery = _dbContext.Orders
            .AsNoTracking()
            .Where(o =>
                !o.Deleted &&
                (string.IsNullOrEmpty(input.Status) || o.Status == input.Status) &&
                (string.IsNullOrEmpty(input.Keyword) || o.OrderCode.Contains(input.Keyword) || o.ShippingAddress.Contains(input.Keyword)));

        // 2. Đếm tổng số lượng (Nhanh và không bị Timeout)
        var totalItems = await baseQuery.CountAsync();

        // 3. Fetch dữ liệu thật (Chỉ Include cho những record của trang hiện tại nhờ Paging)
        var orders = await baseQuery
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .AsSplitQuery()
            .OrderByDescending(o => o.CreatedDate)
            .Paging(input)
            .ToListAsync();

        var itemsDto = orders.Select(MapToDto).ToList();

        return Result<PageResult<OrderDto>>.Success(new PageResult<OrderDto>
        {
            TotalItems = totalItems,
            Items = itemsDto
        });
    }

    public async Task<Result> UpdateOrderStatus(int orderId, UpdateOrderStatusDto input)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            {OrderStatusConst.Pending, [ OrderStatusConst.Confirmed, OrderStatusConst.Processing, OrderStatusConst.Cancelled]},
            { OrderStatusConst.Confirmed, [ OrderStatusConst.Processing, OrderStatusConst.Cancelled]},
            { OrderStatusConst.Processing, [ OrderStatusConst.Shipping, OrderStatusConst.Cancelled]},
            { OrderStatusConst.Shipping, [ OrderStatusConst.Delivered, OrderStatusConst.Returned]}
        };
        var order = await _dbContext.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId && !o.Deleted);
        if (order == null)
            return Result.Failure(ErrorCode.OrderNotFound, this.GetCurrentMethodInfo());

        var currentStatus = order.Status.ToUpper();
        var newStatus = input.NewStatus.ToUpper();

        if (OrderStatusConst.TerminalStatuses.Contains(currentStatus))
            return Result.Failure(ErrorCode.OrderCannotBeCancelled, this.GetCurrentMethodInfo(),
                $"Đơn hàng hiện tại ở trạng thái '{order.Status}' nên không thể cập nhật nữa.");

        if (!validTransitions.ContainsKey(currentStatus) || !validTransitions[currentStatus].Contains(newStatus))
            return Result.Failure(ErrorCode.BadRequest, this.GetCurrentMethodInfo(), $"Không thể chuyển trạng thái từ '{order.Status}' sang '{input.NewStatus}'");

        if (newStatus == OrderStatusConst.Cancelled)
            return await CancelOrder(orderId, input.Reason);

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            order.Status = newStatus;
            order.ModifiedDate = DateTimeUtils.GetDate();

            if (newStatus == OrderStatusConst.Shipping)
            {
                if (string.IsNullOrWhiteSpace(input.TrackingNumber) || string.IsNullOrWhiteSpace(input.ShippingProvider))
                    return Result.Failure(ErrorCode.BadRequest, this.GetCurrentMethodInfo(), "Bắt buộc phải nhập mã vận đơn và đơn vị vận chuyển");

                await _shipmentService.UpdateTrackingByOrderIdAsync(orderId, input.TrackingNumber, input.ShippingProvider);
            }
            else if (newStatus == OrderStatusConst.Delivered)
            {
                await _shipmentService.UpdateShipmentStatusByOrderIdAsync(orderId, ShipmentStatus.Delivered.ToString());

                var payment = order.Payments?.FirstOrDefault();
                if (payment != null && payment.Status == PaymentStatus.Pending.ToString())
                {
                    await _paymentService.UpdatePaymentStatusByOrderIdAsync(orderId, PaymentStatus.Success.ToString());
                }
            }
            else if (newStatus == OrderStatusConst.Returned)
            {

                await _shipmentService.UpdateShipmentStatusByOrderIdAsync(orderId, ShipmentStatus.Returned.ToString());
                var orderItems = await _dbContext.OrderItems
                    .Include(oi => oi.ProductVariant)
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync();

                var payment = order.Payments?.FirstOrDefault();
                if (payment != null)
                {
                    if (payment.Status == PaymentStatus.Pending.ToString())
                    {
                        await _paymentService.UpdatePaymentStatusByOrderIdAsync(orderId, PaymentStatus.Failed.ToString());
                    }
                    else if (payment.Status == PaymentStatus.Success.ToString())
                    {
                        payment.Status = PaymentStatus.Refunded.ToString();
                        payment.RefundedAmount = payment.Amount;
                        payment.RefundedAt = DateTimeUtils.GetDate();
                        payment.RefundReason = input.Reason ?? "Khách hàng trả hàng";

                        // Ghi vào bảng OrderRefunds
                        foreach (var item in orderItems)
                        {
                            var orderRefund = new OrderRefund
                            {
                                OrderItemId = item.Id,
                                RefundQuantity = item.Quantity,
                                Reason = input.Reason ?? "Khách hàng trả hàng",
                                Status = "APPROVED"
                            };
                            _dbContext.OrderRefunds.Add(orderRefund);
                        }
                    }
                }

                // 4. Hoàn lại số lượng tồn kho (Restock)
                foreach (var item in orderItems)
                {
                    if (item.ProductVariant != null)
                    {
                        item.ProductVariant.StockQuantity += item.Quantity;
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi Database/System khi cập nhật trạng thái đơn hàng {OrderCode}", order.OrderCode);
            return Result.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo(), "Hệ thống gặp sự cố khi cập nhật đơn hàng.");
        }
    }

    // ── MAPPER 
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