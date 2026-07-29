namespace CR.Constants.Common.SysVar
{
    public static class VarNames
    {
        public const string API_KEY = "API_KEY";
        
        public const string LOGINMAXTURN = "LOGIN_MAX_TURN"; // Số lần đăng nhập thất bại tối đa trước khi khóa tài khoản
        public const string OTP_MAX_TURN = "OTP_MAX_TURN"; // Số lần nhập OTP thất bại tối đa trước khi khóa tài khoản
        public const string SECOND = "SECOND"; // Số giây
        public const string DEFAULT_OTP = "DEFAULT_OTP"; // Mã OTP mặc định, có thể dùng cho mục đích kiểm thử hoặc trong trường hợp khẩn cấp khi hệ thống gửi OTP gặp sự cố.
        public const string OTP_LENGTH = "OTP_LENGTH"; // Độ dài mã OTP
        public const string OTP_RESEND_COOLDOWN = "OTP_RESEND_COOLDOWN"; // Thời gian chờ gửi lại OTP (giây)
    }
}