namespace CR.InfrastructureBase.Exceptions
{
    public class BaseException : Exception
    {
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public readonly int ErrorCode;

        /// <summary>
        /// Chuỗi cần localize sẽ tra trong từ điển, nếu có truyền chuỗi này thì
        /// sẽ không lấy message của error nữa mà lấy theo chuỗi này
        /// </summary>
        [Obsolete("Bỏ không dùng")]
        public readonly string? MessageLocalize;

        /// <summary>
        /// Chuỗi cần trả ra (Không localize chỉ dùng chung 1 biến ErrorCode)
        /// </summary>
        public string? ErrorMessage;

        /// <summary>
        /// Mảng chuỗi cần chả
        /// </summary>
        public string[]? ListParam;

        /// <summary>
        /// Dữ liệu trả về (không localize)
        /// </summary>
        public new object? Data { get; set; }

        public BaseException(int errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        public BaseException(int errorCode, params string[] listParam)
            : base()
        {
            ErrorCode = errorCode;
            ListParam = listParam;
        }

        public BaseException(int errorCode, object data, params string[] listParam) : base()
        {
            ErrorCode = errorCode;
            ListParam = listParam;
            Data = data;
        }
    }
}
