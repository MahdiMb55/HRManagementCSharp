using HRManagement.Application.Files;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Files;

public sealed class EfEmployeeFileRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmployeeFileRepository
{
    public async Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Employees.AnyAsync(
            employee => employee.Id == employeeId && !employee.IsDeleted,
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

    public async Task<FileRecordResult> CreateContractAsync(
        Contract contract,
        ManagedFile? attachment,
        string? attachmentDescription,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var overlaps = await context.Contracts.AnyAsync(
            existing =>
                existing.EmployeeId == contract.EmployeeId &&
                existing.EmploymentPeriodId == contract.EmploymentPeriodId &&
                !existing.IsDeleted &&
                existing.StartDate <= (contract.EndDate ?? DateOnly.MaxValue) &&
                (existing.EndDate ?? DateOnly.MaxValue) >= contract.StartDate,
            cancellationToken);
        if (overlaps)
        {
            return FileRecordResult.Failure(
                FileRecordFailureCode.DuplicateRecord,
                "بازه قرارداد با قرارداد موجود هم‌پوشانی دارد.");
        }

        if (attachment is not null)
        {
            context.ManagedFiles.Add(attachment);
            await context.SaveChangesAsync(cancellationToken);
        }

        context.Contracts.Add(contract);
        await context.SaveChangesAsync(cancellationToken);

        if (attachment is not null)
        {
            var attachmentEntity = ContractAttachment.Create(contract.Id, attachment.Id, attachmentDescription, nowUtc);
            if (!attachmentEntity.IsSuccess)
            {
                return FileRecordResult.Failure(FileRecordFailureCode.InvalidInput, attachmentEntity.Errors[0].Message);
            }

            context.ContractAttachments.Add(attachmentEntity.Value!);
        }

        AddAudit(context, nameof(Contract), contract.Id, "contract.created", "Contract created.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return FileRecordResult.Success(contract.Id);
    }

    public async Task<FileRecordResult> AddEmployeeDocumentVersionAsync(
        EmployeeDocument document,
        ManagedFile file,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.ManagedFiles.Add(file);
        context.EmployeeDocuments.Add(document);
        await context.SaveChangesAsync(cancellationToken);

        var currentVersions = await context.EmployeeDocumentVersions
            .Where(version => version.EmployeeDocumentId == document.Id && version.IsCurrent)
            .ToListAsync(cancellationToken);
        foreach (var currentVersion in currentVersions)
        {
            currentVersion.ClearCurrent();
        }

        var version = EmployeeDocumentVersion.Create(document.Id, file.Id, 1, isCurrent: true, nowUtc);
        if (!version.IsSuccess)
        {
            return FileRecordResult.Failure(FileRecordFailureCode.InvalidInput, version.Errors[0].Message);
        }

        context.EmployeeDocumentVersions.Add(version.Value!);
        AddAudit(context, nameof(EmployeeDocument), document.Id, "employee_document.added", "Employee document added.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return FileRecordResult.Success(document.Id);
    }

    public async Task<FileRecordResult> SetProfilePhotoAsync(
        long employeeId,
        ManagedFile file,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var employee = await context.Employees.SingleOrDefaultAsync(
            candidate => candidate.Id == employeeId && !candidate.IsDeleted,
            cancellationToken);
        if (employee is null)
        {
            return FileRecordResult.Failure(FileRecordFailureCode.EmployeeNotFound, "کارمند موردنظر پیدا نشد.");
        }

        context.ManagedFiles.Add(file);
        await context.SaveChangesAsync(cancellationToken);
        employee.SetProfilePhoto(file.Id, nowUtc);
        AddAudit(context, nameof(Employee), employee.Id, "profile_photo.updated", "Profile photo updated.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return FileRecordResult.Success(file.Id);
    }

    private static void AddAudit(
        HrManagementDbContext context,
        string entityType,
        long entityId,
        string action,
        string description,
        DateTime nowUtc) =>
        context.AuditLogs.Add(AuditLog.Create(entityType, entityId, action, description, nowUtc));
}
