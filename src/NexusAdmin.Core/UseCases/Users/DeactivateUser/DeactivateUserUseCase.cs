using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.Interfaces.Repositories;

namespace NexusAdmin.Core.UseCases.Users.DeactivateUser;

public class DeactivateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserUseCase(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }
    
    public async Task<DeactivateUserResponse> ExecuteAsync(string userId)
    {
        // Get existing user
        User user = await this._userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }
        
        // Deactivate user
        user.Deactivate();
        
        // Save updated user
        User updatedUser = await this._userRepository.UpdateAsync(user);
        
        return new DeactivateUserResponse
        {
            Id = updatedUser.Id!,
            Email = updatedUser.Email!.Value,
            Name = updatedUser.Name!,
            IsActive = updatedUser.IsActive,
            UpdatedAt = updatedUser.UpdatedAt!.Value 
        };
    }
}
