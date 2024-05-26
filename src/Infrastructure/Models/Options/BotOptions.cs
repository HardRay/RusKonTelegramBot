namespace Infrastructure.Models.Options;

/// <summary>
/// Настройки бота
/// </summary>
public sealed class BotOptions
{
    /// <summary>
    /// Имя бота
    /// </summary>
    public string Username { get; init; } = null!;

    /// <summary>
    /// Токен бота
    /// </summary>
    public string ApiToken { get; init; } = null!;
}
