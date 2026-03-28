using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Core.Domain.User;
using CR.EntitiesBase.interfaces;

namespace CR.Core.Domain.Permission
{
    /// <summary>
    /// Bảng trung gian giữa vai trò và khách hàng
    /// </summary>
    [Table(nameof(CustomerUserRole), Schema = DbSchemas.Default)]
    public class CustomerUserRole : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Id người dùng
        /// </summary>
        public int UserId { get; set; }
        public Users User { get; set; } = null!;

        /// <summary>
        /// Id vai trò
        /// </summary>
        public int CustomerRoleId { get; set; }
        public CustomerRole CustomerRole { get; set; } = null!;

        public DateTime? CreatedDate { get ; set ; }
        public int? CreatedBy { get ; set ; }
        public DateTime? ModifiedDate { get ; set ; }
        public int? ModifiedBy { get ; set ; }
        public DateTime? DeletedDate { get ; set ; }
        public int? DeletedBy { get ; set ; }
        public bool Deleted { get ; set ; }
    }
}