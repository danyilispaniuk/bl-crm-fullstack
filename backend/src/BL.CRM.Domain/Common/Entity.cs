namespace BL.CRM.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides a primary key and basic audit timestamps.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
