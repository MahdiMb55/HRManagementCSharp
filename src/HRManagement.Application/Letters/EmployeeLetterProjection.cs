namespace HRManagement.Application.Letters;

public sealed record EmployeeLetterProjection(
    long EmployeeId,
    string FirstName,
    string LastName,
    string PersonnelNumber,
    string NationalCode,
    string? MobileNumber,
    string? DepartmentName,
    string? PrimaryResponsibility,
    string? CompanyName);
