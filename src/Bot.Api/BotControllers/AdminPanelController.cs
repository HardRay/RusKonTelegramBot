using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Bot.Api.BotControllers
{
    public sealed record AdminPanelState;

    public sealed class AdminPanelController(ITelegramMessageService messageService) : BotControllerState<AdminPanelState>
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

            var message = await Send();

            await messageService.InsertAsync(message);
        }

        [On(Handle.Unknown)]
        [Filter(Filters.CurrentGlobalState)]
        [Filter(And: Filters.Document)]
        public async ValueTask OnDocumentReceived()
        {
            var receivedMessage = Context.Update.Message;
            await messageService.InsertAsync(receivedMessage);

            var document = receivedMessage?.Document;

            var stream = new MemoryStream();
            await Client.GetInfoAndDownloadFileAsync(document!.FileId, stream);

            PushL("Вау");
            var message = await Send();

            await messageService.InsertAsync(message);
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
