namespace CR.Constants.ErrorCodes
{
    public static class ErrorCode
    {
        // Quy ước: mỗi domain nghiệp vụ sở hữu một dải 1000 mã riêng biệt
        // để tránh xung đột (collision) khi mở rộng thêm mã lỗi mới.
        public const int BadRequest = 400;

        // ===== Authentication & User (1xxx) =====
        public const int UsernameOrPasswordIncorrect = 1000;
        public const int UserNotFound = 1001;
        public const int UserIsDeactive = 1003;
        public const int InvalidUserType = 1004;
        public const int UserOldPasswordIncorrect = 1005;
        public const int UserNotHavePermission = 1006;
        public const int UserStatusIsInvalid = 1008;
        public const int UserIsVerify = 1009;
        public const int UserIsRequestVerify = 1010;
        public const int UserIsRegistered = 1011;
        public const int OptCodeNotValid = 1013;
        public const int OptCodeIsExpired = 1014;
        public const int TokenIsInvalid = 1031;
        public const int UserRegisterExistPersonalEmail = 1018;
        public const int UserRegisterExistUsername = 1019;
        public const int UserLoginUserTypeInvalid = 1025;
        public const int UserIsLock = 1027;
        public const int UserCurrentPasswordIncorrect = 1028;
        public const int UserIsInactiveBecauseMultiLoginTime = 1029;
        public const int UserProfileNotFound = 1030;
        public const int AppPasswordIncorrect = 1023;

        //  System / Config (2xxx) 
        public const int SysVarsIsNotConfig = 2000;

        //  Unhandled / Unknown (4xxx) 
        public const int UnknownError = 4013;

        //  Cart & Stock (5xxx) 
        public const int CartEmpty = 5000;
        public const int InsufficientStock = 5001;

        //  Address (6xxx) 
        public const int AddressNotFound = 6000;
        public const int AddressRequired = 6001;

        //  Coupon (7xxx) 
        public const int CouponNotFound = 7000;
        public const int CouponExpired = 7001;
        public const int CouponUsageLimitReached = 7002;
        public const int CouponMinOrderNotMet = 7003;

        //  Paging (8xxx) 
        public const int PAGING_INVALID = 8000;

        // Order (9xxx) 
        public const int OrderNotFound = 9000;
        public const int OrderStatusInvalid = 9001;
        public const int OrderCannotBeCancelled = 9002;

        //  Shipment (10xxx) 
        public const int ShipmentNotFound = 10000;

        //  Product Variant (11xxx) 
        public const int ProductVariantNotFound = 11000;
        public const int InternalServerError = 11001;

        //  Role & Permission (12xxx) 
        public const int RoleNotFound = 12001;
        public const int UserHasNoRoleAssigned = 12002;
        public const int InvalidInput = 12003;
        public const int PermissionKeyInvalid = 12004;
        public const int RoleAlreadyExists = 12005;

        //  Payment (13xxx) — tách riêng khỏi Order để tránh trùng mã 
        public const int PaymentNotFound = 13000;
        public const int PaymentCannotRefund = 13001;

        //  Category (14xxx) 
        public const int CategoryNotFound = 14000;
    }
}