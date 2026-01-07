using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<MongoDbUserRepository> _logger;

    public MongoDbUserRepository(MongoDbContext context, IOptions<MongoDbSettings> settings, ILogger<MongoDbUserRepository> logger)
    {
        this._users = context.GetCollection<UserDocument>(settings.Value.UsersCollectionName);
        _logger = logger;
        
        // Create unique index on email
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
            throw new NotFoundException($"User with ID '{id}' not found");
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
            throw new NotFoundException($"User with ID '{user.Id}' not found");
        }

        return user;
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<UserDocument> filter = Builders<UserDocument>.Filter.Eq(u => u.Id, id);
        DeleteResult result = await this._users.DeleteOneAsync(filter);

        if (result.DeletedCount == 0)
        {
            throw new NotFoundException($"User with ID '{id}' not found");
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

    private User MapToDomain(UserDocument document)
    {
        try
        {
            _logger.LogInformation($"Mapping document: ID={document.Id}");
            
            // Basic validations
            if (string.IsNullOrWhiteSpace(document.Id))
                throw new Exception("Document has empty ID");

            if (string.IsNullOrWhiteSpace(document.Email))
                throw new Exception("Document has empty email");

            if (string.IsNullOrWhiteSpace(document.Name))
                throw new Exception("Document has empty name");

            if (string.IsNullOrWhiteSpace(document.Role))
                throw new Exception("Document has empty role");

            // Create Email value object
            Email email;
            try
            {
                email = Email.Create(document.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating email: {ex.Message}");
                throw;
            }

            // Parse Role enum
            if (!Enum.TryParse<User.UserRole>(document.Role, true, out var role))
            {
                _logger.LogWarning($"Invalid role '{document.Role}', defaulting to User");
                role = User.UserRole.User;
            }

            DateTime? updatedAt = document.UpdatedAt;

            // Reconstruct entity
            var user = User.Reconstruct(
                id: document.Id,
                email: email,
                name: document.Name,
                role: role,
                isActive: document.IsActive,
                createdAt: document.CreatedAt,
                updatedAt: updatedAt
            );

            _logger.LogInformation($"Successfully mapped user: {user.Email!.Value}");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error mapping document to user: {ex.Message}");
            _logger.LogError($"   Document ID: {document?.Id ?? "null"}");
            _logger.LogError($"   Email: {document?.Email ?? "null"}");
            _logger.LogError($"   Name: {document?.Name ?? "null"}");
            _logger.LogError($"   Role: {document?.Role ?? "null"}");
            _logger.LogError($"   CreatedAt: {document?.CreatedAt}");
            _logger.LogError($"   UpdatedAt: {document?.UpdatedAt?.ToString() ?? "null"}");
            throw new Exception($"Failed to map user from database: {ex.Message}", ex);
        }
    }
}
