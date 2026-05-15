using CR.Core.Dtos.Shipment;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.ShipmentModule.Abstracts;

public interface IShipmentService
{
    Task<Result<ShipmentDto>>       GetOrderById(int orderId);
    Task<Result>                    UpdateTracking(UpdateTrackingDto input);
    Task<Result>                    HandleShipmentWebhook(ShipmentWebhookDto input);
    Task<Result>                    UpdateShipmentStatus(int ShipmentID, string newStatus); // Cập nhật trạng thái vận chuyển (ví dụ: "In Transit", "Delivered", "Returned")
}