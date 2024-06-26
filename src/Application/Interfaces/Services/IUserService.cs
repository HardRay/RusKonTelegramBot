﻿using Application.Models;

namespace Application.Interfaces.Services;

/// <summary>
/// Сервис пользователей
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получить или создать пользователя по TelegramId
    /// </summary>
    /// <param name="telegramId">Идентификатор пользователя в Telegram</param>
    /// <returns>Модель пользователя</returns>
    Task<UserModel> GetOrCreateUserByTelegramIdAsync(long telegramId);

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="userModel">Модель пользователя</param>
    Task UpdateUserAsync(UserModel userModel);
}
