using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.Product;

public class ProductRequestDto
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Danh mục sản phẩm không được để trống")]
    public int CategoryId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    public string? Description { get; set; }

    public List<CreateProductVariantDto> Variants { get; set; } = new();
}


public class CreateProductVariantDto
{
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "SKU không được để trống")]
    [MaxLength(100)]
    public string SKU { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "Giá điều chỉnh phải lớn hơn hoặc bằng 0")]
    public decimal PriceAdjustment { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
    public int StockQuantity { get; set; }
}
