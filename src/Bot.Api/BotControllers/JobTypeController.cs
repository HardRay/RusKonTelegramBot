using Application.Interfaces.Services;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.BotControllers;

public sealed record JobTypeState;

public sealed class JobTypeController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BotControllerState<JobTypeState>
{
    public override async ValueTask OnEnter()
    {
        var typesExists = await TypesExist();

        if (typesExists)
            await ShowStartScreen();
        else
            await ShowDirection();

    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowStartScreen()
    {
        var types = await GetJobTypes();

        PushL(SharedResource.JobTypeStartMessage);

        foreach (var type in types)
            RowButton(type, Q(ChooseJobType, [type]));

        RowButton(SharedResource.SkipStepButton, Q(ShowDirection));
        RowButton(SharedResource.ViewAllVacanciesButton, Q(ShowMainMenu));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ChooseJobType(string jobType)
    {
        var user = await userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter.Type = jobType;
        await userService.UpdateUserAsync(user);
    }

    [Action]
    public async ValueTask ShowDirection()
    {
        await GlobalState(new DirectionState());
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

        await GlobalState(new MainMenuState());
    }

    private async Task<bool> TypesExist()
    {
        var types = await GetJobTypes();

        return types.Count() > 1;
    }

    private async Task<IEnumerable<string>> GetJobTypes()
    {
        var userTelegramId = Context.GetSafeChatId();
        if (userTelegramId == null)
            return [];

        var vacancies = await vacancyService.GetFilterdVacanciesAsync(userTelegramId.Value);
        var types = vacancies
            .Select(x => x.Type)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct();

        return types;
    }
}
