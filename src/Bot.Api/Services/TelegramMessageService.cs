using Application.Interfaces.Services;
using Bot.Api.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Api.Services;

/// <inheritdoc/>
public sealed class TelegramMessageService(
    IMessageService messageService,
    ITelegramBotClient botClient,
    ILogger<TelegramMessageService> logger) : ITelegramMessageService
{
    /// <inheritdoc/>
    public async Task InsertAsync(Message? message)
    {
        if (message == null)
            return;

        await messageService.InsertAsync(message.Chat.Id, message.MessageId);
    }

    /// <inheritdoc/>
    public async Task DeleteAllUserMessages(long? chatId)
    {
        if (chatId == null)
            return;

        var messages = await messageService.GetAllUserMessages(chatId.Value);

        foreach (var message in messages)
        {
            try
            {
                await botClient.DeleteMessageAsync(message.ChatId, message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка удаления сообщения");
            }
        }

        await messageService.DeleteAllUserMessages(chatId.Value);
    }
}
