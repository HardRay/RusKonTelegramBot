using Bot.Api.Services;
using Deployf.Botf;
using Infrastructure.Models.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bot.Api.Extensions;

public static class BotExtensions
{
    public static IServiceCollection AddBot(this IServiceCollection services, IConfiguration configuration)
    {
        var botOptions = configuration.GetSection(nameof(BotOptions)).Get<BotOptions>()
            ?? throw new BotfException("Not found BotOptions");

        services.AddBotf(new BotfOptions()
        {
            AutoCleanReplyKeyboard = true,
            Username = botOptions.Username,
            Token = botOptions.ApiToken,
        });

        services.AddTransient<IKeyValueStorage, KeyValueStorage>();
        var descriptor =
          new ServiceDescriptor(
            typeof(IKeyValueStorage),
            typeof(KeyValueStorage),
            ServiceLifetime.Singleton);
        services.Replace(descriptor);

        return services;
    }

    public static IApplicationBuilder UseBot(this IApplicationBuilder app)
    {
        app.UseBotf();

        return app;
    }
}
