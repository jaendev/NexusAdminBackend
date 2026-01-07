using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.Interfaces.Repositories;

namespace NexusAdmin.Core.UseCases.Users.ActivateUser;

public class ActivateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public ActivateUserUseCase(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }
    
    public async Task<ActivateUserResponse> ExecuteAsync(string userId)
    {
        // Get existing user
        User user = await this._userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }
        
        // Activate user
        user.Activate();
        
        // Save updated user
        await this._userRepository.UpdateAsync(user);

        return new ActivateUserResponse
        {
            Id = user.Id!,
            Email = user.Email!.Value,
            Name = user.Name!,
            IsActive = user.IsActive,
            UpdatedAt = user.UpdatedAt!.Value 
        };
    }
}
