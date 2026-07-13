using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class AuditLog : Entity
{
    public string EntityType { get; private set; } = string.Empty;
    public long EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public string? OldValuesJson { get; private set; }
    public string? NewValuesJson { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static AuditLog Create(
        string entityType,
        long entityId,
        string action,
        string description,
        DateTime createdAtUtc) =>
        new()
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Description = description,
            CreatedAtUtc = createdAtUtc.Kind == DateTimeKind.Utc ? createdAtUtc : createdAtUtc.ToUniversalTime(),
        };
}
