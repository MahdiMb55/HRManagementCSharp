namespace HRManagement.Application.Files;

public sealed record ManagedFileSaveRequest(
    string SourcePath,
    ManagedFileKind Kind,
    IReadOnlySet<string> AllowedExtensions);
