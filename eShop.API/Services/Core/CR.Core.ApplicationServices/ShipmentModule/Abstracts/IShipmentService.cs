using CR.Core.Dtos.Shipment;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.ShipmentModule.Abstracts;

public interface IShipmentService
{
    /// <summary>
    /// Lấy Shipment theo Order
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    Task<Result<ShipmentDto>>       GetOrderById(int orderId);
    /// <summary>
    /// Cập nhật thông tin tracking (ví dụ: tracking number, carrier) cho đơn hàng đã có shipment
    /// Sau khi bàn giao hàng cho shipper , admin nhập mã vận đơn
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<Result>                    UpdateTracking(UpdateTrackingDto input);
    /// <summary>
    /// Xử lý webhook callback từ đơn vị vận chuyển (ví dụ: khi có cập nhật trạng thái vận chuyển từ bên giao hàng như GHN/GHTK)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<Result>                    HandleShipmentWebhook(ShipmentWebhookDto input);
    /// <summary>
    /// Cập nhật trạng thái vận chuyển (ví dụ: "In Transit", "Delivered", "Returned") cho đơn hàng đã có shipment
    /// </summary>
    /// <param name="ShipmentID"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    Task<Result>                    UpdateShipmentStatus(int ShipmentID, string newStatus); // Cập nhật trạng thái vận chuyển (ví dụ: "In Transit", "Delivered", "Returned")
    Task<Result>                    CreateInitialShipmentAsync(int orderId, string receiverName, string receiverPhone, string shippingAddress, string shippingProvider, decimal shippingFee);
    Task<Result>                    UpdateShipmentStatusByOrderIdAsync(int orderId, string newStatus);
    Task<Result>                    UpdateTrackingByOrderIdAsync(int orderId, string trackingNumber, string shippingProvider);
}