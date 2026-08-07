using CR.ApplicationBase.Localization;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using CR.Core.Dtos.Auth;
using CR.InfrastructureBase;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Mvc;
using CR.Core.API.Extensions;

namespace CR.Core.API.Controllers;

/// <summary>
/// Xử lý luồng đăng ký tài khoản mới (Registration Flow):
///   • POST  /api/auth/register     — Tạo tài khoản, gửi OTP
///   • POST  /api/auth/verify-otp   — Xác thực OTP
///   • POST  /api/auth/set-password — Đặt mật khẩu → kích hoạt tài khoản
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapErrorCode _mapErrorCode;

    public AuthController(
        IUserService userService,
        IMapErrorCode mapErrorCode)
    {
        _userService = userService;
        _mapErrorCode = mapErrorCode;
    }

    // REGISTRATION FLOW

    /// <summary> Bước 1: Tạo tài khoản và gửi OTP xác thực đến email. </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto input)
        => (await _userService.RegisterUser(input)).ToActionResult(this, "Đăng ký thành công. Vui lòng kiểm tra email để xác thực.");

    /// <summary> Bước 2: Xác thực OTP đã gửi đến email. </summary>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyRegisterOtp([FromBody] VerifyRegisterOtpRequestDto input)
        => (await _userService.VerifyRegisterOtp(input.Email, input.OtpCode)).ToActionResult(this, "Xác thực OTP thành công. Vui lòng đặt mật khẩu.");

    /// <summary> Bước 3: Đặt mật khẩu để kích hoạt tài khoản.</summary>
    [HttpPost("set-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordUserDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Password))
            return BadRequest(ApiResponse<string>.Fail("Mật khẩu không được để trống.", ErrorCode.BadRequest));

        return (await _userService.SetPassword(input)).ToActionResult(this, "Đặt mật khẩu thành công. Tài khoản đã được kích hoạt.");
    }
}
