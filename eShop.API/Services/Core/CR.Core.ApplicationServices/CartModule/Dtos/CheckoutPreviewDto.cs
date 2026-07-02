using CR.Core.ApplicationServices.CartModule.Dtos;

namespace CR.Core.ApplicationServices.CartModule.Dtos;

public class CheckoutPreviewRequestDto
{
    public string? CouponCode { get; set; }
    /// <summary>Phí ship mặc định nếu chưa tính từ bên thứ 3. Tạm thời hardcode.</summary>
    public decimal ShippingFee { get; set; } = 30_000;
}

public class CheckoutPreviewDto
{
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public string? CouponMessage { get; set; }   // mô tả coupon đang áp dụng
    public decimal Total { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}
