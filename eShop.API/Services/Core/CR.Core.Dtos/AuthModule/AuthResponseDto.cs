// Services/Core/CR.Core.Dtos/Auth/AuthResponseDto.cs
using CR.DtoBase;

namespace CR.Core.Dtos.Auth;

public class AuthResponseDto : BaseResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}