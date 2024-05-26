using Application.Interfaces.Repositories.Common;
using Domain.Entities.Common;
using Infrastructure.Providers.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Common;

/// <inheritdoc/>
public abstract class BaseRepository<TEntity>(IMongoCollectionProvider mongoCollectionProvider)
    : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly IMongoCollection<TEntity> _collection = mongoCollectionProvider.GetCollection<TEntity>();

    /// <inheritdoc/>
    public async Task CreateAsync(TEntity entity)
    {
        entity.CreateDateTimeUtc = DateTime.UtcNow;

        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entity.ModifyDateTimeUtc = DateTime.UtcNow;

        await _collection.ReplaceOneAsync(x=>x.Id == entity.Id, entity);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        => _collection.DeleteOneAsync(predicate);

    /// <inheritdoc/>
    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var cursor = await _collection.FindAsync(predicate);

        return await cursor.FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var cursor = await _collection.FindAsync(predicate);

        return await cursor.ToListAsync();
    }
}
