using Application.Interfaces.Services;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record StartState;

public sealed class StartController(
    IUserService userService,
    ITelegramMessageService messageService,
    ILogger<StartController> logger) : BotController
{
    [Action("Start")]
    [Action("/start", "Запуск бота")]
    public async ValueTask Start()
    {
        var userTelegramId = Context.GetSafeUserId();
        if (!userTelegramId.HasValue)
            return;

        await messageService.InsertAsync(Context.Update.Message);

        var user = await userService.GetOrCreateUserByTelegramIdAsync(userTelegramId.Value);

        user.TelegramUserName = Context.GetUsername();
        user.FirstName = Context.Update.Message?.From?.FirstName;
        user.LastName = Context.Update.Message?.From?.LastName;
        await userService.UpdateUserAsync(user);

        await GlobalState(new MainMenuState());
    }

    [On(Handle.Unknown)]
    [Filter(Filters.NotGlobalState)]
    public async ValueTask Unknown()
    {
        await messageService.InsertAsync(Context.Update.Message);

        PushL($"Неизвестная команда. Попробуйте начать с команды /start");
        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [On(Handle.Exception)]
    public ValueTask CatchExeption(Exception ex)
    {
        logger.LogError(ex, "Unhandled exception");

        return ValueTask.CompletedTask;
    }
}