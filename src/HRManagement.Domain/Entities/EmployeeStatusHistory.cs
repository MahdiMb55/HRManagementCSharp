using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeStatusHistory : Entity
{
    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public EmploymentStatus Status { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
