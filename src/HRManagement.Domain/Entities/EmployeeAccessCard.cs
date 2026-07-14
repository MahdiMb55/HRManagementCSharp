using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeAccessCard : Entity
{
    private EmployeeAccessCard()
    {
    }

    private EmployeeAccessCard(
        long employeeId,
        long employmentPeriodId,
        string cardNumber,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        EmploymentPeriodId = employmentPeriodId;
        CardNumber = cardNumber.Trim();
        StartDate = startDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAtUtc = nowUtc;
    }

    public long EmployeeId { get; private set; }
    public long EmploymentPeriodId { get; private set; }
    public string CardNumber { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<EmployeeAccessCard> Create(
        long employeeId,
        long employmentPeriodId,
        string? cardNumber,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || employmentPeriodId <= 0)
        {
            return ValidationResult<EmployeeAccessCard>.Failure(
                "access_card.employee.required",
                "کارمند و دوره استخدام برای ثبت کارت تردد الزامی است.");
        }

        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return ValidationResult<EmployeeAccessCard>.Failure(
                "access_card.number.required",
                "شماره کارت تردد الزامی است.");
        }

        return ValidationResult<EmployeeAccessCard>.Success(
            new EmployeeAccessCard(employeeId, employmentPeriodId, cardNumber, startDate, notes, nowUtc));
    }

    public ValidationResult<bool> End(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            return ValidationResult<bool>.Failure(
                "access_card.end.before_start",
                "تاریخ پایان کارت تردد نمی‌تواند پیش از شروع آن باشد.");
        }

        EndDate = endDate;
        return ValidationResult<bool>.Success(true);
    }
}
