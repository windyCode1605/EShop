using CR.Core.Dto.EmployeeDto;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.EmployeeModule.Abstracts
{
    public interface IEmployeeService
    {
        Task<Result<PageResult<EmployeeResponseDto>>> GetEmployeeAsync(EmployeeQueryDto query);
    }
}