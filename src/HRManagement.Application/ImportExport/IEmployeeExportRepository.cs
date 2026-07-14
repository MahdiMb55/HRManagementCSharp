using HRManagement.Application.Employees.Search;

namespace HRManagement.Application.ImportExport;

public interface IEmployeeExportRepository
{
    Task<IReadOnlyList<EmployeeWorkbookRow>> GetRowsAsync(
        string? normalizedQuery,
        EmployeeFilter filter,
        EmployeeSort sort,
        IReadOnlyCollection<long> selectedEmployeeIds,
        CancellationToken cancellationToken);
}
