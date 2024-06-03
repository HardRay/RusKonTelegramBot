using Application.Interfaces.Services;
using Application.Models;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Domain.Entities;

namespace Bot.Api.BotControllers;

public sealed record NotFoundVacanciesState;

public sealed class NotFoundVacanciesController(
    ITelegramMessageService messageService,
    IUserService userService,
    ISubscriptionService subscriptionService) : BotControllerState<NotFoundVacanciesState>
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
        RowButton(SharedResource.LeaveResumeButton, Q(LeaveResume));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await messageService.InsertAsync(await Send());
    }

    [Action]
    public async ValueTask SubscribeToNotification()
    {
        var userId = Context.GetSafeChatId();
        if (userId == null)
            return;
        var user = await userService.GetOrCreateUserByTelegramIdAsync(userId.Value);

        var subscription = new SubscriptionModel()
        {
            UserTelegramId = userId.Value,
            VacancyFilter = user.VacancyFilter
        };
        await subscriptionService.CreateAsync(subscription);

        PushL(SharedResource.SuccessSubscribtionToNotification);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));
        await messageService.InsertAsync(await Send());
    }

    [Action]
    public async ValueTask LeaveResume()
    {
        await GlobalState(new ResumeState());
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
