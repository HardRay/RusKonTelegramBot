using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Models;
using AutoMapper;
using Domain.Entities;

namespace Application.Serivices;

/// <inheritdoc/>
public sealed class UserService(IMapper mapper, IUserRepository repository) : IUserService
{
    /// <inheritdoc/>
    public async Task<UserModel> GetOrCreateUserByTelegramIdAsync(long telegramId)
    {
        var user = await repository.FindFirstOrDefaultAsync(x=>x.TelegramId == telegramId);

        if (user == null)
        {
            user = new User(telegramId);

            await repository.InsertOneAsync(user);
        }

        var userModel = mapper.Map<UserModel>(user);

        return userModel;
    }

    /// <inheritdoc/>
    public async Task UpdateUserAsync(UserModel userModel)
    {
        var user = mapper.Map<User>(userModel);

        await repository.UpdateAsync(user);
    }
}
