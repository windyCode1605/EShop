using System.Security.Claims;
using CR.Constants.Core.Users;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.Domain.User;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements;

public class ClaimsIdentityFactory : IClaimsIdentityFactory
{
    private readonly IUserClaimsProvider _claimsProvider;

    // Danh sách scopes mà hệ thống hỗ trợ — dùng để intersect với scopes client yêu cầu
    private static readonly string[] SupportedScopes =
    [
        Scopes.OpenId,
        Scopes.Email,
        Scopes.Profile,
        Scopes.Roles,
        Scopes.OfflineAccess,
        "api"
    ];

    public ClaimsIdentityFactory(IUserClaimsProvider claimsProvider)
        => _claimsProvider = claimsProvider;

    public ClaimsIdentity CreateForUser(Users user, IEnumerable<string> requestedScopes, string issuer)
    {
        var set = _claimsProvider.Extract(user);
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

        identity
            .SetClaim(Claims.Username, set.Email)
            .SetClaim(Claims.Email, set.Email)
            .SetClaim(Claims.Subject, set.UserId.ToString())
            .SetClaim(Claims.Issuer, issuer)
            .SetClaim(Claims.Name, set.FullName)
            .SetClaim(UserClaimTypes.UserType, set.UserType.ToString())
            .SetClaim(UserClaimTypes.UserId, set.UserId.ToString());

        foreach (var (roleId, roleName) in set.Roles)
        {
            identity.AddClaim(new Claim(Claims.Role, roleName));
            identity.AddClaim(new Claim(UserClaimTypes.RoleId, roleId.ToString()));
        }

        identity.SetScopes(SupportedScopes.Intersect(requestedScopes));
        identity.SetDestinations(GetDestinations);
        return identity;
    }

    public ClaimsIdentity CreateForClient(string clientId, string displayName)
    {
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role);

        identity.SetClaim(Claims.Subject, clientId);
        identity.SetClaim(Claims.Name, displayName);
        identity.SetDestinations(static claim => claim.Type switch
        {
            Claims.Name when claim.Subject?.HasScope(Scopes.Profile) == true
                => [Destinations.AccessToken, Destinations.IdentityToken],
            _ => [Destinations.AccessToken]
        });
        return identity;
    }

    /// <summary>
    /// Xác định đích đến của các claim: Access Token, Identity Token, hoặc cả hai.
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