using CR.DtoBase.Validations;

namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos
{
    /// <summary>
    /// Thiết lập mật khẩu khi đăng kí tài khoản 
    /// </summary>
    public class UserRegisterDto
    {
        private string _username = null!;
        [Email]
        [CustomRequired(AllowEmptyStrings = false)]
        public required string UserName
        {
            get => _username;
            set => _username = value.Trim();
        }
        private string _email = null!;

        [Email]
        [CustomRequired(AllowEmptyStrings = false)]
        public required string Email
        {
            get => _email;
            set => _email = value.Trim();
        }

        private string _userCode = null!;
        /// <summary>
        /// Mã người dùng, có thể là số điện thoại hoặc mã định danh khác, tùy thuộc vào cách hệ thống xác định người dùng. Mục đích của UserCode là để cung cấp một cách nhận diện duy nhất cho mỗi người dùng trong hệ thống, giúp quản lý và phân biệt giữa các tài khoản người dùng khác nhau.
        /// </summary>
        public string UserCode
        {
            get => _userCode;
            set => _userCode = value.Trim();
        }
        
    }
}