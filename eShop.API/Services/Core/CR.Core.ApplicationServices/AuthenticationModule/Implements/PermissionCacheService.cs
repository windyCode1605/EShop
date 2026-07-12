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
    /// <summary>
    /// Cache-Aside pattern cho Role Permissions.
    ///
    /// Chiến lược Graceful Degradation:
    ///   - Redis available  → Cache-Aside bình thường (Redis → DB → set cache)
    ///   - Redis unavailable → Log warning → query DB trực tiếp (không crash 500)
    ///
    /// Điều này đảm bảo /api/me/permissions luôn trả về kết quả dù Redis chết,
    /// chỉ chậm hơn do phải query DB mỗi lần.
    /// </summary>
    public class PermissionCacheService : IPermissionCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly CoreDbContext _dbContext;
        private readonly ILogger<PermissionCacheService> _logger;

        private const string CacheKeyPrefix = "permissions:role:";

        public PermissionCacheService(
            IDistributedCache cache,
            CoreDbContext dbContext,
            ILogger<PermissionCacheService> logger)
        {
            _cache = cache;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<HashSet<string>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{roleId}";

            // ── Bước 1: Thử đọc từ Cache (Redis hoặc MemoryCache) ──────────────
            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Cache Hit: Permissions for RoleId {RoleId}", roleId);
                    var cached = JsonSerializer.Deserialize<HashSet<string>>(cachedData);
                    if (cached != null)
                        return cached;
                }
            }
            catch (Exception cacheEx)
            {
                // Redis lỗi → log warning và tiếp tục fallback về DB
                // Không throw — đây là Graceful Degradation
                _logger.LogWarning(cacheEx,
                    "Cache unavailable for RoleId {RoleId}. Falling back to DB.", roleId);
            }

            // ── Bước 2: Cache miss hoặc cache lỗi → Query DB ──────────────────
            _logger.LogInformation("Fetching permissions for RoleId {RoleId} from DB", roleId);

            var permissionsFromDb = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionKey)
                .ToListAsync(cancellationToken);

            var permissionSet = new HashSet<string>(permissionsFromDb, StringComparer.OrdinalIgnoreCase);

            // ── Bước 3: Thử ghi lại vào Cache (bỏ qua nếu cache lỗi) ──────────
            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                var serializedData = JsonSerializer.Serialize(permissionSet);
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

                _logger.LogInformation("Cached permissions for RoleId {RoleId}", roleId);
            }
            catch (Exception cacheEx)
            {
                // Ghi cache thất bại → chỉ log, không crash
                // Lần sau sẽ lại query DB (chấp nhận được)
                _logger.LogWarning(cacheEx,
                    "Failed to cache permissions for RoleId {RoleId}. Data returned from DB.", roleId);
            }

            return permissionSet;
        }

        public async Task InvalidateRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeyPrefix}{roleId}";

            try
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
                _logger.LogInformation("Invalidated cache for RoleId {RoleId}", roleId);
            }
            catch (Exception cacheEx)
            {
                // Cache đang lỗi → không cần invalidate (không có gì để xóa)
                _logger.LogWarning(cacheEx,
                    "Failed to invalidate cache for RoleId {RoleId}. Cache may be unavailable.", roleId);
            }
        }
    }
}
