using System.Security.Claims;
using System.Web;
using CR.Constants.Core.Users;
using CR.Constants.ErrorCodes;
using CR.Core.API.Helpers;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.IdentityServerBase.Constants;
using CR.IdentityServerBase.Controllers;
using CR.IdentityServerBase.Dto;
using CR.InfrastructureBase;
using CR.InfrastructureBase.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CR.Core.API.Controllers;

/// <summary>
/// Xử lý toàn bộ luồng OAuth2 / OpenIddict:
///   • GET/POST  /connect/authorize  — Authorization endpoint
///   • POST      /connect/token      — Token exchange endpoint
/// </summary>
[ApiController]
[Route("/")]
public sealed class ConnectController : AuthorizationControllerBase
{
    private readonly IUserAuthenticationService _userAuthenticationService;
    private readonly IUserService _userServices;
    private readonly IClaimsIdentityFactory _claimsIdentityFactory;
    private readonly IOpenIddictScopeManager _scopeManager;

    public ConnectController(
        IOpenIddictApplicationManager applicationManager,
        IUserAuthenticationService userAuthenticationService,
        IUserService userServices,
        IClaimsIdentityFactory claimsIdentityFactory,
        IOpenIddictScopeManager scopeManager,
        ILogger<AuthorizationControllerBase> logger)
        : base(logger, applicationManager)
    {
        _userAuthenticationService = userAuthenticationService;
        _userServices = userServices;
        _claimsIdentityFactory = claimsIdentityFactory;
        _scopeManager = scopeManager;
    }

    // AUTHORIZATION ENDPOINT

    [HttpGet("connect/authorize")]
    [HttpPost("connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()!;
        var parameters = OAuthHelpers.ParseOAuthParameters(HttpContext, [Parameters.Prompt]);
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!OAuthHelpers.IsAuthenticated(cookieResult, request))
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = OAuthHelpers.BuildRedirectUrl(HttpContext.Request, parameters)
                },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        var userId = cookieResult.Principal!.FindFirstValue(UserClaimTypes.UserId);
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
            ?? throw new InvalidOperationException("ClientId không tìm thấy.");

        var consentType = await _applicationManager.GetConsentTypeAsync(application);

        if (consentType == ConsentTypes.External)
            return ForbidWithError(Errors.ConsentRequired, "Người dùng không được phép truy cập ứng dụng này.");

        var userConsent = cookieResult.Principal!.FindFirstValue(PromptValues.Consent);

        if (consentType is ConsentTypes.Implicit ||
            (consentType == ConsentTypes.Explicit && userConsent == ConsentValue.Grant))
            return await SignInWithUserIdAsync(userId!, request);

        if (consentType == ConsentTypes.Explicit && userConsent != ConsentValue.Grant)
            return RedirectToConsent(parameters);

        if ((consentType == ConsentTypes.Explicit || consentType == ConsentTypes.Systematic)
            && request.HasPromptValue(PromptValues.None))
            return ForbidWithError(Errors.ConsentRequired, "Cần có sự đồng ý tương tác của người dùng.");

