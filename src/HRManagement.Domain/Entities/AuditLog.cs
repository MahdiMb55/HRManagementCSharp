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
}
