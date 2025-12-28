using System;

namespace NexusAdmin.Core.UseCases.Users.CreateUser;

public class CreateUserResponse
{
    public string? Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}