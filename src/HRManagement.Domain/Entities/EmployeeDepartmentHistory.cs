using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDepartmentHistory : Entity
{
    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public long DepartmentId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? TransferDescription { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
