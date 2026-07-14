namespace HRManagement.Application.Employees.Search;

public enum EmployeeSortField
{
    PersonnelNumber = 1,
    FirstName = 2,
    LastName = 3,
    Department = 4,
    Responsibility = 5,
    EmploymentStatus = 6,
}

public enum SortDirection
{
    Ascending = 1,
    Descending = 2,
}

public sealed record EmployeeSort(EmployeeSortField Field, SortDirection Direction);
