using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;

namespace CR.Core.Domain.Permission
{
    [Table(nameof(PermissionMax), Schema = DbSchemas.Default)]
    public class PermissionMax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Required]
        public required string PermissionKey { get; set; }
    }
}