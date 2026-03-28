using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.interfaces;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.Permission
{
    /// <summary>
    /// Bảng trung gian giữa vai trò và quyền của khách hàng
    /// </summary>
    [Table(nameof(CustomerRolePermission), Schema = DbSchemas.Default)]
    public class CustomerRolePermission : ICreatedBy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomerRoleId { get; set; }
        public CustomerRole CustomerRole { get; set; } = null!;
        
        /// <summary>
        /// Tên Permission
        /// <see cref="PermissionConfig"/> 
        /// </summary>
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string PermissionKey { get ; set ; } = null!;
        public DateTime? CreatedDate { get ; set ; }
        public int? CreatedBy { get ; set ; }
    }
}