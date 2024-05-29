using Application.Interfaces.Services;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record VacanciesState;

public sealed class VacanciesController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BotControllerState<VacanciesState>
{
    public override async ValueTask OnEnter()
    {
        await ShowVacancies();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowVacancies()
    {
        PushL("Список вакансий");

        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }

    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async Task Start()
    {
        await messageService.InsertAsync(Context.Update.Message);

        await ShowVacancies();
    }
}
