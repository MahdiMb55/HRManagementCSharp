using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employees.Search;

public sealed record AdvancedEmployeeFilter(
    IReadOnlyCollection<AdvancedFilterGroup> Groups,
    bool IncludeArchived)
{
    public static AdvancedEmployeeFilter Empty { get; } = new([], false);
}

public sealed record AdvancedFilterGroup(
    IReadOnlyCollection<AdvancedFilterCondition> AnyOf);

public sealed record AdvancedFilterCondition(
    AdvancedEmployeeFilterField Field,
    AdvancedFilterOperator Operator,
    string? Value,
    string? SecondValue = null);

public enum AdvancedEmployeeFilterField
{
    FirstName = 1,
    LastName = 2,
    PersonnelNumber = 3,
    NationalCode = 4,
    MobileNumber = 5,
    DepartmentId = 6,
    ResponsibilityId = 7,
    EmploymentStatus = 8,
    HireDate = 9,
}

public enum AdvancedFilterOperator
{
    Equals = 1,
    Contains = 2,
    Before = 3,
    After = 4,
    Between = 5,
}

public static class AdvancedEmployeeFilterFactory
{
    public static AdvancedEmployeeFilter FromSimple(EmployeeFilter filter) =>
        new(
            BuildGroups(filter),
            IncludeArchived: filter.IncludeArchived);

    private static IReadOnlyCollection<AdvancedFilterGroup> BuildGroups(EmployeeFilter filter)
    {
        var groups = new List<AdvancedFilterGroup>();
        if (filter.EmploymentStatuses.Count > 0)
        {
            groups.Add(new AdvancedFilterGroup(
                filter.EmploymentStatuses
                    .Select(status => new AdvancedFilterCondition(
                        AdvancedEmployeeFilterField.EmploymentStatus,
                        AdvancedFilterOperator.Equals,
                        ((int)status).ToString()))
                    .ToArray()));
        }

        if (filter.DepartmentIds.Count > 0)
        {
            groups.Add(new AdvancedFilterGroup(
                filter.DepartmentIds
                    .Select(id => new AdvancedFilterCondition(
                        AdvancedEmployeeFilterField.DepartmentId,
                        AdvancedFilterOperator.Equals,
                        id.ToString()))
                    .ToArray()));
        }

        if (filter.ResponsibilityIds.Count > 0)
        {
            groups.Add(new AdvancedFilterGroup(
                filter.ResponsibilityIds
                    .Select(id => new AdvancedFilterCondition(
                        AdvancedEmployeeFilterField.ResponsibilityId,
                        AdvancedFilterOperator.Equals,
                        id.ToString()))
                    .ToArray()));
        }

        return groups;
    }
}
