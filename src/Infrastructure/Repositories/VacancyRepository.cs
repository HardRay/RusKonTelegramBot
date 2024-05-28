using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Providers.Interfaces;
using Infrastructure.Repositories.Common;

namespace Infrastructure.Repositories;

public sealed class VacancyRepository(IMongoCollectionProvider mongoCollectionProvider)
    : BaseRepository<Vacancy>(mongoCollectionProvider), IVacancyRepository
{
}
