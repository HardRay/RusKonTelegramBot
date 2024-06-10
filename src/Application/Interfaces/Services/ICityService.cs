using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис городов
/// </summary>
public interface ICityService
{
    /// <summary>
    /// Загрузить города из файла
    /// </summary>
    /// <param name="stream">Файл</param>
    Task UploadCitiesAsync(Stream stream);

    /// <summary>
    /// Получить все города
    /// </summary>
    /// <returns>Список городов</returns>
    Task<IEnumerable<CityModel>> GetAllCitiesAsync();
}
