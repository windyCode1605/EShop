using CR.Constants.ErrorCodes;
using CR.Constants.Orders;
using CR.Constants.Payment;
using CR.Constants.Shipment;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.ShipmentModule.Abstracts;
using CR.Core.Domain.Logistics;
using CR.Core.Dtos.Shipment;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.ShipmentModule.Implements
{
    public class ShipmentService : CoreServiceBase, IShipmentService
    {
        public ShipmentService(ILogger<ShipmentService> logger, IHttpContextAccessor httpContextAccessor) : base(logger, httpContextAccessor)
        {
        }

        public async Task<Result<ShipmentDto>> GetOrderById(int orderId)
        {
            _logger.LogInformation("{Method}: OrderId:{OrderId}", nameof(GetOrderById), orderId);
            var userId = _httpContext.GetCurrentUserId();

            // User chỉ xem shipment của đơn mình
            var orderBelongsToUser = await _dbContext.Orders
                .AnyAsync(o => o.Id == orderId && o.UserId == userId && !o.Deleted);
            if (!orderBelongsToUser)
                return Result<ShipmentDto>.Failure(ErrorCode.OrderNotFound, this.GetCurrentMethodInfo());
            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment == null)
                return Result<ShipmentDto>.Failure(ErrorCode.ShipmentNotFound, this.GetCurrentMethodInfo());
            return Result<ShipmentDto>.Success(MapToDto(shipment));
        }

        public async Task<Result> UpdateTracking(UpdateTrackingDto input)
        {
            _logger.LogInformation("{Method}: ShipmentId:{ShipmentId}, TrackingNumber:{TrackingNumber}, ShippingProvider:{ShippingProvider}", nameof(UpdateTracking), input.ShipmentId, input.TrackingNumber, input.ShippingProvider);
            
            var shipment = await _dbContext.Shipments
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.Id == input.ShipmentId && !s.Deleted);
            if (shipment == null)                
                return Result.Failure(ErrorCode.ShipmentNotFound, this.GetCurrentMethodInfo());

            // Cập nhật thông tin tracking
            shipment.TrackingNumber         = input.TrackingNumber;
            shipment.ShippingProvider       = input.ShippingProvider;
            shipment.EstimatedDelivery      = input.EstimatedDelivery;
            shipment.Status                 = ShipmentStatus.PickedUp;
            shipment.ModifiedDate           = DateTime.UtcNow;
            
            // Đồng bộ Order Status -> SHIPPING
            var order = shipment.Order;
            if (order.Status == OrderStatusConst.Processing || order.Status == OrderStatusConst.Confirmed)
            {
                order.Status        = OrderStatusConst.Shipping;
                order.ModifiedDate  = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Cập nhật tracking {Tracking} cho ShipmentId {ShipmentId} thành công", input.TrackingNumber, input.ShipmentId);
            return Result.Success();
        }
        public async Task<Result> HandleShipmentWebhook(ShipmentWebhookDto input)
        {
            _logger.LogInformation("{Method}: TrackingNumber:{TrackingNumber}, NewStatus:{NewStatus}", nameof(HandleShipmentWebhook), input.TrackingNumber, input.NewStatus);
            var shipment = await _dbContext.Shipments
                .Include(s => s.Order)
                .ThenInclude(o => o.Payments)
                .FirstOrDefaultAsync(s => s.TrackingNumber == input.TrackingNumber && !s.Deleted);
            if (shipment == null)
            {
                _logger.LogWarning("Không tìm thấy shipment với TrackingNumber {TrackingNumber} khi xử lý webhook", input.TrackingNumber);
                return Result.Success();
             }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                shipment.Status = input.NewStatus;
                shipment.ModifiedDate = DateTimeUtils.GetDate();

                var order = shipment.Order;
                switch (input.NewStatus)
                {
                    // Shipper đã lấy hàng, cập nhật trạng thái đơn sang "Đang giao"
                    case ShipmentStatus.PickedUp:
                        order.Status = OrderStatusConst.Shipping;
                        break;

                    // Đang vận chuyển
                    case ShipmentStatus.InTransit:
                        // Có thể cập nhật trạng thái đơn sang "Đang giao" nếu chưa cập nhật ở trạng thái PICKED_UP
                            order.Status = OrderStatusConst.Shipping;
                        break;
                    
                    // Giao hàng thành công
                    case ShipmentStatus.Delivered:
                        order.Status = OrderStatusConst.Delivered;
                        shipment.ActualDelivery = input.ActualDelivery ?? DateTimeUtils.GetDate();
                        order.ModifiedDate = DateTimeUtils.GetDate();

                        // COD: thu tiền khi giao -> đánh dấu Payment thành công
                        var codPayment = order.Payments.FirstOrDefault(p => 
                            p.Method == PaymentMethod.Cash && 
                            p.Status == PaymentStatus.Pending);
                        if (codPayment != null)
                        {
                            codPayment.Status = PaymentStatus.Success;
                            codPayment.PaidAt = DateTimeUtils.GetDate();
                        }
                        break;
                    
                    // Giao Thất bại
                    case ShipmentStatus.Failed:
                        order.Status = OrderStatusConst.Returned;
                        break;
                    
                    // Hàng bị trả về
                    case ShipmentStatus.Returned:
                        order.Status = OrderStatusConst.Returned;
                        
                        // Hoàn lại tồn kho
                        var orderItems = await _dbContext.OrderItems
                            .Where(oi => oi.OrderId == order.Id)
                            .ToListAsync();
                        foreach (var item in orderItems)
                        {
                            var variant = await _dbContext.ProductVariants
                                .FirstOrDefaultAsync(v => 
                                v.Id == item.ProductVariantId);
                            if (variant != null)                            
                                variant.StockQuantity += item.Quantity;
                        }

                        break;
                }
                order.ModifiedDate = DateTimeUtils.GetDate();
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Cập nhật trạng thái vận chuyển {NewStatus} cho ShipmentId {ShipmentId} thành công", input.NewStatus, shipment.Id);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Lỗi xử lý Webhook shipment {Tracking}", nameof(HandleShipmentWebhook), input.TrackingNumber);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Result> UpdateShipmentStatus(int ShipmentID, string newStatus)
        {
            _logger.LogInformation("{Method}: ShipmentID:{ShipmentID}, NewStatus:{NewStatus}", nameof(UpdateShipmentStatus), ShipmentID, newStatus);
            var shipment = _dbContext.Shipments
                .FirstOrDefault(s => s.Id == ShipmentID && !s.Deleted);
            if (shipment == null) 
                return Result.Failure(ErrorCode.ShipmentNotFound, this.GetCurrentMethodInfo());
            shipment.Status = newStatus;
            shipment.ModifiedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        private static ShipmentDto MapToDto(Shipment s) => new()
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
        };

        public async Task<Result> CreateInitialShipmentAsync(int orderId, string receiverName, string receiverPhone, string shippingAddress, string shippingProvider, decimal shippingFee)
        {
            _dbContext.Shipments.Add(new Shipment
            {
                OrderId = orderId,
                ShippingProvider = shippingProvider,
                ShippingFee = shippingFee,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddress,
                Status = ShipmentStatus.Pending.ToString(),
                CreatedDate = DateTimeUtils.GetDate(),
            });
            return await Task.FromResult(Result.Success());
        }

        public async Task<Result> UpdateShipmentStatusByOrderIdAsync(int orderId, string newStatus)
        {
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment != null)
            {
                shipment.Status = newStatus;
                shipment.ModifiedDate = DateTimeUtils.GetDate();
                if (newStatus == ShipmentStatus.Delivered.ToString())
                {
                    shipment.ActualDelivery = DateTimeUtils.GetDate();
                }
            }
            return Result.Success();
        }

        public async Task<Result> UpdateTrackingByOrderIdAsync(int orderId, string trackingNumber, string shippingProvider)
        {
            var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.OrderId == orderId && !s.Deleted);
            if (shipment != null)
            {
                shipment.Status = ShipmentStatus.InTransit.ToString();
                shipment.TrackingNumber = trackingNumber;
                shipment.ShippingProvider = shippingProvider;
                shipment.ModifiedDate = DateTimeUtils.GetDate();
            }
            return Result.Success();
        }
    }
}
