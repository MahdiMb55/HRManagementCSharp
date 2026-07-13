using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class DocumentCategory : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystemCategory { get; private set; }
    public bool IsActive { get; private set; }
}
