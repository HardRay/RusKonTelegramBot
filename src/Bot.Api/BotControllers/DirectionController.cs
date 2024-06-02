using Application.Interfaces.Services;
using Bot.Api.Helpers;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.BotControllers;

public sealed record DirectionState;

public sealed class DirectionController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BotControllerState<DirectionState>
{
    public override async ValueTask OnEnter()
    {
        var directionsExist = await DirectionExist();

        if (directionsExist)
            await ShowStartScreen();
        else
            await ShowVacancies();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowStartScreen()
    {
        PushL(SharedResource.JobTypeStartMessage);

        Button(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(SharedResource.ShowDirectionsButton));
        RowButton(SharedResource.SkipStepButton, Q(ShowVacancies));
        RowButton(SharedResource.ViewAllVacanciesButton, Q(ShowVacancies));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
    }

    [Action]
    public async ValueTask ShowVacancies()
    {
        await GlobalState(new VacanciesState());
    }

    [Action]
    public async ValueTask ShowMainMenu()
    {
        await GlobalState(new MainMenuState());
    }

    [On(Handle.Unknown)]
    [Filter(Filters.InlineQuery)]
    [Filter(And: Filters.CurrentGlobalState)]
    private async Task InlineQueryHandler()
    {
        var inlineQuery = Context.Update.InlineQuery;
        if (inlineQuery == null)
        {
            return;
        }

        var userTelegramId = inlineQuery.From.Id;
        var directions = await GetDirections(userTelegramId);

        var inlineQueryText = Context.Update.InlineQuery?.Query;
        var queryResults = InlineHelper.GenerateInlineListAsync(directions, inlineQueryText);
        await Context.Bot.Client.AnswerInlineQueryAsync(inlineQuery.Id, queryResults, cacheTime: 1);

        Context.StopHandling();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.Text)]
    [Filter(And: Filters.CurrentGlobalState)]
    private async Task UnknownDirectionText()
    {
        var message = Context.Update.Message;
        if (message == null)
        {
            return;
        }

        Context.StopHandling();

        await messageService.InsertAsync(message);

        var direction = message.Text!;

        var isValid = await vacancyService.ValidateDirectionAsync(direction);

        if (!isValid)
        {
            PushL(SharedResource.WrongDirectionText);
            await messageService.InsertAsync(await Send());
            return;
        }

        var user = await userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter.Direction = direction;
        await userService.UpdateUserAsync(user);

        await ShowVacancies();
    }


    [Action("Start")]
    [Action("/start", "Показать меню")]
    [Filter(Filters.CurrentGlobalState)]
    public async Task Start()
    {
        await messageService.InsertAsync(Context.Update.Message);

        await GlobalState(new MainMenuState());
    }

    private async Task<bool> DirectionExist()
    {

        var userTelegramId = Context.GetSafeChatId();
        if (userTelegramId == null)
            return false;

        var directions = await GetDirections(userTelegramId.Value);

        return directions.Count() > 1;
    }

    private async Task<IEnumerable<string>> GetDirections(long userTelegramId)
    {
        var vacancies = await vacancyService.GetFilterdVacanciesAsync(userTelegramId);
        var directions = vacancies
            .Where(x => !string.IsNullOrEmpty(x.Direction))
            .Select(x => x.Direction!)
            .Distinct();

        return directions;
    }
}
