using Domain.Enums;

namespace Application.Models;

/// <summary>
/// Модель сообщения
/// </summary>
public sealed class MessageModel
{
    /// <summary>
    /// Идентификатор чата в Telegram
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// Идентификатор сообщения в Telegram
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// Отправитель
    /// </summary>
    public MessageSender Sender { get; set; }

    /// <summary>
    /// Тип кнопок
    /// </summary>
    public MessageMarkupType MarkupType { get; set; }

    /// <summary>
    /// Содержит изображение
    /// </summary>
    public bool HasPhoto { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreateDateTimeUtc { get; set; }
}
