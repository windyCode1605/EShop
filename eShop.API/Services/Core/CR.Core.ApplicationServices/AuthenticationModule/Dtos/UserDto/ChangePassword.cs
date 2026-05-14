

using CR.DtoBase.Validations;

namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class ChangePasswordDto
    {
        private string? _oldPassword;
        [CustomMaxLength(128)]
        [CustomRequired(AllowEmptyStrings = false)]
        public string OldPassword
        {
            get => _oldPassword ?? string.Empty;
            set => _oldPassword = value?.Trim();
        }

        private string? _newPassword;
        [CustomMaxLength(128)]
        [CustomRequired(AllowEmptyStrings = false)]
        public string NewPassword
        {
            get => _newPassword ?? string.Empty;
            set => _newPassword = value?.Trim();
        }
    }
}
