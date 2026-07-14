namespace HRManagement.Application.Reports;

public sealed record EmployeeSummaryResult(
    bool IsSuccess,
    string UserMessage,
    string? OutputPath = null)
{
    public static EmployeeSummaryResult Success(string outputPath) =>
        new(true, string.Empty, outputPath);

    public static EmployeeSummaryResult Failure(string userMessage) =>
        new(false, userMessage);
}
