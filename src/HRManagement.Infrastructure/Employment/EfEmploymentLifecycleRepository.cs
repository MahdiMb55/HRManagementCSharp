using HRManagement.Application.Employment;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Employment;

public sealed class EfEmploymentLifecycleRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmploymentLifecycleRepository
{
    public async Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Employees.AnyAsync(
            employee => employee.Id == employeeId && !employee.IsDeleted,
            cancellationToken);
    }

    public async Task<bool> HasOpenEmploymentPeriodAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.EmploymentPeriods.AnyAsync(
            period => period.EmployeeId == employeeId && period.EndDate == null,
            cancellationToken);
    }

    public async Task<long?> GetOpenEmploymentPeriodIdAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.EmploymentPeriods
            .Where(period => period.EmployeeId == employeeId && period.EndDate == null)
            .Select(period => (long?)period.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<long> StartEmploymentAsync(
        long employeeId,
        DateOnly hireDate,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var period = EmploymentPeriod.Create(employeeId, hireDate, notes, nowUtc).Value
            ?? throw new InvalidOperationException("Validated employment period creation failed.");
        context.EmploymentPeriods.Add(period);
        await context.SaveChangesAsync(cancellationToken);

        var status = EmployeeStatusHistory.Create(
            employeeId,
            period.Id,
            EmploymentStatus.Active,
            hireDate,
            notes,
            nowUtc).Value ?? throw new InvalidOperationException("Validated status creation failed.");
        context.EmployeeStatusHistories.Add(status);
        context.AuditLogs.Add(AuditLog.Create(
            nameof(EmploymentPeriod),
            period.Id,
            "employment.started",
            "دوره استخدام کارمند آغاز شد.",
            nowUtc));

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return period.Id;
    }

    public async Task<EmploymentLifecycleResult> TerminateAsync(
        long employeeId,
        DateOnly terminationDate,
        TerminationType terminationType,
        string reason,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var period = await context.EmploymentPeriods.SingleOrDefaultAsync(
            candidate => candidate.EmployeeId == employeeId && candidate.EndDate == null,
            cancellationToken);
        if (period is null)
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.NoOpenEmploymentPeriod,
                "برای این کارمند دوره استخدام باز وجود ندارد.");
        }

        if (await context.EmploymentTerminations.AnyAsync(
                termination => termination.EmploymentPeriodId == period.Id,
                cancellationToken))
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.TerminationAlreadyExists,
                "برای این دوره استخدام قبلاً خاتمه همکاری ثبت شده است.");
        }

        var endResult = period.End(terminationDate, nowUtc);
        if (!endResult.IsSuccess)
        {
            return EmploymentLifecycleResult.Failure(
                EmploymentLifecycleFailureCode.InvalidDateRange,
                endResult.Errors[0].Message);
        }

        var activeStatus = await context.EmployeeStatusHistories.SingleOrDefaultAsync(
            status => status.EmployeeId == employeeId && status.EndDate == null,
            cancellationToken);
        if (activeStatus is not null)
        {
            var statusEnd = activeStatus.End(terminationDate);
            if (!statusEnd.IsSuccess)
            {
                return EmploymentLifecycleResult.Failure(
                    EmploymentLifecycleFailureCode.InvalidDateRange,
                    statusEnd.Errors[0].Message);
            }
        }

        var termination = EmploymentTermination.Create(
            period.Id,
            terminationType,
            terminationDate,
            reason,
            notes,
            nowUtc).Value ?? throw new InvalidOperationException("Validated termination creation failed.");
        context.EmploymentTerminations.Add(termination);

        var finalStatus = EmployeeStatusHistory.Create(
            employeeId,
            period.Id,
            ToEmploymentStatus(terminationType),
            terminationDate,
            reason,
            nowUtc).Value ?? throw new InvalidOperationException("Validated termination status creation failed.");
        var finalStatusEnd = finalStatus.End(terminationDate);
        if (!finalStatusEnd.IsSuccess)
        {
            throw new InvalidOperationException("Validated termination status end failed.");
        }

        context.EmployeeStatusHistories.Add(finalStatus);
        context.AuditLogs.Add(AuditLog.Create(
            nameof(EmploymentTermination),
            period.Id,
            "employment.terminated",
            "خاتمه همکاری کارمند ثبت شد.",
            nowUtc));

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return EmploymentLifecycleResult.Success(period.Id);
    }

    public async Task<EmploymentLifecycleResult> ChangeStatusAsync(
        long employeeId,
        long employmentPeriodId,
        EmploymentStatus status,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var current = await context.EmployeeStatusHistories.SingleOrDefaultAsync(
            candidate => candidate.EmployeeId == employeeId && candidate.EndDate == null,
            cancellationToken);
        if (current is not null)
        {
            var previousEndDate = startDate.AddDays(-1);
            var endResult = current.End(previousEndDate);
            if (!endResult.IsSuccess)
            {
                return EmploymentLifecycleResult.Failure(
                    EmploymentLifecycleFailureCode.InvalidDateRange,
                    endResult.Errors[0].Message);
            }
        }

        var next = EmployeeStatusHistory.Create(
            employeeId,
            employmentPeriodId,
            status,
            startDate,
            notes,
            nowUtc).Value ?? throw new InvalidOperationException("Validated status creation failed.");
        context.EmployeeStatusHistories.Add(next);
        context.AuditLogs.Add(AuditLog.Create(
            nameof(EmployeeStatusHistory),
            employeeId,
            "employment.status_changed",
            "وضعیت استخدام کارمند تغییر کرد.",
            nowUtc));

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return EmploymentLifecycleResult.Success(next.Id);
    }

    private static EmploymentStatus ToEmploymentStatus(TerminationType terminationType) =>
        terminationType switch
        {
            TerminationType.Dismissed => EmploymentStatus.Dismissed,
            TerminationType.Retired => EmploymentStatus.Retired,
            TerminationType.Deceased => EmploymentStatus.Deceased,
            _ => EmploymentStatus.Resigned,
        };
}
