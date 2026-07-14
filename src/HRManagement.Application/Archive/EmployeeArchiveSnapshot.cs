namespace HRManagement.Application.Archive;

public sealed record EmployeeArchiveSnapshot(
    long EmployeeId,
    string PersonnelNumber,
    bool IsArchived);
