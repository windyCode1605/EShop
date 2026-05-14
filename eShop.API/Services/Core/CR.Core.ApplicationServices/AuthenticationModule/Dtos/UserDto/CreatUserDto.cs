using CR.Constants.Core.Users;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public UserTypeEnum UserType { get; set; }
    public int? CustomerId { get; set; }
    public List<int>? Roles { get; set; }
}