using Application.Interfaces.Services;
using Bot.Api.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Api.Services;

/// <inheritdoc/>
public sealed class TelegramMessageService(IMessageService messageService, ITelegramBotClient botClient) : ITelegramMessageService
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
            await botClient.DeleteMessageAsync(message.ChatId, message.MessageId);
        }

        await messageService.DeleteAllUserMessages(chatId.Value);
    }
}
