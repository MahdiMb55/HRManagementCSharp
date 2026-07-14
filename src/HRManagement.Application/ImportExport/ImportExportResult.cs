namespace HRManagement.Application.ImportExport;

public sealed record ImportExportResult(
    bool IsSuccess,
    string UserMessage,
    int AffectedRows = 0,
    string? OutputPath = null)
{
    public static ImportExportResult Success(int affectedRows, string? outputPath = null) =>
        new(true, string.Empty, affectedRows, outputPath);

    public static ImportExportResult Failure(string userMessage) =>
        new(false, userMessage);
}
