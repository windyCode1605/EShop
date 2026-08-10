using CR.Core.Dto.SysvarModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.SysvarModule.Abstracts
{
    public interface ISysvarService
    {
        Task<Result<List<SysvarResponsDto>>> GetSysvarsAsync();
        Task<Result> UpdateSysVarAsync(int id, SysvarUpdateDto dto);
    }
}