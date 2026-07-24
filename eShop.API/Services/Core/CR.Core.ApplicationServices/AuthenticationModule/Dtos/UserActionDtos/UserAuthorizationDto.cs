namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;

/// <summary>
/// DTO phẳng trả về cho Frontend: danh sách tên Role và toàn bộ Permission đã được gom từ nhiều Role.
/// Dùng cho API /me hoặc /auth/profile, không phải dùng cho CRUD Role.
/// </summary>
public class UserAuthorizationDto
{
    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}
