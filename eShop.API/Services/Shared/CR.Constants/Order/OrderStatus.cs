// Services/Shared/CR.Constants/OrderStatus.cs
namespace CR.Constants;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}