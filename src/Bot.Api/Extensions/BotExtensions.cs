using Bot.Api.Services;
using Deployf.Botf;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bot.Api.Extensions;

public static class BotExtensions
{
    public static IServiceCollection AddBot(this IServiceCollection services, IConfiguration configuration)
    {
        var botApiToken = "6843598798:AAFHQiOLWnGS2uLBEnC16_eTxPXToybbcjg";
        var botUserName = "https://t.me/ruskon_s_bot";

        if (string.IsNullOrEmpty(botUserName) || string.IsNullOrEmpty(botApiToken))
            throw new BotfException("Not found bot options");

        services.AddBotf(new BotfOptions()
        {
            AutoCleanReplyKeyboard = true,
            Username = botUserName,
            Token = botApiToken,
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
