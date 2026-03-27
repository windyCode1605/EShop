using System.ComponentModel.DataAnnotations;
using CR.Core.Domain.Order;
using CR.DtoBase;

namespace CR.Core.Dtos.Order;
public class CreateOrderDto : BaseRequestDto
{
    [Required, MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
    public List<OrderItemDto> Items { get; set; } = new();
    public string? Note { get; set; }
}