namespace CR.InfrastructureBase.Exceptions
{
    /// <summary>
    /// Ngoại lệ cấp người dùng
    /// </summary>
    //[Obsolete("Chuyển qua dùng result pattern")]
    public class UserFriendlyException : BaseException
    {
        /// <summary>
        /// Object cần trả ra
        /// </summary>
        public object? DataValue { get; set; }
        public UserFriendlyException(int errorCode)
            : base(errorCode) { }

        public UserFriendlyException(int errorCode, params string[] listParam)
            : base(errorCode, listParam) { }

        public UserFriendlyException(int errorCode, object? dataValue) : base(errorCode)
        {
            DataValue = dataValue;
        }
    }
}
