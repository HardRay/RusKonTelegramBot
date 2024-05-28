using Domain.Entities.Common;

namespace Domain.Entities;

/// <summary>
/// Вакансия
/// </summary>
public sealed class Vacancy : BaseEntity
{
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Город
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Формат
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Онлайн
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// График
    /// </summary>
    public string? Schedule { get; set; }

    /// <summary>
    /// Зарплата
    /// </summary>
    public string? Salary { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public string? Direction { get; set; }

    /// <summary>
    /// Описание направления
    /// </summary>
    public string? DirectionDescription { get; set; }
}
