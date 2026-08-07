using CR.ApplicationBase.Localization;
using CR.Core.API.Extensions;
using CR.Core.API.Models;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.Domain.User;
using CR.Core.Dtos.Auth;
using CR.InfrastructureBase.Exceptions;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CR.Core.API.Controllers;

/// <summary>
/// Xử lý luồng đăng nhập nội bộ (cookie-based) và quản lý mật khẩu:
///   • POST  /authenticate/login       — Đăng nhập lấy JWT + cookie
///   • POST  /api/auth/forgot-password — Quên mật khẩu (gửi OTP)
///   • POST  /api/auth/verify-reset-otp — Xác thực OTP reset mật khẩu
///   • POST  /api/auth/reset-password  — Đặt lại mật khẩu
/// </summary>
[ApiController]
[Route("/")]
public sealed class AccountController : ControllerBase
{
    private readonly IUserAuthenticationService _userAuthenticationService;
    private readonly ITokenService _tokenService;
    private readonly IMapErrorCode _mapErrorCode;
    private readonly LocalizationBase _localization;

    public AccountController(
        IUserAuthenticationService userAuthenticationService,
        ITokenService tokenService,
        IMapErrorCode mapErrorCode,
        LocalizationBase localization)
    {
        _userAuthenticationService = userAuthenticationService;
        _tokenService = tokenService;
        _mapErrorCode = mapErrorCode;
        _localization = localization;
    }

    // LOGIN
    [HttpPost("authenticate/login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> LoginAsync([FromBody] AuthenticateModel input)
    {
        var validationError = ValidateLoginInput(input);
        if (validationError is not null)
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(validationError, 400));

        try
        {
            var user = await _userAuthenticationService.ValidateAppUser(input.UserName!, input.Password!);

            await SignInWithCookieAsync(user);

            var response = BuildAuthResponse(user);
            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Đăng nhập thành công."));
        }
        catch (UserFriendlyException ex)
        {
            var message = _localization.Localize(ex.Message);
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(message, 400));
        }
    }

    // FORGOT / RESET PASSWORD

    [HttpPost("api/auth/forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        => (await _userAuthenticationService.ForgotPasswordAsync(request.Email)).ToActionResult(this, "Email đặt lại mật khẩu đã được gửi.");

    [HttpPost("api/auth/verify-reset-otp")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpRequestDto request)
        => (await _userAuthenticationService.VerifyOtpForResetAsync(request.Email, request.OtpCode)).ToActionResult(this, "Xác thực OTP thành công.");

    [HttpPost("api/auth/reset-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var result = await _userAuthenticationService.ResetPasswordAsync(
            request.Email,
            request.ResetToken,
            request.NewPassword);

        return result.ToActionResult(this, "Đặt lại mật khẩu thành công.");
    }

    // PRIVATE HELPERS

    private string? ValidateLoginInput(AuthenticateModel input)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(input.UserName))
            errors.Add(_localization.Localize("error_validation_AuthorizationUsername"));

        if (string.IsNullOrEmpty(input.Password))
            errors.Add(_localization.Localize("error_validation_AuthorizationPassword"));

        return errors.Count > 0 ? string.Join(", ", errors) : null;
    }

    private async Task SignInWithCookieAsync(Users user)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim("userId", user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        var principal = new ClaimsPrincipal([identity]);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private AuthResponseDto BuildAuthResponse(Users user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            Email = user.Email,
            Role = user.UserType.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(TokenExpiryMinutes)
        };
    }

    /// <summary>
    /// Thời gian hết hạn token mặc định (phút). TokenService đã đọc từ config;
    /// giá trị này chỉ dùng để trả về ExpiresAt trong response.
    /// </summary>
    private const int TokenExpiryMinutes = 60;
}
