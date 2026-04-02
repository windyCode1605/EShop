using CR.Constants.Core.Users;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.User;
using CR.DtoBase;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class UserService : CoreServiceBase, IUserService
    {
        public UserService(
            ILogger<UserService> logger,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContext
        ): base( logger, httpContext) { }

        public async Task<Result<UserDto>> RegisterUser(UserRegisterDto input)
        {
            _logger.LogInformation("{MethodName}: input = {@Input}", nameof(RegisterUser), input);
            if ( await _dbContext.Users.AnyAsync(u => (u.UserName == input.Email) && !u.Deleted) )
            {
                return Result.Failure<UserDto>(
                    ErrorCode.UserRegisterExistPersonalEmail,
                    this.GetCurrentMethodInfo()
                );
            }

            var user = new Users
            {
                UserName = input.Email,
                PassWord = string.Empty,
                UserType = UserTypeEnum.CUSTOMER,
                Status = UserStatus.TEMP,
                UserCode = input.UserCode,
            };

            var passwordHasher = new PasswordHasher<Users>();
            user.PassWord = passwordHasher.HashPassword(user, string.Empty);

            user = _dbContext.Users.Add(user).Entity;
            await _dbContext.SaveChangesAsync();
            return Result<UserDto>.Success(
                new UserDto
                {
                    Id = user.Id,
                    UserName = input.UserName,
                    Email = input.Email,
                    UserType = UserTypeEnum.CUSTOMER,
                    Status = UserStatus.TEMP,
                }
            );
        }
    }
}