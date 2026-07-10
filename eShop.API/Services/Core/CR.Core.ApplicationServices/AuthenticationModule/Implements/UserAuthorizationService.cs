using CR.Constants.Common.SysVar;
using CR.Constants.Core.Users;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.User;
using CR.InfrastructureBase;
using CR.InfrastructureBase.Exceptions;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class UserAuthorizationService : CoreServiceBase, IUserAuthenticationService
    {
        private readonly PasswordHasher<Users> _passwordHasher;

        public UserAuthorizationService(ILogger<UserAuthorizationService> logger, IHttpContextAccessor httpContextAccessor) 
            : base(logger, httpContextAccessor)
        {
            _passwordHasher = new PasswordHasher<Users>();
        }

        public async Task<Users> FindUserAuthorizationById(int id)
        {
            _logger.LogInformation("{MethodName}: id={Id}", nameof(FindUserAuthorizationById), id);
            
            var user = await _dbContext.Users
                           .Include(u => u.Profile)
                           .Include(u => u.UserRoles)
                               .ThenInclude(ur => ur.Role)
                           .FirstOrDefaultAsync(u => u.Id == id && !u.Deleted && u.Status == (int)UserStatus.ACTIVE) 
                       ?? throw new UserFriendlyException(ErrorCode.UserNotFound);
            
            if (new int[] { (int)UserStatus.TEMP, (int)UserStatus.LOCK }.Contains(user.Status))
            {
                throw new UserFriendlyException(ErrorCode.UserNotFound);
            }
            else if (user.Status == (int)UserStatus.DEACTIVE)
            {
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            }
            
            return user;
        }

        /// <summary>
        /// Xác thực người dùng (Cho cả Web và App trong kiến trúc Single-tenant)
        /// Hỗ trợ đăng nhập bằng cả Username hoặc Email
        /// </summary>
        public async Task<Users> ValidateAppUser(string username, string password)
        {
            _logger.LogInformation("{MethodName}: username = {Username}", nameof(ValidateAppUser), username);
            
            var now = DateTime.UtcNow;
            
            var user = await _dbContext.Users
                            .Include(u => u.Profile)
                            .Include(u => u.UserRoles)
                                .ThenInclude(ur => ur.Role)
                            .FirstOrDefaultAsync(u => 
                            (u.Username == username || u.Email == username) 
                            && !u.Deleted 
                            && u.Status == (int)UserStatus.ACTIVE) 
                       ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

            // Kiểm tra quyền
            if (!new UserTypeEnum[] { UserTypeEnum.CUSTOMER, UserTypeEnum.SUPER_ADMIN, UserTypeEnum.ADMIN }.Contains(user.UserType))
            {
                throw new UserFriendlyException(ErrorCode.UserLoginUserTypeInvalid);
            }
            
            // Kiểm tra trạng thái TEMP hoặc DEACTIVE/LOCK
            if (user.Status == (int)UserStatus.TEMP) throw new UserFriendlyException(ErrorCode.UserNotFound);
            if (user.Status == (int)UserStatus.DEACTIVE) throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            if (user.Status == (int)UserStatus.LOCK) throw new UserFriendlyException(ErrorCode.UserIsLock);

            // Kiểm tra khóa tạm thời do nhập sai nhiều lần
            if (user.TimeLockUser.HasValue && user.TimeLockUser.Value >= now)
            {
                throw new UserFriendlyException(ErrorCode.UserIsInactiveBecauseMultiLoginTime);
            }

            // Xác thực mật khẩu
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verifyResult != PasswordVerificationResult.Success)
            {
                await HandleCountIncorrectPasswordAsync(user); // Ném exception bên trong hàm này
            }

            // Reset đếm lỗi khi đăng nhập thành công
            user.LoginFailCount = 0;
            user.DateTimeLoginFailCount = null;

            // Xử lý cờ IsFirstTime
            if (user.IsFirstTime)
            {
                user.IsFirstTime = false;
                await _dbContext.SaveChangesAsync();
                
                // Mẹo nhỏ: Set ngược lại true để truyền state lên tầng tạo Token, 
                // DB thì vẫn lưu false vì đã SaveChanges xong rồi.
                user.IsFirstTime = true; 
            }
            else
            {
                await _dbContext.SaveChangesAsync();
            }

            return user;
        }

        /// <summary>
        /// Do đã bỏ TenantId, ValidateWebUser giờ đây giống hệt ValidateAppUser. 
        /// Anh có thể dùng chung hàm ValidateAppUser, hoặc giữ lại hàm này nếu muốn tách biệt API logic sau này.
        /// </summary>
        public async Task<Users> ValidateWebUser(string userName, string password)
        {
            _logger.LogInformation("{MethodName}: userName={UserName}", nameof(ValidateWebUser), userName);
            
            // Tái sử dụng logic của ValidateAppUser vì kiến trúc là Single-Tenant
            return await ValidateAppUser(userName, password);
        }

        /// <summary>
        /// Xử lý tăng biến đếm khi nhập sai mật khẩu (Đã chuyển sang Async)
        /// </summary>
        private async Task HandleCountIncorrectPasswordAsync(Users user)
        {
            var now = DateTime.UtcNow;

            // 1. Reset lại số lần nếu đã qua 15 phút kể từ lần sai cuối cùng
            if (user.DateTimeLoginFailCount.HasValue && user.DateTimeLoginFailCount.Value < now && user.LoginFailCount != 0)
            {
                user.LoginFailCount = 0;
                user.DateTimeLoginFailCount = null;
            }

            // 2. Tăng số lần sai
            user.LoginFailCount += 1;
            
            // Lấy max turn, nên await nếu hàm lấy DB là Async
            var loginMaxTurn = await GetLimitedInputTurnAsync(VarNames.LOGINMAXTURN); 
            user.DateTimeLoginFailCount = now.AddMinutes(15);

            // 3. Khóa user nếu vượt hạn mức
            if (user.LoginFailCount >= loginMaxTurn)
            {
                user.TimeLockUser = now.AddHours(1); // Khóa 1 tiếng
                user.LoginFailCount = 0;
                user.DateTimeLoginFailCount = null; 
            }

            await _dbContext.SaveChangesAsync();

            // 4. Bắn Exception
            if (user.TimeLockUser.HasValue && user.TimeLockUser.Value >= now) 
                throw new UserFriendlyException(ErrorCode.UserIsInactiveBecauseMultiLoginTime);
            
            if (user.Status == (int)UserStatus.DEACTIVE) 
                throw new UserFriendlyException(ErrorCode.UserIsDeactive);
            
            if (user.LoginFailCount == 1) 
                throw new UserFriendlyException(ErrorCode.UsernameOrPasswordIncorrect);

            throw new UserFriendlyException(
                ErrorCode.AppPasswordIncorrect,
                loginMaxTurn.ToString(),
                (loginMaxTurn - user.LoginFailCount).ToString()
            );
        }

        /// <summary>
        /// Lấy cấu hình số lần sai tối đa (Đã chuyển sang Async)
        /// </summary>
        public async Task<int> GetLimitedInputTurnAsync(string varName)
        {
            var sysVar = await _dbContext.SysVars.FirstOrDefaultAsync(o => o.GrName == GrNames.AUTHMAXTURN && o.VarName == varName)
                         ?? throw new UserFriendlyException(ErrorCode.SysVarsIsNotConfig, GrNames.AUTHMAXTURN, varName);
            
            return int.Parse(sysVar.VarValue);
        }
    }
}