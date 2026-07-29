using CR.Constants.Common.SysVar;
using CR.Constants.ErrorCodes;
using CR.Core.ApplicationServices.Common;
using CR.Core.ApplicationServices.OtpModule.Abstracts;
using CR.Core.Domain.Opts;
using CR.DtoBase;
using CR.InfrastructureBase;
using CR.Utils.DataUtils;
using CR.Utils.Sercurity;
using MailKit.Net.Smtp;       // Thư viện MailKit
using MailKit.Security;       // Tùy chọn bảo mật MailKit
using MimeKit;                // Khởi tạo Message của MailKit
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CR.Core.ApplicationServices.OtpModule.Implements
{
    public class OtpService : CoreServiceBase, IOtpService
    {
        private const int DefaultOtpLength = 6;
        private const int DefaultOtpLifeTimeSeconds = 300;
        private const int DefaultOtpMaxVerify = 5;
        private const int DefaultResendCooldownSeconds = 60;

        private readonly IConfiguration _configuration;

        public OtpService(
            ILogger<OtpService> logger,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
        ) : base(logger, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public Task<Result> SendOtp(int userId)
        {
            return SendOtpInternal(userId);
        }

        public Task<Result> ReSendOtp(int userId)
        {
            return SendOtpInternal(userId);
        }

        public async Task<Result> VerifyOtp(string otp, int UserId) // Tên tham số theo đúng Interface
        {
            _logger.LogInformation("{MethodName}: userId={UserId}", nameof(VerifyOtp), UserId);

            var maxVerify = await GetIntSysVar(GrNames.OTP, VarNames.OTP_MAX_TURN, DefaultOtpMaxVerify);

            var latestOtp = await _dbContext.AuthOtps
                .Where(x => x.UserId == UserId && !x.IsUsed)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (latestOtp == null)
            {
                return Result.Failure(ErrorCode.UserIsRequestVerify, this.GetCurrentMethodInfo());
            }

            // Ưu tiên dùng UTC để đồng bộ server nếu anh deploy lên Cloud
            if (DateTime.UtcNow > latestOtp.ExpireTime)
            {
                latestOtp.IsUsed = true;
                await _dbContext.SaveChangesAsync();
                return Result.Failure(ErrorCode.OptCodeIsExpired, this.GetCurrentMethodInfo());
            }

            if (latestOtp.VerifyTime >= maxVerify)
            {
                return Result.Failure(ErrorCode.UserIsLock, this.GetCurrentMethodInfo());
            }

            var defaultOtp = await _dbContext.SysVars.FirstOrDefaultAsync(x =>
                x.GrName == GrNames.OTP && x.VarName == VarNames.DEFAULT_OTP
            );

            var isDefaultOtp = !string.IsNullOrWhiteSpace(defaultOtp?.VarValue) && otp == defaultOtp.VarValue;
            var isValidOtp = isDefaultOtp || PasswordHasher.VerifyPassword(otp, latestOtp.OtpCode);

            if (!isValidOtp)
            {
                latestOtp.VerifyTime += 1;
                await _dbContext.SaveChangesAsync();

                var errorCode = latestOtp.VerifyTime >= maxVerify
                    ? ErrorCode.UserIsLock
                    : ErrorCode.OptCodeNotValid;

                return Result.Failure(errorCode, this.GetCurrentMethodInfo());
            }

            latestOtp.IsUsed = true;
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        private async Task<Result> SendOtpInternal(int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => !u.Deleted && u.Id == userId);
            if (user == null)
            {
                return Result.Failure(ErrorCode.UserNotFound, this.GetCurrentMethodInfo());
            }

            var now = DateTime.UtcNow;
            var resendCooldown = await GetIntSysVar(GrNames.OTP, VarNames.OTP_RESEND_COOLDOWN, DefaultResendCooldownSeconds);
            var otpLifeTime = await GetIntSysVar(GrNames.OTP, VarNames.SECOND, DefaultOtpLifeTimeSeconds);

            // Bắt đầu Transaction bảo vệ tính toàn vẹn dữ liệu
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {

                var sendOtpDb = await _dbContext.SendOtps
                    .FirstOrDefaultAsync(s => s.Email == user.Email);

                if (sendOtpDb == null)
                {
                    sendOtpDb = new SendOtp
                    {
                        Email = user.Email,
                        LastSentDateTime = now,
                        TimeLimitCanVerifyOtp = now.AddSeconds(resendCooldown),
                        SendCount = 0
                    };
                    await _dbContext.SendOtps.AddAsync(sendOtpDb);
                }
                else if (sendOtpDb.TimeLimitCanVerifyOtp > now)
                {
                    var remainSeconds = (int)Math.Ceiling((sendOtpDb.TimeLimitCanVerifyOtp - now).TotalSeconds);
                    return Result.Failure(
                        ErrorCode.UserIsRequestVerify,
                        this.GetCurrentMethodInfo(),
                        new { ResendOtpsecondTime = remainSeconds }
                    );
                }

                // Hủy các OTP cũ chưa dùng
                var oldOtps = await _dbContext.AuthOtps
                    .Where(x => x.UserId == userId && !x.IsUsed)
                    .ToListAsync();
                foreach (var oldOtp in oldOtps)
                {
                    oldOtp.IsUsed = true;
                }

                var otpLength = await GetIntSysVar(GrNames.OTP, VarNames.OTP_LENGTH, DefaultOtpLength);
                var otpCode = GenerateOTP.GenerateOtp(otpLength);
                var otpHash = PasswordHasher.HashPassword(otpCode);

                await _dbContext.AuthOtps.AddAsync(new AuthOtp
                {
                    OtpCode = otpHash,
                    ExpireTime = now.AddSeconds(otpLifeTime),
                    IsUsed = false,
                    UserId = userId,
                    VerifyTime = 0,
                    CreatedDate = now
                });

                sendOtpDb.SendCount += 1;
                sendOtpDb.LastSentDateTime = now;
                sendOtpDb.TimeLimitCanVerifyOtp = now.AddSeconds(resendCooldown);

                await _dbContext.SaveChangesAsync(); // Update DB context để MailKit có thể chạy độc lập

                // Gọi hàm gửi Mail bằng MailKit
                await SendOtpMailAsync(user.Email, otpCode, otpLifeTime);

                await transaction.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi Database hoặc gửi Mail khi tạo OTP cho User {UserId}", userId);
                return Result.Failure(ErrorCode.UnknownError, this.GetCurrentMethodInfo());
            }
        }

        /// <summary>
        /// Hàm gửi Email thực tế sử dụng sức mạnh của MailKit
        /// </summary>
        private async Task SendOtpMailAsync(string email, string otpCode, int otpLifeTimeSeconds)
        {
            var host = _configuration["Smtp:Host"];
            var userName = _configuration["Smtp:UserName"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            var fromName = _configuration["Smtp:FromName"] ?? "ATELIER eShop";

            var enableSsl = _configuration.GetValue("Smtp:EnableSsl", true);
            var port = _configuration.GetValue("Smtp:Port", 587);

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException("SMTP configuration is missing in appsettings.");
            }

            // 1. Tạo Message chuẩn MimeKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "ATELIER - Mã xác nhận (OTP)";

            // 2. Build giao diện HTML cho Mail
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2 style='color: #2A2A2A;'>Xác nhận đăng ký tài khoản</h2>
                    <p>Mã OTP của bạn là:</p>
                    <h1 style='background: #f4f4f4; padding: 10px 20px; border-radius: 5px; display: inline-block; letter-spacing: 5px;'>{otpCode}</h1>
                    <p>Mã có hiệu lực trong <b>{otpLifeTimeSeconds / 60} phút</b>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                </div>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            // 3. Khởi tạo SmtpClient của MailKit và tiến hành gửi
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                // Bypass SSL Validation cho môi trường Dev (nếu cần thiết)
                // client.ServerCertificateValidationCallback = (s, c, h, e) => true; 

                var secureOptions = enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                await client.ConnectAsync(host, port, secureOptions);
                await client.AuthenticateAsync(userName, password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        private async Task<int> GetIntSysVar(string grName, string varName, int defaultValue)
        {
            var sysVar = await _dbContext.SysVars.FirstOrDefaultAsync(x =>
                x.GrName == grName && x.VarName == varName
            );
            return int.TryParse(sysVar?.VarValue, out var value) ? value : defaultValue;
        }
    }
}