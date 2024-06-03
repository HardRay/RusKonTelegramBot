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
}
