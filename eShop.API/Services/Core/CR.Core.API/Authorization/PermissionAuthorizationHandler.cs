using System.Linq;
using System.Threading.Tasks;
using CR.Constants.Core.Users;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CR.Core.API.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionCacheService _permissionCacheService;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(IPermissionCacheService permissionCacheService, ILogger<PermissionAuthorizationHandler> logger)
        {
            _permissionCacheService = permissionCacheService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity!.IsAuthenticated)
            {
                return;
            }

            var roleIds = context.User.Claims
                .Where(c => c.Type == UserClaimTypes.RoleId)
                .Select(c => int.TryParse(c.Value, out var val) ? val : (int?)null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            if (!roleIds.Any())
            {
                _logger.LogWarning("User {UserId} has no RoleId claims.", context.User.FindFirst(UserClaimTypes.UserId)?.Value);
                return;
            }

            foreach (var roleId in roleIds)
            {
                var permissions = await _permissionCacheService.GetRolePermissionsAsync(roleId);
                if (permissions.Contains(requirement.PermissionKey))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            _logger.LogWarning("User {UserId} does not have required permission {PermissionKey}.", context.User.FindFirst(UserClaimTypes.UserId)?.Value, requirement.PermissionKey);
        }
    }
}
