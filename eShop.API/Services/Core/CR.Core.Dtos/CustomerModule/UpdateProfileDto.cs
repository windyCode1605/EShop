using CR.Constants.Core.Users;

namespace CR.Core.Dtos.CustomerModule
{
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderTypes? Gender { get; set; }
        public string? AvatarUrl { get; set; }
    }
}