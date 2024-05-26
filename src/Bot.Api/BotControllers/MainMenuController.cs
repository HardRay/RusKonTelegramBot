using Bot.Api.Resources;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record MainMenuState;

public sealed class MainMenuController : BotControllerState<MainMenuState>
{
    public override async ValueTask OnEnter()
    {
        await ShowMainMenu();
    }

    public override ValueTask OnLeave()
    {
        return ValueTask.CompletedTask;
    }

    [Action]
    public ValueTask ShowMainMenu()
    {
        PushL(SharedResource.MainMenuGreetings);

        RowButton(SharedResource.AboutCompanyButton, Q(ShowAboutCompany));
        RowButton(SharedResource.VacanciesButton, Q(ShowVacancies));
        RowButton(SharedResource.ContactWithHRButton, Q(ContactWithHR));

        return ValueTask.CompletedTask;
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
    public ValueTask ContactWithHR()
    {
        return ValueTask.CompletedTask;
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async ValueTask Start()
    {
        await ShowMainMenu();
    }
}
