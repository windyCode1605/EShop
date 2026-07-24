namespace CR.Core.Dtos.RoleModule;

/// <summary>
/// Đại diện cho một Permission trong danh mục, dùng cho màn hình Admin quản lý quyền.
/// </summary>
public class PermissionItemDto
{
    public string PermissionKey { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
}

/// <summary>
/// DTO body request khi Admin cập nhật quyền của một Role.
/// </summary>
public class UpdateRolePermissionsDto
{
    public List<string> PermissionKeys { get; set; } = [];
}
