using CR.Core.Dtos.CategoryDto;
using CR.DtoBase;

namespace CR.Core.Application.CategoryModule.Abstract;
public interface ICategoryService
{
    Task<Result<List<CategoryResponseDto>>> GetCategories();
}