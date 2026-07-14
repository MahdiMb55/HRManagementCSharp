namespace HRManagement.Application.ImportExport;

public sealed record EmployeeImportPreview(
    IReadOnlyList<EmployeeWorkbookRow> ValidRows,
    IReadOnlyList<EmployeeImportRowError> Errors)
{
    public bool CanImport => Errors.Count == 0 && ValidRows.Count > 0;
}
