namespace CR.Constants.ErrorCodes
{
    public static class ErrorCode
    {
        //Phân cách các nhóm mã lỗi theo hàng trăm để dễ thêm mới và quản lý
        public const int BadRequest = 400;

        // Authentication error codes (1xxx series)
        public const int UsernameOrPasswordIncorrect = 1000;
        public const int UserNotFound = 1001;
        public const int UserIsDeactive = 1003;                     // User là inactive (đã bị khóa hoặc chưa kích hoạt)
        public const int InvalidUserType = 1004;                    // User type không hợp lệ
        public const int UserOldPasswordIncorrect = 1005;          // Mật khẩu cũ không đúng khi đổi mật khẩu
        public const int UserNotHavePermission = 1006;              
        public const int UserStatusIsInvalid = 1008;  
        public const int UserIsVerify = 1009;
        public const int UserIsRequestVerify = 1010;
        public const int UserIsRegistered = 1011;        
        public const int OptCodeNotValid = 1013;
        public const int OptCodeIsExpired = 1014;
        public const int UserRegisterExistPersonalEmail = 1018;
        public const int UserLoginUserTypeInvalid = 1025;
        public const int UserIsLock = 1027;
        public const int UserCurrentPasswordIncorrect = 1028;
        public const int UserIsInactiveBecauseMultiLoginTime = 1029;  // Account locked (5 failed attempts)
        public const int AppPasswordIncorrect = 1023;                   // With attempt counter
        // User



        public const int SysVarsIsNotConfig = 2000; // sysVar không được cấu hình, cần cấu hình sysVar với GrNames.LOGINMAXTURN và varName tương ứng để lấy số lần đăng nhập thất bại tối đa


        public const int UnknownError = 4013; // Lỗi không xác định, có thể dùng cho các trường hợp ngoại lệ chưa được xử lý cụ thể
    
    
        public const int CartEmpty = 5000; // Giỏ hàng trống
        public const int InsufficientStock = 5001; // Sản phẩm không đủ tồn kho

        public const int AddressNotFound = 6000; // Địa chỉ không tồn tại
        public const int AddressRequired = 6001; // Địa chỉ giao hàng là bắt buộc khi tạo đơn hàng

        public const int CouponNotFound = 7000; // Mã giảm giá không tồn tại
        public const int CouponExpired = 7001; // Mã giảm giá đã hết hạn
        public const int CouponUsageLimitReached = 7002; // Mã giảm giá đã đạt giới hạn sử dụng
        public const int CouponMinOrderNotMet = 7003; // Đơn hàng không đạt giá trị tối thiểu để áp dụng mã giảm giá

        public  const int PAGING_INVALID = 8000; // Thông tin phân trang không hợp lệ (ví dụ: pageNumber < 1 hoặc pageSize < 1)
        public const int OrderNotFound = 9000; // Đơn hàng không tồn tại
        public const int OrderStatusInvalid = 9001; // Trạng thái đơn hàng không hợp lệ
        public const int OrderCannotBeCancelled = 9002; // Đơn hàng không thể hủy (ví dụ: đã giao hàng hoặc đang trong quá trình giao hàng)
    
        public const int PaymentNotFound = 9000; // Thông tin thanh toán không tồn tại
    }
}
