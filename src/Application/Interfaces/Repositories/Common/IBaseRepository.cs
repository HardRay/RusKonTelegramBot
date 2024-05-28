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
    Task InsertOneAsync(TEntity entity);

    /// <summary>
    /// Вставка множества записей
    /// </summary>
    /// <param name="entities">Список данных</param>
    Task InsertManyAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Обновление поля записи
    /// </summary>
    /// <typeparam name="TField">Тип поля для обновления</typeparam>
    /// <param name="predicate">Условия выборки</param>
    /// <param name="fieldPredicate">Поле для изменения</param>
    /// <param name="value">Новое значение</param>
    Task UpdateOneAsync<TField>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TField>> fieldPredicate, TField value);

    /// <summary>
    /// Удаление одной записи
    /// </summary>
    /// <param name="predicate">Условие выборки</param>
    Task DeleteOneAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Удаление множества записей
    /// </summary>
    /// <param name="predicate">Условие выборки</param>
    Task DeleteManyAsync(Expression<Func<TEntity, bool>> predicate);

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

    /// <summary>
    /// Проверка существования записи по заданным критериям
    /// </summary>
    /// <param name="predicate">Условие выборки</param>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
}
