namespace CR.Constants.Common.SysVar
{
    /// <summary>
    /// Tên các nhóm sysvar : Nhóm này có tác dụng phân loại các sysvar theo chức năng hoặc mục đích sử dụng,
    ///  giúp quản lý và truy xuất sysvar dễ dàng hơn trong hệ thống.
    /// ví dụ : GrName = "Authentication" có thể chứa các sysvar liên quan đến xác thực người dùng, 
    /// như thời gian hết hạn token, số lần đăng nhập thất bại tối đa, v.v.
    /// </summary>
    public static class GrNames
    {
        public const string EKYC = "EKYC";
        public const string OTP = "OTP";
        public const string AUTHMAXTURN = "AUTH_MAX_TURN";
        public const string USER_FORGOT_PASSWORD = "USER_FORGOT_PASSWORD";
    }
}