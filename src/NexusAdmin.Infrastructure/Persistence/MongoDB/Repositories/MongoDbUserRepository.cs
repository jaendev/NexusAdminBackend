using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.Interfaces.Repositories;
using NexusAdmin.Core.ValueObjects;
using NexusAdmin.Infrastructure.Persistence.MongoDB.Configuration;

namespace NexusAdmin.Infrastructure.Persistence.MongoDB.Repositories;

public class MongoDbUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _users;

    public MongoDbUserRepository(MongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        this._users = context.GetCollection<UserDocument>(settings.Value.UsersCollectionName);
        
        // Create unique index in email
        IndexKeysDefinition<UserDocument> indexKeys = Builders<UserDocument>.IndexKeys.Ascending(u => u.Email);
        CreateIndexOptions indexOptions = new CreateIndexOptions() { Unique = true };
        CreateIndexModel<UserDocument> indexModel = new CreateIndexModel<UserDocument>(indexKeys, indexOptions);
        
        this._users.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<User> CreateAsync(User user)
    {
        UserDocument document = MapToDocument(user);
        await this._users.InsertOneAsync(document);
        return user;
    }

    public async Task<User> GetByIdAsync(string id)
    {
        FilterDefinition<UserDocument> filter = Builders<UserDocument>.Filter.Eq(u => u.Id, id);
        UserDocument document = await this._users.Find(filter).FirstOrDefaultAsync();

        if (document == null)
        {
            throw new NotFoundException($"User with id {id} not found");
        }

        return MapToDomain(document);
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        FilterDefinition<UserDocument> filter = Builders<UserDocument>.Filter.Eq(u => u.Email, email.Value);
        UserDocument document = await this._users.Find(filter).FirstOrDefaultAsync();

        return document != null ? MapToDomain(document) : null;
    }

    public async Task<List<User>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;
        
        List<UserDocument> documents = await this._users
            .Find(_ => true)
            .SortByDescending(u => u.CreatedAt)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        return documents.Select(MapToDomain).ToList();
    }

    public async Task<User> UpdateAsync(User user)
    {
        UserDocument document = MapToDocument(user);
        FilterDefinition<UserDocument> filter = Builders<UserDocument>.Filter.Eq(u => u.Id, user.Id);
        
        ReplaceOneResult result = await this._users.ReplaceOneAsync(filter, document);

        if (result.MatchedCount == 0)
        {
            throw new NotFoundException($"User with id {user.Id} not found");
        }

        return user;
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<UserDocument> filter = Builders<UserDocument>.Filter.Eq(u => u.Id, id);
        DeleteResult result = await this._users.DeleteOneAsync(filter);

        if (result.DeletedCount == 0)
        {
            throw new NotFoundException($"User with id {id} not found");
        }
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        FilterDefinition<UserDocument> filter = Builders<UserDocument>.Filter.Eq(u => u.Email, email.Value);
        long count = await this._users.CountDocumentsAsync(filter);
        return count > 0;
    }

    public async Task<int> CountAsync()
    {
        return (int)await this._users.CountDocumentsAsync(_ => true);
    }

    // Mappers
    private static UserDocument MapToDocument(User user)
    {
        return new UserDocument
        {
            Id = user.Id!,
            Email = user.Email!.Value,
            Name = user.Name!,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }

    private static User MapToDomain(UserDocument document)
    {
        Email email = Email.Create(document.Email);
        User.UserRole role = Enum.Parse<User.UserRole>(document.Role);
        
        return User.Reconstruct(
            document.Id,
            email,
            document.Name,
            role,
            document.IsActive,
            document.CreatedAt,
            document.UpdatedAt!.Value
        );
    }
}