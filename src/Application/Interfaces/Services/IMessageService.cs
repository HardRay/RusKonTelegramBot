using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис сообщений
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Получение последнего сообщения чата
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    /// <returns>Сообщение</returns>
    Task<MessageModel> GetLastUserMessageAsync(long chatId);

    /// <summary>
    /// Получение всех сообщений чата
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    /// <returns>Список сообщений</returns>
    Task<IEnumerable<MessageModel>> GetAllUserMessagesAsync(long chatId);

    /// <summary>
    /// Создание записи сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    Task InsertAsync(MessageModel message);

    /// <summary>
    /// Удаление всех сообщений пользователя
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    Task DeleteAllUserMessagesAsync(long chatId);

    /// <summary>
    /// Удаление сообщений по идентификаторам
    /// </summary>
    /// <param name="ids">Идентификаторы сообщений</param>
    Task DeleteMessagesByIdsAsync(IEnumerable<int> ids);
}
