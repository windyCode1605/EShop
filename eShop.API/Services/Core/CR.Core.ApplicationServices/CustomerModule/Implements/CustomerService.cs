using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.CustomerModule.Implements
{
    public class CustomerService : CoreServiceBase, ICustomerService
    {
        public CustomerService(ILogger<CustomerService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public async Task<Result<CR.Core.Domain.User.UserProfile>> GetMyProfile()
        {
            _logger.LogInformation("Method {method} called ", nameof(GetMyProfile));
            var userId = _httpContext.GetCurrentUserId();
            var userProfile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(up => up.UserId == userId);
            if (userProfile == null)
            {
                return Result<CR.Core.Domain.User.UserProfile>.Failure(
                    ErrorCode.UserProfileNotFound, this.GetCurrentMethodInfo()
                );
            }
            return Result<CR.Core.Domain.User.UserProfile>.Success(userProfile);
        }
        public async Task<Result<CR.Core.Domain.User.UserProfile>> UpdateMyProfile(UpdateProfileDto dto)
        {
            _logger.LogInformation("Method {method} called ", nameof(UpdateMyProfile));
            var userId = _httpContext.GetCurrentUserId();
            var userProfile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
                return Result<CR.Core.Domain.User.UserProfile>.Failure(
                    ErrorCode.UserProfileNotFound, this.GetCurrentMethodInfo()
                );
            if (dto.FullName != null)
                userProfile.FullName = dto.FullName;

            if (dto.PhoneNumber != null)
                userProfile.PhoneNumber = dto.PhoneNumber;

            if (dto.DateOfBirth.HasValue)
                userProfile.DateOfBirth = dto.DateOfBirth;

            if (dto.Gender.HasValue)
                userProfile.Gender = dto.Gender;

            if (dto.AvatarUrl != null)
                userProfile.AvatarUrl = dto.AvatarUrl;

            await _dbContext.SaveChangesAsync();
            return Result<CR.Core.Domain.User.UserProfile>.Success(userProfile);
        }

    }
}