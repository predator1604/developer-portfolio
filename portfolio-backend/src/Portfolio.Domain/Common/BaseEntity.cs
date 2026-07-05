using MongoDB.Bson;

namespace Portfolio.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides MongoDB _id, creation and update timestamps.
/// </summary>
public abstract class BaseEntity
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    public void Touch() => UpdatedAt = DateTime.UtcNow;
}
