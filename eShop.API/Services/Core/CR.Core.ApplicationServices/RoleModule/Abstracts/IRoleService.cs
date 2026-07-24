using CR.Core.Dtos.RoleModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.RoleModule.Abstracts
{
    public interface IRoleService
    {
        Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequest dto);
        Task<Result<RoleDto>> AssignPermissionForRoleAsync(AssignPermissionDto dto);
        Task<Result<List<RoleDto>>> GetRolesAsync();
        Task<Result<List<string>>> GetRolePermissionsAsync(int roleId);
        Task<Result<bool>> UpdateRolePermissionsAsync(int roleId, List<string> permissionKeys);
    }

}