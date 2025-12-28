namespace NexusAdmin.Core.UseCases.Users.CreateUser;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}