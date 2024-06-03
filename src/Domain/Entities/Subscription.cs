using Domain.Entities.Common;
using Domain.Models;

namespace Domain.Entities;

/// <summary>
/// Подписка на уведомления о появлении новых вакансиях
/// </summary>
public sealed class Subscription : BaseEntity
{
    /// <summary>
    /// Идентификатор пользователя в Telegram
    /// </summary>
    public long UserTelegramId { get; set; }

    /// <summary>
    /// Параметры поиска вакансии
    /// </summary>
    public VacancyFilter VacancyFilter { get; set; } = null!;
}
