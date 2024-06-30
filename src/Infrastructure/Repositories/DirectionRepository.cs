using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Providers.Interfaces;
using Infrastructure.Repositories.Common;

namespace Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class DirectionRepository(IMongoCollectionProvider mongoCollectionProvider)
    : BaseRepository<Direction>(mongoCollectionProvider), IDirectionRepository
{
}