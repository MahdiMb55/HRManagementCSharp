using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeResponsibilityHistory : Entity
{
    private EmployeeResponsibilityHistory()
    {
    }

    private EmployeeResponsibilityHistory(
        long employeeId,
        long employmentPeriodId,
        long responsibilityId,
        DateOnly startDate,
        bool isPrimary,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        EmploymentPeriodId = employmentPeriodId;
        ResponsibilityId = responsibilityId;
        StartDate = startDate;
        IsPrimary = isPrimary;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAtUtc = nowUtc;
    }

    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public long ResponsibilityId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<EmployeeResponsibilityHistory> Create(
        long employeeId,
        long employmentPeriodId,
        long responsibilityId,
        DateOnly startDate,
        bool isPrimary,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || employmentPeriodId <= 0 || responsibilityId <= 0)
        {
            return ValidationResult<EmployeeResponsibilityHistory>.Failure(
                "employee_responsibility.required",
                "کارمند، دوره استخدام و مسئولیت برای ثبت مسئولیت الزامی است.");
        }

        return ValidationResult<EmployeeResponsibilityHistory>.Success(
            new EmployeeResponsibilityHistory(employeeId, employmentPeriodId, responsibilityId, startDate, isPrimary, notes, nowUtc));
    }

    public ValidationResult<bool> End(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            return ValidationResult<bool>.Failure(
                "employee_responsibility.end.before_start",
                "تاریخ پایان مسئولیت نمی‌تواند پیش از شروع آن باشد.");
        }

        EndDate = endDate;
        return ValidationResult<bool>.Success(true);
    }

    public void MarkPrimary()
    {
        IsPrimary = true;
    }

    public void ClearPrimary()
    {
        IsPrimary = false;
    }
}
