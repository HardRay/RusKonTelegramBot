using Domain.Entities.Common;
using Domain.Models;

namespace Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public sealed class User(long telegramId) : BaseEntity
{
    /// <summary>
    /// Идентификатор в Telegram
    /// </summary>
    public long TelegramId { get; set; } = telegramId;

    /// <summary>
    /// Имя пользователя в Telegram
    /// </summary>
    public string? TelegramUserName { get; set; }

    /// <summary>
    /// Параметры поиска вакансии
    /// </summary>
    public VacancyFilter VacancyFilter { get; set; } = new();
}
