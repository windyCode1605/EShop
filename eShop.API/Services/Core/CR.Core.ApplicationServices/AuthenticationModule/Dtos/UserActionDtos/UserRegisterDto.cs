using System.ComponentModel.DataAnnotations;
using CR.DtoBase.Validations;

namespace CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos
{
    /// <summary>
    /// Thông tin đăng ký ban đầu cho tài khoản 
    /// </summary>
    public class UserRegisterDto
    {

        [CustomRequired(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [CustomRequired(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public required string UserName { get; set; }
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
        /// Mã người dùng, có thể là số điện thoại hoặc mã định danh khác, 
        /// tùy thuộc vào cách hệ thống xác định người dùng. Mục đích của UserCode là để cung cấp một cách nhận diện duy nhất cho mỗi người dùng trong hệ thống, 
        /// giúp quản lý và phân biệt giữa các tài khoản người dùng khác nhau.
        /// </summary>
        public string UserCode
        {
            get => _userCode;
            set => _userCode = value.Trim();
        }

    }
}