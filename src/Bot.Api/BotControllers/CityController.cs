using Application.Interfaces.Services;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.BotControllers;

public sealed record CityState;

public sealed class CityController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BotControllerState<CityState>
{
    public override async ValueTask OnEnter()
    {
        await ShowStartScreen();
    }

    public override async ValueTask OnLeave()
    {
        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowStartScreen()
    {
        PushL(SharedResource.CitiesStartMessage);

        Button(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(SharedResource.ShowCitiesButton));
        RowButton(SharedResource.OnlineJobButton, Q(ShowMainMenu));
        RowButton(SharedResource.SkipStepButton, Q(ShowMainMenu));
        RowButton(SharedResource.ViewAllVacanciesButton, Q(ShowMainMenu));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
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

        var inlineQueryText = Context.Update.InlineQuery?.Query;
        var cities = await vacancyService.GetAllCitiesAsync();
        var queryResults = GenerateInlineCitiesListAsync(cities, inlineQueryText);
        await Context.Bot.Client.AnswerInlineQueryAsync(inlineQuery.Id, queryResults);

        Context.StopHandling();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.Text)]
    [Filter(And: Filters.CurrentGlobalState)]
    private async Task UnknownText()
    {
        var message = Context.Update.Message;
        if (message == null)
        {
            return;
        }
        await messageService.InsertAsync(message);

        var city = message.Text!;

        var isValid = await vacancyService.ValidateCityAsync(city);

        if (!isValid)
        {
            PushL(SharedResource.WrongCityText);
            await messageService.InsertAsync(await Send());
            return;
        }

        var user = await userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter.City = city;
        user.VacancyFilter.IsOnline = false;
        await userService.UpdateUserAsync(user);

        Context.StopHandling();
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

    private static List<InlineQueryResultArticle> GenerateInlineCitiesListAsync(IEnumerable<string> allCities, string? paramSearch = "")
    {
        if (string.IsNullOrEmpty(paramSearch))
        {
            return allCities
              .Select(r => new InlineQueryResultArticle(r, r, new InputTextMessageContent(r)))
              .Take(10)
              .ToList();
        }

        var result = allCities
          .Where(c => c.Contains(paramSearch, StringComparison.OrdinalIgnoreCase))
          .Select(r => new InlineQueryResultArticle(r, r, new InputTextMessageContent(r)))
          .ToList();

        if (result.Count > 50)
        {
            return result.Take(10).ToList();
        }

        return result;
    }
}
