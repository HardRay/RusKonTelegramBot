using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record NotFoundVacanciesState;

public sealed class NotFoundVacanciesController(ITelegramMessageService messageService) : BotControllerState<NotFoundVacanciesState>
{
    public override async ValueTask OnEnter()
    {
        await ShowNotFoundVacanciesMessage();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowNotFoundVacanciesMessage()
    {
        PushL(SharedResource.NotFoundVacanciesButton);

        RowButton(SharedResource.SubscribeToNotificationButton, Q(SubscribeToNotification));
        RowButton(SharedResource.LeaveResumeButton, Q(ShowMainMenu));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await messageService.InsertAsync(await Send());
    }

    [Action]
    public async ValueTask SubscribeToNotification()
    {
        PushL(SharedResource.SuccessSubscribtionToNotification);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));
        await messageService.InsertAsync(await Send());
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async Task Start()
    {
        await messageService.InsertAsync(Context.Update.Message);

        await GlobalState(new MainMenuState());
    }
}
