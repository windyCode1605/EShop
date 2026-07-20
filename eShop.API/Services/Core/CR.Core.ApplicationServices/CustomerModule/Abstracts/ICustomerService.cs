
using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;

namespace CR.Core.ApplicationServices.CustomerModule.Abstracts
{
    public interface ICustomerService
    {
        Task<Result<CR.Core.Domain.User.UserProfile>> GetMyProfile();
        Task<Result<CR.Core.Domain.User.UserProfile>> UpdateMyProfile(UpdateProfileDto dto);
    }
}