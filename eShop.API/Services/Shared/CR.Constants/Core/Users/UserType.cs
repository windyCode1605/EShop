namespace CR.Constants.Core.Users
{
    public static class UserType
    {
        public const int SUPPER_ADMIN = 1;
        public const int ADMIN = 2;
        public const int CUSTOMER = 3;
        public static readonly int[] ALL_ADMIN = [SUPPER_ADMIN, ADMIN];
    }
    public enum UserTypeEnum
    {
        SUPPER_ADMIN = 1,
        ADMIN = 2,
        CUSTOMER = 3,
    }
}