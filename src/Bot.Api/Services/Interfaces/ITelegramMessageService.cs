using Telegram.Bot.Types;

namespace Bot.Api.Services.Interfaces;

/// <summary>
/// Сервис сообщений в Telegram
/// </summary>
public interface ITelegramMessageService
{
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
