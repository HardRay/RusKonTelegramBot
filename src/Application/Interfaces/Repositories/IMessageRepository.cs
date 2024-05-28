using Application.Interfaces.Repositories.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Репозиторий для <see cref="Message"/>
/// </summary>
public interface IMessageRepository : IBaseRepository<Message>
{
}
