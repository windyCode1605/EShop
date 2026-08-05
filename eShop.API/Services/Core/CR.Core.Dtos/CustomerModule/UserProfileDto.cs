using System.ComponentModel.DataAnnotations;
using CR.Constants.Core.Users;

namespace CR.Core.Dtos.CustomerModule
{
    public class UserProfileDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? FullName { get; set; }
        [Required]
        public required string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderTypes? Gender { get; set; }

        public string? AvatarUrl { get; set; }
    }

}