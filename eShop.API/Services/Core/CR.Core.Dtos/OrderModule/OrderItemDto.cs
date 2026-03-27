using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.Order;
public class OrderItemDto
{
    public int ProductId { get; set; }

    [Range(1, 999)]
    public int Quantity { get; set; }
}