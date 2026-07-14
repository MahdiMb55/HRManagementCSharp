using HRManagement.Application.Dashboard;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Dashboard;

public sealed class EfDashboardService(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IDashboardService
{
    private static readonly EmploymentStatus[] DepartedStatuses =
    [
        EmploymentStatus.Resigned,
        EmploymentStatus.Dismissed,
        EmploymentStatus.Retired,
        EmploymentStatus.Deceased,
    ];

    public async Task<DashboardSnapshot> GetSnapshotAsync(CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var currentStatuses = context.EmployeeStatusHistories
            .Where(history => history.EndDate == null)
            .Select(history => new { history.EmployeeId, history.Status });

        var activeEmployees = await context.Employees
            .Where(employee => !employee.IsDeleted)
            .Where(employee => !currentStatuses.Any(status =>
                status.EmployeeId == employee.Id && DepartedStatuses.Contains(status.Status)))
            .CountAsync(cancellationToken);
        var archivedOrDeparted = await context.Employees
            .Where(employee => employee.IsDeleted || currentStatuses.Any(status =>
                status.EmployeeId == employee.Id && DepartedStatuses.Contains(status.Status)))
            .CountAsync(cancellationToken);
        var activeContracts = await context.Contracts
            .CountAsync(contract => !contract.IsDeleted && contract.EndDate == null, cancellationToken);
        var departmentCounts = await (
            from history in context.EmployeeDepartmentHistories.AsNoTracking()
            join department in context.Departments.AsNoTracking() on history.DepartmentId equals department.Id
            join employee in context.Employees.AsNoTracking() on history.EmployeeId equals employee.Id
            where history.EndDate == null && !employee.IsDeleted
            group employee by department.Name into departmentGroup
            orderby departmentGroup.Key
            select new DepartmentEmployeeCount(departmentGroup.Key, departmentGroup.Count()))
            .ToListAsync(cancellationToken);

        return new DashboardSnapshot(activeEmployees, archivedOrDeparted, activeContracts, departmentCounts);
    }
}
