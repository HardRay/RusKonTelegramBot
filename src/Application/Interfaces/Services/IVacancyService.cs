namespace Application.Interfaces.Services;

/// <summary>
/// Сервис вакансий
/// </summary>
public interface IVacancyService
{
    /// <summary>
    /// Загрузить вакансии из файла
    /// </summary>
    /// <param name="stream">Файл</param>
    Task UploadVacanciesAsync(Stream stream);

    /// <summary>
    /// Получить список всех городов
    /// </summary>
    /// <returns>Список городов</returns>
    Task<IEnumerable<string>> GetAllCitiesAsync();

    /// <summary>
    /// Валидация города
    /// </summary>
    Task<bool> ValidateCityAsync(string city);
}
