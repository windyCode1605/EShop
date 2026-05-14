namespace CR.Constants.Core.Users
{
    public static class UserType
    {
        public const int SUPER_ADMIN = 1;
        public const int ADMIN = 2;
        public const int CUSTOMER = 3;
        public static readonly int[] ALL_ADMIN = [SUPER_ADMIN, ADMIN];
    }
    public enum UserTypeEnum
    {
        SUPER_ADMIN = 1,
        ADMIN = 2,
        CUSTOMER = 3,
    }
}