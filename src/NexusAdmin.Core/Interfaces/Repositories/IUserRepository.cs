using System.Collections.Generic;
using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.ValueObjects;

namespace NexusAdmin.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(Email email);
    Task<List<User>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(string id);
    Task<bool> ExistsByEmailAsync(Email email);
    Task<int> CountAsync();
}