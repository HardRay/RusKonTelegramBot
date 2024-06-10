using Domain.Entities.Common;

namespace Domain.Entities;

/// <summary>
/// Город
/// </summary>
public sealed class City : BaseEntity
{
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Ссылка на миниатюру
    /// </summary>
    public string? PhotoUrl { get; set; } 
}
