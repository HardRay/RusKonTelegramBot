namespace Infrastructure.Models.Options;

/// <summary>
/// Настройки MongoDB
/// </summary>
public sealed class MongoDBOptions
{
    /// <summary>
    /// Строка подключения
    /// </summary>
    public string ConnectionString { get; init; } = null!;

    /// <summary>
    /// Название базы данных
    /// </summary>
    public string DatabaseName { get; init; } = null!;
}
