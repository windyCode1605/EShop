namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class SetPasswordUserDto
    {
        public int Id { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Bắt đổi mật khẩu không
        /// </summary>
        public bool IsPasswordTemp { get; set; } = true;
    }
}
