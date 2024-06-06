using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record MainMenuState;

public sealed class MainMenuController(
    ITelegramMessageService messageService,
    IUserService userService,
    IOptions<AppOptions> appOptions) : BaseController<MainMenuState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ShowMainMenuMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowMainMenuMessage()
    {
        var userId = Context.GetSafeChatId();

        PushL(SharedResource.MainMenuGreetings);

        RowButton(SharedResource.AboutCompanyButton, Q(ShowAboutCompany));
        RowButton(SharedResource.VacanciesButton, Q(ShowCities));
        RowButton(SharedResource.ContactWithHRButton, Q(ContactWithHR));

        if (userId == appOptions.Value.AdminTelegramId)
        {
            RowButton(SharedResource.AdminPanelButton, Q(ShowAdminPanel));
        }

        await SendMessage();
    }
}
