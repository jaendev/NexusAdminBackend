using System.Threading.Tasks;
using NexusAdmin.Core.Interfaces.Repositories;

namespace NexusAdmin.Core.UseCases.Users.GetUser;

public class GetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUserResponse> ExecuteAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return new GetUserResponse
        {
            Id = user.Id,
            Email = user.Email.Value,
            Name = user.Name,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt 
        };
    }
}