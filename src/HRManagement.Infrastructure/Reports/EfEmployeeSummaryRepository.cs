using HRManagement.Application.Reports;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Reports;

public sealed class EfEmployeeSummaryRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmployeeSummaryRepository
{
    public async Task<IReadOnlyList<EmployeeSummaryRow>> GetRowsAsync(
        IReadOnlyCollection<long> employeeIds,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var selected = employeeIds.ToArray();
        return await (
            from employee in context.Employees.AsNoTracking()
            join person in context.Persons.AsNoTracking() on employee.PersonId equals person.Id
            where selected.Length == 0 || selected.Contains(employee.Id)
            select new EmployeeSummaryRow(
                employee.Id,
                employee.PersonnelNumber,
                person.FirstName + " " + person.LastName,
                person.NationalCode,
                employee.MobileNumber,
                (
                    from history in context.EmployeeDepartmentHistories
                    join department in context.Departments on history.DepartmentId equals department.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null
                    select department.Name).FirstOrDefault(),
                (
                    from history in context.EmployeeResponsibilityHistories
                    join responsibility in context.Responsibilities on history.ResponsibilityId equals responsibility.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary
                    select responsibility.Title).FirstOrDefault(),
                (context.EmployeeStatusHistories
                    .Where(history => history.EmployeeId == employee.Id && history.EndDate == null)
                    .Select(history => (EmploymentStatus?)history.Status)
                    .FirstOrDefault() ?? EmploymentStatus.Active).ToString()))
            .ToListAsync(cancellationToken);
    }
}
