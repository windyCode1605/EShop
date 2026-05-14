// Services/Shared/CR.Constants/OrderStatus.cs
namespace CR.Constants;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1, // Đơn đang được xử lý
    Shipped = 2, // Đơn đã được giao cho đơn vị vận chuyển nhưng chưa giao đến khách
    Delivered = 3, // Đơn đã giao thành công
    Cancelled = 4
}