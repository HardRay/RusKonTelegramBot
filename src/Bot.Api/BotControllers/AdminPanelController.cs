using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
using Bot.Api.Constants;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.BotControllers;

public sealed record AdminPanelState;

public sealed class AdminPanelController(
    IUserService userService,
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    ISubscriptionService subscriptionService,
    ILogger<AdminPanelController> logger) : BaseController<AdminPanelState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ShowAdminPanelMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowAdminPanelMessage()
    {
        PushL(SharedResource.AdminPanelText);
        RowButton(SharedResource.BackButton, Q(ShowMainMenu));

        await SendMessage();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.CurrentGlobalState)]
    [Filter(And: Filters.Document)]
    public async ValueTask OnDocumentReceived()
    {
        var receivedMessage = Context.Update.Message;
        await _messageService.InsertAsync(receivedMessage);

        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var document = receivedMessage?.Document;
        var documentExt = document?.FileName?.Split('.').Last();

        var excelExtentions = new HashSet<string>() { "xlsx", "xls" };

        if (string.IsNullOrEmpty(documentExt) || !excelExtentions.Contains(documentExt))
        {
            PushL(SharedResource.InvalidDocumentExtentions);
            await SendMessage();
            return;
        }

        var stream = new MemoryStream();
        await Client.GetInfoAndDownloadFileAsync(document!.FileId, stream);

        try
        {
            await vacancyService.UploadVacanciesAsync(stream);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, SharedResource.ErrorDocumentUploading);

            PushL(SharedResource.ErrorDocumentUploading);
            await SendMessage();
            return;
        }

        await NotifySubscribersAboutNewVacancies();

        PushL(SharedResource.SuccessDocumentUploading);

        await SendMessage();
    }

    private async ValueTask NotifySubscribersAboutNewVacancies()
    {
        var subscriptions = await subscriptionService.GetSubscriptionWithVacanciesAsync();

        foreach(var subscription in subscriptions)
        {
            var keyboardButton = new InlineKeyboardButton(SharedResource.ShowNewVacanciesButton)
            {
                CallbackData = BotConstants.ShowNewVacanciesCallbackData
            };
            var markup = new InlineKeyboardMarkup(keyboardButton);
            var message = await Client.SendTextMessageAsync(subscription.UserTelegramId, SharedResource.NewVacanciesMessage, replyMarkup: markup);

            await _messageService.InsertAsync(message);
        }
    }
}
