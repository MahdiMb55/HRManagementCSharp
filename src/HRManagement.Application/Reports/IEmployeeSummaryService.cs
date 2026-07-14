namespace HRManagement.Application.Reports;

public interface IEmployeeSummaryService
{
    Task<EmployeeSummaryResult> CreateAsync(
        EmployeeSummaryRequest request,
        CancellationToken cancellationToken);
}
