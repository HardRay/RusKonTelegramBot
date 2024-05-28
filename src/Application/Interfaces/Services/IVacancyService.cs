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
    public Task UploadVacanciesAsync(Stream stream);
}
