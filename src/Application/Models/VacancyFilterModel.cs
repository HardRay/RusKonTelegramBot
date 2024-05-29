namespace Application.Models;

/// <summary>
/// Параметры поиска вакансии пользователя
/// </summary>
public sealed class VacancyFilterModel
{
    /// <summary>
    /// Город
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Онлайн
    /// </summary>
    public bool? IsOnline { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public string? Direction { get; set; }
}
