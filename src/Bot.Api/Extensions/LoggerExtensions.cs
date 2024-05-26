using NLog.Web;

namespace Bot.Api.Extensions;

public static class LoggerExtensions
{
    public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        return builder;
    }
}
