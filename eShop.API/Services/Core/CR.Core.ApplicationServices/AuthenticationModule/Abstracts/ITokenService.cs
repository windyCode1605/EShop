using CR.Core.Domain.User;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts
{
    public interface ITokenService
    {
        /// <summary>
        /// Tạo JWT token cho người dùng đã xác thực
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>JWT token string</returns>
        string GenerateAccessToken(Users user);

        /// <summary>
        /// Xác thực JWT token và trả về thông tin người dùng nếu token hợp lệ
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>True if valid, false otherwise</returns>
        bool ValidateToken(string token);
    }
}
