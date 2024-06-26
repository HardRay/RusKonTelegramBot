﻿using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Providers.Interfaces;
using Infrastructure.Repositories.Common;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class UserRepository(IMongoCollectionProvider mongoCollectionProvider)
    : BaseRepository<User>(mongoCollectionProvider), IUserRepository
{
    public async Task UpdateAsync(User user)
    {
        var updater = Builders<User>.Update
            .Set(x => x.TelegramId, user.TelegramId)
            .Set(x => x.TelegramUserName, user.TelegramUserName)
            .Set(x => x.FirstName, user.FirstName)
            .Set(x => x.LastName, user.LastName)
            .Set(x => x.VacancyFilter, user.VacancyFilter); 

        await FindOneAndUpdateAsync(x=>x.Id == user.Id, updater);
    }
}
