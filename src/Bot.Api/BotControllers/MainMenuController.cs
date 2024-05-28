using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record MainMenuState;

public sealed class MainMenuController(ITelegramMessageService messageService) : BotControllerState<MainMenuState>
{
    public override async ValueTask OnEnter()
    {
        await ShowMainMenu();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        PushL(SharedResource.MainMenuGreetings);

        RowButton(SharedResource.AboutCompanyButton, Q(ShowAboutCompany));
        RowButton(SharedResource.VacanciesButton, Q(ShowVacancies));
        RowButton(SharedResource.ContactWithHRButton, Q(ContactWithHR));
        RowButton(SharedResource.AdminPanelButton, Q(ShowAdminPanel));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowAboutCompany()
    {
        await GlobalState(new AboutCompanyState());
    }

    [Action]
    public ValueTask ShowVacancies()
    {
        return ValueTask.CompletedTask;
    }

    [Action]
    public async ValueTask ContactWithHR()
    {
        await GlobalState(new HRState());
    }

    [Action]
    public async ValueTask ShowAdminPanel()
    {
        await GlobalState(new AdminPanelState());
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async Task Start()
    {
        await messageService.InsertAsync(Context.Update.Message);

        await ShowMainMenu();
    }
}
