namespace HRManagement.Application.Reports;

public interface IEmployeeSummaryRepository
{
    Task<IReadOnlyList<EmployeeSummaryRow>> GetRowsAsync(
        IReadOnlyCollection<long> employeeIds,
        CancellationToken cancellationToken);
}
