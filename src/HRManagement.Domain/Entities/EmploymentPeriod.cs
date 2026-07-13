using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmploymentPeriod : AuditableEntity
{
    private EmploymentPeriod()
    {
    }

    private EmploymentPeriod(long employeeId, DateOnly hireDate, string? notes, DateTime nowUtc)
    {
        EmployeeId = employeeId;
        HireDate = hireDate;
        Notes = notes?.Trim();
        InitializeTimestamps(nowUtc);
    }

    public long EmployeeId { get; private set; }

    public DateOnly HireDate { get; private set; }

    public DateOnly? EndDate { get; private set; }

    public string? Notes { get; private set; }

    public static ValidationResult<EmploymentPeriod> Create(
        long employeeId,
        DateOnly hireDate,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0)
        {
            return ValidationResult<EmploymentPeriod>.Failure(
                "employment_period.employee.required",
                "کارمند برای دوره استخدام الزامی است.");
        }

        return ValidationResult<EmploymentPeriod>.Success(
            new EmploymentPeriod(employeeId, hireDate, notes, nowUtc));
    }

    public ValidationResult<bool> End(DateOnly endDate, DateTime nowUtc)
    {
        if (endDate < HireDate)
        {
            return ValidationResult<bool>.Failure(
                "employment_period.end.before_hire",
                "تاریخ پایان استخدام نمی‌تواند پیش از تاریخ استخدام باشد.");
        }

        EndDate = endDate;
        Touch(nowUtc);
        return ValidationResult<bool>.Success(true);
    }
}
