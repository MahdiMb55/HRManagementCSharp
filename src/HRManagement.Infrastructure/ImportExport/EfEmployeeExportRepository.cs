using HRManagement.Application.Employees.Search;
using HRManagement.Application.ImportExport;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.ImportExport;

public sealed class EfEmployeeExportRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmployeeExportRepository
{
    private static readonly EmploymentStatus[] DepartedStatuses =
    [
        EmploymentStatus.Resigned,
        EmploymentStatus.Dismissed,
        EmploymentStatus.Retired,
        EmploymentStatus.Deceased,
    ];

    public async Task<IReadOnlyList<EmployeeWorkbookRow>> GetRowsAsync(
        string? normalizedQuery,
        EmployeeFilter filter,
        EmployeeSort sort,
        IReadOnlyCollection<long> selectedEmployeeIds,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var selected = selectedEmployeeIds.ToArray();
        var rows =
            from employee in context.Employees.AsNoTracking()
            join person in context.Persons.AsNoTracking() on employee.PersonId equals person.Id
            where filter.IncludeArchived || !employee.IsDeleted
            select new ExportRow
            {
                EmployeeId = employee.Id,
                PersonnelNumber = employee.PersonnelNumber,
                NormalizedPersonnelNumber = employee.NormalizedPersonnelNumber,
                FirstName = person.FirstName,
                NormalizedFirstName = person.NormalizedFirstName,
                LastName = person.LastName,
                NormalizedLastName = person.NormalizedLastName,
                NationalCode = person.NationalCode,
                MobileNumber = employee.MobileNumber,
                NormalizedMobileNumber = employee.NormalizedMobileNumber,
                DepartmentId = context.EmployeeDepartmentHistories
                    .Where(history => history.EmployeeId == employee.Id && history.EndDate == null)
                    .Select(history => (long?)history.DepartmentId)
                    .FirstOrDefault(),
                DepartmentName = (
                    from history in context.EmployeeDepartmentHistories
                    join department in context.Departments on history.DepartmentId equals department.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null
                    select department.Name).FirstOrDefault(),
                PrimaryResponsibilityId = context.EmployeeResponsibilityHistories
                    .Where(history => history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary)
                    .Select(history => (long?)history.ResponsibilityId)
                    .FirstOrDefault(),
                PrimaryResponsibility = (
                    from history in context.EmployeeResponsibilityHistories
                    join responsibility in context.Responsibilities on history.ResponsibilityId equals responsibility.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary
                    select responsibility.Title).FirstOrDefault(),
                EmploymentStatus = context.EmployeeStatusHistories
                    .Where(history => history.EmployeeId == employee.Id && history.EndDate == null)
                    .Select(history => (EmploymentStatus?)history.Status)
                    .FirstOrDefault() ?? EmploymentStatus.Active,
            };

        if (selected.Length > 0)
        {
            rows = rows.Where(row => selected.Contains(row.EmployeeId));
        }

        if (!filter.IncludeDeparted)
        {
            rows = rows.Where(row => !DepartedStatuses.Contains(row.EmploymentStatus));
        }

        if (filter.EmploymentStatuses.Count > 0)
        {
            var statuses = filter.EmploymentStatuses.ToArray();
            rows = rows.Where(row => statuses.Contains(row.EmploymentStatus));
        }

        if (filter.DepartmentIds.Count > 0)
        {
            var departmentIds = filter.DepartmentIds.ToArray();
            rows = rows.Where(row => row.DepartmentId.HasValue && departmentIds.Contains(row.DepartmentId.Value));
        }

        if (filter.ResponsibilityIds.Count > 0)
        {
            var responsibilityIds = filter.ResponsibilityIds.ToArray();
            rows = rows.Where(row => row.PrimaryResponsibilityId.HasValue && responsibilityIds.Contains(row.PrimaryResponsibilityId.Value));
        }

        if (!string.IsNullOrEmpty(normalizedQuery))
        {
            rows = rows.Where(row =>
                row.NormalizedFirstName.Contains(normalizedQuery) ||
                row.NormalizedLastName.Contains(normalizedQuery) ||
                row.NormalizedPersonnelNumber.Contains(normalizedQuery) ||
                row.NationalCode.Contains(normalizedQuery) ||
                (row.NormalizedMobileNumber != null && row.NormalizedMobileNumber.Contains(normalizedQuery)));
        }

        return await ApplySort(rows, sort)
            .Select(row => new EmployeeWorkbookRow(
                row.EmployeeId,
                row.PersonnelNumber,
                row.FirstName,
                row.LastName,
                row.NationalCode,
                row.MobileNumber,
                row.DepartmentName,
                row.PrimaryResponsibility,
                row.EmploymentStatus.ToString()))
            .ToListAsync(cancellationToken);
    }

    private static IOrderedQueryable<ExportRow> ApplySort(IQueryable<ExportRow> rows, EmployeeSort sort) =>
        sort.Field switch
        {
            EmployeeSortField.FirstName => sort.Direction == SortDirection.Ascending
                ? rows.OrderBy(row => row.FirstName)
                : rows.OrderByDescending(row => row.FirstName),
            EmployeeSortField.LastName => sort.Direction == SortDirection.Ascending
                ? rows.OrderBy(row => row.LastName)
                : rows.OrderByDescending(row => row.LastName),
            _ => sort.Direction == SortDirection.Ascending
                ? rows.OrderBy(row => row.PersonnelNumber)
                : rows.OrderByDescending(row => row.PersonnelNumber),
        };

    private sealed class ExportRow
    {
        public long EmployeeId { get; init; }
        public string PersonnelNumber { get; init; } = string.Empty;
        public string NormalizedPersonnelNumber { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string NormalizedFirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string NormalizedLastName { get; init; } = string.Empty;
        public string NationalCode { get; init; } = string.Empty;
        public string? MobileNumber { get; init; }
        public string? NormalizedMobileNumber { get; init; }
        public long? DepartmentId { get; init; }
        public string? DepartmentName { get; init; }
        public long? PrimaryResponsibilityId { get; init; }
        public string? PrimaryResponsibility { get; init; }
        public EmploymentStatus EmploymentStatus { get; init; }
    }
}
