using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeAccessCard : Entity
{
    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public string CardNumber { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
