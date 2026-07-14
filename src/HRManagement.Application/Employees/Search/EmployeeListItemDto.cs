using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employees.Search;

public sealed record EmployeeListItemDto(
    long EmployeeId,
    string PersonnelNumber,
    string FirstName,
    string LastName,
    string NationalCode,
    string? MobileNumber,
    long? DepartmentId,
    string? DepartmentName,
    long? PrimaryResponsibilityId,
    string? PrimaryResponsibility,
    EmploymentStatus EmploymentStatus);
