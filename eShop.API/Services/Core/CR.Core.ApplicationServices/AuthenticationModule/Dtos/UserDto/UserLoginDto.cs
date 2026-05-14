namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    /// <summary>
    /// Trả về cho UI kèm với JWT Token để lưu vào State Management (Redux/NgRx/LocalStore)
    /// </summary>
    public class UserLoginDto
    {
        public int Id { get; set; }
        
        public string Email { get; set; } = null!;
        
        public string Username { get; set; } = null!;

        public string? FullName { get; set; }

        public string? AvatarUrl { get; set; }

        public int UserType { get; set; }

        public int Status { get; set; }

        public bool IsPasswordTemp { get; set; }
        
        // Không cần trả DateTimeLoginFailCount hay LoginFailCount về cho Frontend 
        // vì đây là thông tin bảo mật chạy ngầm ở Backend

        public IEnumerable<string>? RoleNames { get; set; }
        public IEnumerable<int>? RoleIds { get; set; }
    }
}