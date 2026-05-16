using CR.Constants.ErrorCodes;
using CR.Constants.Orders;
using CR.Constants.Payment;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.PaymentModule.Abstracts;
using CR.Core.Domain.Orders;
using CR.Core.Dtos.Payment;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.PaymentModule.Implement
{
    public class PaymentService : CoreServiceBase, IPaymentService
    {
        public PaymentService(
            ILogger<PaymentService> logger,
            IHttpContextAccessor httpContext
        ) : base(logger, httpContext) { }

        /// <summary>
        /// Lấy Payments theo Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Result<PaymentDto>> GetOrderById(int orderId)
        {
            _logger.LogInformation("{methad}: OrderId: {orderId}", nameof(GetOrderById), orderId);
            var userId = _httpContext.GetCurrentUserId();

            // Kiểm tra order thuộc về user hiện tại 
            var orderExists = await _dbContext.Orders.AnyAsync(o =>
                o.Id == orderId && o.UserId == userId && !o.Deleted);

            if (!orderExists)
            {
                return Result<PaymentDto>.Failure(
                    ErrorCode.OrderNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            var payment = await _dbContext.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                return Result<PaymentDto>.Failure(
                    ErrorCode.PaymentNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            return Result<PaymentDto>.Success(
                new PaymentDto
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    Method = payment.Method,
                    Status = payment.Status,
                    PaidAt = payment.PaidAt
                }
            );
        }
        public async Task<Result<PaymentUrlDto>> CreatePaymentUrl(int orderId)
        {
            _logger.LogInformation("{method}: OrderId: {orderId}", nameof(CreatePaymentUrl), orderId);
            var userId = _httpContext.GetCurrentUserId();

            var order = await _dbContext.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId && o.UserId == userId && !o.Deleted && !o.Deleted);
            if (order == null)
            {
                return Result<PaymentUrlDto>.Failure(
                    ErrorCode.OrderNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            var payment =  order.Payments.FirstOrDefault();
            if(payment == null )
            {
                return Result<PaymentUrlDto>.Failure(
                    ErrorCode.PaymentNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            payment.Status = PaymentStatus.Pending;
            await _dbContext.SaveChangesAsync();
            var paymentUrl = BuildMockPaymentUrl(order.OrderCode, payment.Amount, payment.Method);
            _logger.LogInformation("{method}: Generated payment URL: {paymentUrl}", nameof(CreatePaymentUrl), paymentUrl);
            return Result<PaymentUrlDto>.Success(
                new PaymentUrlDto
                {
                    PaymentUrl = paymentUrl,
                    OrderCode = order.OrderCode,
                    Amount = payment.Amount,
                    ExpireAt = DateTime.UtcNow.AddMinutes(15) // URL hết hạn sau 15 phút
                }
            );
        }
        public async Task<Result> HandleGatewayCallback(GatewayCallbackDto input)
        {
            _logger.LogInformation("{method}: OrderCode={@OrderCode}, IsSuccess={@IsSuccess}", nameof(HandleGatewayCallback), input.OrderCode, input.IsSuccess);

            var order = await _dbContext.Orders
                .Include(o => o.Payments)
                .Include(o => o.Shipments)
                .FirstOrDefaultAsync(o => o.OrderCode == input.OrderCode && !o.Deleted); 
            if (order == null)
            {
                return Result.Failure(
                    ErrorCode.OrderNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            var payment = order.Payments.FirstOrDefault( p =>
                p.Status == PaymentStatus.Processing);
            if (payment == null)
            {
                return Result.Failure(
                    ErrorCode.PaymentNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if(input.IsSuccess)
                {
                    // ---- Thanh toán thành công ----
                    payment.Status = PaymentStatus.Success;
                    payment.PaidAt = DateTime.UtcNow;
                    order.Status = OrderStatusConst.Confirmed; // Cập nhật trạng thái đơn hàng sang "Đã xác nhận"

                    _logger.LogInformation("Đơn hàng {OrderCode} đã thanh toán thành công ", order.OrderCode);
                }
                else
                {
                    // ---- Thanh toán thất bại ----
                    payment.Status = PaymentStatus.Failed;
                    order.Status = OrderStatusConst.PaymentFailed; // Cập nhật trạng thái đơn hàng sang "Thanh toán thất bại"

                    var orderItems = await _dbContext.OrderItems
                        .Where(oi => oi.OrderId == order.Id)
                        .ToListAsync();
                    // Hoàn lại tồn kho khi thanh toán thất bại
                    foreach (var item in orderItems)
                    {
                        var productVariant = await _dbContext.ProductVariants
                            .FirstOrDefaultAsync(pv => pv.Id == item.ProductVariantId);
                        if (productVariant != null)
                        {
                            productVariant.StockQuantity += item.Quantity; // Hoàn trả lại số lượng đã đặt về kho
                        }
                    }
                    // Hoàn lại coupon đã sử dụng (nếu có)
                    var couponUsage = await _dbContext.CouponUsages
                        .FirstOrDefaultAsync(cu => cu.OrderId == order.Id);
                    if (couponUsage != null)
                    {
                        couponUsage.Coupon.UsedCount = Math.Max(0, couponUsage.Coupon.UsedCount - 1); // Giảm UsedCount của coupon đi 1 (không để âm)
                        _dbContext.CouponUsages.Remove(couponUsage); 
                    }
                    _logger.LogWarning("Đơn hàng {OrderCode} thanh toán thất bại. Code: {Code}", order.OrderCode, input.ResponseCode);

                }
                order.ModifiedDate = DateTimeUtils.GetDate();
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý callback thanh toán cho đơn hàng {OrderCode}", order.OrderCode);
                await transaction.RollbackAsync();
                return Result.Failure(
                    ErrorCode.UnknownError,
                    this.GetCurrentMethodInfo()
                );
            }
        }
        public async Task<Result> ConfirmBankTransfer(int orderId)
        {
            _logger.LogInformation("{method}: OrderId: {orderId}", nameof(ConfirmBankTransfer), orderId);

            var order = _dbContext.Orders
                .Include(o => o.Payments)
                .FirstOrDefault(o =>
                    o.Id == orderId && !o.Deleted);
            if (order == null) 
                return Result.Failure(
                    ErrorCode.OrderNotFound,
                    this.GetCurrentMethodInfo()
                    );
            var payment = order.Payments.FirstOrDefault(p => 
            p.Method == PaymentMethod.BankTransfer && p.Status == PaymentStatus.Pending);
            if (payment == null)
            {
                return Result.Failure(
                    ErrorCode.PaymentNotFound,
                    this.GetCurrentMethodInfo()
                );
            }
            // Cập nhật trạng thái thanh toán
            payment.Status = PaymentStatus.Success;
            payment.PaidAt = DateTimeUtils.GetDate();
            order.Status = OrderStatusConst.Confirmed; // Cập nhật trạng thái đơn hàng sang "Đã xác nhận"
            _logger.LogInformation("Đơn hàng {OrderCode} đã xác nhận chuyển khoản ngân hàng ", order.OrderCode);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public Task<Result> RefundPayment(int orderId, string reason)
        {
            throw new NotImplementedException();
        }
        private static string BuildMockPaymentUrl(string orderCode, decimal amount, string method)
        {
            // TODO: Thay bằng SDK thực tế của VNPay/MoMo
            return $"https://payment.example.com/{method.ToLower()}?order={orderCode}&amount={amount}";
        }
    }
}