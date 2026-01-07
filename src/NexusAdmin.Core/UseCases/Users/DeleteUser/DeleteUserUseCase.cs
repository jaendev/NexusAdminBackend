using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.Interfaces.Repositories;

namespace NexusAdmin.Core.UseCases.Users.DeleteUser;

public class DeleteUserUseCase
{
    private readonly IUserRepository _userRepository;

    public DeleteUserUseCase(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    public async Task ExecuteAsync(string userId)
    {
        User user = await this._userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }

        await this._userRepository.DeleteAsync(userId);
    }
}