using Domain.Entities.Common;
using Infrastructure.Providers.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Providers;

/// <inheritdoc/>
internal class MongoCollectionProvider(IMongoDatabaseProvider databaseProvider) : IMongoCollectionProvider
{
    public IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : BaseEntity
    {
        var database = databaseProvider.GetDatabase();

        return database.GetCollection<TEntity>(typeof(TEntity).Name);
    }
}
