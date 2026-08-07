using System.Security.Claims;
using CR.Core.Domain.User;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
public interface IClaimsIdentityFactory
{
    ClaimsIdentity CreateForUser(Users user, IEnumerable<string> requestedScopes, string issuer);
    ClaimsIdentity CreateForClient(string clientId, string displayName);
}