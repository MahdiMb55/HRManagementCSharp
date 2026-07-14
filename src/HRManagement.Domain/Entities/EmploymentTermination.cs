using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EmploymentTermination : Entity
{
    private EmploymentTermination()
    {
    }

    private EmploymentTermination(
        long employmentPeriodId,
        TerminationType terminationType,
        DateOnly terminationDate,
        string reason,
        string? notes,
        DateTime nowUtc)
    {
        EmploymentPeriodId = employmentPeriodId;
        TerminationType = terminationType;
        TerminationDate = terminationDate;
        Reason = reason.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAtUtc = nowUtc;
    }

    public long EmploymentPeriodId { get; private set; }
    public TerminationType TerminationType { get; private set; }
    public DateOnly TerminationDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<EmploymentTermination> Create(
        long employmentPeriodId,
        TerminationType terminationType,
        DateOnly terminationDate,
        string reason,
        string? notes,
        DateTime nowUtc)
    {
        if (employmentPeriodId <= 0)
        {
            return ValidationResult<EmploymentTermination>.Failure(
                "employment_termination.period.required",
                "دوره استخدام برای خاتمه همکاری الزامی است.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return ValidationResult<EmploymentTermination>.Failure(
                "employment_termination.reason.required",
                "دلیل خاتمه همکاری الزامی است.");
        }

        return ValidationResult<EmploymentTermination>.Success(
            new EmploymentTermination(employmentPeriodId, terminationType, terminationDate, reason, notes, nowUtc));
    }
}
