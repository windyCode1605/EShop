// Services/Core/CR.Core.Dtos/ProductModule/ProductVariantResponseDto.cs
namespace CR.Core.Dtos.Product;

/// <summary>
/// Đại diện cho một Product Variant (SKU, Size, Color, Stock...).
/// Bao gồm cả dynamic attributes từ bảng ProductVariantAttribute.
/// </summary>
public class ProductVariantResponseDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;

    /// <summary>Size tĩnh từ cột ProductVariant.Size (nếu có).</summary>
    public string? Size { get; set; }

    /// <summary>Color tĩnh từ cột ProductVariant.Color (nếu có).</summary>
    public string? Color { get; set; }

    /// <summary>Mức điều chỉnh giá so với BasePrice của product.</summary>
    public decimal PriceAdjustment { get; set; }

    public int StockQuantity { get; set; }

    /// <summary>Danh sách thuộc tính động (từ ProductVariantAttribute + Attribute + AttributeValue).</summary>
    public List<VariantAttributeDto> Attributes { get; set; } = new();
}
