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
    public async Task InsertOneAsync(TEntity entity)
    {
        entity.CreateDateTimeUtc = DateTime.UtcNow;

        await _collection.InsertOneAsync(entity);
    }

    /// <inheritdoc/>
    public async Task UpdateOneAsync<TField>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TField>> fieldPredicate, TField value)
    {
        var updater = Builders<TEntity>.Update
            .Set(fieldPredicate, value)
            .Set(o => o.ModifyDateTimeUtc, DateTime.UtcNow);

        await _collection.UpdateOneAsync(predicate, updater);
    }

    /// <summary>
    /// Найти запись и обновить
    /// </summary>
    /// <param name="predicate">Условия выборки</param>
    /// <param name="updater">Условия обновления</param>
    protected async Task FindOneAndUpdateAsync(Expression<Func<TEntity, bool>> predicate, UpdateDefinition<TEntity> updater)
    {
        updater = updater.Set(x => x.ModifyDateTimeUtc, DateTime.UtcNow);

        await _collection.FindOneAndUpdateAsync(predicate, updater);
    }

    /// <inheritdoc/>
    public Task DeleteOneAsync(Expression<Func<TEntity, bool>> predicate)
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

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var cursor = await _collection.FindAsync(predicate);

        return await cursor.AnyAsync();
    }
}
