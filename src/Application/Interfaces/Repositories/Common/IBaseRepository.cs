using Domain.Entities.Common;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories.Common;

/// <summary>
/// Базовый репозиторий
/// </summary>
/// <typeparam name="TEntity"><see cref="BaseEntity"/></typeparam>
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Создание записи
    /// </summary>
    /// <param name="entity">Данные</param>
    Task CreateAsync(TEntity entity);

    /// <summary>
    /// Обновление записи
    /// </summary>
    /// <param name="entity">Данные</param>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// Удаление записи
    /// </summary>
    /// <param name="predicate">Условие выборки</param>
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Поиск первой записи, удовлетворяющей условиям.
    /// </summary>
    /// <param name="predicate">Условие выборки</param>
    /// <returns>Найденная запись.</returns>
    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Поиск записей, удовлетворяющих условиям.
    /// </summary>
    /// <param name="predicate">Условие выборки</param>
    /// <returns>Найденные записи.</returns>
    Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate);
}
