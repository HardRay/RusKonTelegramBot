using Bot.Api.Options;
using Bot.Api.Resources;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record AboutCompanyState;

public sealed class AboutCompanyController(IOptions<AppOptions> appOptions) : BotControllerState<AboutCompanyState>
{
    public override async ValueTask OnEnter()
    {
        await ShowAboutCompany();
    }

    public override ValueTask OnLeave()
    {
        return ValueTask.CompletedTask;
    }

    [Action]
    public ValueTask ShowAboutCompany()
    {
        PushL(SharedResource.AboutCompanyText);

        RowButton(SharedResource.CompanyVideoButton, appOptions.Value.CompanyVideoUrl);
        RowButton(SharedResource.CompanyWebsiteButton, appOptions.Value.CompanyWebsiteUrl);
        RowButton(SharedResource.SocialMediaButton, Q(ShowAboutCompany));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        return ValueTask.CompletedTask;
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
