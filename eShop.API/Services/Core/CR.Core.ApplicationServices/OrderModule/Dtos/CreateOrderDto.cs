using System.ComponentModel.DataAnnotations;

namespace CR.Core.ApplicationServices.OrderModule.Dtos;
public class CreateOrderDto
{
    /// <summary> Id địa chỉ đã lưu , hoặc null nếu nhập địa chỉ mới </summary>
    public int? AddressId { get; set; }

    // Nếu không chọn AddressID thì nhập thủ công 
    [Required]
    public string? ReceiverName { get; set; }
    [Required]
    public string? ReceiverPhone { get; set; }
    [Required]
    public string? Street { get; set; }
    [Required]
    public string? City { get; set; }
    [Required]
    public string? Province { get; set; }


    [Required]
    public string PaymentMethod { get; set; } = null!;

    public string? ShippingProvider { get; set; } = "GHN";
    public string? CouponCode { get; set; }
    public string? Note { get; set; }
}