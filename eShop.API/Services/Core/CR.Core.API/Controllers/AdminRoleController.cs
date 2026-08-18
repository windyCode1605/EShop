using CR.Core.ApplicationServices.RoleModule;
using CR.Core.ApplicationServices.RoleModule.Abstracts;
using CR.Core.Dtos.RoleModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.WebAPIBase.Responses;
using CR.Core.API.Extensions;

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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoles()
        => (await _roleService.GetRolesAsync()).ToActionResult(this, "Success");

    /// <summary>
    /// Lấy danh sách Permission của một Role cụ thể.
    /// Admin dùng để xem/chỉnh sửa quyền của Role.
    /// </summary>
    [HttpGet("{roleId:int}/permissions")]
    [Authorize(Policy = "Permission:Identity.Roles.View")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRolePermissions([FromRoute] int roleId)
        => (await _roleService.GetRolePermissionsAsync(roleId)).ToActionResult(this, "Success");

    /// <summary>
    /// Cập nhật toàn bộ danh sách Permission của một Role.
    /// Sau khi cập nhật, cache của Role đó sẽ bị xóa (cache invalidation).
    /// </summary>
    [HttpPut("{roleId:int}/permissions")]
    [Authorize(Policy = "Permission:Identity.Roles.Manage")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRolePermissions(
        [FromRoute] int roleId,
        [FromBody] UpdateRolePermissionsDto input)
        => (await _roleService.UpdateRolePermissionsAsync(roleId, input.PermissionKeys)).ToActionResult(this, "Cập nhật quyền thành công");

    /// <summary>
    /// Lấy toàn bộ danh mục Permission, gom nhóm theo PermissionGroup.
    /// Dùng để hiển thị checklist trên màn hình quản lý quyền của Admin.
    /// </summary>
    [HttpGet("all-permissions")]
    [Authorize(Policy = "Permission:Identity.Roles.View")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPermissions()
        => (await _permissionService.GetAllPermissionsGroupedAsync()).ToActionResult(this, "Success");
}
