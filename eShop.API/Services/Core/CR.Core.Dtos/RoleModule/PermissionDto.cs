using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.RoleModule
{
    public class PermissionKeyDto
    {
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9\.]+$")]
        public required string PermissionKey { get; set; }
        [StringLength(255)]
        public required string DisplayName { get; set; }
        public string? Description { get; set; }
    }

    public class PermissionGroupDto
    {
        public required string PermissionGroupName { get; set; }
        public List<PermissionKeyDto> permissionKeyDtos = new List<PermissionKeyDto>();
    }
}