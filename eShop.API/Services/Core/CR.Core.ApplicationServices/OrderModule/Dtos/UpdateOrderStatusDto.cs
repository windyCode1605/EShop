using System.ComponentModel.DataAnnotations;

namespace CR.Core.ApplicationServices.OrderModule.Dtos;
public class UpdateOrderStatusDto
{
    [Required]
    public required string NewStatus { get; set; }

    public string? TrackingNumber { get; set; }
    public string? ShippingProvider { get; set; }

    public string? Reason { get; set; }
}