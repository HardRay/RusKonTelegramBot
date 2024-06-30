namespace Application.Models;

public sealed class DirectionModel
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
