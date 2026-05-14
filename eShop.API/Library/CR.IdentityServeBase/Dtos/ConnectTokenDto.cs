using Microsoft.AspNetCore.Mvc;

namespace CR.IdentityServerBase.Dto
{
    /// <summary>
    /// Đối tượng truyền tải dữ liệu (DTO) chứa thông tin yêu cầu cấp phát Token từ Client.
    /// Dữ liệu bắt buộc gửi lên theo định dạng x-www-form-urlencoded.
    /// </summary>
    public class ConnectTokenDto
    {
        /// <summary>
        /// (Bắt buộc) Phương thức xác thực muốn sử dụng. 
        /// Các giá trị phổ biến: "password", "client_credentials", "authorization_code", "refresh_token".
        /// </summary>
        [FromForm(Name = "grant_type")]
        public string GrantType { get; set; } = null!;

        /// <summary>
        /// Mã xác thực (Authorization Code) được trả về từ máy chủ sau khi user đăng nhập thành công.
        /// Chỉ sử dụng trong luồng Authorization Code (dành cho Web SPA / Mobile dùng PKCE).
        /// </summary>
        [FromForm(Name = "code")]
        public string? Code { get; set; }

        /// <summary>
        /// Chuỗi băm bảo mật đi kèm với Authorization Code để chống đánh cắp mã.
        /// Thuộc cơ chế PKCE (Proof Key for Code Exchange).
        /// </summary>
        [FromForm(Name = "code_verifier")]
        public string? CodeVerifier { get; set; }

        /// <summary>
        /// Tên đăng nhập hoặc Email của người dùng.
        /// Chỉ bắt buộc khi grant_type = "password".
        /// </summary>
        [FromForm(Name = "username")]
        public string? Username { get; set; }

        /// <summary>
        /// Mật khẩu của người dùng.
        /// Chỉ bắt buộc khi grant_type = "password".
        /// </summary>
        [FromForm(Name = "password")]
        public string? Password { get; set; }

        /// <summary>
        /// Danh sách các quyền (phạm vi truy cập) mà Client muốn xin cấp phát, phân cách bằng dấu cách.
        /// VD: "openid profile email offline_access api.read".
        /// </summary>
        [FromForm(Name = "scope")]
        public string? Scope { get; set; }

        /// <summary>
        /// Mã định danh của ứng dụng gọi API (Client Application).
        /// Dùng để định danh các Microservices gọi nhau (client_credentials) hoặc xác định App nào đang xin Token.
        /// </summary>
        [FromForm(Name = "client_id")]
        public string? ClientId { get; set; }

        /// <summary>
        /// Khóa bí mật của ứng dụng gọi API.
        /// Bắt buộc đi kèm với client_id trong luồng client_credentials. Không dùng cho Web SPA/Mobile (Public Client).
        /// </summary>
        [FromForm(Name = "client_secret")]
        public string? ClientSecret { get; set; }

        /// <summary>
        /// (Tùy chỉnh) Mã thiết bị của Firebase Cloud Messaging.
        /// Dùng để Backend lưu lại và bắn Push Notification (Android/Cross-platform).
        /// </summary>
        [FromForm(Name = "fcm_token")]
        public string? FcmToken { get; set; }

        /// <summary>
        /// (Tùy chỉnh) Mã thiết bị của Apple Push Notification Service.
        /// Dùng để Backend lưu lại và bắn Push Notification (dành riêng cho iOS).
        /// </summary>
        [FromForm(Name = "apns_token")]
        public string? ApnsToken { get; set; }
    }
}