using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Bảng lưu thông tin gửi OTP cho người dùng,
/// bao gồm số lần gửi và thời gian gửi cuối cùng
/// </summary>
namespace CR.Core.Domain.Opts
{
    [Table(nameof(SendOtp), Schema = DbSchemas.Default)]
    [Index(nameof(Email), IsUnique = false, Name = $"IX_{nameof(SendOtp)}")]
    public class SendOtp
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập hoặc email của người dùng
        /// </summary>
        [Unicode(false)]
        [MaxLength(128)]
        public required string Email { get; set; }

        /// <summary>
        /// Số lần gửi đếm từ 0 , khi đạt giới hạn sẽ không cho gửi nữa
        /// </summary>
        public int SendCount { get; set; } = 0;

        /// <summary>
        /// Thời gian gửi OTP cuối cùng, dùng để kiểm tra thời gian chờ giữa
        /// các lần gửi OTP, tránh việc spam liên tục
        /// </summary>
        public DateTime LastSentDateTime { get; set; }

        /// <summary>
        /// Khoảng thời gian mà người dùng phải chờ trước khi có thể gửi lại OTP nếu đã đạt giới hạn gửi
        /// </summary>
        public DateTime TimeLimitCanVerifyOtp { get; set; }
    }
}