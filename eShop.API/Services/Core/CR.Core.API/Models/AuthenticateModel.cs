namespace CR.Core.API.Models
{
    public class AuthenticateModel
    {
        /// <summary>
        /// Tài khoản 
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// Đường dẫn đièu hướng lại connect authorize
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}