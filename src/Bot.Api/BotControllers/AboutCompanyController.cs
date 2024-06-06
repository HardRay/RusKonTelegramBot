using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record AboutCompanyState;

public sealed class AboutCompanyController(
    IOptions<AppOptions> appOptions,
    IUserService userService,
    ITelegramMessageService messageService) : BaseController<AboutCompanyState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ShowAboutCompanyMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }


    [Action]
    public async ValueTask ShowAboutCompanyMessage()
    {
        PushL(SharedResource.AboutCompanyText);

        RowButton(SharedResource.CompanyVideoButton, appOptions.Value.CompanyVideoUrl);
        RowButton(SharedResource.CompanyWebsiteButton, appOptions.Value.CompanyWebsiteUrl);
        RowButton(SharedResource.SocialMediaButton, Q(ShowSocialMedia));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
    }

    [Action]
    public ValueTask EmptyAction()
    {
        return ValueTask.CompletedTask;
    }
}
