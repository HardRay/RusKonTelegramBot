using Application.Interfaces.Services;
using Bot.Api.BotControllers.Common;
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
    IUserService userService) : BaseController<DirectionState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        var directionsExist = await DirectionExist();

        if (directionsExist)
            await ShowDirectionChoosingMessage();
        else
            await ShowVacancies();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowDirectionChoosingMessage()
    {
        PushL(SharedResource.JobTypeStartMessage);

        Button(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(SharedResource.ShowDirectionsButton));
        RowButton(SharedResource.SkipStepButton, Q(ShowVacancies));
        RowButton(SharedResource.ViewAllVacanciesButton, Q(ShowVacancies));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
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

        await _messageService.InsertAsync(message);

        var direction = message.Text!;

        var isValid = await vacancyService.ValidateDirectionAsync(direction);

        if (!isValid)
        {
            PushL(SharedResource.WrongDirectionText);
            await SendMessage();
            return;
        }

        var user = await _userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter.Direction = direction;
        await _userService.UpdateUserAsync(user);

        await ShowVacancies();
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
