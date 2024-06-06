using Bot.Api.Constants;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers.Common;

public class BaseController<TState>(ITelegramMessageService messageService) : BotControllerState<TState>
{
    protected readonly ITelegramMessageService _messageService = messageService;

    public async Task SendMessage()
    {
        await _messageService.UpdateOrSendMessageAsync(Context.GetSafeChatId(), Message.Message, Message.Markup);

        ClearMessage();
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async Task Start()
    {
        var receivedMessage = Context.Update.Message;
        await _messageService.InsertAsync(receivedMessage);

        await ShowMainMenu();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.CurrentGlobalState)]
    [Filter(And: Filters.CallbackQuery)]
    public async Task UnknownCallback()
    {
        var callbackQuery = Context.GetCallbackQuery();
        if (!string.IsNullOrWhiteSpace(callbackQuery.Data) && callbackQuery.Data == BotConstants.ShowNewVacanciesCallbackData)
        {
            Context.StopHandling();
            await GlobalState(new VacanciesState());
        }
    }

    [Action("SocialMedia")]
    protected async ValueTask ShowSocialMedia()
    {
        await GlobalState(new SocialMediaState());
    }

    [Action("MainMenu")]
    protected async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }

    [Action("AboutCompany")]
    protected async ValueTask ShowAboutCompany()
    {
        await GlobalState(new AboutCompanyState());
    }

    [Action("Cities")]
    protected async ValueTask ShowCities()
    {
        await GlobalState(new CityState());
    }

    [Action("Vacancies")]
    protected async ValueTask ShowVacancies()
    {
        await GlobalState(new VacanciesState());
    }

    [Action("ContactWithHR")]
    protected async ValueTask ContactWithHR()
    {
        await GlobalState(new HRState());
    }

    [Action("AdminPanel")]
    protected async ValueTask ShowAdminPanel()
    {
        await GlobalState(new AdminPanelState());
    }

    [Action("JobTypes")]
    protected async ValueTask ShowJobTypes()
    {
        await GlobalState(new JobTypeState());
    }

    [Action("Directions")]
    protected async ValueTask ShowDirections()
    {
        await GlobalState(new DirectionState());
    }

    [Action("NotFoundVacancies")]
    protected async ValueTask ShowNotFoundVacancies()
    {
        await GlobalState(new NotFoundVacanciesState());
    }

    [Action("Resume")]
    protected async ValueTask LeaveResume()
    {
        await GlobalState(new ResumeState());
    }
}
