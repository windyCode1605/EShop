namespace CR.Core.Dtos.Payment;
public class PaymentDto
{
    public int Id               { get; set; }
    public int OrderId          { get; set; }
    public string Method         { get; set; } = null!;
    public string Status         { get; set; } = null!;
    public decimal Amount        { get; set; }
    public DateTime? PaidAt      { get; set; }

}

/// <summary>
/// Payload từ Payment Gateway gửi về (VNPay/MoMo webhook)
/// </summary>
public class GatewayCallbackDto
{
    public string OrderCode     { get; set; } = null!;
    public string TransactionId { get; set; } = null!;
    public string ResponseCode  { get; set; } = null!;
    public bool IsSuccess       { get; set; }
    public decimal Amount        { get; set; }
    public string GatewayName    { get; set; } = null!; // "VNPay" hoặc "MoMo"
    /// <summary> Raw JSON response tử gateway dùng để lưu audit </summary>
    public string RawResponse    { get; set; } = null!;
}
/// <summary> Kết quả tạo payment URL cho online payment </summary>
public class PaymentUrlDto
{
    public string PaymentUrl   { get; set; }  = null!; // URL thanh toán nếu tạo thành công, null nếu thất bại
    public string OrderCode     { get; set; } = null!; // Mã đơn hàng liên quan
    public Decimal Amount        { get; set; } // Số tiền thanh toán
    /// <summary> Thời gian hết hạn thanh toán  </summary>
    public DateTime ExpireAt    { get; set; }
}