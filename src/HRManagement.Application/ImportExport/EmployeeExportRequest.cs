using HRManagement.Application.Employees.Search;

namespace HRManagement.Application.ImportExport;

public sealed record EmployeeExportRequest(
    string OutputPath,
    string? Query,
    EmployeeFilter Filter,
    EmployeeSort Sort,
    IReadOnlyCollection<long> SelectedEmployeeIds);
