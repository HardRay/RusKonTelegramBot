using Bot.Api.Constants;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record MainMenuState;

public sealed class MainMenuController(ITelegramMessageService messageService, IOptions<AppOptions> appOptions) : BotControllerState<MainMenuState>
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
        var userId = Context.GetSafeChatId();

        PushL(SharedResource.MainMenuGreetings);

        RowButton(SharedResource.AboutCompanyButton, Q(ShowAboutCompany));
        RowButton(SharedResource.VacanciesButton, Q(ShowVacancies));
        RowButton(SharedResource.ContactWithHRButton, Q(ContactWithHR));

        if (userId == appOptions.Value.AdminTelegramId)
        {
            RowButton(SharedResource.AdminPanelButton, Q(ShowAdminPanel));
        }

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowAboutCompany()
    {
        await GlobalState(new AboutCompanyState());
    }

    [Action]
    public async ValueTask ShowVacancies()
    {
        await GlobalState(new CityState());
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
}
