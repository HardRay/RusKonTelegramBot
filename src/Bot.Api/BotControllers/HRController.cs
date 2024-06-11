using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
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
    IUserService userService,
    ITelegramMessageService messageService,
    ITelegramBotClient botClient) : BaseController<HRState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ShowContacts();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action("ShowContacts")]
    public async ValueTask ShowContacts()
    {
        PushL(BotText.HRContactsText);

        RowButton(BotText.CallButton, Q(ShowHRPhone));
        RowButton(BotText.HRChatButton, appOptions.Value.TelegramHRChat);
        RowButton(BotText.BackButton, Q(ShowMainMenu));

        await SendMessage();
    }

    [Action("ShowHRPhone")]
    public async ValueTask ShowHRPhone()
    {
        var message = await botClient.SendContactAsync(Context.GetSafeChatId()!, appOptions.Value.HRPhone, "РусКон HR");

        await _messageService.InsertAsync(message);
    }
}
