using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeResponsibilityHistory : Entity
{
    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public long ResponsibilityId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
