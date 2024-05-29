using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис вакансий
/// </summary>
public interface IVacancyService
{
    /// <summary>
    /// Получить отфильтрованные вакансии по фильтру пользователя
    /// </summary>
    /// <param name="userTelegramId">Идентификатор пользователя в Telegram</param>
    /// <returns>Список вакансий</returns>
    Task<IEnumerable<VacancyModel>> GetFilterdVacanciesAsync(long userTelegramId);

    /// <summary>
    /// Получить вакансию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор вакансии</param>
    /// <returns>Вакансия</returns>
    Task<VacancyModel?> GetVacancyById(string id);

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

    /// <summary>
    /// Валидация направления
    /// </summary>
    Task<bool> ValidateDirectionAsync(string direction);
}
