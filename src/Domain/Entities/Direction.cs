using Domain.Entities.Common;

namespace Domain.Entities;

/// <summary>
/// Направление
/// </summary>
public sealed class Direction : BaseEntity
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
