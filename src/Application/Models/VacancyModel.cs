using Domain.Enums;
using MongoDB.Bson;

namespace Application.Models;

/// <summary>
/// Модель вакансии
/// </summary>
public sealed class VacancyModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public ObjectId Id { get; set; }

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
    /// Дополнительные атрибуты
    /// </summary>
    public IEnumerable<AdditionalAtributeModel> AdditionalAtributes { get; set; } = [];

    public string FormatString => Format switch 
    { 
        JobFormat.Offline => "Оффлайн",
        JobFormat.Online => "Онлайн",
        JobFormat.Hybrid => "Гибрид",
        _ => string.Empty
    };
}
