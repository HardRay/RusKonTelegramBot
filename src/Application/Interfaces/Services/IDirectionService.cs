using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис городов
/// </summary>
public interface IDirectionService
{
    /// <summary>
    /// Загрузить направления из файла
    /// </summary>
    /// <param name="stream">Файл</param>
    Task UploadDirectionsAsync(Stream stream);

    /// <summary>
    /// Получить все направления
    /// </summary>
    /// <returns>Список городов</returns>
    Task<IEnumerable<DirectionModel>> GetAllDirectionsAsync();
}
