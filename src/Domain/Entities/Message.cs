using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Сообщение
/// </summary>
public sealed class Message : BaseEntity
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
}
