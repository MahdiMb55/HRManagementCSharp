using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class Contract : AuditableEntity
{
    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public string ContractNumber { get; private set; } = string.Empty;
    public ContractType ContractType { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
}
