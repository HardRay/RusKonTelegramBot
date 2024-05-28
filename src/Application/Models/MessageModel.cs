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
}
