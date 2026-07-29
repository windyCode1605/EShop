using System.ComponentModel.DataAnnotations;
using CR.DtoBase;

namespace CR.Core.Dtos.Auth;

public class ResetPasswordRequestDto : BaseRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string ResetToken { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string NewPassword { get; set; } = string.Empty;
}
