using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace NexusAdmin.Infrastructure.Persistence.MongoDB.Configuration;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        MongoDbSettings mongoSettings = settings.Value;
        MongoClient client = new MongoClient(mongoSettings.ConnectionString);
        this._database = client.GetDatabase(mongoSettings.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return this._database.GetCollection<T>(collectionName);
    }
}