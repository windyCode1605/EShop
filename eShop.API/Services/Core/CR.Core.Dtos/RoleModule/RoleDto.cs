using System.ComponentModel.DataAnnotations;

namespace CR.Core.Dtos.RoleModule
{
    public class CreateRoleRequest
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<PermissionKeyDto>? PermissionKeys { get; set; }
    }
    public class RoleDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<PermissionKeyDto> PermissionKeys { get; set; } = new();
    }
}