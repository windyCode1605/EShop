// ApplicationServices/ShipmentModule/Dtos/ShipmentDtos.cs
namespace CR.Core.Dtos.Shipment;

public class ShipmentDto
{
    public int      Id               { get; set; }
    public int      OrderId          { get; set; }
    public string   ShippingProvider { get; set; } = null!;
    public string?  TrackingNumber   { get; set; }
    public decimal  ShippingFee      { get; set; }
    public string   ReceiverName     { get; set; } = null!;
    public string   ReceiverPhone    { get; set; } = null!;
    public string   ShippingAddress  { get; set; } = null!;
    public string   Status           { get; set; } = null!;
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime? ActualDelivery    { get; set; }
    public DateTime? CreatedDate       { get; set; }
}

/// <summary>
/// Webhook từ GHN/NinjaVan khi trạng thái vận đơn thay đổi.
/// </summary>
public class ShipmentWebhookDto
{
    public string  TrackingNumber { get; set; } = null!;
    public string  NewStatus      { get; set; } = null!;  // PICKED_UP, IN_TRANSIT,...
    public string? Note           { get; set; }
    public DateTime? ActualDelivery { get; set; }
}

/// <summary>Admin cập nhật tracking number sau khi bàn giao shipper.</summary>
public class UpdateTrackingDto
{
    public int    ShipmentId     { get; set; }
    public string TrackingNumber { get; set; } = null!;
    public string ShippingProvider { get; set; } = null!;
    public DateTime? EstimatedDelivery { get; set; }        // Tùy chọn: admin có thể nhập luôn ngày dự kiến giao hàng nếu có thông tin từ shipper
}