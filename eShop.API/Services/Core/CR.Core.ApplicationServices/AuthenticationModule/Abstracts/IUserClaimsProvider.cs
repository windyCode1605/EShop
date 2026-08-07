using CR.Core.Domain.User;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts;

public record UserClaimSet(
    int UserId,
    string Email,
    string FullName,
    int UserType,
    IReadOnlyList<(int RoleId, string RoleName)> Roles
);

public interface IUserClaimsProvider
{
    UserClaimSet Extract(Users user);
    IEnumerable<(string Type, string Value)> ToRoleClaims(UserClaimSet set);
}