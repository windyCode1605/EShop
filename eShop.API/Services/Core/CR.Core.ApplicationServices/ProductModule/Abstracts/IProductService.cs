using CR.Common;
using CR.Core.Dtos.Product;
using CR.DtoBase;
using CR.ApplicationBase.Common;

namespace CR.Core.ApplicationServices.ProductModule.Abstracts;
public interface IProductService
{
    Task<Result<PageResult<ProductResponseDto>>> GetAllAsync(ProductQueryDto query);
    Task<Result<ProductResponseDto>> CreateAsync(ProductRequestDto dto);
    Task<Result<ProductResponseDto>> UpdateAsync(int id, ProductRequestDto dto);
    Task<Result<ProductResponseDto>> GetByIdAsync(int id);
    /// <summary>Tạo Variant mới cho Product đã tồn tại. <c>dto.ProductId</c> là bắt buộc.</summary>
    Task<Result<ProductVariantResponseDto>> CreateProductVariantAsync(CreateProductVariantDto dto);
    Task<Result<ProductVariantResponseDto>> UpdateProductVariantAsync(int variantId, UpdateProductVariantDto dto);
}