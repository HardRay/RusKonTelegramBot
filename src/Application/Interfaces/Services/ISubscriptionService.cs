using Application.Models;
using Domain.Entities;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис подписок
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Создание подписки
    /// </summary>
    /// <param name="subscription">Подписка</param>
    Task CreateAsync(SubscriptionModel subscription);

    /// <summary>
    /// Получение списка подписок, у которых есть вакансии
    /// </summary>
    /// <returns>Список подписок, у которых есть вакансии</returns>
    Task<IEnumerable<SubscriptionModel>> GetSubscriptionWithVacanciesAsync();
}
