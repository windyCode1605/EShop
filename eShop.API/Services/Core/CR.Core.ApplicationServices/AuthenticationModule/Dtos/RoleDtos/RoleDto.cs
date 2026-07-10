namespace CR.Core.ApplicationServices.AuthenticationModule.RoleDtos;

/// <summary>
/// DTO đại diện cho một Role entity, dùng cho CRUD Role (Admin quản lý).
/// Không dùng DTO này để trả về danh sách permission của user.
/// </summary>
public class RoleDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}