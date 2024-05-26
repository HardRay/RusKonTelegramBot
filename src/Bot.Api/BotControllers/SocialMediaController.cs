using Bot.Api.Options;
using Bot.Api.Resources;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record SocialMediaState;

public sealed class SocialMediaController(IOptions<AppOptions> appOptions) : BotControllerState<MainMenuState>
{
    public override async ValueTask OnEnter()
    {
        await ShowSocialMedia();
    }

    public override ValueTask OnLeave()
    {
        return ValueTask.CompletedTask;
    }

    [Action]
    public ValueTask ShowSocialMedia()
    {
        PushL(SharedResource.SocialMediaText);

        RowButton(SharedResource.TelegramChannelButton, appOptions.Value.TelegramChannelUrl);
        RowButton(SharedResource.VKGroupButton, appOptions.Value.VKGroupUrl);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        return ValueTask.CompletedTask;
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }
}
