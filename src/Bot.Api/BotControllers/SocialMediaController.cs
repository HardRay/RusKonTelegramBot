using Application.Interfaces.Services;
using Application.Models.Constansts;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record SocialMediaState;

public sealed class SocialMediaController(
    IOptions<AppOptions> appOptions,
    IUserService userService,
    ITelegramMessageService messageService) : BaseController<SocialMediaState>(messageService, userService)
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
        PushL(BotText.SocialMediaText);

        RowButton(BotText.TelegramChannelButton, appOptions.Value.TelegramChannelUrl);
        RowButton(BotText.VKGroupButton, appOptions.Value.VKGroupUrl);
        RowButton(BotText.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessageWithImage(ImageFiles.SocialMedia);
    }
}
