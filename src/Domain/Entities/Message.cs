using Domain.Entities.Common;

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
}
