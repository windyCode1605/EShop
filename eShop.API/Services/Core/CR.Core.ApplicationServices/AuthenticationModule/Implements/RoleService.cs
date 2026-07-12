using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.RoleDtos;
using CR.Core.ApplicationServices.Common;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements;

public class RoleService : CoreServiceBase, IRoleService
{
    private readonly IPermissionCacheService _permissionCacheService;

    public RoleService(
        ILogger<RoleService> logger,
        IHttpContextAccessor httpContext,
        IPermissionCacheService permissionCacheService)
        : base(logger, httpContext)
    {
        _permissionCacheService = permissionCacheService;
    }

    /// <summary>
    /// Lấy toàn bộ danh sách Role trong hệ thống.
    /// Mục đích: Admin CRUD — xem, tạo, sửa, xóa Role.
    /// </summary>
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

    /// <summary>
    /// Lấy thông tin phân quyền của user đang đăng nhập hiện tại.
    /// Gom tất cả permissions từ nhiều roles của user (nếu user có nhiều roles).
    /// Cache-Aside: lấy từ Redis trước, cache miss mới query DB.
    /// </summary>
    public async Task<Result<UserAuthorizationDto>> GetCurrentUserAuthorizationAsync()
    {
        _logger.LogInformation("{Method} called", nameof(GetCurrentUserAuthorizationAsync));

        var userId = _httpContext.GetCurrentUserId();
        if (userId == null)
            return Result<UserAuthorizationDto>.Failure(
                ErrorCode.UserNotFound,
                this.GetCurrentMethodInfo(),
                "Không tìm thấy user id"
            );

        var userRoles = await _dbContext.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && !ur.Deleted)
            .Select(ur => new { ur.RoleId, ur.Role.Name })
            .ToListAsync();

        var roleNames = userRoles.Select(r => r.Name).ToList();
        var allPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var role in userRoles)
        {
            var permissions = await _permissionCacheService.GetRolePermissionsAsync(role.RoleId);
            foreach (var p in permissions)
                allPermissions.Add(p);
        }

        return Result<UserAuthorizationDto>.Success(new UserAuthorizationDto
        {
            Roles = roleNames,
            Permissions = allPermissions.ToList()
        });
    }

    /// <summary>
    /// Lấy danh sách PermissionKey đang được gán cho một Role.
    /// Dùng cho Admin mở popup chỉnh sửa quyền của Role.
    /// </summary>
    public async Task<Result<List<string>>> GetRolePermissionsAsync(int roleId)
    {
        _logger.LogInformation("{Method}: roleId={RoleId}", nameof(GetRolePermissionsAsync), roleId);

        var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == roleId && !r.Deleted);
        if (!roleExists)
            return Result<List<string>>.Failure(ErrorCode.RoleNotFound, this.GetCurrentMethodInfo(), $"Role {roleId} không tồn tại");

        // Dùng Cache-Aside để tránh query DB mỗi lần
        var permissions = await _permissionCacheService.GetRolePermissionsAsync(roleId);
        return Result<List<string>>.Success(permissions.ToList());
    }

    /// <summary>
    /// Cập nhật toàn bộ permissions của một Role (replace, không merge).
    /// Sau khi lưu DB, xóa cache của Role đó để request tiếp theo lấy dữ liệu mới.
    /// </summary>
    public async Task<Result<bool>> UpdateRolePermissionsAsync(int roleId, List<string> permissionKeys)
    {
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


        var newMappings = permissionKeys.Select(key => new CR.Core.Domain.User.RolePermission
        {
            RoleId = roleId,
            PermissionKey = key,
            CreatedDate = DateTime.UtcNow
        });
        await _dbContext.RolePermissions.AddRangeAsync(newMappings);
        await _dbContext.SaveChangesAsync();

        // Invalidate cache để request tiếp theo lấy dữ liệu mới từ DB
        await _permissionCacheService.InvalidateRolePermissionsAsync(roleId);

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Lấy toàn bộ danh mục Permission, gom nhóm theo PermissionGroup.
    /// Admin dùng để render checklist checkbox trên UI quản lý quyền.
    /// </summary>
    public async Task<Result<Dictionary<string, List<PermissionItemDto>>>> GetAllPermissionsGroupedAsync()
    {
        _logger.LogInformation("{Method} called", nameof(GetAllPermissionsGroupedAsync));

        var permissions = await _dbContext.Permissions
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