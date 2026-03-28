using CR.Constants.Core.Users;

namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class UserDto
    {
        /// <summary>
        /// Id của người dùng
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập của người dùng
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// Id khách hàng 
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Tên người dùng 
        /// </summary>
        public string? FullName { get; set; }

        public GenderTypes? Gender { get; set;}
        public string? AvatarImageUri { get; set; }

        /// <summary>
        /// Kiểu khách hàng 
        /// </summary>
        /// <example>1</example>
        // public UserTypeEnum UserType { get; set; }

        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Loai tài khoản, ví dụ: Admin, Customer, Staff, v.v.
        /// </summary>
        public UserTypeEnum UserType { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public int Status { get; set; }
        public string? Email { get; set; }
        public bool IsPasswordTemp { get; set; }
        public IEnumerable<UserRoleDto>? Role { get; set; }
        public int? AssignedMachineId { get; set; }
    }
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Status { get; set; }
        public int PermissionInWeb { get; set; }
    }
}