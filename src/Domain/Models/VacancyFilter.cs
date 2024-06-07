using Domain.Enums;

namespace Domain.Models;

/// <summary>
/// Параметры поиска вакансии пользователя
/// </summary>
public sealed class VacancyFilter
{
    /// <summary>
    /// Город
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Формат
    /// </summary>
    public JobFormat? Format { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public string? Direction { get; set; }

    /// <summary>
    /// Для студентов
    /// </summary>
    public bool ForStudents { get; set; } = false;
}
