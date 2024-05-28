using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record AboutCompanyState;

public sealed class AboutCompanyController(
    IOptions<AppOptions> appOptions,
    ITelegramMessageService messageService) : BotControllerState<AboutCompanyState>
{
    public override async ValueTask OnEnter()
    {
        await ShowAboutCompany();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowAboutCompany()
    {
        PushL(SharedResource.AboutCompanyText);

        RowButton(SharedResource.CompanyVideoButton, appOptions.Value.CompanyVideoUrl);
        RowButton(SharedResource.CompanyWebsiteButton, appOptions.Value.CompanyWebsiteUrl);
        RowButton(SharedResource.SocialMediaButton, Q(ShowSocialMedia));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowSocialMedia()
    {
        await GlobalState(new SocialMediaState());
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }
}
