namespace HRManagement.Application.Reports;

public sealed record EmployeeSummaryRequest(
    IReadOnlyCollection<long> EmployeeIds,
    string OutputPath);
