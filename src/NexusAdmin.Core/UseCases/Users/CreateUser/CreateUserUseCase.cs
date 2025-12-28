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
        // Validate and create value object
        Email? email = Email.Create(request.Email);
        
        // Verify if exist
        if (await this._userRepository.ExistsByEmailAsync(email))
        {
            throw new UserAlreadyExistsException(
                $"Is already exists with email {email.Value}"
            );
        }
        
        // Create domain entity
        User user = User.Create(email, request.Name, request.Role);
        
        // Persist
        User createdUser = await this._userRepository.CreateAsync(user);
        
        // Send welcome Email
        try
        {
            await this._emailService.SendWelcomeEmailAsync(
                createdUser.Email!.Value,
                createdUser.Name
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
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