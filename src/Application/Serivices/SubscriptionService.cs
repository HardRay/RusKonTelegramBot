﻿using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class SubscriptionService(
    ISubscriptionRepository repository,
    IVacancyService vacancyService,
    IMapper mapper) : ISubscriptionService
{
    /// <inheritdoc/>
    public async Task CreateAsync(SubscriptionModel subscription)
    {
        var entity = mapper.Map<Subscription>(subscription);

        var oldSubscription = await repository.FindFirstOrDefaultAsync(x => x.UserTelegramId == entity.UserTelegramId);
        if (oldSubscription == null)
        {
            await repository.InsertOneAsync(entity);
        }
        else
        {
            await repository.UpdateOneAsync(x => x.UserTelegramId == entity.UserTelegramId, x => x.VacancyFilter, entity.VacancyFilter);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SubscriptionModel>> GetSubscriptionWithVacanciesAsync()
    {
        var vacancies = await vacancyService.GetAllAsync();
        var subscriptions = await repository.FindManyAsync(x => true);

        subscriptions = subscriptions.Where(subscription => vacancies.Any(x =>
            (subscription.VacancyFilter.IsOnline == null || x.IsOnline == subscription.VacancyFilter.IsOnline) &&
            (subscription.VacancyFilter.City == null || x.City == subscription.VacancyFilter.City) &&
            (subscription.VacancyFilter.Type == null || x.Type == subscription.VacancyFilter.Type) &&
            (subscription.VacancyFilter.Direction == null || x.Direction == subscription.VacancyFilter.Direction)));

        return mapper.Map<IEnumerable<SubscriptionModel>>(subscriptions);
    }


}
