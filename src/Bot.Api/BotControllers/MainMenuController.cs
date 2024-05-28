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
    public ValueTask ContactWithHR()
    {
        return ValueTask.CompletedTask;
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    public async ValueTask Start()
    {
        await messageService.InsertAsync(Context.Update.Message);

        await ShowMainMenu();
    }
}
