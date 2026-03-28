using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.EntitiesBase.Entities;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.Domain.User
{
    [Table(nameof(RolePermission), Schema = DbSchemas.Default)]
    public class RolePermission : IRolePermission<int>  
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public  Role Role { get; set; } = null!;
        
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string PermissionKey { get ; set ; } = null!;
        public DateTime? CreatedDate { get ; set ; }
        public int? CreatedBy { get ; set ; }
        int IRolePermission<int>.Role { get ; set ; }
    }
}