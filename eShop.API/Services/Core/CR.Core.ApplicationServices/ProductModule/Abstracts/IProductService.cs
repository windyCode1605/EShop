using CR.Common;
using CR.Core.Dtos.Product;

namespace CR.Core.ApplicationServices.ProductModule.Abstracts;
public interface IProductService
{
    Task<PaginatedResult<ProductResponseDto>> GetAllAsync (int Page, int size);
    Task<ProductResponseDto> CreateAsync (ProductRequestDto dto);
}