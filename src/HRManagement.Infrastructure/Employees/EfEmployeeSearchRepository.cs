using HRManagement.Application.Employees.Search;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Employees;

public sealed class EfEmployeeSearchRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmployeeSearchRepository
{
    private static readonly EmploymentStatus[] DepartedStatuses =
    [
        EmploymentStatus.Resigned,
        EmploymentStatus.Dismissed,
        EmploymentStatus.Retired,
        EmploymentStatus.Deceased,
    ];

    public async Task<PagedResult<EmployeeListItemDto>> SearchAsync(
        EmployeeSearchRequest request,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var rows =
            from employee in context.Employees.AsNoTracking()
            join person in context.Persons.AsNoTracking() on employee.PersonId equals person.Id
            where request.Filter.IncludeArchived || !employee.IsDeleted
            select new EmployeeSearchRow
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
                NormalizedDepartmentName = (
                    from history in context.EmployeeDepartmentHistories
                    join department in context.Departments on history.DepartmentId equals department.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null
                    select department.NormalizedName).FirstOrDefault(),
                PrimaryResponsibilityId = context.EmployeeResponsibilityHistories
                    .Where(history => history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary)
                    .Select(history => (long?)history.ResponsibilityId)
                    .FirstOrDefault(),
                PrimaryResponsibility = (
                    from history in context.EmployeeResponsibilityHistories
                    join responsibility in context.Responsibilities on history.ResponsibilityId equals responsibility.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary
                    select responsibility.Title).FirstOrDefault(),
                NormalizedPrimaryResponsibility = (
                    from history in context.EmployeeResponsibilityHistories
                    join responsibility in context.Responsibilities on history.ResponsibilityId equals responsibility.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary
                    select responsibility.NormalizedTitle).FirstOrDefault(),
                EmploymentStatus = context.EmployeeStatusHistories
                    .Where(history => history.EmployeeId == employee.Id && history.EndDate == null)
                    .Select(history => (EmploymentStatus?)history.Status)
                    .FirstOrDefault() ?? EmploymentStatus.Active,
            };

        if (!request.Filter.IncludeDeparted)
        {
            rows = rows.Where(row => !DepartedStatuses.Contains(row.EmploymentStatus));
        }

        if (request.Filter.EmploymentStatuses.Count > 0)
        {
            var statuses = request.Filter.EmploymentStatuses.ToArray();
            rows = rows.Where(row => statuses.Contains(row.EmploymentStatus));
        }

        if (request.Filter.DepartmentIds.Count > 0)
        {
            var departmentIds = request.Filter.DepartmentIds.ToArray();
            rows = rows.Where(row => row.DepartmentId.HasValue && departmentIds.Contains(row.DepartmentId.Value));
        }

        if (request.Filter.ResponsibilityIds.Count > 0)
        {
            var responsibilityIds = request.Filter.ResponsibilityIds.ToArray();
            rows = rows.Where(row =>
                row.PrimaryResponsibilityId.HasValue &&
                responsibilityIds.Contains(row.PrimaryResponsibilityId.Value));
        }

        if (!string.IsNullOrEmpty(request.Query))
        {
            var query = request.Query;
            rows = rows.Where(row =>
                row.NormalizedFirstName.Contains(query) ||
                row.NormalizedLastName.Contains(query) ||
                row.NormalizedPersonnelNumber.Contains(query) ||
                row.NationalCode.Contains(query) ||
                (row.NormalizedMobileNumber != null && row.NormalizedMobileNumber.Contains(query)) ||
                (row.NormalizedDepartmentName != null && row.NormalizedDepartmentName.Contains(query)) ||
                (row.NormalizedPrimaryResponsibility != null && row.NormalizedPrimaryResponsibility.Contains(query)));
        }

        var totalCount = await rows.CountAsync(cancellationToken);
        var ordered = ApplySort(rows, request.Sort);
        var items = await ordered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(row => new EmployeeListItemDto(
                row.EmployeeId,
                row.PersonnelNumber,
                row.FirstName,
                row.LastName,
                row.NationalCode,
                row.MobileNumber,
                row.DepartmentId,
                row.DepartmentName,
                row.PrimaryResponsibilityId,
                row.PrimaryResponsibility,
                row.EmploymentStatus))
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeListItemDto>(items, totalCount, request.PageNumber, request.PageSize);
    }

    private static IOrderedQueryable<EmployeeSearchRow> ApplySort(
        IQueryable<EmployeeSearchRow> rows,
        EmployeeSort sort)
    {
        var descending = sort.Direction == SortDirection.Descending;
        IOrderedQueryable<EmployeeSearchRow> ordered = sort.Field switch
        {
            EmployeeSortField.FirstName => descending
                ? rows.OrderByDescending(row => row.NormalizedFirstName)
                : rows.OrderBy(row => row.NormalizedFirstName),
            EmployeeSortField.LastName => descending
                ? rows.OrderByDescending(row => row.NormalizedLastName)
                : rows.OrderBy(row => row.NormalizedLastName),
            EmployeeSortField.Department => descending
                ? rows.OrderByDescending(row => row.NormalizedDepartmentName)
                : rows.OrderBy(row => row.NormalizedDepartmentName),
            EmployeeSortField.Responsibility => descending
                ? rows.OrderByDescending(row => row.NormalizedPrimaryResponsibility)
                : rows.OrderBy(row => row.NormalizedPrimaryResponsibility),
            EmployeeSortField.EmploymentStatus => descending
                ? rows.OrderByDescending(row => row.EmploymentStatus)
                : rows.OrderBy(row => row.EmploymentStatus),
            _ => descending
                ? rows.OrderByDescending(row => row.NormalizedPersonnelNumber)
                : rows.OrderBy(row => row.NormalizedPersonnelNumber),
        };

        return ordered.ThenBy(row => row.EmployeeId);
    }

    private sealed class EmployeeSearchRow
    {
        public long EmployeeId { get; init; }
        public required string PersonnelNumber { get; init; }
        public required string NormalizedPersonnelNumber { get; init; }
        public required string FirstName { get; init; }
        public required string NormalizedFirstName { get; init; }
        public required string LastName { get; init; }
        public required string NormalizedLastName { get; init; }
        public required string NationalCode { get; init; }
        public string? MobileNumber { get; init; }
        public string? NormalizedMobileNumber { get; init; }
        public long? DepartmentId { get; init; }
        public string? DepartmentName { get; init; }
        public string? NormalizedDepartmentName { get; init; }
        public long? PrimaryResponsibilityId { get; init; }
        public string? PrimaryResponsibility { get; init; }
        public string? NormalizedPrimaryResponsibility { get; init; }
        public EmploymentStatus EmploymentStatus { get; init; }
    }
}
