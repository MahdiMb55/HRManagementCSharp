using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EmploymentTermination : Entity
{
    public long EmploymentPeriodId { get; private set; }
    public TerminationType TerminationType { get; private set; }
    public DateOnly TerminationDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
