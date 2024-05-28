using Application.Interfaces.Repositories.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Репозиторий для <see cref="Vacancy"/>
/// </summary>
public interface IVacancyRepository : IBaseRepository<Vacancy>
{
}
