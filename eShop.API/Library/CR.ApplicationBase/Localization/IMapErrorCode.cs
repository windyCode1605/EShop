// Contract để map error code -> message theo ngôn ngữ 
// Dùng khi hệ thống hỗ trợ đa ngôn ngữ (vi/en)
namespace CR.ApplicationBase.Localization;
public interface IMapErrorCode
{
    /// <summary>
        /// Lấy error message
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        string GetErrorMessage(int errorCode);

        /// <summary>
        /// Lấy message key cho error code
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        string GetErrorMessageKey(int errorCode);

        /// <summary>
        /// Thử lấy error message nếu có
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        string? TryGetErrorMessage(int errorCode);
}