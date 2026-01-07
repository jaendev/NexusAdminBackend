using NexusAdmin.Core.Entities;

namespace NexusAdmin.Core.UseCases.Users.UpdateUser;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public User.UserRole? Role { get; set; }
}