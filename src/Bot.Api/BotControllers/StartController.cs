using Application.Interfaces.Services;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record StartState;

public sealed class StartController(IUserService userService, ILogger<StartController> logger) : BotController
{
    [Action("Start")]
    [Action("/start", "Запуск бота")]
    public async ValueTask Start()
    {
        var userTelegramId = Context.GetSafeUserId();
        if (!userTelegramId.HasValue)
            return;

        var user = await userService.GetOrCreateUserByTelegramIdAsync(userTelegramId.Value);

        user.TelegramUserName = Context.GetUsername();
        await userService.UpdateUserAsync(user);

        await GlobalState(new MainMenuState());
    }

    [On(Handle.Unknown)]
    [Filter(Filters.NotGlobalState)]
    public ValueTask Unknown()
    {
        PushL($"Неизвестная команда. Попробуйте начать с команды /start");

        return ValueTask.CompletedTask;
    }

    [On(Handle.Exception)]
    public ValueTask CatchExeption(Exception ex)
    {
        logger.LogError(ex, "Unhandled exception");

        return ValueTask.CompletedTask;
    }
}