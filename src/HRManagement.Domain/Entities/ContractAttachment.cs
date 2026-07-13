using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class ContractAttachment : Entity
{
    public long ContractId { get; private set; }
    public long ManagedFileId { get; private set; }
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
