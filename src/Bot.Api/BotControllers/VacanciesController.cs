using Application.Interfaces.Services;
using Application.Models;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot.Api.BotControllers;

public sealed record VacanciesState;

public sealed class VacanciesController(
    IOptions<AppOptions> appOptions,
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BaseController<VacanciesState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        await ShowVacanciesMessage();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowVacanciesMessage(int page = 1)
    {
        var userTelegramId = Context.GetSafeChatId();
        if (userTelegramId == null)
            return;

        var vacancies = await vacancyService.GetFilterdVacanciesAsync(userTelegramId.Value);

        if (!vacancies.Any())
        {
            await ShowNotFoundVacancies();
            return;
        }

        ShowVacanciesWithPagination(vacancies, page);

        RowButton(SharedResource.NoSuitableVacancyButton, Q(ShowNotFoundVacancies));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
    }

    [On(Handle.Unknown)]
    [Filter(Filters.Text)]
    [Filter(And: Filters.CurrentGlobalState)]
    private async Task UnknownText()
    {
        var message = Context.Update.Message;
        if (message == null || message.Text == null)
        {
            return;
        }

        if (!message.Text.Contains("vacancy"))
            return;

        var commandParts = message.Text.Split('_');
        if (commandParts.Length != 2)
            return;

        await _messageService.InsertAsync(message);

        var vacancyId = commandParts.Last();
        await ShowVacancy(vacancyId);

        Context.StopHandling();
    }

    private async ValueTask ShowVacancy(string? vacancyId)
    {
        if (vacancyId == null)
        {
            await ShowWrongVacancyIdMessage();
            return;
        }

        var vacancy = await vacancyService.GetVacancyById(vacancyId);
        if (vacancy == null)
        {
            await ShowWrongVacancyIdMessage();
            return;
        }

        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());

        var vacancyDescription = await GetVacancyDescription(vacancy);
        PushLL(vacancyDescription);

        Button(SharedResource.ApplyVacancyButton, Q(ApplyVacancy, vacancyId));
        Button(SharedResource.QuestionButton, appOptions.Value.TelegramHRChat);
        RowButton(SharedResource.BackButton, Q(ShowVacancies));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await SendMessage();
    }

    private async ValueTask ShowWrongVacancyIdMessage()
    {
        PushL(SharedResource.WrongVacancyId);
        RowButton(SharedResource.BackButton, Q(ShowVacancies));
        await SendMessage();
    }

    private static ValueTask<string> GetVacancyDescription(VacancyModel vacancy)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"<b>{vacancy.Name}</b>");
        if (!string.IsNullOrEmpty(vacancy.City))
            stringBuilder.Append($"{Environment.NewLine}Город: {vacancy.City}");
        if (!string.IsNullOrEmpty(vacancy.Format))
            stringBuilder.Append($"{Environment.NewLine}Формат: {vacancy.Format}");
        if (!string.IsNullOrEmpty(vacancy.Type))
            stringBuilder.Append($"{Environment.NewLine}Тип: {vacancy.Type}");
        if (!string.IsNullOrEmpty(vacancy.Salary))
            stringBuilder.Append($"{Environment.NewLine}Зарплата: {vacancy.Salary}");
        if (!string.IsNullOrEmpty(vacancy.Direction))
            stringBuilder.Append($"{Environment.NewLine}Направление: {vacancy.Direction}");
        foreach (var additionalAttribute in vacancy.AdditionalAtributes)
            stringBuilder.Append($"{Environment.NewLine}{additionalAttribute.Name}: {additionalAttribute.Value}");

        var description = stringBuilder.ToString();
        return ValueTask.FromResult(description);
    }

    [Action]
    public async ValueTask ApplyVacancy(string vacancyId)
    {
        await SendRequestToHr(vacancyId);

        PushL(SharedResource.SuccessApplyVacancyText);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));
        await SendMessage();
    }

    [Action]
    public ValueTask EmptyAction()
    {
        return ValueTask.CompletedTask;
    }

    private void ShowVacanciesWithPagination(IEnumerable<VacancyModel> vacancies, int page = 1)
    {
        const int vacanciesOnPage = 5;

        var maxPage = (int)Math.Ceiling(vacancies.Count() / (double)vacanciesOnPage);

        var filteredVacancies = vacancies
            .Skip((page - 1) * vacanciesOnPage)
            .Take(vacanciesOnPage);

        foreach (var vacancy in filteredVacancies)
        {
            PushL($"<b>{vacancy.Name}</b>");
            if (!string.IsNullOrEmpty(vacancy.Salary))
                PushL(vacancy.Salary);
            PushL($"Подробнее: /vacancy_{vacancy.Id}");

            PushL();
        }

        if (page > 1)
            Button("\u2190 Назад", Q(ShowVacanciesMessage, page - 1));
        if (maxPage > 1)
            Button($"{page}/{maxPage}", Q(EmptyAction));
        if (page < maxPage)
            Button("Вперёд \u2192", Q(ShowVacanciesMessage, page + 1));
    }

    private async ValueTask SendRequestToHr(string vacancyId)
    {
        var vacancy = await vacancyService.GetVacancyById(vacancyId);
        if (vacancy == null)
        {
            await ShowWrongVacancyIdMessage();
            return;
        }

        var chatId = Context.GetSafeChatId();
        if (chatId == null)
            return;

        var user = await _userService.GetOrCreateUserByTelegramIdAsync(chatId.Value);
        var firstName = user.FirstName;
        var lastName = user.LastName;

        var nameBuilder = new StringBuilder();
        nameBuilder.Append(firstName);
        if (!string.IsNullOrEmpty(lastName))
        {
            nameBuilder.Append(' ');
            nameBuilder.Append(lastName);
        }

        var userName = $"<a href=\"tg://user?id={chatId}\">{nameBuilder}</a>";
        var vacancyDescription = GetVacancyDescription(vacancy);

        var messageText = string.Format(SharedResource.NewRequestText, userName, vacancyDescription);
        await Client.SendTextMessageAsync(appOptions.Value.TelegramHRChatId, messageText, ParseMode.Html);
    }
}
