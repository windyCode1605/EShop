using CR.Core.Dtos.AttributeModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.AttributeModule.Abstract
{
    public interface IAttributeValueService
    {
        Task<Result<PageResult<AttributeValueResponseDto>>> GetValuesByAttributeIdAsync(FilterAttributeValuePagingDto input);
        Task<Result<AttributeValueResponseDto>> CreatAsync(AttributeValueRequestDto input);
    }
}