using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CR.Constants.Common.Database;
using CR.Constants.Core.Users;

namespace CR.Core.Domain.User
{
    [Table(nameof(UserProfile), Schema = DbSchemas.Default)]
    public class UserProfile
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