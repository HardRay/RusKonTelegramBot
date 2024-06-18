using Application.Interfaces.Services;
using Application.Models.Constansts;
using Bot.Api.BotControllers.Common;
using Bot.Api.Resources;
using Bot.Api.Services.Interfaces;
using Deployf.Botf;

namespace Bot.Api.BotControllers;

public sealed record JobTypeState;

public sealed class JobTypeController(
    ITelegramMessageService messageService,
    IVacancyService vacancyService,
    IUserService userService) : BaseController<JobTypeState>(messageService, userService)
{
    public override async ValueTask OnEnter()
    {
        var typesExists = await TypesExist();

        if (typesExists)
            await ShowJobTypeChoosingMessage();
        else
            await ShowDirections();
    }

    public override async ValueTask OnLeave()
    {
        await _messageService.DeleteAllUserMessagesExceptLastAsync(Context.GetSafeChatId());
    }

    [Action]
    public async ValueTask ShowJobTypeChoosingMessage()
    {
        var types = await GetJobTypes();

        PushL(BotText.JobTypeStartMessage);

        int row = 0;
        foreach (var type in types)
        {
            if (row % 2 == 0)
                RowButton(type, Q(ChooseJobType, [type]));
            else
                Button(type, Q(ChooseJobType, [type]));
            row++;
        }
            

        RowButton(BotText.BackButton, Q(ShowCities));
        Button(BotText.SkipStepButton, Q(ShowDirections));

        await SendMessageWithImage(ImageFiles.SelectJobType);
    }

    [Action]
    public async ValueTask ChooseJobType(string jobType)
    {
        var user = await _userService.GetOrCreateUserByTelegramIdAsync(Context.GetSafeChatId()!.Value);
        user.VacancyFilter.Type = jobType;
        await _userService.UpdateUserAsync(user);

        await ShowDirections();
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
            .Where(x => !string.IsNullOrEmpty(x.Type))
            .Select(x => x.Type!)
            .Distinct();

        return types;
    }
}
