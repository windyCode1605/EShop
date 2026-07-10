using CR.Core.ApplicationServices.AuthenticationModule.RoleDtos;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
public interface IRoleService
{
    /// <summary>Lấy danh sách tất cả Role (Admin CRUD)</summary>
    Task<Result<List<RoleDto>>> GetRolesAsync();

    /// <summary>Lấy roles + permissions của user đang đăng nhập (GET /api/me/permissions)</summary>
    Task<Result<UserAuthorizationDto>> GetCurrentUserAuthorizationAsync();

    /// <summary>Lấy danh sách PermissionKey của một Role cụ thể (Admin xem quyền)</summary>
    Task<Result<List<string>>> GetRolePermissionsAsync(int roleId);

    /// <summary>Cập nhật permissions của một Role + invalidate cache (Admin chỉnh quyền)</summary>
    Task<Result<bool>> UpdateRolePermissionsAsync(int roleId, List<string> permissionKeys);

    /// <summary>Lấy tất cả permissions gom nhóm theo PermissionGroup (Admin hiển thị checklist)</summary>
    Task<Result<Dictionary<string, List<PermissionItemDto>>>> GetAllPermissionsGroupedAsync();
}