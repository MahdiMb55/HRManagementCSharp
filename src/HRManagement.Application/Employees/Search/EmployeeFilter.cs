using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employees.Search;

public sealed record EmployeeFilter(
    IReadOnlyCollection<EmploymentStatus> EmploymentStatuses,
    IReadOnlyCollection<long> DepartmentIds,
    IReadOnlyCollection<long> ResponsibilityIds,
    bool IncludeDeparted)
{
    public static EmployeeFilter Default { get; } = new([], [], [], false);
}
