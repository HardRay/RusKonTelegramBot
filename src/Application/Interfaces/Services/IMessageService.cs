using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис сообщений
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Получение всех сообщений пользователя
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    /// <returns>Список сообщений</returns>
    Task<IEnumerable<MessageModel>> GetAllUserMessages(long chatId);

    /// <summary>
    /// Создание записи сообщения
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    /// <param name="messageId">Идентификатор сообщения в Telegram</param>
    Task InsertAsync(long chatId, int messageId);

    /// <summary>
    /// Удаление всех сообщений пользователя
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    Task DeleteAllUserMessages(long chatId);
}
