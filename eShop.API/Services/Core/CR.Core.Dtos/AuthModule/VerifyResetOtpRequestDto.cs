using System.ComponentModel.DataAnnotations;
using CR.DtoBase;

namespace CR.Core.Dtos.Auth;

public class VerifyResetOtpRequestDto : BaseRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]{6}$")]
    public string OtpCode { get; set; } = string.Empty;
}
