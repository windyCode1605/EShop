using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.Domain.User;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements;

public class UserClaimsProvider : IUserClaimsProvider
{
    public UserClaimSet Extract(Users user)
    {
        var roles = user.UserRoles?
            .Where(ur => !ur.Deleted && ur.Role != null)
            .Select(ur => (ur.RoleId, ur.Role!.Name))
            .ToList() ?? new List<(int, string)>();

        return new UserClaimSet(
            UserId: user.Id,
            Email: user.Email,
            FullName: user.Profile?.FullName ?? string.Empty,
            UserType: (int)user.UserType,
            Roles: roles
        );
    }

    /// <summary>
    /// Chuyển đổi Roles trong UserClaimSet thành danh sách (Type, Value) cho claim.
    /// Khi user không có role nào, fallback về UserType.
    /// </summary>
    public IEnumerable<(string Type, string Value)> ToRoleClaims(UserClaimSet set)
    {
        if (set.Roles.Count == 0)
            yield return ("__fallback_role", set.UserType.ToString());
        else
            foreach (var (roleId, name) in set.Roles)
                yield return (roleId.ToString(), name);
    }
}