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
using Microsoft.AspNetCore.Http.Features;


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

        // ==========================================
        // [1] LẤY VÀ KIỂM TRA GIỎ HÀNG
        // ==========================================
        var cart = await _dbContext.Carts.Include(c => c.Items)
                                         .ThenInclude(ci => ci.ProductVariant)
                                         .ThenInclude(pv => pv.Product)
                                         .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null || !cart.Items.Any())
        {
            return Result<OrderDto>.Failure(ErrorCode.CartEmpty, this.GetCurrentMethodInfo());
        }

        // ==========================================
        // [2] VALIDATE TỒN KHO VÀ TRẠNG THÁI SẢN PHẨM
        // ==========================================
        var stockErrors = new List<string>();
        foreach (var item in cart.Items)
        {
            var variant = item.ProductVariant;

            // Check sản phẩm có bị xóa không
            if (variant == null || variant.Deleted || variant.Product.Deleted)
            {
                stockErrors.Add($"Sản phẩm '{variant?.Product?.Name ?? "Không xác định"}' không tồn tại hoặc đã ngừng kinh doanh.");
                continue;
            }

            // Check số lượng tồn kho cứng
            if (variant.StockQuantity < item.Quantity)
            {
                stockErrors.Add($"'{variant.Product.Name} - Size/Màu: {variant.SKU}': Chỉ còn {variant.StockQuantity} sản phẩm.");
            }
        }

        if (stockErrors.Any())
        {
            return Result<OrderDto>.Failure(
                    ErrorCode.InsufficientStock,
                    this.GetCurrentMethodInfo(),
                    stockErrors);
        }

        // ==========================================
        // [3] XỬ LÝ ĐỊA CHỈ GIAO HÀNG
        // ==========================================
        string shippingAddressSnapshot;
        string receiverName;
        string receiverPhone;

        if (input.AddressId.HasValue)
        {
            // User chọn địa chỉ có sẵn trong Address Book
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == input.AddressId.Value && a.UserId == userId && !a.IsDeleted);
            if (address == null)
            {
                return Result<OrderDto>.Failure(
                    ErrorCode.AddressNotFound,
                    this.GetCurrentMethodInfo(),
                    $"Địa chỉ với ID {input.AddressId.Value} không tồn tại.");
            }

            var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
            receiverName = userProfile?.FullName ?? "Khách hàng";

            // Nếu bảng Profile chưa có sđt, lấy fallback từ bảng Users (nếu thiết kế của anh cho phép)
            receiverPhone = userProfile?.PhoneNumber ?? string.Empty;

            shippingAddressSnapshot = $"{address.Street}, {address.City}, {address.Province}";
        }
        else
        {
            // User nhập địa chỉ mới vãng lai (Guest Address / One-time Address)
            if (string.IsNullOrWhiteSpace(input.ReceiverName) ||
                string.IsNullOrWhiteSpace(input.ReceiverPhone) ||
                string.IsNullOrWhiteSpace(input.Province) ||
                string.IsNullOrWhiteSpace(input.City) ||
                string.IsNullOrWhiteSpace(input.Street))
            {
                return Result<OrderDto>.Failure(
                    ErrorCode.AddressRequired,
                    this.GetCurrentMethodInfo(),
                    "Vui lòng cung cấp đầy đủ thông tin địa chỉ giao hàng.");
            }

            receiverName = input.ReceiverName;
            receiverPhone = input.ReceiverPhone;
            shippingAddressSnapshot = $"{input.Street}, {input.City}, {input.Province}";
        }

        // Tính toán trước tổng tiền hàng (Subtotal) để đem đi Validate điều kiện Mã giảm giá
        var tempSubtotal = cart.Items.Sum(ci => (ci.ProductVariant.Product.BasePrice + ci.ProductVariant.PriceAdjustment) * ci.Quantity);

        // ==========================================
        // [4] KIỂM TRA VÀ TÍNH TOÁN MÃ GIẢM GIÁ (COUPON)
        // ==========================================
        decimal discountAmount = 0;
        Coupons? coupons = null;

        if (!string.IsNullOrWhiteSpace(input.CouponCode))
        {
            var now = DateTime.UtcNow;
            coupons = await _dbContext.Coupons
                .FirstOrDefaultAsync(c =>
                    c.Code == input.CouponCode &&
                    c.IsActive &&
                    c.StartDate <= now &&
                    c.ExpiryDate >= now);

            if (coupons == null)
            {
                return Result<OrderDto>.Failure(
                    ErrorCode.CouponNotFound,
                    this.GetCurrentMethodInfo(),
                    $"Mã giảm giá '{input.CouponCode}' không tồn tại, chưa đến thời gian áp dụng hoặc đã hết hạn.");
            }

            // Check giới hạn tổng số lượng xuất bản của mã
            if (coupons.UsageLimit.HasValue && coupons.UsedCount >= coupons.UsageLimit.Value)
            {
                return Result<OrderDto>.Failure(
                    ErrorCode.CouponUsageLimitReached,
                    this.GetCurrentMethodInfo(),
                    $"Mã giảm giá '{input.CouponCode}' đã hết lượt sử dụng trên toàn hệ thống.");
            }

            // Check giới hạn số lần sử dụng của riêng từng User
            if (coupons.UsageLimitPerUser.HasValue)
            {
                var userUsageCount = await _dbContext.CouponUsages.CountAsync(cu => cu.CouponId == coupons.Id && cu.UserId == userId);
                if (userUsageCount >= coupons.UsageLimitPerUser.Value)
                {
                    return Result<OrderDto>.Failure(
                        ErrorCode.CouponUsageLimitReached,
                        this.GetCurrentMethodInfo(),
                        $"Bạn đã sử dụng mã giảm giá '{input.CouponCode}' đủ số lần cho phép ({coupons.UsageLimitPerUser.Value} lần).");
                }
            }

            // Check điều kiện giá trị đơn hàng tối thiểu
            if (coupons.MinOrderValue.HasValue && tempSubtotal < coupons.MinOrderValue.Value)
            {
                return Result<OrderDto>.Failure(
                    ErrorCode.CouponMinOrderNotMet,
                    this.GetCurrentMethodInfo(),
                    $"Đơn hàng của bạn chưa đạt giá trị tối thiểu {coupons.MinOrderValue.Value:N0}đ để áp dụng mã giảm giá này.");
            }

            // Tính số tiền được giảm
            if (coupons.DiscountType == DiscountType.Percentage)
            {
                discountAmount = tempSubtotal * (coupons.DiscountValue / 100);
                // Giới hạn số tiền giảm tối đa (Ví dụ: Giảm 10% nhưng tối đa 50k)
                if (coupons.MaxDiscountValue.HasValue)
                {
                    discountAmount = Math.Min(discountAmount, coupons.MaxDiscountValue.Value);
                }
            }
            else
            {
                discountAmount = coupons.DiscountValue; // Giảm tiền cố định (Fixed)
            }

            // Đảm bảo tiền giảm không được phép lớn hơn tiền hàng
            discountAmount = Math.Min(discountAmount, tempSubtotal);
        }

        // ==========================================
        // [5] TÍNH PHÍ VẬN CHUYỂN
        // ==========================================
        // Tạm thời dùng giá cố định. Thực tế Production sẽ tích hợp gọi API GHN/AhaMove/GHTK tại đây
        const decimal shippingFee = 30000;

        // ==========================================
        // [6] SAGA TRANSACTION: CHỐT ĐƠN & GHI NHẬN HỆ THỐNG
        // ==========================================
        // Mọi thay đổi thao tác Insert/Update từ điểm này trở đi sẽ được đóng gói chung vào 1 Transaction
        // Đảm bảo không có trường hợp "Đã trừ kho nhưng chưa lưu hóa đơn".
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // [6a] Tính tổng hóa đơn cuối cùng (Thanh toán = Tiền hàng - Giảm giá + Phí Ship)
            var totalAmount = tempSubtotal - discountAmount + shippingFee;

            // [6b] Tạo chứng từ Order gốc
            var order = _dbContext.Orders.Add(new Order
            {
                OrderCode = GenerateOrderCode(),
                UserId = userId,
                ShippingAddress = shippingAddressSnapshot,
                TotalAmount = totalAmount,
                Status = OrderStatusConst.Pending.ToString(),
                PaymentMethod = input.PaymentMethod,
                CreatedDate = DateTimeUtils.GetDate(),
            }).Entity;

            await _dbContext.SaveChangesAsync(); // Ép lưu xuống DB để EF Core sinh ra order.Id

            // [6c] Snapshot OrderItems: Sao chép cứng Tên, Giá, SKU vào thời điểm hiện tại 
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

            // [6d] Optimistic Locking: Trừ tồn kho trực tiếp trên các Entity đã được tracking
            foreach (var ci in cart.Items)
            {
                var variant = ci.ProductVariant;

                // Double check kho lần cuối trước khi trừ
                if (variant.StockQuantity < ci.Quantity)
                {
                    await transaction.RollbackAsync();
                    return Result<OrderDto>.Failure(
                        ErrorCode.InsufficientStock,
                        this.GetCurrentMethodInfo(),
                        $"Sản phẩm '{variant.Product.Name}' vừa hết hàng hoặc không đủ số lượng trong kho.");
                }

                // Chỉ giảm biến bộ nhớ. EF Core sẽ lo việc so sánh RowVersion dưới DB khi gọi SaveChanges
                variant.StockQuantity -= ci.Quantity;
            }

            // [6e] Ghi nhận lịch sử đốt Coupon
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

            // [6f] Khởi tạo Payment Tracking (Quan trọng: Chưa thanh toán thì PaidAt = null)
            _dbContext.Payments.Add(new Payments
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending.ToString(),
                Method = input.PaymentMethod,
                PaidAt = null
            });

            // [6g] Khởi tạo Shipment Tracking để bàn giao cho đối tác vận chuyển
            _dbContext.Shipments.Add(new Shipment
            {
                OrderId = order.Id,
                ShippingProvider = input.ShippingProvider ?? "Standard", // Lưu đơn vị vận chuyển
                ShippingFee = shippingFee,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddressSnapshot,
                Status = ShipmentStatus.Pending.ToString(),
                CreatedDate = DateTimeUtils.GetDate(),
            });

            // [6h] Dọn dẹp giỏ hàng
            _dbContext.CartItems.RemoveRange(cart.Items);
            cart.LastUpdatedAt = DateTimeUtils.GetDate();

            // [6i] Thực thi SQL: EF Core sẽ đẩy toàn bộ lệnh Update/Insert xuống Database
            // Nếu có xung đột về RowVersion ở bảng ProductVariant, exception sẽ ném ra tại hàm này.
            await _dbContext.SaveChangesAsync();

            // [6j] Chốt giao dịch an toàn
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
            // Bắt cờ đỏ từ Optimistic Locking: Chặn đứng tình huống bán vượt quá số lượng kho (Overselling)
            await transaction.RollbackAsync();
            _logger.LogWarning(ex, "Conflict RowVersion (Tồn kho) khi chốt đơn cho User: {UserId}", userId);

            return Result<OrderDto>.Failure(
                ErrorCode.InsufficientStock,
                this.GetCurrentMethodInfo(),
                "Sản phẩm bạn chọn vừa có sự thay đổi về số lượng tồn kho bởi người mua khác. Vui lòng thử lại.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi Database/System nội bộ khi tạo đơn hàng cho User: {UserId}", userId);

            return Result<OrderDto>.Failure(
                ErrorCode.UnknownError,
                this.GetCurrentMethodInfo(),
                "Hệ thống đang quá tải hoặc gặp sự cố khi tạo đơn hàng. Vui lòng thử lại sau.");
        }
    }

    // ==========================================
    // CÁC HÀM XỬ LÝ PHỤ TRỢ (STUBS)
    // ==========================================

    public async Task<Result> CancelOrder(int orderId, string? reason = null)
    {
        _logger.LogInformation("{method} : OrderId={OrderId} with reason: {Reason}", nameof(CancelOrder), orderId, reason);
        var userId = _httpContext.GetCurrentUserId();

        var order = _dbContext.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .FirstOrDefault(o => o.Id == orderId && o.UserId == userId && !o.Deleted);

        if (order == null)       
            return Result.Failure(ErrorCode.OrderNotFound ,this.GetCurrentMethodInfo());
        
        // Chỉ hủy khi PENDING hoặc CONFIRM, đã thanh toán thì không cho hủy nữa
        if (!OrderStatusConst.CancellableStatuses.Contains(order.Status))
            return Result.Failure(
                ErrorCode.BadRequest,
                this.GetCurrentMethodInfo(),
                $"Đơn hàng hiện tại ở trạng thái '{order.Status}' nên không thể hủy.");
        

        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            // [1] Đánh giấu hủy đơn
            order.Status = OrderStatusConst.Cancelled;
            order.ModifiedDate = DateTimeUtils.GetDate();

            // [2] Hoàn lại tồn kho 
            foreach(var item in order.OrderItems)
            {
                var variant = _dbContext.ProductVariants.FirstOrDefault(pv => pv.Id == item.ProductVariantId);
                if (variant != null)
                {
                    variant.StockQuantity += item.Quantity;
                }
            }
            // [3] Hoàn lại coupon nếu đã dùng
            var couponUsage = await _dbContext.CouponUsages.Include(cu => cu.Coupon)
                .FirstOrDefaultAsync(cu => cu.OrderId == orderId);
            if (couponUsage != null)
            {
                couponUsage.Coupon.UsedCount = Math.Max(0, couponUsage.Coupon.UsedCount - 1);
                _dbContext.CouponUsages.Remove(couponUsage);
            }
            // [4] Cập nhật trạng thái thanh toán nếu đã tạo
            var payment = order.Payments.FirstOrDefault();
            if (payment != null)
            {
                payment.Status = payment.Status == PaymentStatus.Success
                    ? PaymentStatus.Refunded // Nếu đã thanh toán thành công thì chuyển sang Refunded
                    : PaymentStatus.Failed; // Nếu chưa thanh toán thì chuyển sang Cancelled
            }
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Đơn hàng {OrderCode} đã được hủy thành công cho User {UserId}", order.OrderCode, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi Database/System nội bộ khi hủy đơn hàng {OrderCode} cho User: {UserId}", order.OrderCode, userId);
            return Result.Failure(
                ErrorCode.UnknownError,
                this.GetCurrentMethodInfo(),
                "Hệ thống đang quá tải hoặc gặp sự cố khi hủy đơn hàng. Vui lòng thử lại sau.");
        }
    }

    public async Task<Result<OrderDto>> GetById(int orderId)
    {
        _logger.LogInformation("{method}: orderId={OrderId}", nameof(GetById), orderId);
        var userId = _httpContext.GetCurrentUserId();
        var order = await _dbContext.Orders
        .AsNoTracking()
        .Include(o => o.OrderItems)
        .Include(o => o.Payments)
        .Include(o => o.Shipments)
        .FirstOrDefaultAsync(o =>
        o.Id == orderId &&
        o.UserId == userId &&       // User chỉ xem đơn của chính mình
        !o.Deleted);
        if (order == null)
        {
            return Result<OrderDto>.Failure(
                ErrorCode.OrderNotFound,
                this.GetCurrentMethodInfo(),
                $"Không tìm thấy đơn hàng với ID {orderId} của bạn.");
        }
        return Result<OrderDto>.Success(new OrderDto
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedDate = order.CreatedDate
        });
    }
    private string GenerateOrderCode()
    {
        // Ví dụ: ORD-20260512-A1B2
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        return $"ORD-{datePart}-{randomPart}";
    }

    public async Task<Result<PageResult<OrderDto>>> GetMyOrders(FilterOrderPagingDto input)
    {
        _logger.LogInformation("{method} : input={@input}", nameof(GetMyOrders), input);
        var userID = _httpContext.GetCurrentUserId();

        var query = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .Where(o =>
                o.UserId == userID &&
                !o.Deleted &&
                (string.IsNullOrEmpty(input.Status) || o.Status == input.Status)) // Lọc theo trạng thái nếu có
            .OrderByDescending(o => o.CreatedDate); // Mới nhất trước

        return Result<PageResult<OrderDto>>.Success(new PageResult<OrderDto>
        {
            TotalItems = await query.CountAsync(),
            Items = await query.Paging(input)
                            .Select(order => MapToDto(order))
                            .ToListAsync()
        });
    }

    // ── MAPPER ─────────────────────────────────────────────────────────────────

    private static OrderDto MapToDto(Order order) => new()
    {
        Id = order.Id,
        OrderCode = order.OrderCode,
        Status = order.Status,
        PaymentMethod = order.PaymentMethod,
        TotalAmount = order.TotalAmount,
        ShippingAddress = order.ShippingAddress,
        CreatedDate = order.CreatedDate,
        Items = order.OrderItems.Select(i => new OrderItemDto
        {
            Id = i.Id,
            ProductName = i.ProductName,
            VariantSKU = i.VariantSKU,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            LineTotal = i.UnitPrice * i.Quantity,
        }),
        Payment = order.Payments.FirstOrDefault() is { } p ? new PaymentDto
        {
            Id = p.Id,
            OrderId = p.OrderId,
            Method = p.Method,
            Status = p.Status,
            Amount = p.Amount,
            PaidAt = p.PaidAt,
        } : null,
        Shipment = order.Shipments.FirstOrDefault() is { } s ? new Dtos.ShipmentDto
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

    public async Task<Result<PageResult<OrderDto>>> GetAllOrders(FilterOrderPagingDto input)
    {
        // Dành cho Admin - không filter theo userId
        _logger.LogInformation("{method} : input={@input}", nameof(GetAllOrders), input);
        var query = _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shipments)
            .Where(o =>
                !o.Deleted &&
                (string.IsNullOrEmpty(input.Status) || o.Status == input.Status) && 
                (string.IsNullOrEmpty(input.Keyword) || 
                o.OrderCode.Contains(input.Keyword) ||
                o.ShippingAddress.Contains(input.Keyword))) 
            .OrderByDescending(o => o.CreatedDate); // Mới nhất trước
        return Result<PageResult<OrderDto>>.Success(new PageResult<OrderDto>
        {
            TotalItems = await query.CountAsync(),
            Items = await query.Paging(input)
                            .Select(order => MapToDto(order))
                            .ToListAsync()
        });
    }

    public async Task<Result> UpdateOrderStatus(int orderId, string newStatus)
    {
        // Admin cập nhật trạng thái đơn hàng (PENDING, CONFIRM, SHIPPING, DELIVERED,...)
        _logger.LogInformation("{method} : orderId={OrderId}, newStatus={NewStatus}", nameof(UpdateOrderStatus), orderId, newStatus);
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId && !o.Deleted);
        if (order == null)        
            return Result.Failure(ErrorCode.OrderNotFound, this.GetCurrentMethodInfo());
        
        // Không cho đơn cập nhật ở trạng thái cuối
        if(OrderStatusConst.TerminalStatuses.Contains(order.Status))
            return Result.Failure(
                ErrorCode.OrderCannotBeCancelled,
                this.GetCurrentMethodInfo(),
                $"Đơn hàng hiện tại ở trạng thái '{order.Status}' nên không thể cập nhật trạng thái nữa.");
    
        order.Status = newStatus;
        order.ModifiedDate = DateTimeUtils.GetDate();

        // Nếu chuyển sang SHIPPING cập nhật shipment
        if(newStatus == OrderStatusConst.Shipping)
        {
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment != null)
            {
                shipment.Status = ShipmentStatus.InTransit;
            }
        }
        // Nếu chuyển sang DELIVERED cập nhật shipment và payment
        if(newStatus == OrderStatusConst.Delivered)
        {
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment != null)
            {
                shipment.Status = ShipmentStatus.Delivered;
                shipment.ActualDelivery = DateTimeUtils.GetDate();
            }
        }
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }
}