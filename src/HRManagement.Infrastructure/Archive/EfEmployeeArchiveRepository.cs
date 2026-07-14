using HRManagement.Application.Archive;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Archive;

public sealed class EfEmployeeArchiveRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmployeeArchiveRepository
{
    public async Task<EmployeeArchiveSnapshot?> FindAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Employees
            .Where(employee => employee.Id == employeeId)
            .Select(employee => new EmployeeArchiveSnapshot(
                employee.Id,
                employee.PersonnelNumber,
                employee.IsDeleted))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ArchiveResult> ArchiveAsync(
        long employeeId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var employee = await context.Employees.SingleOrDefaultAsync(
            candidate => candidate.Id == employeeId,
            cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.Archive(nowUtc);
        AddAudit(context, employeeId, "employee.archived", "Employee archived.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        return ArchiveResult.Success(employeeId);
    }

    public async Task<ArchiveResult> RestoreAsync(
        long employeeId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var employee = await context.Employees.SingleOrDefaultAsync(
            candidate => candidate.Id == employeeId,
            cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.Restore(nowUtc);
        AddAudit(context, employeeId, "employee.restored", "Employee restored from archive.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        return ArchiveResult.Success(employeeId);
    }

    public async Task<ArchiveResult> DeletePermanentlyAsync(
        long employeeId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var employee = await context.Employees.SingleOrDefaultAsync(
            candidate => candidate.Id == employeeId,
            cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        AddAudit(context, employeeId, "employee.deleted_permanently", "Employee permanently deleted.", nowUtc);
        context.Employees.Remove(employee);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ArchiveResult.Success(employeeId);
    }

    private static ArchiveResult NotFound() =>
        ArchiveResult.Failure(ArchiveFailureCode.EmployeeNotFound, "کارمند موردنظر پیدا نشد.");

    private static void AddAudit(
        HrManagementDbContext context,
        long employeeId,
        string action,
        string description,
        DateTime nowUtc) =>
        context.AuditLogs.Add(AuditLog.Create(nameof(Employee), employeeId, action, description, nowUtc));
}
