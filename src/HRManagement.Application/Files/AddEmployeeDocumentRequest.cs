namespace HRManagement.Application.Files;

public sealed record AddEmployeeDocumentRequest(
    long EmployeeId,
    long CategoryId,
    string? Title,
    string? Description,
    string SourcePath);
