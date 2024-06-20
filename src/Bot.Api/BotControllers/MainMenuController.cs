using Application.Interfaces.Services;
using Application.Models.Constansts;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Domain.Enums;
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

        await ShowKeyboard();

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

    private async Task ShowKeyboard()
    {
        PushL(BotText.MainMenuKeyboardMessage);

        RowKButton(Q(ShowAboutCompanyByKeyboard));
        RowKButton(Q(ShowCitiesByKeyboard));
        RowKButton(Q(ShowJobForStudentsByKeyboard));
        RowKButton(Q(ContactWithHRByKeyboard));

        var message = await Send();

        await _messageService.InsertAsync(message, MessageMarkupType.Keyboard);

        await _messageService.DeleteAllUserMessagesWithKeyboardExceptLastAsync(Context.GetSafeChatId());
    }
}
