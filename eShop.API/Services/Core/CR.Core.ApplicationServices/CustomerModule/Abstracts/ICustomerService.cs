
using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.CustomerModule.Abstracts
{
    public interface ICustomerService
    {
        Task<Result<UserProfileDto>> GetMyProfile();
        Task<Result<UserProfileDto>> UpdateMyProfile(UpdateProfileDto dto);
    }
}