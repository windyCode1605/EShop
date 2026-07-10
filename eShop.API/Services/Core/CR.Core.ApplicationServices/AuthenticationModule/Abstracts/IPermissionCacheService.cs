using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts
{
    public interface IPermissionCacheService
    {
        Task<HashSet<string>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);

        Task InvalidateRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    }
}
