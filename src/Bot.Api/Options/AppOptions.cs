namespace Bot.Api.Options;

/// <summary>
/// Настройки приложения
/// </summary>
public sealed class AppOptions
{
    /// <summary>
    /// Ссылка на видео о компании
    /// </summary>
    public string CompanyVideoUrl { get; init; } = null!;

    /// <summary>
    /// Ссылка на сайт компании
    /// </summary>
    public string CompanyWebsiteUrl { get; init; } = null!;

    /// <summary>
    /// Ссылка на канал в Telegram
    /// </summary>
    public string TelegramChannelUrl { get; init; } = null!;

    /// <summary>
    /// Ссылка на группу в VK
    /// </summary>
    public string VKGroupUrl { get; init; } = null!;
}
