using System;

namespace NexusAdmin.Core.UseCases.Users.ActivateUser;

public class ActivateUserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
}