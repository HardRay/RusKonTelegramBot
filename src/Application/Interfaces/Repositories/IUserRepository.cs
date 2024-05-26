using Application.Interfaces.Repositories.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Репозиторий для <see cref="User"/>
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    /// Обновление данных пользователя
    /// </summary>
    /// <param name="user">Данные пользователя</param>
    Task UpdateAsync(User user);
}
