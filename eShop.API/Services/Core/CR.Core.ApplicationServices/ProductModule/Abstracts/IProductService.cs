using CR.Common;
using CR.Core.Dtos.Product;
using CR.DtoBase;
using CR.ApplicationBase.Common;

namespace CR.Core.ApplicationServices.ProductModule.Abstracts;
public interface IProductService
{
    Task<PaginatedResult<ProductResponseDto>> GetAllAsync(int Page, int size);
    Task<ProductResponseDto> CreateAsync(ProductRequestDto dto);
    // Task<ProductResponseDto> UpdateAsync(ProductRequestDto dto);
    Task<Result<ProductResponseDto>> GetByIdAsync(int id);
}