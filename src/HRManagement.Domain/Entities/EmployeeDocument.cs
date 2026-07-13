using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDocument : AuditableEntity
{
    public long EmployeeId { get; private set; }
    public long CategoryId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
}
