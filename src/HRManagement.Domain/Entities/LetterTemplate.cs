using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class LetterTemplate : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long ManagedFileId { get; private set; }
    public bool IsActive { get; private set; }
}
