using System.ComponentModel.DataAnnotations;
using CR.DtoBase;

namespace CR.Core.Dtos.Auth;
public class LoginRequestDto : BaseRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}