using Domain.Models;
using MongoDB.Bson;

namespace Application.Models;

/// <summary>
/// Подписка на уведомления о появлении новых вакансиях
/// </summary>
public sealed class SubscriptionModel
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя в Telegram
    /// </summary>
    public long UserTelegramId { get; set; }

    /// <summary>
    /// Параметры поиска вакансии
    /// </summary>
    public VacancyFilterModel VacancyFilter { get; set; } = null!;
}
