using CR.Constants.Common.SysVar;
using CR.Constants.Core.Users;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.OtpModule.Abstracts;
using CR.Core.Domain.User;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.InfrastructureBase.Exceptions;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class UserAuthorizationService : CoreServiceBase, IUserAuthenticationService
    {
        private readonly PasswordHasher<Users> _passwordHasher;
        private readonly IOtpService _otpService;
        private readonly IDataProtector _protector;

        public UserAuthorizationService(
            ILogger<UserAuthorizationService> logger,
            IHttpContextAccessor httpContextAccessor,
            IOtpService otpService,
            IDataProtectionProvider dataProtectionProvider)
            : base(logger, httpContextAccessor)
        {
            _passwordHasher = new PasswordHasher<Users>();
            _otpService = otpService;
            _protector = dataProtectionProvider.CreateProtector("Atelier.eShop.ResetPassword_Token");
        }

        public async Task<Result<Users>> FindUserAuthorizationById(int id)
        {
            _logger.LogInformation("{MethodName}: id={Id}", nameof(FindUserAuthorizationById), id);

            var user = await _dbContext.Users
                           .Include(u => u.Profile)
                           .Include(u => u.UserRoles)
                               .ThenInclude(ur => ur.Role)
                           .FirstOrDefaultAsync(u => u.Id == id && !u.Deleted && u.Status == (int)UserStatus.ACTIVE);

            if (user == null) return Result.Failure<Users>(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());

            if (new int[] { (int)UserStatus.TEMP, (int)UserStatus.LOCK }.Contains(user.Status))
            {
                return Result.Failure<Users>(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }
            else if (user.Status == (int)UserStatus.DEACTIVE)
            {
                return Result.Failure<Users>(ErrorCode.UserIsDeactive, this.GetCurrentMethodInfo());
            }

            return Result.Success(user);
        }

        /// <summary>
        /// Xác thực người dùng (Cho cả Web và App trong kiến trúc Single-tenant)
        /// Hỗ trợ đăng nhập bằng cả Username hoặc Email
        /// </summary>
        public async Task<Result<Users>> ValidateAppUser(string username, string password)
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
                            && u.Status == (int)UserStatus.ACTIVE);

            if (user == null) return Result.Failure<Users>(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());

            // Kiểm tra quyền
            if (!new UserTypeEnum[] { UserTypeEnum.CUSTOMER, UserTypeEnum.SUPER_ADMIN, UserTypeEnum.ADMIN }.Contains(user.UserType))
            {
                return Result.Failure<Users>(ErrorCode.UserLoginUserTypeInvalid, this.GetCurrentMethodInfo());
            }

            // Kiểm tra trạng thái TEMP hoặc DEACTIVE/LOCK
            if (user.Status == (int)UserStatus.TEMP) return Result.Failure<Users>(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            if (user.Status == (int)UserStatus.DEACTIVE) return Result.Failure<Users>(ErrorCode.UserIsDeactive, this.GetCurrentMethodInfo());
            if (user.Status == (int)UserStatus.LOCK) return Result.Failure<Users>(ErrorCode.UserIsLock, this.GetCurrentMethodInfo());

            // Kiểm tra khóa tạm thời do nhập sai nhiều lần
            if (user.TimeLockUser.HasValue && user.TimeLockUser.Value >= now)
            {
                return Result.Failure<Users>(ErrorCode.UserIsInactiveBecauseMultiLoginTime, this.GetCurrentMethodInfo());
            }

            // Xác thực mật khẩu
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verifyResult != PasswordVerificationResult.Success)
            {
                var handleFailResult = await HandleCountIncorrectPasswordAsync(user);
                return Result.Failure<Users>(this.GetCurrentMethodInfo(), handleFailResult);
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

            return Result.Success(user);
        }

        /// <summary>
        /// Do đã bỏ TenantId, ValidateWebUser giờ đây giống hệt ValidateAppUser. 
        /// Anh có thể dùng chung hàm ValidateAppUser, hoặc giữ lại hàm này nếu muốn tách biệt API logic sau này.
        /// </summary>
        public async Task<Result<Users>> ValidateWebUser(string userName, string password)
        {
            _logger.LogInformation("{MethodName}: userName={UserName}", nameof(ValidateWebUser), userName);

            // Tái sử dụng logic của ValidateAppUser vì kiến trúc là Single-Tenant
            return await ValidateAppUser(userName, password);
        }

        /// <summary>
        /// Xử lý tăng biến đếm khi nhập sai mật khẩu (Đã chuyển sang Async)
        /// </summary>
        private async Task<Result> HandleCountIncorrectPasswordAsync(Users user)
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
                return Result.Failure(ErrorCode.UserIsInactiveBecauseMultiLoginTime, this.GetCurrentMethodInfo());

            if (user.Status == (int)UserStatus.DEACTIVE)
                return Result.Failure(ErrorCode.UserIsDeactive, this.GetCurrentMethodInfo());

            if (user.LoginFailCount == 1)
                return Result.Failure(ErrorCode.UsernameOrPasswordIncorrect, this.GetCurrentMethodInfo());

            return Result.Failure(
                ErrorCode.AppPasswordIncorrect,
                this.GetCurrentMethodInfo(),
                loginMaxTurn.ToString(),
                (loginMaxTurn - user.LoginFailCount).ToString()
            );
        }

        /// <summary>
        /// Lấy cấu hình số lần sai tối đa (Đã chuyển sang Async)
        /// </summary>
        public async Task<int> GetLimitedInputTurnAsync(string varName)
        {
            var sysVar = await _dbContext.SysVars.FirstOrDefaultAsync(o => o.GrName == GrNames.AUTHMAXTURN && o.VarName == varName);
            if (sysVar != null && int.TryParse(sysVar.VarValue, out var turns))
            {
                return turns;
            }
            return 5;
        }

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Method {method}: email = {email}", nameof(ForgotPasswordAsync), email);
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.Deleted && u.Status == (int)UserStatus.ACTIVE);
            if (user == null)
                return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            var otpResult = await _otpService.SendOtp(user.Id);
            if (!otpResult.IsSuccess)
            {
                return otpResult;
            }
            return Result.Success();
        }
        public async Task<Result<object>> VerifyOtpForResetAsync(string email, string otpCode)
        {
            _logger.LogInformation("Method Name {Method}: email = {email}", nameof(VerifyOtpForResetAsync), email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.Deleted && u.Status == (int)UserStatus.ACTIVE);
            if (user == null)
                return Result.Failure<object>(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            var verifyResult = await _otpService.VerifyOtp(otpCode, user.Id);
            if (!verifyResult.IsSuccess) return Result.Failure<object>(verifyResult.ErrorCode, verifyResult.StackTrace);

            // Sửa lại thành ToUnixTimeSeconds để đồng bộ với hàm ResetPassword
            var expireUnix = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
            var plainTextToken = $"{user.Id}|{expireUnix}";

            // Phải gọi hàm Protect để mã hóa
            var resetToken = _protector.Protect(plainTextToken);
            return Result.Success<object>(new { ResetToken = resetToken });
        }
        public async Task<Result> ResetPasswordAsync(string email, string resetToken, string newPassword)
        {
            _logger.LogInformation("{MethodName}: email={Email}", nameof(ResetPasswordAsync), email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.Deleted && u.Status == (int)UserStatus.ACTIVE);
            if (user == null) return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());

            try
            {
                var plainTextToken = _protector.Unprotect(resetToken);
                var parts = plainTextToken.Split('|');
                var userIdToken = int.Parse(parts[0]);
                var expireUnix = long.Parse(parts[1]);
                if (userIdToken != user.Id) return Result.Failure(ErrorCode.TokenIsInvalid, this.GetCurrentMethodInfo());
                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expireUnix) return Result.Failure(ErrorCode.OptCodeIsExpired, this.GetCurrentMethodInfo());
            }
            catch
            {
                return Result.Failure(ErrorCode.TokenIsInvalid, this.GetCurrentMethodInfo());
            }
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            user.LoginFailCount = 0;
            user.TimeLockUser = null;
            user.DateTimeLoginFailCount = null;
            await _dbContext.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();
            return Result.Success(new { Message = "Đặt lại mật khẩu thành công!" });
        }
    }
}