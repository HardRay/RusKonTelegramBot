using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Constansts;
using Bot.Api.BotControllers.Common;
using Bot.Api.Helpers;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.BotControllers;

public sealed record CityState;

public sealed class CityController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BaseController<CityState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ClearVacancyFilter();

        await ShowCityChoosingMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowCityChoosingMessage()
    {
        PushL(BotText.CitiesStartMessage);

        Button(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(BotText.ShowCitiesButton));
        RowButton(BotText.OnlineJobButton, Q(ShowOnlineJobs));
        RowButton(BotText.BackButton, Q(ShowMainMenu));
        Button(BotText.SkipStepButton, Q(ShowJobTypes));

        await SendMessageWithImage(ImageFiles.SelectCity);
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
        var queryResults = InlineHelper.GenerateCitiesInlineListAsync(cities, inlineQueryText);
        await Context.Bot.Client.AnswerInlineQueryAsync(inlineQuery.Id, queryResults, cacheTime: 1);

        Context.StopHandling();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.Text)]
    [Filter(And: Filters.CurrentGlobalState)]
    private async Task UnknownCityText()
    {
        var message = Context.Update.Message;
        if (message == null)
        {
            return;
        }

        Context.StopHandling();

        await _messageService.InsertAsync(message);

        var city = message.Text!;

        var isValid = await vacancyService.ValidateCityAsync(city);

        if (!isValid)
        {
            PushL(BotText.WrongCityText);
            await SendMessage();
            return;
        }

        var user = await _userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter.City = city;
        user.VacancyFilter.Format = JobFormat.Offline;
        await _userService.UpdateUserAsync(user);

        await ShowJobTypes();
    }

    [Action]
    public async ValueTask ShowOnlineJobs()
    {
        var user = await _userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter = new VacancyFilterModel()
        {
            Format = JobFormat.Online,
        };
        await _userService.UpdateUserAsync(user);

        await GlobalState(new VacanciesState());
    }
}
