// Services/Core/CR.Core.Dtos/ProductModule/VariantAttributeDto.cs
namespace CR.Core.Dtos.Product;

/// <summary>
/// Đại diện cho một thuộc tính động gắn với variant (Color: Red, Size: XL, Material: Cotton...).
/// </summary>
public class VariantAttributeDto
{
    public int AttributeId { get; set; }
    public string AttributeName { get; set; } = string.Empty;

    /// <summary>Text, Number, Color, Boolean...</summary>
    public string AttributeType { get; set; } = string.Empty;

    public int? AttributeValueId { get; set; }

    /// <summary>Giá trị từ bảng AttributeValue (nếu có).</summary>
    public string? AttributeValue { get; set; }

    /// <summary>Giá trị tùy chỉnh (nếu không dùng AttributeValue cố định).</summary>
    public string? CustomValue { get; set; }

    /// <summary>Trả về giá trị hiển thị: ưu tiên AttributeValue, fallback CustomValue.</summary>
    public string DisplayValue => AttributeValue ?? CustomValue ?? string.Empty;
}
