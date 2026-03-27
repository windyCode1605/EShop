using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.Product;
public class ProductRequestDto
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải lớn hơn hoặc bằng 0")]
    public int Stock { get; set; }

    public string? Description { get; set; }

    [Required (ErrorMessage = "Danh mục sản phẩm không được để trống")]
    public int CategoryId { get; set; }
}