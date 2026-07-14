using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employment;

public interface IEmploymentLifecycleService
{
    Task<EmploymentLifecycleResult> StartEmploymentAsync(
        long employeeId,
        DateOnly hireDate,
        string? notes,
        CancellationToken cancellationToken);

    Task<EmploymentLifecycleResult> TerminateAsync(
        long employeeId,
        DateOnly terminationDate,
        TerminationType terminationType,
        string reason,
        string? notes,
        CancellationToken cancellationToken);

    Task<EmploymentLifecycleResult> ReturnToWorkAsync(
        long employeeId,
        DateOnly returnDate,
        string? notes,
        CancellationToken cancellationToken);

    Task<EmploymentLifecycleResult> ChangeStatusAsync(
        long employeeId,
        EmploymentStatus status,
        DateOnly startDate,
        string? notes,
        CancellationToken cancellationToken);
}
