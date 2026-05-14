using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts
{
    public interface IUserService
    {
        Task<Result<UserDto>> RegisterUser(UserRegisterDto input);
        Task<Result> VerifyRegisterOtp(string email, string otpCode);
        /// <summary>
        /// Set password cho user
        /// </summary>
        /// <param name="input"></param>
        Task<Result> SetPassword(SetPasswordUserDto input);
        /// <summary>
        /// Lưu thông tin ngày gần nhất + thiết bị khi đăng nhập
        /// </summary>
        /// <param name="userId"></param>
        Task<Result> LoginInfor(int userId);

    }
}
