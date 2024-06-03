using Application.Interfaces.Repositories.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Репозиторий для <see cref="Subscription"/>
/// </summary>
public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
}
