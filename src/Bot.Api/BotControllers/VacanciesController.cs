﻿using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Constansts;
using Bot.Api.BotControllers.Common;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Domain.Enums;
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

        RowButton(BotText.NoSuitableVacancyButton, Q(ShowNotFoundVacancies));
        RowButton(BotText.BackButton, Q(ShowCities));

        var user = await _userService.GetOrCreateUserByTelegramIdAsync(userTelegramId.Value);

        var imageFileName = ImageFiles.VacanciesList;
        if (user.VacancyFilter.Format == JobFormat.Online)
            imageFileName = ImageFiles.OnlineJob;
        if (user.VacancyFilter.ForStudents)
            imageFileName = ImageFiles.StudentsJob;

        await SendMessageWithImage(imageFileName);
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

        if (!message.Text.Contains("more"))
            return;

        var commandParts = message.Text.Split('_');
        if (commandParts.Length != 2)
            return;

        await _messageService.InsertAsync(message);

        var vacancyNumberStr = commandParts.Last();
        if (!int.TryParse(vacancyNumberStr, out int vacancyNumber))
            return;

        await ShowVacancy(vacancyNumber);

        Context.StopHandling();
    }

    private async ValueTask ShowVacancy(int vacancyNumber)
    {
        var vacancy = await vacancyService.GetVacancyByNumberAsync(vacancyNumber);
        if (vacancy == null)
        {
            await ShowWrongVacancyIdMessage();
            return;
        }

        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());

        var vacancyDescription = await GetVacancyDescription(vacancy);
        PushLL(vacancyDescription);

        RowButton(BotText.ApplyVacancyButton, Q(ApplyVacancy, vacancy.Id));
        RowButton(BotText.QuestionButton, appOptions.Value.TelegramHRChat);
        RowButton(BotText.BackButton, Q(ShowVacancies));

        await SendMessage();
    }

    private async ValueTask ShowWrongVacancyIdMessage()
    {
        PushL(BotText.WrongVacancyId);
        RowButton(BotText.BackButton, Q(ShowVacancies));
        await SendMessage();
    }

    private static ValueTask<string> GetVacancyDescription(VacancyModel vacancy)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"<b>{vacancy.Name}</b>");
        if (!string.IsNullOrEmpty(vacancy.City))
            stringBuilder.Append($"{Environment.NewLine}Город: {vacancy.City}");
        if (!string.IsNullOrEmpty(vacancy.FormatString))
            stringBuilder.Append($"{Environment.NewLine}Формат: {vacancy.FormatString}");
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

        PushL(BotText.SuccessApplyVacancyText);
        RowButton(BotText.BackToMainMenuButton, Q(ShowMainMenu));
        await SendMessageWithImage(ImageFiles.ApplyForJob);
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
                PushL($"💵 {vacancy.Salary}");

            var address = string.IsNullOrEmpty(vacancy.Address) ? vacancy.City : vacancy.Address;
            if (!string.IsNullOrEmpty(address))
                PushL($"📍 {address}");
            PushL($"Подробнее: /more_{vacancy.Number}");

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
        var vacancy = await vacancyService.GetVacancyByIdAsync(vacancyId);
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

        var messageText = string.Format(BotText.NewRequestText, userName, vacancyDescription);
        await Client.SendTextMessageAsync(appOptions.Value.TelegramHRChatId, messageText, ParseMode.Html);
    }
}
