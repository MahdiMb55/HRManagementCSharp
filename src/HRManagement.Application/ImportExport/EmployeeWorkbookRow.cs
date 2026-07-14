namespace HRManagement.Application.ImportExport;

public sealed record EmployeeWorkbookRow(
    long? EmployeeId,
    string PersonnelNumber,
    string FirstName,
    string LastName,
    string NationalCode,
    string? MobileNumber,
    string? DepartmentName,
    string? PrimaryResponsibility,
    string EmploymentStatus);
