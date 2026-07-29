using CR.Constants.Core.Users;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.AuthenticationModule.Abstracts;
using CR.Core.ApplicationServices.OtpModule.Abstracts;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserActionDtos;
using CR.Core.ApplicationServices.AuthenticationModule.Dtos.UserDto;
using CR.Core.ApplicationServices.Common;
using CR.Core.Domain.User;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.InfrastructureBase.Exceptions;
using CR.Utils.DataUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.AuthenticationModule.Implements
{
    public class UserService : CoreServiceBase, IUserService
    {
        private readonly IOtpService _otpService;
        private readonly PasswordHasher<Users> _passwordHasher;
        private readonly IPermissionCacheService _permissionCacheService;

        public UserService(
            ILogger<UserService> logger,
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContext,
            IOtpService otpService,
            IPermissionCacheService permissionCacheService
        ) : base(logger, httpContext)
        {
            _otpService = otpService;
            _permissionCacheService = permissionCacheService;
            // Khởi tạo Hasher chuẩn của Microsoft Identity
            _passwordHasher = new PasswordHasher<Users>();
        }

        /// <summary>
        /// B1. Đăng ký người dùng mới
        /// Tự động tạo bản ghi trong cả 2 bảng Users và UserProfile
        /// </summary>
        public async Task<Result<UserDto>> RegisterUser(UserRegisterDto input)
        {
            _logger.LogInformation("{MethodName}: input = {@Input}", nameof(RegisterUser), input);

            // Kiểm tra theo Email
            var existingUserByEmail = await _dbContext.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == input.Email && !u.Deleted);
            if (existingUserByEmail != null && existingUserByEmail.Status != (int)UserStatus.TEMP)
            {
                return Result.Failure<UserDto>(ErrorCode.UserRegisterExistPersonalEmail, this.GetCurrentMethodInfo());
            }

            // Kiểm tra theo Username
            var existingUserByUsername = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == input.UserName && !u.Deleted);
            if (existingUserByUsername != null && (existingUserByEmail == null || existingUserByUsername.Id != existingUserByEmail.Id))
            {
                // Username đã bị người khác (hoặc email khác) chiếm dụng, dù là TEMP hay ACTIVE đều không được phép tạo đè
                return Result.Failure<UserDto>(ErrorCode.UserRegisterExistUsername, this.GetCurrentMethodInfo());
            }

            try
            {
                Users? user = existingUserByEmail;

                if (user == null)
                {
                    user = new Users
                    {
                        Username = input.UserName,
                        Email = input.Email,
                        PasswordHash = _passwordHasher.HashPassword(new Users(), Guid.NewGuid().ToString()), // Mật khẩu rác tạm thời
                        UserType = UserTypeEnum.CUSTOMER,
                        Status = (int)UserStatus.TEMP,
                        IsTempPassword = true,
                        IsFirstTime = true,

                        // Tự động sinh UserProfile
                        Profile = new UserProfile
                        {
                            FullName = input.FullName,
                            PhoneNumber = string.Empty
                        }
                    };

                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync(); // Lưu để EF Core gen Id
                }
                else
                {
                    // Nếu user nhập lại thông tin (resend OTP), cập nhật lại Username và FullName nếu họ có thay đổi
                    user.Username = input.UserName;
                    if (user.Profile != null)
                    {
                        user.Profile.FullName = input.FullName;
                    }
                    else
                    {
                        user.Profile = new UserProfile { FullName = input.FullName, PhoneNumber = string.Empty };
                    }
                    await _dbContext.SaveChangesAsync();
                }

                // Gửi OTP (OtpService quản lý transaction của nó)
                var sendOtpResult = await _otpService.SendOtp(user.Id);
                if (sendOtpResult.IsFailure)
                {
                    return Result.Failure<UserDto>(this.GetCurrentMethodInfo(), sendOtpResult);
                }

                return Result<UserDto>.Success(new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = input.FullName,
                    UserType = UserTypeEnum.CUSTOMER,
                    Status = UserStatus.TEMP,
                    IsPasswordTemp = user.IsTempPassword,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi Database khi đăng ký User mới: {Email}", input.Email);
                throw;
            }
        }

        /// <summary>
        /// B2. Xác nhận OTP
        /// Sau khi verify OTP thành công, user cần gọi SetPassword để hoàn tất đăng ký
        /// </summary>
        public async Task<Result> VerifyRegisterOtp(string email, string otpCode)
        {
            _logger.LogInformation("{MethodName} : email: {email}, otpCode: {otpCode}", nameof(VerifyRegisterOtp), email, otpCode);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.Deleted && u.UserType == UserTypeEnum.CUSTOMER);

            if (user == null)
            {
                return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }
            if (user.Status != (int)UserStatus.TEMP)
            {
                return Result.Failure(ErrorCode.UserIsRegistered, this.GetCurrentMethodInfo());
            }

            var verifyResult = await _otpService.VerifyOtp(otpCode, user.Id);
            if (verifyResult.IsFailure)
            {
                return Result.Failure(this.GetCurrentMethodInfo(), verifyResult);
            }

            // Verify xong -> User được phép đặt mật khẩu (nhưng trạng thái vẫn TEMP cho đến khi SetPassword)
            // Không tự động Active ở đây, để user đặt mật khẩu trước
            return Result.Success();
        }

        /// <summary>
        /// B4. Cập nhật thông tin LastLogin
        /// Đã loại bỏ bóc tách OS/Browser do Entity không còn lưu trữ
        /// </summary>
        public async Task<Result> LoginInfor(int userId)
        {
            _logger.LogInformation("{MethodName}: userId = {UserId}", nameof(LoginInfor), userId);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.Deleted);

            if (user == null)
            {
                return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }

            user.LastLogin = DateTime.UtcNow; // Chuẩn hóa dùng UTC
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        /// <summary>
        /// B3. Đặt mật khẩu lần đầu
        /// Khi user đặt mật khẩu thành công, tài khoản trở thành ACTIVE
        /// </summary>
        public async Task<Result> SetPassword(SetPasswordUserDto input)
        {
            _logger.LogInformation("{MethodName}: input = {@Input}", nameof(SetPassword), input);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == input.Id && !u.Deleted);
            if (user == null)
            {
                return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }

            // Chỉ cho phép SetPassword nếu user ở trạng thái TEMP (chưa hoàn tất đăng ký)
            if (user.Status != (int)UserStatus.TEMP)
            {
                return Result.Failure(ErrorCode.UserIsRegistered, this.GetCurrentMethodInfo());
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);
            user.IsTempPassword = false;
            user.IsFirstTime = false;
            user.Status = (int)UserStatus.ACTIVE; // Tài khoản chính thức kích hoạt

            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<Result> ChangePassword(ChangePasswordDto input)
        {
            var userId = _httpContext.GetCurrentUserId();
            _logger.LogInformation("{MethodName}: userId = {UserId}", nameof(ChangePassword), userId);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.Deleted);
            if (user == null)
            {
                return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }

            // Dùng hàm Verify chuẩn của hệ thống Identity
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, input.OldPassword);

            if (!user.IsTempPassword && verifyResult != PasswordVerificationResult.Success)
            {
                return Result.Failure(ErrorCode.UserOldPasswordIncorrect, this.GetCurrentMethodInfo());
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, input.NewPassword!);
            user.IsTempPassword = false;

            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<bool>> AssignRoleToUser(int userId, int roleId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return Result<bool>.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());

            var role = await _dbContext.Roles.FindAsync(roleId);
            if (role == null)
                return Result<bool>.Failure(ErrorCode.RoleNotFound, this.GetCurrentMethodInfo());
            var isRoleExist = await _dbContext.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.Deleted);
            if (isRoleExist)
                return Result<bool>.Failure(ErrorCode.BadRequest, "User đã tồn tại quyền này");
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            _dbContext.UserRoles.Add(userRole);
            await _dbContext.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.Deleted);

            if (user == null)
            {
                return Result<UserDto>.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }

            return Result<UserDto>.Success(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.Profile?.FullName,
                Phone = user.Profile?.PhoneNumber,
                UserType = user.UserType,
                Status = user.Status,
                IsPasswordTemp = user.IsTempPassword
            });
        }

        /// <summary>
        /// Lấy thông tin phân quyền của user đang đăng nhập hiện tại.
        /// Gom tất cả permissions từ nhiều roles của user (nếu user có nhiều roles).
        /// Cache-Aside: lấy từ Redis trước, cache miss mới query DB.
        /// </summary>
        public async Task<Result<UserAuthorizationDto>> GetCurrentUserAuthorizationAsync()
        {
            _logger.LogInformation("{Method} called", nameof(GetCurrentUserAuthorizationAsync));

            var userId = _httpContext.GetCurrentUserId();
            if (userId <= 0)
                return Result<UserAuthorizationDto>.Failure(
                    ErrorCode.UserNotFound,
                    this.GetCurrentMethodInfo(),
                    "Không tìm thấy user id"
                );

            var userRoles = await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && !ur.Deleted)
                .Select(ur => new { ur.RoleId, ur.Role.Name })
                .ToListAsync();

            var roleNames = userRoles.Select(r => r.Name).ToList();
            var allPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var role in userRoles)
            {
                var permissions = await _permissionCacheService.GetRolePermissionsAsync(role.RoleId);
                foreach (var p in permissions)
                    allPermissions.Add(p);
            }

            return Result<UserAuthorizationDto>.Success(new UserAuthorizationDto
            {
                Roles = roleNames,
                Permissions = allPermissions.ToList()
            });
        }
    }
}