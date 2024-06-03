using Application.Interfaces.Services;
using Bot.Api.Constants;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.BotControllers;

public sealed record AdminPanelState;

public sealed class AdminPanelController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    ISubscriptionService subscriptionService,
    ILogger<AdminPanelController> logger) : BotControllerState<AdminPanelState>
{
    public override async ValueTask OnEnter()
    {
        await ShowAdminPanel();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowAdminPanel()
    {
        PushL(SharedResource.AdminPanelText);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();
        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }

    [On(Handle.Unknown)]
    [Filter(Filters.CurrentGlobalState)]
    [Filter(And: Filters.Document)]
    public async ValueTask OnDocumentReceived()
    {
        var receivedMessage = Context.Update.Message;
        await messageService.InsertAsync(receivedMessage);

        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var document = receivedMessage?.Document;
        var documentExt = document?.FileName?.Split('.').Last();

        var excelExtentions = new HashSet<string>() { "xlsx", "xls" };

        if (string.IsNullOrEmpty(documentExt) || !excelExtentions.Contains(documentExt))
        {
            PushL(SharedResource.InvalidDocumentExtentions);
            await messageService.InsertAsync(await Send());
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
            await messageService.InsertAsync(await Send());
            return;
        }

        await NotifySubscribersAboutNewVacancies();

        PushL(SharedResource.SuccessDocumentUploading);

        await messageService.InsertAsync(await Send());
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async ValueTask Start()
    {
        await messageService.InsertAsync(Context.Update.Message);

        await GlobalState(new MainMenuState());
    }

    [On(Handle.Unknown)]
    [Filter(Filters.CurrentGlobalState)]
    [Filter(And: Filters.CallbackQuery)]
    public async Task UnknownCallback()
    {
        var callbackQuery = Context.GetCallbackQuery();
        if (!string.IsNullOrWhiteSpace(callbackQuery.Data) && callbackQuery.Data == BotConstants.ShowNewVacanciesCallbackData)
        {
            Context.StopHandling();
            await GlobalState(new VacanciesState());
        }
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

            await messageService.InsertAsync(message);
        }
    }
}
