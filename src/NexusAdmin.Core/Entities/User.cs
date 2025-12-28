using System;
using System.ComponentModel.DataAnnotations;
using NexusAdmin.Core.ValueObjects;

namespace NexusAdmin.Core.Entities;

public class User
{
    public string? Id { get; private set; }
    public Email? Email { get; private set; }
    public string? Name { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Private constructor for avoid directly creation
    private User() { }
    
    // Factory method for user creation
    public static User Create(Email email, string name, UserRole role)
    {
        ValidateName(name);

        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name.Trim(),
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    // Factory method for reconstruct from DB
    public static User Reconstruct(
        string id,
        Email email,
        string name,
        UserRole role,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt
        )
    {
        return new User
        {
            Id = id,
            Email = email,
            Name = name,
            Role = role,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
    
    // Business methods

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole newRole)
    {
        if (Role == newRole)
        {
            throw new ValidationException("The user is already this role");
        }
        
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw new ValidationException("The user is already active");
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new ValidationException("The user is already inactive");
        }

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Private validations
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Name can't be empty");
        }

        if (name.Trim().Length < 3)
        {
            throw new ValidationException("Name can't be less than 3 characters");
        }

        if (name.Trim().Length > 100)
        {
            throw new ValidationException("Name can't be more than 100 characters");
        }
    }

    public enum UserRole
    {
        User = 0,
        Manager = 1,
        Admin = 2
    }
}