using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Api.Services.Interfaces;

/// <summary>
/// Сервис сообщений в Telegram
/// </summary>
public interface ITelegramMessageService
{
    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="replyMarkup">Разметка кнопок</param>
    Task UpdateOrSendMessageAsync(long? chatId, string text, IReplyMarkup? replyMarkup);

    /// <summary>
    /// Создание записи сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    Task InsertAsync(Message? message);

    /// <summary>
    /// Удаление всех сообщений пользователя
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    Task DeleteAllUserMessages(long? chatId);
}
