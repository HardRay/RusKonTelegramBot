using Application.Interfaces.Services;
using Application.Models;
using Bot.Api.Options;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;
using Microsoft.Extensions.Options;

namespace Bot.Api.BotControllers;

public sealed record VacanciesState;

public sealed class VacanciesController(
    IOptions<AppOptions> appOptions,
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
        var userTelegramId = Context.GetSafeChatId();
        if (userTelegramId == null)
            return;

        var vacancies = await vacancyService.GetFilterdVacanciesAsync(userTelegramId.Value);

        if (!vacancies.Any())
        {
            await ShowNotFoundVacanciesMessage();
            return;
        }

        foreach(var vacancy in vacancies)
        {
            PushL($"<b>{vacancy.Name}</b>");
            if (!string.IsNullOrEmpty(vacancy.Salary))
                PushL(vacancy.Salary);
            PushL($"Подробнее: /vacancy_{vacancy.Id}");

            PushL();
        }

        RowButton(SharedResource.NoSuitableVacancyButton, Q(ShowNotFoundVacanciesMessage));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        var message = await Send();

        await messageService.InsertAsync(message);
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

        await messageService.InsertAsync(message);

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

        await messageService.DeleteAllUserMessages(Context.GetSafeChatId());

        await ShowVacancyDescription(vacancy);
        Button(SharedResource.ApplyVacancyButton, Q(ApplyVacancy, vacancyId));
        Button(SharedResource.QuestionButton, appOptions.Value.TelegramHRGroup);
        RowButton(SharedResource.BackButton, Q(ShowVacancies));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await messageService.InsertAsync(await Send());
    }

    private async ValueTask ShowWrongVacancyIdMessage()
    {
        PushL(SharedResource.WrongVacancyId);
        RowButton(SharedResource.BackButton, Q(ShowVacancies));
        await messageService.InsertAsync(await Send());
    }

    private ValueTask ShowVacancyDescription(VacancyModel vacancy)
    {
        PushL($"<b>{vacancy.Name}</b>");
        if (!string.IsNullOrEmpty(vacancy.City))
            PushL($"Город: {vacancy.City}");
        if (!string.IsNullOrEmpty(vacancy.Format))
            PushL($"Формат: {vacancy.Format}");
        if (!string.IsNullOrEmpty(vacancy.Type))
            PushL($"Тип: {vacancy.Type}");
        if (!string.IsNullOrEmpty(vacancy.Schedule))
            PushL($"График: {vacancy.Schedule}");
        if (!string.IsNullOrEmpty(vacancy.Salary))
            PushL($"Зарплата: {vacancy.Salary}");
        if (!string.IsNullOrEmpty(vacancy.Direction))
            PushL($"Направление: {vacancy.Direction}");
        if (!string.IsNullOrEmpty(vacancy.DirectionDescription))
            PushL($"Описание направления: {vacancy.DirectionDescription}");

        return ValueTask.CompletedTask;
    }

    [Action]
    public async ValueTask ApplyVacancy(string vacancyId)
    {
        PushL(SharedResource.SuccessApplyVacancyText);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));
        await messageService.InsertAsync(await Send());
    }

    [Action]
    public async ValueTask ShowNotFoundVacanciesMessage()
    {
        PushL(SharedResource.NotFoundVacanciesButton);

        RowButton(SharedResource.SubscribeToNotificationButton, Q(SubscribeToNotification));
        RowButton(SharedResource.LeaveResumeButton, Q(ShowMainMenu));
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));

        await messageService.InsertAsync(await Send());
    }

    [Action]
    public async ValueTask SubscribeToNotification()
    {
        PushL(SharedResource.SuccessSubscribtionToNotification);
        RowButton(SharedResource.BackToMainMenuButton, Q(ShowMainMenu));
        await messageService.InsertAsync(await Send());
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
