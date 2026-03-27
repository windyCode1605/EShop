// Contract để map error code -> message theo ngôn ngữ 
// Dùng khi hệ thống hỗ trợ đa ngôn ngữ (vi/en)
namespace CR.ApplicationBase.Localization;
public interface IMapErrorCode
{
    string GetMessage(string errorCode, string lang = "vi");
}