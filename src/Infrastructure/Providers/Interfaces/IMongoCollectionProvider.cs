using Domain.Entities.Common;
using MongoDB.Driver;

namespace Infrastructure.Providers.Interfaces;

/// <summary>
/// Поставщик коллекции для сущностей
/// </summary>
public interface IMongoCollectionProvider
{
    /// <summary>
    /// Получение коллекции для сущности
    /// </summary>
    /// <typeparam name="TEntity"><see cref="BaseEntity"/></typeparam>
    /// <returns>Коллекция для сущности</returns>
    IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : BaseEntity;
}
