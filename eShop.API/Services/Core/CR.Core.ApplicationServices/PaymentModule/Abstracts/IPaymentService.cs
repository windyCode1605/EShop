using CR.Core.Dtos.Payment;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.PaymentModule.Abstracts;
public interface IPaymentService
{
    Task<Result<PaymentDto>>        GetOrderById(int orderId);
    /// <summary>
    /// Tạo URL thanh toán cho đơn hàng (dùng cho online payment)
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    Task<Result<PaymentUrlDto>>     CreatePaymentUrl(int orderId);     
    /// <summary>
    /// Xử lý callback từ payment gateway (VNPay/MoMo webhook)
    /// </summary> <param name="input"></param>
    /// <returns></returns>             
    Task<Result>                    HandleGatewayCallback(GatewayCallbackDto input);// Xử lý callback từ payment gateway (VNPay/MoMo webhook)
    /// <summary>
    /// Admin xác nhận đã nhận được chuyển khoản ngân hàng (dùng cho offline payment)
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    Task<Result>                    ConfirmBankTransfer(int orderId);               // Xác nhận đã nhận được chuyển khoản ngân hàng (dùng cho offline payment)  
    Task<Result>                    RefundPayment(int orderId, string reason);      // Hoàn tiền cho đơn hàng (có thể dùng cho cả online và offline payment)
}