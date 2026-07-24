using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CR.WebAPIBase.Responses;
using CR.Core.API.Extensions; // <--- Import extension method mới

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
    private readonly IUserService _userService;

    public MeController(IUserService userService)
    {
        _userService = userService;
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
        var result = await _userService.GetCurrentUserAuthorizationAsync();
        
        // Tự động map ErrorCode -> Message nếu có lỗi (Dùng Extension)
        return result.ToActionResult(this, "Lấy quyền thành công");
    }
}
