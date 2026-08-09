using CR.ApplicationBase;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.RoleModule.Abstracts;
using CR.Core.Domain.User;
using CR.Core.Dtos.RoleModule;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.RoleModule.Implement
{
    public class RoleService : ServiceBase<CoreDbContext>, IRoleService
    {
        private readonly IPermissionCacheService _permissionCacheService;

        public RoleService(ILogger<RoleService> logger, IHttpContextAccessor httpContext, IPermissionCacheService permissionCacheService)
            : base(logger, httpContext)
        {
            _permissionCacheService = permissionCacheService;
        }
        public async Task<Result<RoleDto>> AssignPermissionForRoleAsync(AssignPermissionDto dto)
        {
            _logger.LogInformation("Method {method} called", nameof(AssignPermissionForRoleAsync));
            var userId = _httpContext.GetCurrentUserId();

            var role = await _dbContext.Roles.FirstOrDefaultAsync(p => p.Id == dto.RoleId);
            if (role == null)
                return Result<RoleDto>.Failure(ErrorCode.RoleNotFound, this.GetCurrentMethodInfo());
            var distinctKeys = dto.PermissionKeys.Select(x => x.PermissionKey).Distinct().ToList();

            var existingKeys = await _dbContext.Permissions
            .Where(p => distinctKeys.Contains(p.PermissionKey))
            .Select(p => new PermissionKeyDto
            {
                PermissionKey = p.PermissionKey,
                DisplayName = p.DisplayName,
                Description = p.Description,
            })
            .ToListAsync();

            var existingKeyStrings = existingKeys.Select(k => k.PermissionKey).ToList();
            var invalidKeys = distinctKeys.Except(existingKeyStrings, StringComparer.Ordinal).ToList();

            if (invalidKeys.Count > 0)
                return Result<RoleDto>.Failure(ErrorCode.PermissionKeyInvalid, this.GetCurrentMethodInfo());

            await using var tx = await _dbContext.Database.BeginTransactionAsync();

            var oldPermission = _dbContext.RolePermissions.Where(rp => rp.RoleId == dto.RoleId);
            _dbContext.RolePermissions.RemoveRange(oldPermission);

            var newPermissions = existingKeys.Select(key => new RolePermission
            {
                RoleId = dto.RoleId,
                PermissionKey = key.PermissionKey
            });
            await _dbContext.RolePermissions.AddRangeAsync(newPermissions);
            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();

            return Result<RoleDto>.Success(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                PermissionKeys = existingKeys,
            });
        }

        public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequest dto)
        {
            _logger.LogInformation("Method {method} called with Name={name}", nameof(CreateRoleAsync), dto.Name);

            var exists = await _dbContext.Roles.AnyAsync(r => r.Name == dto.Name && !r.Deleted);
            if (exists)
                return Result<RoleDto>.Failure(ErrorCode.RoleAlreadyExists, this.GetCurrentMethodInfo());

            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = 1
            };

            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync(); // Lưu để sinh ra Id cho role

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                PermissionKeys = new List<PermissionKeyDto>()
            };

            // Nếu request có truyền danh sách quyền thì tiến hành gán quyền
            if (dto.PermissionKeys != null && dto.PermissionKeys.Any())
            {
                var assignDto = new AssignPermissionDto
                {
                    RoleId = role.Id,
                    PermissionKeys = dto.PermissionKeys
                };

                var assignResult = await AssignPermissionForRoleAsync(assignDto);

                // Nếu gán quyền lỗi, rollback lại bằng cách xóa Role vừa tạo
                if (!assignResult.IsSuccess)
                {
                    _dbContext.Roles.Remove(role);
                    await _dbContext.SaveChangesAsync();
                    return assignResult; // Trả về nguyên nhân lỗi
                }

                // Nếu thành công, cập nhật danh sách quyền vào DTO để trả về cho người dùng
                roleDto.PermissionKeys = assignResult.Value.PermissionKeys;
            }

            return Result<RoleDto>.Success(roleDto);
        }

        public async Task<Result<List<RoleDto>>> GetRolesAsync()
        {
            _logger.LogInformation("{Method} called", nameof(GetRolesAsync));

            var roles = await _dbContext.Roles
                .Where(r => !r.Deleted)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();

            return Result<List<RoleDto>>.Success(roles);
        }

        public async Task<Result<List<string>>> GetRolePermissionsAsync(int roleId)
        {
            _logger.LogInformation("{Method}: roleId={RoleId}", nameof(GetRolePermissionsAsync), roleId);

            var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == roleId && !r.Deleted);
            if (!roleExists)
                return Result<List<string>>.Failure(ErrorCode.RoleNotFound, this.GetCurrentMethodInfo(), $"Role {roleId} không tồn tại");

            var permissions = await _permissionCacheService.GetRolePermissionsAsync(roleId);
            return Result<List<string>>.Success(permissions.ToList());
        }

        public async Task<Result<bool>> UpdateRolePermissionsAsync(int roleId, List<string> permissionKeys)
        {
            permissionKeys = permissionKeys.Distinct().ToList();
            _logger.LogInformation("{Method}: roleId={RoleId}, count={Count}", nameof(UpdateRolePermissionsAsync), roleId, permissionKeys.Count);

            var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == roleId && !r.Deleted);
            if (!roleExists)
                return Result<bool>.Failure(ErrorCode.RoleNotFound, this.GetCurrentMethodInfo(), $"Role {roleId} không tồn tại");

            var validKeys = await _dbContext.Permissions
                .Where(p => permissionKeys.Contains(p.PermissionKey))
                .Select(p => p.PermissionKey)
                .ToListAsync();

            var invalidKeys = permissionKeys.Except(validKeys).ToList();
            if (invalidKeys.Any())
                return Result<bool>.Failure(ErrorCode.PermissionKeyInvalid, this.GetCurrentMethodInfo(),
                    $"PermissionKey không hợp lệ: {string.Join(", ", invalidKeys)}");

            await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ExecuteDeleteAsync();

            var newMappings = permissionKeys.Select(key => new RolePermission
            {
                RoleId = roleId,
                PermissionKey = key,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            if (newMappings.Any())
            {
                try
                {

                    await _dbContext.Database.ExecuteSqlRawAsync(@"
                        SELECT setval(
                            pg_get_serial_sequence('public.""RolePermission""', 'Id'), 
                            COALESCE((SELECT MAX(""Id"") FROM public.""RolePermission""), 0) + 1, 
                            false
                        );
                    ");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync RolePermission Id sequence.");
                }

                await _dbContext.RolePermissions.AddRangeAsync(newMappings);
                await _dbContext.SaveChangesAsync();
            }

            await _permissionCacheService.InvalidateRolePermissionsAsync(roleId);

            return Result<bool>.Success(true);
        }
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            _logger.LogInformation("Method : {method}, ID = {id}", nameof(DeleteAsync), id);
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
            if (role == null)
                return Result<bool>.Failure(ErrorCode.RoleNotFound, this.GetCurrentMethodInfo());
            role.Deleted = true;
            await _dbContext.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}