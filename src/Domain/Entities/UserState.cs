using Domain.Entities.Common;

namespace Domain.Entities;

/// <summary>
/// Модель состояния пользователя. Обновляется в при каждом перемещении пользователя между BotController'ами.
/// </summary>
/// <remarks>
/// Для корректной работы не рекомендуется переименовывать и менять неймспейс у стейтов для BotController'ов.
/// </remarks>
public sealed class UserState : BaseEntity
{
    public long UserId { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
