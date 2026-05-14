using System.Security.Claims;
using CR.ConstantBase.MultiTenancy;
using CR.Constants.Core.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CR.InfrastructureBase
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Lấy tenantId từ HttpContext.
        /// - Admin/SuperAdmin: trả về null (không bị giới hạn tenant)
        /// - User thường đã đăng nhập: lấy từ Claims, bắt buộc phải có
        /// - Anonymous: lấy từ middleware (HttpContext.Items)
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        /// <returns>TenantId (int) hoặc null nếu không áp dụng multi-tenancy</returns>
        /// <exception cref="InvalidOperationException">
        /// Khi user thường đăng nhập nhưng không có TenantId claim hoặc format không hợp lệ
        /// </exception>
        public static int? GetCurrentTenantId(this IHttpContextAccessor httpContextAccessor)
        {
            var logger = httpContextAccessor.HttpContext?.RequestServices?.GetService<ILogger<HttpContextAccessor>>();
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

            if (claims?.IsAuthenticated == true)
            {
                // Admin/SuperAdmin không bị giới hạn tenant
                var userType = httpContextAccessor.GetCurrentUserType();
                if (userType == UserTypeEnum.ADMIN || userType == UserTypeEnum.SUPER_ADMIN)
                {
                    logger?.LogDebug(
                        "{Method}: Admin user detected, returning null tenantId",
                        nameof(GetCurrentTenantId)
                    );
                    return null;
                }

                // User thường bắt buộc phải có tenantId
                var claimTenantId = claims.FindFirst(UserClaimTypes.TenantId);
                if (claimTenantId == null)
                {
                    object? tenantIdInItems = null;
                    httpContextAccessor.HttpContext?.Items.TryGetValue(
                        MultiTenancyQuery.TenantId,
                        out tenantIdInItems
                    );
                    var tenantFromItems = tenantIdInItems as int?;
                    logger?.LogWarning(
                        "{Method}: Missing claim {TenantClaim}, fallback tenant from Items = {TenantId}",
                        nameof(GetCurrentTenantId),
                        UserClaimTypes.TenantId,
                        tenantFromItems
                    );
                    return tenantFromItems;
                }

                if (!int.TryParse(claimTenantId.Value, out var tenantId))
                {
                    logger?.LogWarning(
                        "{Method}: Invalid TenantId format in claim: '{ClaimValue}', fallback to null",
                        nameof(GetCurrentTenantId),
                        claimTenantId.Value
                    );
                    return null;
                }

                logger?.LogDebug(
                    "{Method}: Authenticated user with tenantId = {TenantId}",
                    nameof(GetCurrentTenantId),
                    tenantId
                );
                return tenantId;
            }
            else
            {
                // Anonymous user: lấy từ middleware
                if (httpContextAccessor.HttpContext?.Items == null)
                {
                    logger?.LogWarning("{Method}: HttpContext.Items is null", nameof(GetCurrentTenantId));
                    return null;
                }

                httpContextAccessor.HttpContext.Items.TryGetValue(
                    MultiTenancyQuery.TenantId,
                    out var tenantIdInItems
                );

                var tenantId = tenantIdInItems as int?;
                logger?.LogDebug(
                    "{Method}: Anonymous user with tenantId from middleware = {TenantId}",
                    nameof(GetCurrentTenantId),
                    tenantId
                );
                return tenantId;
            }
        }



        public static UserTypeEnum GetCurrentUserType(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst(UserClaimTypes.UserType);
            if (claim == null)
            {
                throw new InvalidOperationException($"Claim {UserClaimTypes.UserType} not found");
            }
            
            if (!int.TryParse(claim.Value, out var userTypeInt))
            {
                throw new InvalidOperationException($"Invalid UserType format in claim: '{claim.Value}'");
            }
            
            return (UserTypeEnum)userTypeInt;
        }



        public static int GetCurrentUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim =
                (claims?.FindFirst(UserClaimTypes.UserId))
                ?? throw new InvalidOperationException($"Claim {UserClaimTypes.UserId} not found.");
            int userId = int.Parse(claim.Value);
            return userId;
        }

    }
}