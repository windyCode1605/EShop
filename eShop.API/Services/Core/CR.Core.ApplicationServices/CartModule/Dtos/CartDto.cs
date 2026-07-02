// ApplicationServices/CartModule/Dtos/CartDtos.cs
namespace CR.Core.ApplicationServices.CartModule.Dtos;

public class AddToCartDto
{
    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }
}

public class UpdateCartItemDto
{
    public int CartItemId { get; set; }

    public int Quantity { get; set; }
}

public class CartDto
{
    public int Id { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public int TotalItems { get; set; }
    public decimal Subtotal { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string? Size { get; set; }
    public string? Color { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public string? ImageUrl { get; set; }

    /// <summary>
    /// False khi: variant bị xóa, sản phẩm bị xóa, hoặc hết hàng.
    /// Frontend dùng để hiển thị cảnh báo trước khi checkout.
    /// </summary>
    public bool IsAvailable { get; set; }
    public int MaxQuantity { get; set; }  // StockQuantity hiện tại
}


public class CartValidationResultDto
{
    /// <summary>True nếu giỏ hàng hợp lệ, sẵn sàng checkout.</summary>
    public bool IsValid { get; set; }

    /// <summary> Danh sách item có vấn đề </summary>
    public List<CartItemIssueDto> Issues { get; set; } = [];
}

public class CartItemIssueDto
{
    public int CartItemId { get; set; }
    public string ProductName { get; set; } = null!;
    public string SKU { get; set; } = null!;

    /// <summary> "OUT_OF_STOCK" | "INSUFFICIENT_STOCK" | "UNAVAILABLE" </summary>
    public string IssueType { get; set; } = null!;

    /// <summary> Thông báo thân thiện </summary>
    public string Message { get; set; } = null!;
    /// <summary> Số lượng tối đa có thể đặt ( 0 nếu hết hàng ) </summary>
    public int AvailableQuantity { get; set; }
}