using Application;
using Application.Interfaces.Repositories;
using Infrastructure.Models.Options;
using Infrastructure.Providers;
using Infrastructure.Providers.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();

        services.Configure<MongoDBOptions>(configuration.GetSection(nameof(MongoDBOptions)));

        services.AddSingleton<IMongoDatabaseProvider, MongoDatabaseProvider>();
        services.AddTransient<IMongoCollectionProvider, MongoCollectionProvider>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserStateRepository, UserStateRepository>();
        services.AddTransient<IMessageRepository, MessageRepository>();

        return services;
    }
}
