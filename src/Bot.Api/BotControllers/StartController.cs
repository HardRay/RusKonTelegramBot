using Application.Interfaces.Services;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public record StartState;

public class StartController(IUserService userService, ILogger<StartController> logger) : BotController
{
    [Action("Start")]
    [Action("/start", "Запуск бота")]
    public async Task Start()
    {
        var userTelegramId = Context.GetSafeUserId();
        if (!userTelegramId.HasValue)
            return;

        var user = await userService.GetOrCreateUserByTelegramIdAsync(userTelegramId.Value);

        user.TelegramUserName = Context.GetUsername();
        await userService.UpdateUserAsync(user);

        PushL($"Твой ник {user.TelegramUserName}");
    }

    [On(Handle.Unknown)]
    [Filter(Filters.NotGlobalState)]
    public void Unknown()
    {
        PushL($"Неизвестная команда. Попробуйте начать с команды /start");
    }

    [On(Handle.Exception)]
    public void CatchExeption(Exception ex)
    {
        logger.LogError(ex, "Unhandled exception");
    }
}