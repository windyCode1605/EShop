using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CR.ApplicationBase.Localization;
using CR.Constants.Core.Users;
using CR.Core.API.Models;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using CR.Core.Domain.User;
using CR.Core.Dtos.Auth;
using CR.InfrastructureBase.Exceptions;
using CR.InfrastructureBase;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Web;
using OpenIddict.Server.AspNetCore;
using CR.Constants.ErrorCodes;
using CR.IdentityServerBase.Controllers;
using CR.IdentityServerBase.Dto;
using CR.IdentityServerBase.Constants;
using Microsoft.AspNetCore;

namespace CR.Core.API.Controllers;

[ApiController]
[Route("/")]
public class AuthorizationController : AuthorizationControllerBase
{
    private readonly IUserService _userServices;
    private readonly IUserAuthenticationService _userAuthorizationService;
    private readonly IMapErrorCode _mapErrorCode;
    private readonly LocalizationBase _localization;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ITokenService _tokenService;
    private readonly INotificationTokenService _authTokenService; // Nếu anh chưa tạo service này, có thể comment lại

    public AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IUserService userServices,
        IUserAuthenticationService userAuthorizationService,
        ILogger<AuthorizationControllerBase> logger,
        IMapErrorCode mapErrorCode,
        IOpenIddictScopeManager scopeManager,
        LocalizationBase localization,
        IHttpContextAccessor httpContext,
        ITokenService tokenService,
        INotificationTokenService authTokenService
    )
        : base(logger, applicationManager)
    {
        _userServices = userServices;
        _userAuthorizationService = userAuthorizationService;
        _mapErrorCode = mapErrorCode;
        _localization = localization;
        _scopeManager = scopeManager;
        _httpContext = httpContext;
        _tokenService = tokenService;
        _authTokenService = authTokenService;
    }

    // ==========================================
    // 1. API ĐĂNG KÝ TÀI KHOẢN MỚI
    // ==========================================
    [HttpPost("api/auth/register")] // Sửa lại route cho chuẩn RESTful
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] UserRegisterDto input)
    {
        var result = await _userServices.RegisterUser(input);
        if (result.IsFailure)
        {
            // Trả về lỗi có localization
            return BadRequest(ApiResponse<UserDto>.Fail($"Register failed: {result.ErrorCode}", ErrorCode.BadRequest));
        }

        return Ok(ApiResponse<UserDto>.Ok(result.Value, "Register successful"));
    }

    [HttpPost("api/auth/verify-otp")]
    public async Task<ActionResult<ApiResponse<string>>> VerifyRegisterOtp([FromQuery] string email, [FromQuery] string otpCode)
    {
        var result = await _userServices.VerifyRegisterOtp(email, otpCode);
        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<string>.Fail($"Verify OTP failed: {result.ErrorCode}", ErrorCode.BadRequest));
        }

        return Ok(ApiResponse<string>.Ok("OK", "Verify OTP successful. Please set your password."));
    }

    [HttpPost("api/auth/set-password")]
    public async Task<ActionResult<ApiResponse<string>>> SetPassword([FromBody] SetPasswordUserDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Password))
        {
            return BadRequest(ApiResponse<string>.Fail("Password is required", ErrorCode.BadRequest));
        }

        var result = await _userServices.SetPassword(input);
        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<string>.Fail($"Set password failed: {result.ErrorCode}", ErrorCode.BadRequest));
        }

        return Ok(ApiResponse<string>.Ok("OK", "Password set successfully. Account is now active."));
    }

    // ==========================================
    // 2. TRẠM KIỂM SOÁT OAUTH2 / OPENID DICT
    // ==========================================
    [HttpGet("connect/authorize")]
    [HttpPost("connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()!;
        var parameters = ParseOAuthParameters(HttpContext, [Parameters.Prompt]);
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!IsAuthenticated(result, request))
        {
            return Challenge(
                properties: new AuthenticationProperties
                {
                    RedirectUri = BuildRedirectUrl(HttpContext.Request, parameters)
                },
                CookieAuthenticationDefaults.AuthenticationScheme
            );
        }

        var userId = result.Principal!.FindFirstValue(UserClaimTypes.UserId);

        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role
        );

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
            ?? throw new InvalidOperationException("Không tìm thấy ClientId");

        var consentType = await _applicationManager.GetConsentTypeAsync(application);
        switch (consentType)
        {
            case ConsentTypes.External:
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(
                        new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Người dùng không được phép truy cập ứng dụng này."
                        }
                    )
                );

            case ConsentTypes.Implicit:
            case ConsentTypes.Explicit when result.Principal!.FindFirstValue(PromptValues.Consent) == ConsentValue.Grant:

                identity
                .SetClaim(Claims.Subject, userId)
                .SetClaim(UserClaimTypes.UserId, userId);

                identity.SetScopes(request.GetScopes());

                var resources = new List<string>();
                await foreach (var resource in _scopeManager.ListResourcesAsync(identity.GetScopes()))
                {
                    resources.Add(resource);
                }
                identity.SetResources(resources);
                identity.SetDestinations(GetDestinations);

                return SignIn(
                    new ClaimsPrincipal(identity),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );

            case ConsentTypes.Explicit
                when result.Principal!.FindFirstValue(PromptValues.Consent) != ConsentValue.Grant:

                var returnUrl = HttpUtility.UrlEncode(BuildRedirectUrl(HttpContext.Request, parameters));
                var consentRedirectUrl = $"/authenticate/consent?returnUrl={returnUrl}";
                return Redirect(consentRedirectUrl);

            case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
            case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(
                        new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Cần có sự đồng ý tương tác của người dùng."
                        }
                    )
                );

            default:
                return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(
                            new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = $"ConsentType: \"{consentType}\" is không hợp lệ."
                            }
                        )
                    );
        }
    }

    // ==========================================
    // 3. QUẦY LỄ TÂN: XỬ LÝ ĐĂNG NHẬP NỘI BỘ
    // ==========================================
    [HttpPost("authenticate/login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> AuthenticateAsync([FromBody] AuthenticateModel input)
    {
        try
        {
            // Sửa input.UserName thành input.Username hoặc input.Email tùy cấu hình class AuthenticateModel
            if (string.IsNullOrEmpty(input.UserName) || string.IsNullOrEmpty(input.Password))
            {
                var errors = new List<string>();
                if (string.IsNullOrEmpty(input.UserName))
                    errors.Add(_localization.Localize("error_validation_AuthorizationUsername"));

                if (string.IsNullOrEmpty(input.Password))
                    errors.Add(_localization.Localize("error_validation_AuthorizationPassword"));

                return BadRequest(ApiResponse<AuthResponseDto>.Fail(string.Join(", ", errors), 400));
            }

            var user = await _userAuthorizationService.ValidateAppUser(input.UserName, input.Password);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Đã cập nhật SetClaims để dùng Entity mới
            SetClaims(identity, user);

            var principal = new ClaimsPrincipal([identity]);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            if (!string.IsNullOrEmpty(input.ReturnUrl))
            {
                return Redirect(input.ReturnUrl);
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var expiresInMinutes = int.Parse(HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["Jwt:ExpiresInMinutes"] ?? "60");

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                Email = user.Email, // Đã sửa từ user.UserName thành user.Email
                Role = user.UserType.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes)
            };

            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful"));
        }
        catch (UserFriendlyException ex)
        {
            var errorMessage = _localization.Localize(ex.Message);
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(errorMessage, 400));
        }
    }

    // ==========================================
    // 4. EXCHANGE ENDPOINT: ĐỔI MÃ LẤY TOKEN
    // ==========================================
    [
        HttpPost("/connect/token"),
        IgnoreAntiforgeryToken,
        Produces("application/json"),
        Consumes("application/x-www-form-urlencoded")
    ]
    public async Task<IActionResult> Exchange([FromForm] ConnectTokenDto _)
    {
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role
        );
        var request = HttpContext.GetOpenIddictServerRequest()!;
        try
        {
            if (request.IsAuthorizationCodeGrantType())
            {
                var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                int userId = int.Parse(result.Principal!.GetClaim(UserClaimTypes.UserId)!);
                
                // Ở đây cần Includes(u => u.Profile) bên trong hàm FindUserAuthorizationById để SetClaims không bị null FullName
                var user = await _userAuthorizationService.FindUserAuthorizationById(userId)
                    ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
                    
                SetClaims(identity, user);
                
                identity.SetScopes(
                    new[]
                    {
                        Scopes.OpenId,
                        Scopes.Email,
                        Scopes.Profile,
                        Scopes.OfflineAccess
                    }.Intersect(request.GetScopes())
                );
                identity.SetDestinations(GetDestinations);
                return SignIn(
                    new ClaimsPrincipal(identity),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );
            }
            else if (request.IsPasswordGrantType())
            {
                var user = await _userAuthorizationService.ValidateAppUser(
                    request.Username!.ToLower(),
                    request.Password!
                );
                
                // Cập nhật Last Login
                await _userServices.LoginInfor(user.Id);
                
                // _authTokenService.AddNotificationToken(user.Id, _.FcmToken, _.ApnsToken);
                
                SetClaims(identity, user);
                
                identity.SetScopes(
                    new[]
                    {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                    }.Intersect(request.GetScopes())
                );
                identity.SetDestinations(GetDestinations);
                
                var authenticationProperties = new AuthenticationProperties();
                authenticationProperties.SetParameter(AuthParameters.IsTempPin, user.IsTempPin);
                authenticationProperties.SetParameter(AuthParameters.IsTempPassword, user.IsTempPassword);
                authenticationProperties.SetParameter(AuthParameters.IsHasPin, !string.IsNullOrEmpty(user.PinCode));
                
                return SignIn(
                    new ClaimsPrincipal(identity),
                    authenticationProperties,
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );
            }
            else if (request.IsClientCredentialsGrantType())
            {
                var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
                    ?? throw new InvalidOperationException("Không tìm thấy ClientId");
                    
                identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application)); 
                identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));
                identity.SetDestinations(static claim => 
                    claim.Type switch 
                    {
                        Claims.Name when claim.Subject?.HasScope(Scopes.Profile) == true 
                            => new[] { Destinations.AccessToken, Destinations.IdentityToken },
                        _ => new[] { Destinations.AccessToken }
                    }
                );
            }
            else if (request.IsRefreshTokenGrantType())
            {
                var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                var user = await _userAuthorizationService.FindUserAuthorizationById(
                    int.Parse(result.Principal!.GetClaim(UserClaimTypes.UserId)!)
                ) ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
                
                if(user.Status != (int)UserStatus.ACTIVE)
                {
                    throw new UserFriendlyException(ErrorCode.UserIsDeactive);
                }
                SetClaims(identity, user);
                identity.SetDestinations(GetDestinations);
                return SignIn(
                    new ClaimsPrincipal(identity),
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                );
            }
        }
        catch (UserFriendlyException ex)
        {
            var properties = new AuthenticationProperties(
                new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = 
                    _localization.Localize(_mapErrorCode.GetErrorMessageKey(ex.ErrorCode), ex.ListParam)
                }
            );
            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        catch (Exception ex)
        {
            var properties = new AuthenticationProperties(
                new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ServerError,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = ex.Message
                }
            );
            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        return BadRequest(
            new OpenIddictResponse
            {
                Error = Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            }
        );
    }

    // ==========================================
    // CÁC HÀM TIỆN ÍCH (HELPER METHODS)
    // ==========================================

    // <summary>
    // Xác định đích đến của các claim để quyết định chúng sẽ được bao gồm trong Access Token,
    //  Identity Token, hoặc cả hai
    // </summary>
    // <param name="claim">Claim cần xác định đích đến</param>
    // <returns>Danh sách các đích đến của claim</returns>
    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name:
            case Claims.Email:
            case Claims.Role:
                yield return Destinations.AccessToken;
                if (claim.Subject?.HasScope(Scopes.Profile) == true ||
                    claim.Subject?.HasScope(Scopes.Email) == true ||
                    claim.Subject?.HasScope(Scopes.Roles) == true)
                {
                    yield return Destinations.IdentityToken;
                }
                yield break;

            case "AspNet.Identity.SecurityStamp":
                yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }

    /// <summary>
    /// Đóng mộc thông tin (Claims) từ DB vào Thẻ định danh
    /// </summary>
    private void SetClaims(ClaimsIdentity identity, Users user)
    {
        // Sử dụng Entity chuẩn: Email và lấy FullName từ Profile
        identity
            .SetClaim(Claims.Username, user.Email) // Cập nhật sang Email
            .SetClaim(Claims.Email, user.Email)
            .SetClaim(Claims.Subject, $"{user.Id}")
            .SetClaim(Claims.Issuer, $"{Request.Scheme}://{Request.Host.Value}") 
            .SetClaim(Claims.Name, user.Profile?.FullName ?? string.Empty) // Lấy từ Profile
            .SetClaim(UserClaimTypes.UserType, (int)user.UserType)
            .SetClaim(UserClaimTypes.UserId, user.Id);

        // ĐÃ XÓA: Toàn bộ logic kiểm tra và gán TenantId ở đây vì kiến trúc là Single-Tenant
    }

    // <summary> 
    /// Phân tích các tham số OAuth từ HttpContext, loại trừ những tham số không cần thiết
    /// </summary>
    static IDictionary<string, StringValues> ParseOAuthParameters(HttpContext httpContext, List<string>? excluding = null)
    {
        excluding ??= new List<string>();

        var parameters = httpContext.Request.HasFormContentType
            ? httpContext.Request.Form.Where(v => !excluding.Contains(v.Key)).ToDictionary(v => v.Key, v => v.Value)
            : httpContext.Request.Query.Where(v => !excluding.Contains(v.Key)).ToDictionary(v => v.Key, v => v.Value);
        return parameters;
    }

    /// <summary>
    /// Xây dựng URL chuyển hướng sau khi xác thực thất bại, 
    /// bao gồm các tham số OAuth cần thiết để tiếp tục luồng xác thực
    /// </summary>
    static string BuildRedirectUrl(HttpRequest request, IDictionary<string, StringValues> oAuthParameters)
    {
        var url = request.PathBase + request.Path + QueryString.Create(oAuthParameters);
        return url;
    }

    /// <summary>
    /// Kiểm tra xem người dùng đã được xác thực hay chưa,
    ///  và nếu có tham số MaxAge, kiểm tra xem phiên xác thực có còn hợp lệ hay không
    /// </summary>
    /// <param name="authenticateResult"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    static bool IsAuthenticated(AuthenticateResult authenticateResult, OpenIddictRequest request)
    {
        if (!authenticateResult.Succeeded) return false;

        if (request.MaxAge.HasValue && authenticateResult.Properties != null)
        {
            var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);
            var expired = !authenticateResult.Properties.IssuedUtc.HasValue
                || DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc > maxAgeSeconds;

            if (expired) return false;
        }
        return true;
    }
}