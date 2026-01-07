using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Interfaces.Repositories;

namespace NexusAdmin.Core.UseCases.Users.UpdateUser;

public class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    public async Task<UpdateUserResponse> ExecuteAsync(string userId, UpdateUserRequest request)
    {
        // Get existing user
        User user = await this._userRepository.GetByIdAsync(userId);
        
        // Update user fields if provided
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.UpdateName(request.Name);
        }

        if (request.Role.HasValue && request.Role != user.Role)
        {
            user.ChangeRole(request.Role.Value);
        }
        
        // Save updated user
        User updatedUser = await this._userRepository.UpdateAsync(user);
        
        return new UpdateUserResponse
        {
            Id = updatedUser.Id!,
            Email = updatedUser.Email!.Value,
            Name = updatedUser.Name!,
            Role = updatedUser.Role.ToString(),
            IsActive = updatedUser.IsActive,
            UpdatedAt = updatedUser.UpdatedAt!.Value 
        };
    }
}
