using CR.Core.ApplicationServices.PaymentModule.Abstracts;
using CR.Core.Dtos.Payment;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentController(IPaymentService paymentService)
        => _paymentService = paymentService;
    
    /// <summary>Lấy thông tin thanh toán của đơn hàng.</summary>
    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
        => Ok(await _paymentService.GetOrderById(orderId));
    /// <summary>Tạo URL thanh toán cho đơn hàng (dùng cho online payment).</summary>
    [HttpPost("order/{orderId:int}/create-url")]
    public async Task<IActionResult> CreatePaymentUrl([FromRoute] int orderId)
        => Ok(await _paymentService.CreatePaymentUrl(orderId));

    /// <summary>Xử lý callback từ payment gateway (VNPay/MoMo webhook).</summary>
    [HttpPost("gateway/callback")]
    public async Task<IActionResult> HandleGatewayCallback([FromBody] GatewayCallbackDto input)
        => Ok(await _paymentService.HandleGatewayCallback(input));
    
    /// <summary>Admin xác nhận đã nhận được chuyển khoản ngân hàng (dùng cho offline payment).</summary>
    [HttpPost("order/{orderId:int}/confirm-bank-transfer")]
    public async Task<IActionResult> ConfirmBankTransfer([FromRoute] int orderId)
        => Ok(await _paymentService.ConfirmBankTransfer(orderId));

    /// <summary>Hoàn tiền cho đơn hàng (có thể dùng cho cả online và offline payment).</summary>
    [HttpPost("order/{orderId:int}/refund")]
    public async Task<IActionResult> RefundPayment(
        [FromRoute] int orderId,
        [FromQuery] string reason)
        => Ok(await _paymentService.RefundPayment(orderId, reason));
}