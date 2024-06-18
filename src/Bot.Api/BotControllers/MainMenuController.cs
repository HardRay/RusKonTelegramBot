using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Constansts;
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

        PushL(BotText.MainMenuGreetings);

        RowButton(BotText.AboutCompanyButton, Q(ShowAboutCompany));
        RowButton(BotText.VacanciesButton, Q(ShowCities));
        RowButton(BotText.JobForStudentsButton, Q(ShowJobForStudents));
        RowButton(BotText.ContactWithHRButton, Q(ContactWithHR));

        if (userId == appOptions.Value.AdminTelegramId)
        {
            RowButton(BotText.AdminPanelButton, Q(ShowAdminPanel));
        }

        await SendMessageWithImage(ImageFiles.MainMenu);
    }

    [Action]
    public async ValueTask ShowJobForStudents()
    {
        var user = await _userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter = new VacancyFilterModel()
        {
            ForStudents = true,
        };
        await _userService.UpdateUserAsync(user);

        await GlobalState(new VacanciesState());
    }
}
