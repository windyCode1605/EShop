using System.Net.NetworkInformation;

namespace CR.Constants.Core.Users
{
    public static class UserStatus
    {
        public const int ACTIVE = 1;
        public const int DEACTIVE = 2;
        public const int LOCK = 3;
        /// <summary>
        /// Đăng kí tài khoản chưa OTP
        /// </summary>
        public const int TEMP = 4;
        /// <summary>
        /// Đăng kí tài khoản đã OTP
        /// </summary>
        public const int TEMP_OTP = 5;
        public static readonly int[] USER_NOT_VALID = [LOCK, TEMP, TEMP_OTP];
    }
}