﻿using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record SocialMediaState;

public sealed class SocialMediaController(
    IOptions<AppOptions> appOptions,
    ITelegramMessageService messageService) : BotControllerState<SocialMediaState>
{
    public override async ValueTask OnEnter()
    {
        await ShowSocialMedia();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowSocialMedia()
    {
        PushL(SharedResource.SocialMediaText);

        RowButton(SharedResource.TelegramChannelButton, appOptions.Value.TelegramChannelUrl);
        RowButton(SharedResource.VKGroupButton, appOptions.Value.VKGroupUrl);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }
}
