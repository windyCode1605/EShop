using System.Text.RegularExpressions;
using CR.ApplicationBase;
using CR.Core.ApplicationServices.Common;
using CR.Core.Dtos.RoleModule;
using CR.DtoBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.RoleModule.Implement
{
    public class PermissionService : ServiceBase<CoreDbContext>, IPermissionService
    {
        public PermissionService(ILogger<PermissionService> logger, IHttpContextAccessor httpContext)
        : base(logger, httpContext) { }

        public async Task<Result<List<PermissionGroupDto>>> GetAllPermissionAsync()
        {
            _logger.LogInformation("Method {method} called", nameof(GetAllPermissionAsync));
            var PermissionKeyList = await _dbContext.Permissions
                .AsNoTracking()
                .OrderBy(p => p.PermissionGroup)
                .ThenBy(p => p.PermissionKey)
                .Select(p => new
                {
                    p.PermissionKey,
                    p.PermissionGroup,
                    p.DisplayName,
                    p.Description,
                }).ToListAsync();
            var result = PermissionKeyList.GroupBy(p => p.PermissionGroup)
            .Select(g => new PermissionGroupDto
            {
                PermissionGroupName = g.Key,
                permissionKeyDtos = g.Select(x => new PermissionKeyDto
                {
                    PermissionKey = x.PermissionKey,
                    DisplayName = x.DisplayName,
                    Description = x.Description
                }).ToList()
            }).ToList();
            return Result<List<PermissionGroupDto>>.Success(result);
        }

        public async Task<Result<Dictionary<string, List<PermissionItemDto>>>> GetAllPermissionsGroupedAsync()
        {
            _logger.LogInformation("{Method} called", nameof(GetAllPermissionsGroupedAsync));

            var permissions = await _dbContext.Permissions
                .AsNoTracking()
                .OrderBy(p => p.PermissionGroup)
                .ThenBy(p => p.PermissionKey)
                .Select(p => new PermissionItemDto
                {
                    PermissionKey = p.PermissionKey,
                    DisplayName = p.DisplayName,
                    Description = p.Description
                })
                .ToListAsync();

            var grouped = permissions
                .GroupBy(p => p.PermissionKey.Split('.')[0]) // Group by module prefix: Products, Orders...
                .ToDictionary(g => g.Key, g => g.ToList());

            return Result<Dictionary<string, List<PermissionItemDto>>>.Success(grouped);
        }
    }
}