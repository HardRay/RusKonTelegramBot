using Domain.Enums;
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
    /// Отправка сообщения c изображением
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="imageFileName">Название файла с изображением</param>
    /// <param name="replyMarkup">Разметка кнопок</param>
    /// <param name="eternalMessage">Вечное сообщение</param>
    Task UpdateOrSendMessageWithImageAsync(long? chatId, string text, string imageFileName, IReplyMarkup? replyMarkup, bool eternalMessage = false);

    /// <summary>
    /// Создание записи сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="markupType">Тип кнопок</param>
    Task InsertAsync(Message? message, MessageMarkupType markupType = MessageMarkupType.Inline);

    /// <summary>
    /// Удаление всех сообщений пользователя, кроме последнего
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    Task DeleteAllUserMessagesExceptLastAsync(long? chatId);

    /// <summary>
    /// Удаление всех сообщений пользователя c клавиатурой
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram</param>
    Task DeleteAllUserMessagesWithKeyboardExceptLastAsync(long? chatId);
}
