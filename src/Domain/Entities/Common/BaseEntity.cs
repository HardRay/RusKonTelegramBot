using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.Common;

/// <summary>
/// Базовая сущность
/// </summary>
public abstract class BaseEntity
{
    public BaseEntity()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }

    /// <summary>
    /// Идентификатор
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public virtual string Id { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public virtual DateTime? CreateDateTimeUtc { get; set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public virtual DateTime? ModifyDateTimeUtc { get; set; }
}
