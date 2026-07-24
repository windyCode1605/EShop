using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.RoleModule
{
    public class AssignPermissionDto
    {
        [Required]
        public int RoleId { get; set; }
        [Required]
        [MinLength(1)]
        public List<PermissionKeyDto> PermissionKeys { get; set; }

    }
}