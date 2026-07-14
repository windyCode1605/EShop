using CR.Core.Dtos.AttributeModule;
using CR.DtoBase;
using CR.Common;

namespace CR.Core.ApplicationServices.AttributeModule.Abstract;
public interface IAttributeService
{
    Task<Result<AttributeResponseDto>> CreateAsync(AttributeRequest dto);
    Task<Result<AttributeResponseDto>> GetByIdAsync(int id);
    Task<PaginatedResult<AttributeResponseDto>> GetAllAsync(int page, int size);
    Task<Result<AttributeResponseDto>> UpdateAsync(int id, AttributeRequest dto);
    Task<Result> DeleteAsync(int id);
}