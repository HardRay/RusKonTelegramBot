using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Constansts;
using Bot.Api.BotControllers.Common;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record NotFoundVacanciesState;

public sealed class NotFoundVacanciesController(
    ITelegramMessageService messageService,
    IUserService userService,
    ISubscriptionService subscriptionService) : BaseController<NotFoundVacanciesState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ShowNotFoundVacanciesMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowNotFoundVacanciesMessage()
    {
        PushL(BotText.NotFoundVacanciesButton);

        RowButton(BotText.SubscribeToNotificationButton, Q(SubscribeToNotification));
        RowButton(BotText.LeaveResumeButton, Q(LeaveResume));
        RowButton(BotText.BackButton, Q(ShowVacancies));
        RowButton(BotText.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessageWithImage(ImageFiles.NoSuitableVacancy);
    }

    [Action]
    public async ValueTask SubscribeToNotification()
    {
        var userId = Context.GetSafeChatId();
        if (userId == null)
            return;
        var user = await _userService.GetOrCreateUserByTelegramIdAsync(userId.Value);

        var subscription = new SubscriptionModel()
        {
            UserTelegramId = userId.Value,
            VacancyFilter = user.VacancyFilter
        };
        await subscriptionService.CreateAsync(subscription);

        PushL(BotText.SuccessSubscribtionToNotification);
        RowButton(BotText.BackToMainMenuButton, Q(ShowMainMenu));
        await SendMessageWithImage(ImageFiles.NotificationSubscription);
    }
}
