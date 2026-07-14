using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employment;

public interface IEmploymentLifecycleRepository
{
    Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken);
    Task<bool> HasOpenEmploymentPeriodAsync(long employeeId, CancellationToken cancellationToken);
    Task<long?> GetOpenEmploymentPeriodIdAsync(long employeeId, CancellationToken cancellationToken);

    Task<long> StartEmploymentAsync(
        long employeeId,
        DateOnly hireDate,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<EmploymentLifecycleResult> TerminateAsync(
        long employeeId,
        DateOnly terminationDate,
        TerminationType terminationType,
        string reason,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<EmploymentLifecycleResult> ChangeStatusAsync(
        long employeeId,
        long employmentPeriodId,
        EmploymentStatus status,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
