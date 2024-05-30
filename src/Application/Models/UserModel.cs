using Domain.Models;

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

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Параметры поиска вакансии
    /// </summary>
    public VacancyFilterModel VacancyFilter { get; set; } = new();
}
