// Services/Core/CR.Core.Dtos/ProductModule/ProductVariantResponseDto.cs
namespace CR.Core.Dtos.Product;

/// <summary>
/// Đại diện cho một Product Variant. Thông tin Size, Color và các thuộc tính khác
/// được quản lý hoàn toàn qua danh sách Attributes (EAV model).
/// </summary>
public class ProductVariantResponseDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;

    /// <summary>Mức điều chỉnh giá so với BasePrice của product.</summary>
    public decimal PriceAdjustment { get; set; }

    public int StockQuantity { get; set; }

    /// <summary>Danh sách thuộc tính động (từ ProductVariantAttribute + Attribute + AttributeValue).</summary>
    public List<VariantAttributeDto> Attributes { get; set; } = new();

    public List<string> ImageUrls { get; set; } = new();
}