        return ForbidWithError(Errors.ConsentRequired, $"ConsentType \"{consentType}\" không hợp lệ.");
    }

    // TOKEN EXCHANGE ENDPOINT

    [HttpPost("/connect/token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Exchange([FromForm] ConnectTokenDto _)
    {
        var request = HttpContext.GetOpenIddictServerRequest()!;

        try
        {
            if (request.IsAuthorizationCodeGrantType())
                return await HandleAuthorizationCodeAsync(request);

            if (request.IsPasswordGrantType())
                return await HandlePasswordGrantAsync(request, _);

            if (request.IsClientCredentialsGrantType())
                return await HandleClientCredentialsAsync(request);

            if (request.IsRefreshTokenGrantType())
                return await HandleRefreshTokenAsync();
        }
        catch (UserFriendlyException ex)
        {
            return ForbidWithError(Errors.InvalidGrant, ex.Message);
        }
        catch (Exception ex)
        {
            return ForbidWithError(Errors.ServerError, ex.Message);
        }

        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType,
            ErrorDescription = "The specified grant type is not supported."
        });
    }

    // PRIVATE — Grant type handlers

    private async Task<IActionResult> HandleAuthorizationCodeAsync(OpenIddictRequest request)
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var userId = int.Parse(result.Principal!.GetClaim(UserClaimTypes.UserId)!);
        var user = await _userAuthenticationService.FindUserAuthorizationById(userId)
            ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

        var issuer = $"{Request.Scheme}://{Request.Host.Value}";
        var identity = _claimsIdentityFactory.CreateForUser(user, request.GetScopes(), issuer);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandlePasswordGrantAsync(OpenIddictRequest request, ConnectTokenDto dto)
    {
        var user = await _userAuthenticationService.ValidateAppUser(
            request.Username!.ToLower(),
            request.Password!);

        await _userServices.LoginInfor(user.Id);

        var issuer = $"{Request.Scheme}://{Request.Host.Value}";
        var identity = _claimsIdentityFactory.CreateForUser(user, request.GetScopes(), issuer);

        var properties = new AuthenticationProperties();
        properties.SetParameter(AuthParameters.IsTempPin, user.IsTempPin);
        properties.SetParameter(AuthParameters.IsTempPassword, user.IsTempPassword);
        properties.SetParameter(AuthParameters.IsHasPin, !string.IsNullOrEmpty(user.PinCode));

        return SignIn(
            new ClaimsPrincipal(identity),
            properties,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleClientCredentialsAsync(OpenIddictRequest request)
    {
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
            ?? throw new InvalidOperationException("ClientId không tìm thấy.");

        var clientId = await _applicationManager.GetClientIdAsync(application) ?? string.Empty;
        var displayName = await _applicationManager.GetDisplayNameAsync(application) ?? string.Empty;
        var identity = _claimsIdentityFactory.CreateForClient(clientId, displayName);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleRefreshTokenAsync()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var userId = int.Parse(result.Principal!.GetClaim(UserClaimTypes.UserId)!);
        var user = await _userAuthenticationService.FindUserAuthorizationById(userId)
            ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

        if (user.Status != (int)UserStatus.ACTIVE)
            throw new UserFriendlyException(ErrorCode.UserIsDeactive);

        var issuer = $"{Request.Scheme}://{Request.Host.Value}";
        var identity = _claimsIdentityFactory.CreateForUser(user, [], issuer);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    // PRIVATE — Authorize helper actions

    private async Task<IActionResult> SignInWithUserIdAsync(string userId, OpenIddictRequest request)
    {
        var user = await _userAuthenticationService.FindUserAuthorizationById(int.Parse(userId))
            ?? throw new InvalidOperationException("Người dùng không tồn tại.");

        var issuer = $"{Request.Scheme}://{Request.Host.Value}";
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role);

        identity
            .SetClaim(Claims.Subject, userId)
            .SetClaim(UserClaimTypes.UserId, userId);

        identity.SetScopes(request.GetScopes());

        var resources = new List<string>();
        await foreach (var resource in _scopeManager.ListResourcesAsync(identity.GetScopes()))
            resources.Add(resource);

        identity.SetResources(resources);
        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private IActionResult RedirectToConsent(IDictionary<string, StringValues> parameters)
    {
        var returnUrl = HttpUtility.UrlEncode(OAuthHelpers.BuildRedirectUrl(HttpContext.Request, parameters));
        return Redirect($"/authenticate/consent?returnUrl={returnUrl}");
    }

    private IActionResult ForbidWithError(string error, string description)
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description
        });
        return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Xác định đích đến của từng claim: Access Token, Identity Token, hoặc cả hai.
    /// Tách riêng tại đây vì Authorize endpoint vẫn cần scope-based destinations.
    /// </summary>
    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name:
            case Claims.Email:
            case Claims.Role:
            case UserClaimTypes.RoleId:
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
}
