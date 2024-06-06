﻿using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot.Api.BotControllers;

public sealed record ResumeState;

public class ResumeController(
    ITelegramMessageService messageService,
    IUserService userService,
    IOptions<AppOptions> appOptions) : BaseController<ResumeState>(messageService)
{
    public override async ValueTask OnEnter()
    {
        await ShowStartMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowStartMessage()
    {
        PushL(SharedResource.ResumeStartMessage);
        RowButton(SharedResource.BackButton, Q(ShowNotFoundVacancies));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.CurrentGlobalState)]
    public async ValueTask OnResumeReceived()
    {
        var message = Context.Update.Message;
        if (message == null)
        {
            return;
        }
        Context.StopHandling();
        await _messageService.InsertAsync(message);

        await SendResumeToHr();

        PushL(SharedResource.AcceptedResumeMessage);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
    }

    private async ValueTask SendResumeToHr()
    {
        var chatId = Context.GetSafeChatId();
        if (chatId == null)
            return;

        var messageId = Context.Update.Message?.MessageId;
        if (messageId == null)
            return;

        var user = await userService.GetOrCreateUserByTelegramIdAsync(chatId.Value);
        var firstName = user.FirstName;
        var lastName = user.LastName;

        var nameBuilder = new StringBuilder();
        nameBuilder.Append(firstName);
        if (!string.IsNullOrEmpty(lastName))
        {
            nameBuilder.Append(' ');
            nameBuilder.Append(lastName);
        }

        var userName = $"<a href=\"tg://user?id={chatId}\">{nameBuilder}</a>";
        var messageText = string.Format(SharedResource.NewResumeText, userName);
        await Client.SendTextMessageAsync(appOptions.Value.TelegramHRChatId, messageText, ParseMode.Html);

        await Client.ForwardMessageAsync(appOptions.Value.TelegramHRChatId, chatId, messageId.Value);
    }
}
