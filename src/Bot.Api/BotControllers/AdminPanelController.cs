using Application.Interfaces.Services;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Telegram.Bot;

namespace Bot.Api.BotControllers
{
    public sealed record AdminPanelState;

    public sealed class AdminPanelController(
        ITelegramMessageService messageService,
        IVacancyService vacancyService,
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

            PushL(SharedResource.SuccessDocumentUploading);

            await messageService.InsertAsync(await Send());
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
}
