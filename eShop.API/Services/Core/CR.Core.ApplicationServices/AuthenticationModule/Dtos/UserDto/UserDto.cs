using CR.Constants.Core.Users;
using System.ComponentModel.DataAnnotations;

namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    /// <summary>
    /// Thông tin hiển thị danh sách người dùng
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public UserTypeEnum UserType { get; set; }
        public int Status { get; set; }
        public bool IsPasswordTemp { get; set; }

        // --- Dữ liệu phẳng từ UserProfile ---
        public string? FullName { get; set; }
        public GenderTypes? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }

        public IEnumerable<UserRoleDto>? Roles { get; set; }
    }

    public class UserRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Status { get; set; }
    }
}