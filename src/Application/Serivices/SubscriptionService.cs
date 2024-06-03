using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class SubscriptionService(ISubscriptionRepository repository, IMapper mapper) : ISubscriptionService
{
    /// <inheritdoc/>
    public async Task CreateAsync(SubscriptionModel subscription)
    {
        var entity = mapper.Map<Subscription>(subscription);

        var oldSubscription = await repository.FindOneAsync(x => x.UserTelegramId == entity.UserTelegramId);
        if (oldSubscription == null)
        {
            await repository.InsertOneAsync(entity);
        }
        else
        {
            await repository.UpdateOneAsync(x => x.UserTelegramId == entity.UserTelegramId, x => x.VacancyFilter, entity.VacancyFilter);
        }
    }
}
