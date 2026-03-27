using CR.Constants;
using CR.Core.Dtos.Order;
using CR.DtoBase;

namespace CR.Core.Dtos.Product;
public class OrderResponseDto : BaseResponseDto
{
    public int UserId { get; set; }
    public OrderStatus status { get; set; }
    public string StatusDisplay => status.ToString();
    public decimal Total { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();

}