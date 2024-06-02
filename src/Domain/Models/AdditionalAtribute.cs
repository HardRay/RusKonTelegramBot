namespace Domain.Models;

/// <summary>
/// Дополнительный атрибут вакансии
/// </summary>
public sealed class AdditionalAtribute
{
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; set; } = null!;
}
