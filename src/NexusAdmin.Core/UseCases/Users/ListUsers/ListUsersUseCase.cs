using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Interfaces.Repositories;

namespace NexusAdmin.Core.UseCases.Users.ListUsers;

public class ListUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public ListUsersUseCase(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    public async Task<ListUsersResponse> ExecuteAsync(ListUsersRequest request)
    {
        // Validate params
        int page = request.Page <= 0 ? 1 : request.Page;
        int pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = Math.Min(pageSize, 100);

        // Get users from repository
        List<User> users = await this._userRepository.GetAllAsync(page, pageSize);
        int totalCount = await this._userRepository.CountAsync();

        List<UserDto> userDtos = users.Select(u => new UserDto
        {
            Id = u.Id!,
            Email = u.Email!.Value,
            Name = u.Name!,
            Role = u.Role.ToString(),
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt 
        }).ToList();

        return new ListUsersResponse
        {
            Users = userDtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}
