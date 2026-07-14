using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class Contract : AuditableEntity
{
    private Contract()
    {
    }

    private Contract(
        long employeeId,
        long employmentPeriodId,
        string contractNumber,
        ContractType contractType,
        DateOnly startDate,
        DateOnly? endDate,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        EmploymentPeriodId = employmentPeriodId;
        ContractNumber = contractNumber.Trim();
        ContractType = contractType;
        StartDate = startDate;
        EndDate = endDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        InitializeTimestamps(nowUtc);
    }

    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public string ContractNumber { get; private set; } = string.Empty;
    public ContractType ContractType { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public static ValidationResult<Contract> Create(
        long employeeId,
        long employmentPeriodId,
        string? contractNumber,
        ContractType contractType,
        DateOnly startDate,
        DateOnly? endDate,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || employmentPeriodId <= 0)
        {
            return ValidationResult<Contract>.Failure(
                "contract.employee.required",
                "کارمند و دوره استخدام برای ثبت قرارداد الزامی است.");
        }

        if (string.IsNullOrWhiteSpace(contractNumber))
        {
            return ValidationResult<Contract>.Failure(
                "contract.number.required",
                "شماره قرارداد الزامی است.");
        }

        if (endDate is not null && endDate < startDate)
        {
            return ValidationResult<Contract>.Failure(
                "contract.end.before_start",
                "تاریخ پایان قرارداد نمی‌تواند پیش از شروع باشد.");
        }

        return ValidationResult<Contract>.Success(
            new Contract(employeeId, employmentPeriodId, contractNumber, contractType, startDate, endDate, notes, nowUtc));
    }
}
