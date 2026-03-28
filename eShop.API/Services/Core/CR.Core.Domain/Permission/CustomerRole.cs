using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Core.Users;
using CR.EntitiesBase.interfaces;

namespace CR.Core.Domain.Permission
{
    /// <summary>
    /// Vai trò của khách hàng
    /// </summary>
    [Table(nameof(CustomerRole), Schema = DbSchemas.Default)]
    public class CustomerRole : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// Tên vai trò
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// UserType
        /// </summary>
        public UserTypeEnum UserType { get; set; }

        /// <summary>
        /// Id của khách hàng
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Mô tả 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set;}

        /// <summary>
        /// Quyền thuộc web nào 
        /// </summary>
        public int PermissionInWeb { get; set; }

        public List<CustomerUserRole> CustomerUserRoles { get; set; } = new();
        public List<CustomerRolePermission> CustomerRolePermissions { get; set; } = new();

        public DateTime? ModifiedDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? ModifiedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? DeletedDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? DeletedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Deleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? CreatedDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}