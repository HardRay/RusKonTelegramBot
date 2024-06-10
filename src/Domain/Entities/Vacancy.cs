using Domain.Entities.Common;
using Domain.Enums;
using Domain.Models;

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
    public JobFormat? Format { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Зарплата
    /// </summary>
    public string? Salary { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public string? Direction { get; set; }

    /// <summary>
    /// Подходит для студентов
    /// </summary>
    public bool ForStudents { get; set; }

    /// <summary>
    /// Уникальный номер вакансии для отображения
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Дополнительные атрибуты
    /// </summary>
    public IEnumerable<AdditionalAtribute> AdditionalAtributes { get; set; } = [];
}
