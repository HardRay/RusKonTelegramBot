using Application.Interfaces.Services;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record DirectionState;

public sealed class DirectionController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BotControllerState<DirectionState>
{
    public override async ValueTask OnEnter()
    {
        //await ShowStartScreen();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }
}
