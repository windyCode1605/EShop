using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;

namespace CR.Core.API.Helpers;

/// <summary>
/// Tập hợp các hàm tiện ích dùng chung cho OAuth2 / OpenIddict flow.
/// Không phụ thuộc vào bất kỳ service nào → có thể test dễ dàng.
/// </summary>
internal static class OAuthHelpers
{
    /// <summary>
    /// Phân tích các tham số OAuth từ HttpContext (Form hoặc QueryString),
    /// loại bỏ những key nằm trong danh sách <paramref name="excluding"/>.
    /// </summary>
    internal static IDictionary<string, StringValues> ParseOAuthParameters(
        HttpContext httpContext,
        IReadOnlyList<string>? excluding = null)
    {
        excluding ??= [];

        return httpContext.Request.HasFormContentType
            ? httpContext.Request.Form
                .Where(v => !excluding.Contains(v.Key))
                .ToDictionary(v => v.Key, v => v.Value)
            : httpContext.Request.Query
                .Where(v => !excluding.Contains(v.Key))
                .ToDictionary(v => v.Key, v => v.Value);
    }

    /// <summary>
    /// Xây dựng URL chuyển hướng sau khi xác thực thất bại,
    /// bao gồm các tham số OAuth cần thiết để tiếp tục luồng.
    /// </summary>
    internal static string BuildRedirectUrl(
        HttpRequest request,
        IDictionary<string, StringValues> oAuthParameters)
        => request.PathBase + request.Path + QueryString.Create(oAuthParameters);

    /// <summary>
    /// Kiểm tra xem cookie session đã xác thực hay chưa.
    /// Nếu request có MaxAge, kiểm tra thêm thời hạn hợp lệ của session.
    /// </summary>
    internal static bool IsAuthenticated(
        AuthenticateResult authenticateResult,
        OpenIddictRequest request)
    {
        if (!authenticateResult.Succeeded)
            return false;

        if (request.MaxAge.HasValue && authenticateResult.Properties is not null)
        {
            var maxAge = TimeSpan.FromSeconds(request.MaxAge.Value);
            var expired = !authenticateResult.Properties.IssuedUtc.HasValue
                || DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc > maxAge;

            if (expired) return false;
        }

        return true;
    }
}
