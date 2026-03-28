using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.AuthToken
{
    [Table(nameof(NotificationToken), Schema = DbSchemas.Default)]
    public class NotificationToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// FcmToken là token dùng để gửi thông báo đến thiết bị Android, 
        /// ApnsToken là token dùng để gửi thông báo đến thiết bị iOS. 
        /// Tuy nhiên, hiện tại hệ thống chỉ sử dụng FcmToken để gửi thông báo đến cả hai loại thiết bị,
        ///  do đó ApnsToken đã được đánh dấu là obsolete và sẽ không còn được sử dụng trong tương lai. 
        /// Việc này giúp đơn giản hóa quá trình quản lý token và giảm thiểu sự phức tạp trong việc gửi thông báo đến các thiết bị khác nhau.
        /// </summary>
        [MaxLength(128)]
        [Unicode(false)]
        public string? FcmToken { get; set; }

        [MaxLength(128)]
        [Unicode(false)]
        [Obsolete("bỏ chỉ cần dùng FcmToken")]
        public string? ApnsToken { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; } = null!;
    }
}