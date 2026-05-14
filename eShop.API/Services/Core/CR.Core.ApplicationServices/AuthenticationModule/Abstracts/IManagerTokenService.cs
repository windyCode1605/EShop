namespace CR.Core.ApplicationServices.AuthenticationModule.Abstracts;

public interface IManagerTokenService
{
    /// <summary>
    /// Thu hồi tất cả token
    /// </summary>
    /// <returns></returns>
    Task RevokeAllToken();

    /// <summary>
    /// Thu hồi toàn bộ token đăng nhập theo sub
    /// </summary>
    /// <returns></returns>
    Task RevokeAllTokenBySubject();

    /// <summary>
    /// Thu hồi tất cả token trừ token đang sử dụng hiện tại
    /// </summary>
    /// <returns></returns>
    Task RevokeOtherToken();
}
