// CR.Constants/Shipment/ShipmentStatus.cs
namespace CR.Constants.Shipment;

public static class ShipmentStatus
{
    public const string Pending   = "PENDING";
    public const string PickedUp  = "PICKED_UP";
    public const string InTransit = "IN_TRANSIT";
    public const string Delivered = "DELIVERED";
    public const string Failed    = "FAILED";
    public const string Returned  = "RETURNED";
}