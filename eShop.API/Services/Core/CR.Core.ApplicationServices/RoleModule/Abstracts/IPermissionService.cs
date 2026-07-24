using CR.Core.Dtos.RoleModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.RoleModule
{
    public interface IPermissionService
    {
        Task<Result<List<PermissionGroupDto>>> GetAllPermissionAsync();
        Task<Result<Dictionary<string, List<PermissionItemDto>>>> GetAllPermissionsGroupedAsync();
    }
}