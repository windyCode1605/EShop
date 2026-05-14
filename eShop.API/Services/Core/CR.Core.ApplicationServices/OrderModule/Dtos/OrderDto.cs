// ApplicationServices/OrderModule/Dtos/OrderDto.cs
namespace CR.Core.ApplicationServices.OrderModule.Dtos;

public class OrderDto
{
    public int     Id              { get; set; }
    public string  OrderCode       { get; set; } = null!;
    public string  Status          { get; set; } = null!;
    public string  PaymentMethod   { get; set; } = null!;
    public decimal Subtotal        { get; set; }            // Tổng tiền hàng trước khi áp dụng giảm giá và phí vận chuyển
    public decimal DiscountAmount  { get; set; }
    public decimal ShippingFee     { get; set; }
    public decimal TotalAmount     { get; set; }
    public string  ShippingAddress { get; set; } = null!;
    public string? Note            { get; set; }
    public DateTime? CreatedDate   { get; set; }

    public IEnumerable<OrderItemDto> Items     { get; set; } = [];
    public PaymentDto?               Payment   { get; set; }
    public ShipmentDto?              Shipment  { get; set; }
}

public class OrderItemDto
{
    public int     Id               { get; set; }
    public string  ProductName      { get; set; } = null!;
    public string  VariantSKU       { get; set; } = null!;
    public int     Quantity         { get; set; }
    public decimal UnitPrice        { get; set; }
    public decimal LineTotal        { get; set; }
}

public class PaymentDto
{
    public int      Id     { get; set; }
    public string   Method { get; set; } = null!;
    public string   Status { get; set; } = null!;
    public decimal  Amount { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class ShipmentDto
{
    public int     Id               { get; set; }
    public string  ShippingProvider { get; set; } = null!;
    public string? TrackingNumber   { get; set; }
    public string  Status           { get; set; } = null!;
    public DateTime? EstimatedDelivery { get; set; }
}