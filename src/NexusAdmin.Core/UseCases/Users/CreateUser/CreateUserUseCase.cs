using System;
using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.Interfaces.Repositories;
using NexusAdmin.Core.Interfaces.Services;
using NexusAdmin.Core.ValueObjects;

namespace NexusAdmin.Core.UseCases.Users.CreateUser;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public CreateUserUseCase(IUserRepository userRepository, IEmailService emailService)
    {
        this._userRepository = userRepository;
        this._emailService = emailService;
    }

    public async Task<CreateUserResponse> ExecuteAsync(CreateUserRequest request)
    {
        // Validate and create email value object
        Email? email = Email.Create(request.Email);
        
        // Check if user already exists
        if (await this._userRepository.ExistsByEmailAsync(email))
        {
            throw new UserAlreadyExistsException(
                $"A user with email '{email.Value}' already exists"
            );
        }
        
        // Create domain entity
        User user = User.Create(email, request.Name, request.Role);
        
        // Persist to database
        User createdUser = await this._userRepository.CreateAsync(user);
        
        // Send welcome email (non-critical operation)
        try
        {
            await this._emailService.SendWelcomeEmailAsync(
                createdUser.Email!.Value,
                createdUser.Name
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send welcome email: {ex.Message}");
        }

        return new CreateUserResponse
        {
            Id = createdUser.Id,
            Email = createdUser.Email!.Value,
            Name = createdUser.Name,
            Role = createdUser.Role.ToString(),
            IsActive = createdUser.IsActive,
            CreatedAt = createdUser.CreatedAt
        };
    }
}
