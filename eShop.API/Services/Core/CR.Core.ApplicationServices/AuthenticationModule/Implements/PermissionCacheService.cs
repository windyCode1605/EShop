using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class PermissionCacheService : IPermissionCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly CoreDbContext _dbContext;
        private readonly ILogger<PermissionCacheService> _logger;

        private const string CacheKeyPrefix = "permissions:role:";

        public PermissionCacheService(IDistributedCache cache, CoreDbContext dbContext, ILogger<PermissionCacheService> logger)
        {
            _cache = cache;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<HashSet<string>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{roleId}";
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache Hit: Permissions for RoleId {RoleId}", roleId);
                var permissions = JsonSerializer.Deserialize<HashSet<string>>(cachedData);
                if (permissions != null)
                {
                    return permissions;
                }
            }

            _logger.LogInformation("Cache Miss: Fetching permissions for RoleId {RoleId} from DB", roleId);

            var permissionsFromDb = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionKey)
                .ToListAsync(cancellationToken);

            var permissionSet = new HashSet<string>(permissionsFromDb, StringComparer.OrdinalIgnoreCase);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Cache for 1 hour
            };

            var serializedData = JsonSerializer.Serialize(permissionSet);
            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

            return permissionSet;
        }

        public async Task InvalidateRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{roleId}";
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogInformation("Invalidated cache for RoleId {RoleId}", roleId);
        }
    }
}
