namespace HRManagement.Application.Employees.Search;

public sealed record EmployeeSearchRequest(
    string? Query,
    int PageNumber,
    int PageSize,
    EmployeeSort Sort,
    EmployeeFilter Filter);
