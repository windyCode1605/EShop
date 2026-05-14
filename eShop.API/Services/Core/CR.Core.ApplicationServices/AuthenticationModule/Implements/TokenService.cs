using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CR.Constants.Core.Users;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.Domain.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateAccessToken(Users user)
        {
            try
            {
                // Ném lỗi ngay nếu cấu hình môi trường (Environment Variables / appsettings) bị thiếu Key
                var secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
                var issuer = _configuration["Jwt:Issuer"] ?? "CR.API";
                var audience = _configuration["Jwt:Audience"] ?? "CR.Web";
                var expiryMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email), // Đã cập nhật theo Entity mới
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(UserClaimTypes.UserType, ((int)user.UserType).ToString()),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                };

                // Lấy FullName từ bảng Profile (Lưu ý: Khi gọi hàm này, phải Include(u => u.Profile) ở query)
                if (user.Profile != null && !string.IsNullOrEmpty(user.Profile.FullName))
                {
                    claims.Add(new Claim(ClaimTypes.Name, user.Profile.FullName));
                }

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes), // Ưu tiên dùng UTC
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                
                _logger.LogInformation("Generated token for user {UserId}", user.Id);
                
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
                throw;
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
                var issuer = _configuration["Jwt:Issuer"] ?? "CR.API";
                var audience = _configuration["Jwt:Audience"] ?? "CR.Web";

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Chặn độ trễ mặc định của Token
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                // Để LogWarning thay vì LogError vì việc user gửi lên 1 token hết hạn là bình thường
                _logger.LogWarning(ex, "Token validation failed"); 
                return false;
            }
        }
    }
}