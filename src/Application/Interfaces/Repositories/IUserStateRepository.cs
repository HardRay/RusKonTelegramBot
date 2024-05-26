using Application.Interfaces.Repositories.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Репозиторий для <see cref="UserState"/>
/// </summary>
public interface IUserStateRepository : IBaseRepository<UserState>
{
}
