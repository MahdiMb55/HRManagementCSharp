namespace HRManagement.Application.ImportExport;

public interface IEmployeeWorkbookService
{
    Task<ImportExportResult> CreateTemplateAsync(
        string outputPath,
        CancellationToken cancellationToken);

    Task<EmployeeImportPreview> PreviewImportAsync(
        string inputPath,
        CancellationToken cancellationToken);

    Task<ImportExportResult> ExportAsync(
        EmployeeExportRequest request,
        CancellationToken cancellationToken);
}
