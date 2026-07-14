using HRManagement.Application.Abstractions;
using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employment;

public sealed class EmploymentLifecycleService(
    IEmploymentLifecycleRepository repository,
    IClock clock) : IEmploymentLifecycleService
{
    public async Task<EmploymentLifecycleResult> StartEmploymentAsync(
        long employeeId,
        DateOnly hireDate,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        if (await repository.HasOpenEmploymentPeriodAsync(employeeId, cancellationToken))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.OpenEmploymentPeriodExists,
                "برای این کارمند دوره استخدام باز وجود دارد.");
        }

        var periodId = await repository.StartEmploymentAsync(
            employeeId,
            hireDate,
            notes,
            clock.UtcNow,
            cancellationToken);

        return EmploymentLifecycleResult.Success(periodId);
    }

    public async Task<EmploymentLifecycleResult> TerminateAsync(
        long employeeId,
        DateOnly terminationDate,
        TerminationType terminationType,
        string reason,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.InvalidDateRange,
                "دلیل خاتمه همکاری الزامی است.");
        }

        if (!await repository.EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        if (!await repository.HasOpenEmploymentPeriodAsync(employeeId, cancellationToken))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.NoOpenEmploymentPeriod,
                "برای این کارمند دوره استخدام باز وجود ندارد.");
        }

        return await repository.TerminateAsync(
            employeeId,
            terminationDate,
            terminationType,
            reason,
            notes,
            clock.UtcNow,
            cancellationToken);
    }

    public Task<EmploymentLifecycleResult> ReturnToWorkAsync(
        long employeeId,
        DateOnly returnDate,
        string? notes,
        CancellationToken cancellationToken) =>
        StartEmploymentAsync(employeeId, returnDate, notes, cancellationToken);

    public async Task<EmploymentLifecycleResult> ChangeStatusAsync(
        long employeeId,
        EmploymentStatus status,
        DateOnly startDate,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(employeeId, cancellationToken))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        var periodId = await repository.GetOpenEmploymentPeriodIdAsync(employeeId, cancellationToken);
        if (periodId is null)
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.NoOpenEmploymentPeriod,
                "برای این کارمند دوره استخدام باز وجود ندارد.");
        }

        return await repository.ChangeStatusAsync(
            employeeId,
            periodId.Value,
            status,
            startDate,
            notes,
            clock.UtcNow,
            cancellationToken);
    }
}
