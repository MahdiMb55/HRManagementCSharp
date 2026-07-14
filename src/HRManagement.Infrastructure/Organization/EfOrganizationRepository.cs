using HRManagement.Application.Organization;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Organization;

public sealed class EfOrganizationRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IOrganizationRepository
{
    public async Task<long?> GetOpenEmploymentPeriodIdAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.EmploymentPeriods
            .Where(period => period.EmployeeId == employeeId && period.EndDate == null)
            .Select(period => (long?)period.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ActiveDepartmentExistsAsync(long departmentId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Departments.AnyAsync(
            department => department.Id == departmentId && department.IsActive,
            cancellationToken);
    }

    public async Task<bool> ActiveResponsibilityExistsAsync(long responsibilityId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Responsibilities.AnyAsync(
            responsibility => responsibility.Id == responsibilityId && responsibility.IsActive,
            cancellationToken);
    }

    public async Task<OrganizationResult> AssignDepartmentAsync(
        long employeeId,
        long employmentPeriodId,
        long departmentId,
        DateOnly startDate,
        string? transferDescription,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var current = await context.EmployeeDepartmentHistories.SingleOrDefaultAsync(
            history => history.EmployeeId == employeeId && history.EndDate == null,
            cancellationToken);
        if (current is not null)
        {
            if (current.DepartmentId == departmentId)
            {
                return OrganizationResult.Failure(
                    OrganizationFailureCode.DuplicateActiveDepartment,
                    "این کارمند هم‌اکنون در همین واحد فعال است.");
            }

            var endResult = current.End(startDate.AddDays(-1));
            if (!endResult.IsSuccess)
            {
                return OrganizationResult.Failure(
                    OrganizationFailureCode.InvalidDateRange,
                    endResult.Errors[0].Message);
            }
        }

        var assignment = EmployeeDepartmentHistory.Create(
            employeeId,
            employmentPeriodId,
            departmentId,
            startDate,
            transferDescription,
            nowUtc).Value ?? throw new InvalidOperationException("Validated department assignment failed.");
        context.EmployeeDepartmentHistories.Add(assignment);
        context.AuditLogs.Add(AuditLog.Create(
            nameof(EmployeeDepartmentHistory),
            employeeId,
            "organization.department_assigned",
            "واحد سازمانی کارمند تغییر کرد.",
            nowUtc));

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return OrganizationResult.Success(assignment.Id);
    }

    public async Task<OrganizationResult> AssignResponsibilityAsync(
        long employeeId,
        long employmentPeriodId,
        long responsibilityId,
        DateOnly startDate,
        bool isPrimary,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var activeAssignments = await context.EmployeeResponsibilityHistories
            .Where(history => history.EmployeeId == employeeId && history.EndDate == null)
            .ToListAsync(cancellationToken);

        if (activeAssignments.Any(history => history.ResponsibilityId == responsibilityId))
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.DuplicateActiveResponsibility,
                "این مسئولیت هم‌اکنون برای کارمند فعال است.");
        }

        if (isPrimary)
        {
            foreach (var activePrimary in activeAssignments.Where(history => history.IsPrimary))
            {
                activePrimary.ClearPrimary();
            }
        }
        else if (!activeAssignments.Any(history => history.IsPrimary))
        {
            isPrimary = true;
        }

        var assignment = EmployeeResponsibilityHistory.Create(
            employeeId,
            employmentPeriodId,
            responsibilityId,
            startDate,
            isPrimary,
            notes,
            nowUtc).Value ?? throw new InvalidOperationException("Validated responsibility assignment failed.");
        context.EmployeeResponsibilityHistories.Add(assignment);
        context.AuditLogs.Add(AuditLog.Create(
            nameof(EmployeeResponsibilityHistory),
            employeeId,
            "organization.responsibility_assigned",
            "مسئولیت کارمند ثبت شد.",
            nowUtc));

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return OrganizationResult.Success(assignment.Id);
    }

    public async Task<OrganizationResult> EndResponsibilityAsync(
        long assignmentId,
        DateOnly endDate,
        long? newPrimaryAssignmentId,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var assignment = await context.EmployeeResponsibilityHistories.SingleOrDefaultAsync(
            history => history.Id == assignmentId && history.EndDate == null,
            cancellationToken);
        if (assignment is null)
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.ResponsibilityNotFound,
                "مسئولیت فعال پیدا نشد.");
        }

        var endResult = assignment.End(endDate);
        if (!endResult.IsSuccess)
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.InvalidDateRange,
                endResult.Errors[0].Message);
        }

        if (assignment.IsPrimary)
        {
            if (newPrimaryAssignmentId is null)
            {
                var hasOtherActive = await context.EmployeeResponsibilityHistories.AnyAsync(
                    history => history.EmployeeId == assignment.EmployeeId &&
                               history.Id != assignment.Id &&
                               history.EndDate == null,
                    cancellationToken);
                if (hasOtherActive)
                {
                    return OrganizationResult.Failure(
                        OrganizationFailureCode.PrimaryResponsibilityRequired,
                        "برای پایان مسئولیت اصلی باید مسئولیت اصلی جدید انتخاب شود.");
                }
            }
            else
            {
                var newPrimary = await context.EmployeeResponsibilityHistories.SingleOrDefaultAsync(
                    history => history.Id == newPrimaryAssignmentId.Value &&
                               history.EmployeeId == assignment.EmployeeId &&
                               history.EndDate == null,
                    cancellationToken);
                if (newPrimary is null)
                {
                    return OrganizationResult.Failure(
                        OrganizationFailureCode.ResponsibilityNotFound,
                        "مسئولیت اصلی جایگزین پیدا نشد.");
                }

                newPrimary.MarkPrimary();
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return OrganizationResult.Success(assignment.Id);
    }
}
