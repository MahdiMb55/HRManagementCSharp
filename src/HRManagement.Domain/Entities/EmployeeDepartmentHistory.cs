using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDepartmentHistory : Entity
{
    private EmployeeDepartmentHistory()
    {
    }

    private EmployeeDepartmentHistory(
        long employeeId,
        long employmentPeriodId,
        long departmentId,
        DateOnly startDate,
        string? transferDescription,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        EmploymentPeriodId = employmentPeriodId;
        DepartmentId = departmentId;
        StartDate = startDate;
        TransferDescription = string.IsNullOrWhiteSpace(transferDescription) ? null : transferDescription.Trim();
        CreatedAtUtc = nowUtc;
    }

    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public long DepartmentId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? TransferDescription { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<EmployeeDepartmentHistory> Create(
        long employeeId,
        long employmentPeriodId,
        long departmentId,
        DateOnly startDate,
        string? transferDescription,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || employmentPeriodId <= 0 || departmentId <= 0)
        {
            return ValidationResult<EmployeeDepartmentHistory>.Failure(
                "employee_department.required",
                "کارمند، دوره استخدام و واحد سازمانی برای ثبت انتقال الزامی است.");
        }

        return ValidationResult<EmployeeDepartmentHistory>.Success(
            new EmployeeDepartmentHistory(employeeId, employmentPeriodId, departmentId, startDate, transferDescription, nowUtc));
    }

    public ValidationResult<bool> End(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            return ValidationResult<bool>.Failure(
                "employee_department.end.before_start",
                "تاریخ پایان واحد نمی‌تواند پیش از شروع آن باشد.");
        }

        EndDate = endDate;
        return ValidationResult<bool>.Success(true);
    }
}
