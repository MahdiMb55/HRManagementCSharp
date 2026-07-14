namespace HRManagement.Application.Reports;

public sealed record EmployeeSummaryRow(
    long EmployeeId,
    string PersonnelNumber,
    string FullName,
    string NationalCode,
    string? MobileNumber,
    string? DepartmentName,
    string? PrimaryResponsibility,
    string EmploymentStatus);
