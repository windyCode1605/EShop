using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.CustomerModule.Abstracts;
using CR.Core.Dtos.CustomerModule;
using CR.DtoBase;
using CR.InfrastructureBase;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.CustomerModule.Implements
{
    public class CustomerService : CoreServiceBase, ICustomerService
    {
        public CustomerService(ILogger<CustomerService> logger, IHttpContextAccessor httpContext)
            : base(logger, httpContext) { }

        public async Task<Result<UserProfileDto>> GetMyProfile()
        {
            _logger.LogInformation("Method {method} called ", nameof(GetMyProfile));
            var userId = _httpContext.GetCurrentUserId();
            var userProfile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(up => up.UserId == userId);
            if (userProfile == null)
            {
                return Result<UserProfileDto>.Success(null);
            }
            return Result<UserProfileDto>.Success(MapToDto(userProfile));
        }
        public async Task<Result<UserProfileDto>> UpdateMyProfile(UpdateProfileDto dto)
        {
            _logger.LogInformation("Method {method} called ", nameof(UpdateMyProfile));
            var userId = _httpContext.GetCurrentUserId();
            var userProfile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
            {
                userProfile = new CR.Core.Domain.User.UserProfile
                {
                    UserId = userId,
                    PhoneNumber = dto.PhoneNumber ?? string.Empty
                };
                _dbContext.UserProfiles.Add(userProfile);
            }
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
            return Result<UserProfileDto>.Success(MapToDto(userProfile));
        }

        private UserProfileDto MapToDto(CR.Core.Domain.User.UserProfile userProfile)
        {
            if (userProfile == null) return null;

            return new UserProfileDto
            {
                Id = userProfile.Id,
                UserId = userProfile.UserId,
                FullName = userProfile.FullName,
                PhoneNumber = userProfile.PhoneNumber,
                DateOfBirth = userProfile.DateOfBirth,
                Gender = userProfile.Gender,
                AvatarUrl = userProfile.AvatarUrl,
            };
        }

    }
}