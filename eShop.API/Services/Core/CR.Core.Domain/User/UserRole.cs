using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.User;
using CR.EntitiesBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.User
{
    [Table(nameof(UserRole), Schema = DbSchemas.Default)]
    [Index(nameof(UserId), nameof(RoleId), nameof(Deleted), Name = $"IX_{nameof(UserRole)}")]
    public class UserRole : IUserRole<int, int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Người dùng được cấu hình vai trò
        /// </summary>
        public Users User { get; set; } = null!;

        /// <summary>
        /// Vai trò thuộc người dùng
        /// </summary>
        public Role Role { get; set; } = null!;
        int IUserRole<int, int>.User { get ; set ; }
        int IUserRole<int, int>.Role { get ; set ; }
    }
}
