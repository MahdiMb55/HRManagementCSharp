namespace HRManagement.Application.ImportExport;

public sealed record EmployeeImportRowError(
    int RowNumber,
    string Field,
    string Message);
