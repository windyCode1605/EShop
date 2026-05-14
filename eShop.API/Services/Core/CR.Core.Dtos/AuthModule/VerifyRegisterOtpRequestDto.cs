using System.ComponentModel.DataAnnotations;
using CR.DtoBase;

namespace CR.Core.Dtos.Auth;

public class VerifyRegisterOtpRequestDto : BaseRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$")]
    public string OtpCode { get; set; } = string.Empty;
}
