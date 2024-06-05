using Application.Models;
using Domain.Enums;

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
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    /// <param name="messageId">Идентификатор сообщения в Telegram</param>
    /// <param name="sender">Отправитель</param>
    Task InsertAsync(long chatId, int messageId, MessageSender sender);

    /// <summary>
    /// Удаление всех сообщений пользователя
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    Task DeleteAllUserMessagesAsync(long chatId);
}
