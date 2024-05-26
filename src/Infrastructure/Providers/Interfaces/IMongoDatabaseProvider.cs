using MongoDB.Driver;

namespace Infrastructure.Providers.Interfaces;

/// <summary>
/// Поставщик базы данных
/// </summary>
internal interface IMongoDatabaseProvider
{
    /// <summary>
    /// Получение БД
    /// </summary>
    /// <returns>БД</returns>
    IMongoDatabase GetDatabase();
}
