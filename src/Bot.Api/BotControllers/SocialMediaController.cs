﻿using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record SocialMediaState;

public sealed class SocialMediaController(
    IOptions<AppOptions> appOptions,
    ITelegramMessageService messageService) : BaseController<SocialMediaState>(messageService)
{
    public override async ValueTask OnEnter()
    {
        await ShowSocialMediaMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowSocialMediaMessage()
    {
        PushL(SharedResource.SocialMediaText);

        RowButton(SharedResource.TelegramChannelButton, appOptions.Value.TelegramChannelUrl);
        RowButton(SharedResource.VKGroupButton, appOptions.Value.VKGroupUrl);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
    }
}
