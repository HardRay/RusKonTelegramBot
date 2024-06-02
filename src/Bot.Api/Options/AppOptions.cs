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

    /// <summary>
    /// Идентификатор чата HR-специалистов
    /// </summary>
    public long TelegramHRChatId { get; init; }

    /// <summary>
    /// Ссылка на чат HR-специалистов
    /// </summary>
    public string TelegramHRChat { get; init; } = null!;

    /// <summary>
    /// Телефон HR-специалистов
    /// </summary>
    public string HRPhone { get; init; } = null!;

    /// <summary>
    /// Идентификатор администратора в Telegram
    /// </summary>
    public long AdminTelegramId { get; init; }
}
