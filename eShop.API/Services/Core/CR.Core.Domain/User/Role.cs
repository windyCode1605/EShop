using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Authorization.Role;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CR.Core.Domain.User
{
    [Table(nameof(Role), Schema = DbSchemas.Default)]
    public class Role : IRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get ; set; }
        [Required]
        [MaxLength(256)]
        public required string Name { get ; set ; }
        [MaxLength(1024)]
        public string? Description { get ; set ; }

        public int UserType { get ; set ; }
        public  int Status { get ; set ; }
        /// <summary>
        /// Quyền thuộc web nào 
        /// <see cref="PermissionInWeb"/> 
        /// </summary>
        public int PermissionInWeb { get ; set ; }
    
        public DateTime? CreatedDate { get ; set ; }
        public int? CreatedBy { get ; set ; }
        public DateTime? ModifiedDate { get ; set ; }
        public int? ModifiedBy { get ; set ; }
        public DateTime? DeletedDate { get ; set ; }
        public int? DeletedBy { get ; set ; }
        public bool Deleted { get ; set ; }
    }
}