using System.Runtime.CompilerServices;
using Azure.Identity;

/// <summary>
/// ObjectUtils : File này chứa các hàm tiện ích để thao tác với object
/// </summary>
namespace CR.Utils.DataUtils;
public static class ObjectUtils
{
    /// <summary>
    /// GetCurrentMethodInfo: Hàm này dùng để lấy thông tin về method đang được gọi, 
    /// bao gồm file path, line number và member name. Dùng để log trace khi có lỗi xảy ra.
    /// </summary>
    /// <param name="obj"> Đối tượng gọi hàm </param>
    /// <param name="lineNumber"> Số dòng trong file </param>
    /// <param name="filePath"> Đường dẫn file </param>
    /// <param name="memberName"> Tên member </param>
    /// <returns> Chuỗi thông tin method </returns>
    public static string GetCurrentMethodInfo(
        this object obj,
        [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string filePath = null!,
        [CallerMemberName] string memberName = null!
    )
    {
        return $"\nTrace: {filePath} (line {lineNumber}) - {memberName}\n";
    }
}