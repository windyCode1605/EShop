// CR.Constants/Orders/OrderStatusConst.cs
namespace CR.Constants.Orders;

public static class OrderStatusConst
{
    public const string Pending       = "PENDING";
    public const string Confirmed     = "CONFIRMED";
    public const string Processing    = "PROCESSING";
    public const string Shipping      = "SHIPPING";
    public const string Delivered     = "DELIVERED";
    public const string Cancelled     = "CANCELLED";
    public const string PaymentFailed = "PAYMENT_FAILED";
    public const string Returned      = "RETURNED";

    // Trạng thái cho phép hủy đơn
    public static readonly string[] CancellableStatuses =
        [Pending, Confirmed];

    // Trạng thái cuối (không thể thay đổi)
    public static readonly string[] TerminalStatuses =
        [Delivered, Cancelled, Returned];
}