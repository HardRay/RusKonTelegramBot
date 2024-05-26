using Infrastructure.Models.Options;
using Infrastructure.Providers.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Providers;

/// <inheritdoc/>
internal class MongoDatabaseProvider(IOptions<MongoDBOptions> options) : IMongoDatabaseProvider
{
    private IMongoDatabase? _database;

    /// <inheritdoc/>
    public IMongoDatabase GetDatabase()
    {
        if (_database == null)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.DatabaseName);
        }

        return _database;
    }
}
