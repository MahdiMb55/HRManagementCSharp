using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDocumentVersion : Entity
{
    public long EmployeeDocumentId { get; private set; }
    public long ManagedFileId { get; private set; }
    public int VersionNumber { get; private set; }
    public bool IsCurrent { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
