using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts
{
    public interface IUserService
    {
        Task<Result<UserDto>> RegisterUser(UserRegisterDto input);
    }
}