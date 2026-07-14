namespace HRManagement.Application.Files;

public sealed record SetProfilePhotoRequest(
    long EmployeeId,
    string SourcePath);
