using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.RoleDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.WebAPIBase.Responses;

namespace CR.Core.API.Controllers;

/// <summary>
/// API cho user hiện tại truy vấn thông tin của chính mình.
/// Route: /api/me
/// </summary>
[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly IRoleService _roleService;

    public MeController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Lấy toàn bộ vai trò và quyền của user đang đăng nhập.
    /// Frontend dùng response này để:
    ///   1. Lưu vào state/store
    ///   2. Guard route Angular
    ///   3. Ẩn/hiện nút bấm theo quyền
    /// </summary>
    /// <returns>{ roles: [...], permissions: [...] }</returns>
    [HttpGet("permissions")]
    public async Task<IActionResult> GetMyPermissions()
    {
        var result = await _roleService.GetCurrentUserAuthorizationAsync();
        if (result.IsFailure)
            return BadRequest(ApiResponse<UserAuthorizationDto>.Fail("Lấy quyền thất bại", result.ErrorCode));

        return Ok(ApiResponse<UserAuthorizationDto>.Ok(result.Value, "Success"));
    }
}
