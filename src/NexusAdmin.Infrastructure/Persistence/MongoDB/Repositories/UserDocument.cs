using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NexusAdmin.Infrastructure.Persistence.MongoDB.Repositories;

public class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("email")]
    [BsonRequired]
    public string Email { get; set; } = string.Empty;

    [BsonElement("name")]
    [BsonRequired]
    public string Name { get; set; } = string.Empty;

    [BsonElement("role")]
    [BsonRequired]
    public string Role { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    [BsonIgnoreIfNull]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdatedAt { get; set; } 
}