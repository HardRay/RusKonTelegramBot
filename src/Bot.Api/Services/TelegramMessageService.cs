using Application.Interfaces.Services;
using Bot.Api.Services.Interfaces;
using Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.Services;

/// <inheritdoc/>
public sealed class TelegramMessageService(
    IMessageService messageService,
    ITelegramBotClient client,
    ILogger<TelegramMessageService> logger) : ITelegramMessageService
{
    /// <inheritdoc/>
    public async Task UpdateOrSendMessageAsync(long? chatId, string text, IReplyMarkup? replyMarkup)
    {
        if (chatId == null)
            return;

        var lastMessage = await messageService.GetLastUserMessageAsync(chatId.Value);

        if (lastMessage != null)
        {
            const int maxHoursSinceLastMessage = 48;
            var hoursSinceLastMessage = (DateTime.UtcNow - lastMessage.CreateDateTimeUtc).Hours;

            if (lastMessage.Sender == MessageSender.Bot && hoursSinceLastMessage < maxHoursSinceLastMessage)
            {
                try
                {
                    await client.EditMessageTextAsync(chatId.Value, lastMessage.MessageId, text, ParseMode.Html, replyMarkup: replyMarkup as InlineKeyboardMarkup);

                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Ошибка редактирования сообщения");
                }
            }
        }

        var message = await client.SendTextMessageAsync(chatId, text, ParseMode.Html, replyMarkup: replyMarkup);

        await InsertAsync(message);
    }

    /// <inheritdoc/>
    public async Task InsertAsync(Message? message)
    {
        if (message == null)
            return;

        var sender = (message.From?.IsBot ?? false) ? MessageSender.Bot : MessageSender.User;

        await messageService.InsertAsync(message.Chat.Id, message.MessageId, sender);
    }

    /// <inheritdoc/>
    public async Task DeleteAllUserMessagesExceptLastAsync(long? chatId)
    {
        if (chatId == null)
            return;

        var messages = await messageService.GetAllUserMessagesAsync(chatId.Value);
        if (!messages.Any())
            return;
        messages = messages.OrderBy(x => x.CreateDateTimeUtc);
        int allButLastCount = messages.Count() - 1;
        messages = messages.Take(allButLastCount);

        foreach (var message in messages)
        {
            try
            {
                await client.DeleteMessageAsync(message.ChatId, message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка удаления сообщения");
            }
        }

        var messageIds = messages.Select(x => x.MessageId);
        await messageService.DeleteMessagesByIdsAsync(messageIds);
    }
}
