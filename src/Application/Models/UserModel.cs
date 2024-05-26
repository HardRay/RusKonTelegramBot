namespace Application.Models;

/// <summary>
/// Модель пользователя
/// </summary>
public sealed class UserModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Идентификатор в Telegram
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// Имя пользователя в Telegram
    /// </summary>
    public string? TelegramUserName { get; set; }
}
