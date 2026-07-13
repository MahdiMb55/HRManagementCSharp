using HRManagement.Domain.Enums;

namespace HRManagement.Application.Employees;

public sealed record ValidatedEmployeeSave(
    long? EmployeeId,
    string FirstName,
    string LastName,
    string PersonnelNumber,
    string NationalCode,
    string? FatherName,
    Gender Gender,
    DateOnly? BirthDate,
    string? MobileNumber,
    DateTime OccurredAtUtc,
    string AuditAction);
