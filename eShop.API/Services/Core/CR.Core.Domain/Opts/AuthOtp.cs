using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using CR.Constants.Common.Database;
using CR.Core.Domain.User;
using CR.EntitiesBase;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CR.Core.Domain.Opts
{
    [Table(nameof(AuthOtp), Schema = DbSchemas.Default)]
    public class AuthOtp : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// Otp code đã được mã hóa (hash)
        /// </summary>
        [MaxLength(128)]
        [Unicode(false)]
        public required string OtpCode { get; set; }

        /// <summary>
        /// Thời giạn hết hạn của mã OTP
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// Check xem mã OTP đã được sử dụng hay chưa
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Otp cho User nào
        /// </summary>
        public int UserId { get; set; }
        public Users User { get; set; } = null!;
        
        /// <summary>
        /// số lần verify 
        /// </summary>
        public int VerifyTime { get; set; } 
    }
}