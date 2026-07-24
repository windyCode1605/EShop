using CR.Core.ApplicationServices.RoleModule;
using CR.Core.ApplicationServices.RoleModule.Abstracts;
using CR.Core.Dtos.RoleModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.WebAPIBase.Responses;

namespace CR.Core.API.Controllers;

/// <summary>
/// API Admin quản lý Role và Permission.
/// Route: /api/admin/roles
/// Yêu cầu quyền: Roles.View / Roles.Manage
/// </summary>
[ApiController]
[Route("api/admin/roles")]
public class AdminRoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;

    public AdminRoleController(IRoleService roleService, IPermissionService permissionService)
    {
        _roleService = roleService;
        _permissionService = permissionService;
    }

    /// <summary>
    /// Lấy danh sách tất cả Role trong hệ thống.
    /// Dùng để hiển thị dropdown khi gán Role cho User.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:Identity.Roles.View")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _roleService.GetRolesAsync();
        if (result.IsFailure)
            return BadRequest(ApiResponse<List<RoleDto>>.Fail("Lấy danh sách vai trò thất bại", result.ErrorCode));

        return Ok(ApiResponse<List<RoleDto>>.Ok(result.Value, "Success"));
    }

    /// <summary>
    /// Lấy danh sách Permission của một Role cụ thể.
    /// Admin dùng để xem/chỉnh sửa quyền của Role.
    /// </summary>
    [HttpGet("{roleId:int}/permissions")]
    [Authorize(Policy = "Permission:Identity.Roles.View")]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int roleId)
    {
        var result = await _roleService.GetRolePermissionsAsync(roleId);
        if (result.IsFailure)
            return BadRequest(ApiResponse<List<string>>.Fail("Lấy quyền của vai trò thất bại", result.ErrorCode));

        return Ok(ApiResponse<List<string>>.Ok(result.Value, "Success"));
    }

    /// <summary>
    /// Cập nhật toàn bộ danh sách Permission của một Role.
    /// Sau khi cập nhật, cache của Role đó sẽ bị xóa (cache invalidation).
    /// </summary>
    [HttpPut("{roleId:int}/permissions")]
    [Authorize(Policy = "Permission:Identity.Roles.Manage")]
    public async Task<IActionResult> UpdateRolePermissions(
        [FromRoute] int roleId,
        [FromBody] UpdateRolePermissionsDto input)
    {
        var result = await _roleService.UpdateRolePermissionsAsync(roleId, input.PermissionKeys);
        if (result.IsFailure)
            return BadRequest(ApiResponse<bool>.Fail("Cập nhật quyền thất bại", result.ErrorCode));

        return Ok(ApiResponse<bool>.Ok(true, "Cập nhật quyền thành công"));
    }

    /// <summary>
    /// Lấy toàn bộ danh mục Permission, gom nhóm theo PermissionGroup.
    /// Dùng để hiển thị checklist trên màn hình quản lý quyền của Admin.
    /// </summary>
    [HttpGet("all-permissions")]
    [Authorize(Policy = "Permission:Identity.Roles.View")]
    public async Task<IActionResult> GetAllPermissions()
    {
        var result = await _permissionService.GetAllPermissionsGroupedAsync();
        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.Fail("Lấy danh sách quyền thất bại", result.ErrorCode));

        return Ok(ApiResponse<Dictionary<string, List<PermissionItemDto>>>.Ok(result.Value, "Success"));
    }
}
