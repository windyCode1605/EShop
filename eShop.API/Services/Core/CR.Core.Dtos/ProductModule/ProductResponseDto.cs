// Services/Core/CR.Core.Dtos/Product/ProductResponseDto.cs
using CR.DtoBase;

namespace CR.Core.Dtos.Product;

public class ProductResponseDto : BaseResponseDto
{
    public required int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
    public string? AI_Description { get; set; }
    public bool AI_Generated { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Danh sách variants của sản phẩm (Size, Color, Stock riêng từng loại).</summary>
    public List<ProductVariantResponseDto> Variants { get; set; } = [];
}