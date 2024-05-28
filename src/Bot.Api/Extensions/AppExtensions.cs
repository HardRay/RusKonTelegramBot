using Bot.Api.Options;
using Bot.Api.Services;
using Bot.Api.Services.Interfaces;
using Infrastructure;

namespace Bot.Api.Extensions;

public static class AppExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddBot(configuration);

        services.Configure<AppOptions>(configuration.GetSection(nameof(AppOptions)));

        services.AddTransient<ITelegramMessageService, TelegramMessageService>();

        return services;
    }
}
