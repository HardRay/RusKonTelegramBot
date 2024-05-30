using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Bot.Api.BotControllers;

public sealed record HRState;

public sealed class HRController(
    IOptions<AppOptions> appOptions,
    ITelegramMessageService messageService,
    ITelegramBotClient botClient) : BotControllerState<HRState>
{
    public override async ValueTask OnEnter()
    {
        await ShowContacts();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowContacts()
    {
        PushL(SharedResource.HRContactsText);

        RowButton(SharedResource.CallButton, Q(ShowHRPhone));
        RowButton(SharedResource.HRChatButton, appOptions.Value.TelegramHRChat);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowHRPhone()
    {
        var message = await botClient.SendContactAsync(Context.GetSafeChatId()!, appOptions.Value.HRPhone, "HR");

        await messageService.InsertAsync(message);
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
