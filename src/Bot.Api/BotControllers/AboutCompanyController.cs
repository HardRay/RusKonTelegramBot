using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;
using Telegram.Bot;

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
        PushL(SharedResource.AboutCompanyShortText);
        ShowButtons();

        await SendMessage();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.Text)]
    [Filter(And: Filters.CurrentGlobalState)]
    private async Task UnknownText()
    {
        var message = Context.Update.Message;
        if (message == null || message.Text == null || message.Text != "/more")
        {
            return;
        }
        Context.StopHandling();

        await Client.DeleteMessageAsync(message.Chat.Id, message.MessageId);

        PushL(SharedResource.AboutCompanyFullText);
        ShowButtons();

        await SendMessage();
    }

    private void ShowButtons()
    {
        RowButton(SharedResource.CompanyVideoButton, appOptions.Value.CompanyVideoUrl);
        RowButton(SharedResource.CompanyWebsiteButton, appOptions.Value.CompanyWebsiteUrl);
        RowButton(SharedResource.SocialMediaButton, Q(ShowSocialMedia));
        RowButton(SharedResource.BackButton, Q(ShowMainMenu));
    }
}
