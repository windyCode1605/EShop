using CR.Core.ApplicationServices.PaymentModule.Abstracts;
using CR.Core.Dtos.Payment;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;
using CR.WebAPIBase.Responses;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentController(IPaymentService paymentService)
        => _paymentService = paymentService;
    
    /// <summary>Lấy thông tin thanh toán của đơn hàng.</summary>
    [HttpGet("order/{orderId:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById([FromRoute] int orderId)
        => (await _paymentService.GetOrderById(orderId)).ToActionResult(this);
    /// <summary>Tạo URL thanh toán cho đơn hàng (dùng cho online payment).</summary>
    [HttpPost("order/{orderId:int}/create-url")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePaymentUrl([FromRoute] int orderId)
        => (await _paymentService.CreatePaymentUrl(orderId)).ToActionResult(this);

    /// <summary>Xử lý callback từ payment gateway (VNPay/MoMo webhook).</summary>
    [HttpPost("gateway/callback")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleGatewayCallback([FromBody] GatewayCallbackDto input)
        => (await _paymentService.HandleGatewayCallback(input)).ToActionResult(this);
    
    /// <summary>Admin xác nhận đã nhận được chuyển khoản ngân hàng (dùng cho offline payment).</summary>
    [HttpPost("order/{orderId:int}/confirm-bank-transfer")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmBankTransfer([FromRoute] int orderId)
        => (await _paymentService.ConfirmBankTransfer(orderId)).ToActionResult(this);

    /// <summary>Hoàn tiền cho đơn hàng (có thể dùng cho cả online và offline payment).</summary>
    [HttpPost("order/{orderId:int}/refund")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefundPayment(
        [FromRoute] int orderId,
        [FromQuery] string reason)
        => (await _paymentService.RefundPayment(orderId, reason)).ToActionResult(this);
}