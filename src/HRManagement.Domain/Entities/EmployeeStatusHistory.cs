using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeStatusHistory : Entity
{
    private EmployeeStatusHistory()
    {
    }

    private EmployeeStatusHistory(
        long employeeId,
        long employmentPeriodId,
        EmploymentStatus status,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        EmploymentPeriodId = employmentPeriodId;
        Status = status;
        StartDate = startDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAtUtc = nowUtc;
    }

    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public EmploymentStatus Status { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<EmployeeStatusHistory> Create(
        long employeeId,
        long employmentPeriodId,
        EmploymentStatus status,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || employmentPeriodId <= 0)
        {
            return ValidationResult<EmployeeStatusHistory>.Failure(
                "employee_status.employee.required",
                "کارمند و دوره استخدام برای ثبت وضعیت الزامی است.");
        }

        return ValidationResult<EmployeeStatusHistory>.Success(
            new EmployeeStatusHistory(employeeId, employmentPeriodId, status, startDate, notes, nowUtc));
    }

    public ValidationResult<bool> End(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            return ValidationResult<bool>.Failure(
                "employee_status.end.before_start",
                "تاریخ پایان وضعیت نمی‌تواند پیش از شروع آن باشد.");
        }

        EndDate = endDate;
        return ValidationResult<bool>.Success(true);
    }
}
