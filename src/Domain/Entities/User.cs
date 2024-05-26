using Domain.Entities.Common;

namespace Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public sealed class User : BaseEntity
{
    /// <summary>
    /// Ид в Telegram
    /// </summary>
    public string TelegramId { get; set; } = null!;
}
