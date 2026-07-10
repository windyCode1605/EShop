using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Base;

namespace CR.Core.Domain.User
{
    [Table(nameof(Permission), Schema = DbSchemas.Default)]
    public class Permission : IEntity<string>
    {
        [Key]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string PermissionKey { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string DisplayName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string PermissionGroup { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }

        // Navigation property for 1-N relationship
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        string IEntity<string>.Id
        {
            get => PermissionKey;
            set => PermissionKey = value;
        }
    }
}
