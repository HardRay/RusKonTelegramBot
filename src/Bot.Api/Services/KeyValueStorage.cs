using Application.Interfaces.Repositories;
using Deployf.Botf;
using Domain.Entities;
using Newtonsoft.Json;

namespace Bot.Api.Services;

public class KeyValueStorage(IUserStateRepository repository) : IKeyValueStorage
{
    public async ValueTask<T?> Get<T>(long userId, string key, T? defaultValue)
    {
        var stateObject = await repository.FindOneAsync(x => x.Key == key && x.UserId == userId);
        if (stateObject == null)
        {
            return defaultValue;
        }

        var stateValue = JsonConvert.DeserializeObject(stateObject.Value, GetJsonSettings());

        return (T?)stateValue;
    }

    public async ValueTask<object?> Get(long userId, string key, object? defaultValue)
    {
        var stateObject = await repository.FindOneAsync(x => x.Key == key && x.UserId == userId);
        if (stateObject == null)
        {
            return defaultValue;
        }

        var stateValue = JsonConvert.DeserializeObject(stateObject.Value, GetJsonSettings());

        return stateValue;
    }

    public async ValueTask Set(long userId, string key, object value)
    {
        var jsonValue = JsonConvert.SerializeObject(value, GetJsonSettings());
        var stateObject = new UserState() { Value = jsonValue, Key = key, UserId = userId };

        if (await Contain(userId, key))
        {
            await repository.UpdateOneAsync(
              x => x.Key == key && x.UserId == userId,
              x => x.Value,
              jsonValue);
        }
        else
        {
            await repository.InsertOneAsync(stateObject);
        }
    }

    public async ValueTask Remove(long userId, string key)
        => await repository.DeleteOneAsync(x => x.UserId == userId && x.Key == key);

    public async ValueTask<bool> Contain(long userId, string key)
        => await repository.AnyAsync(x => x.UserId == userId && x.Key == key);

    private static JsonSerializerSettings GetJsonSettings()
        => new()
        {
            TypeNameHandling = TypeNameHandling.All
        };
}
