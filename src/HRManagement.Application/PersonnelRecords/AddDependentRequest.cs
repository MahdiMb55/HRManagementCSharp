using HRManagement.Domain.Enums;

namespace HRManagement.Application.PersonnelRecords;

public sealed record AddDependentRequest(
    long EmployeeId,
    string? FirstName,
    string? LastName,
    string? NationalCode,
    Gender Gender,
    DateOnly? BirthDate,
    RelationshipType RelationshipType,
    DependentEducationStatus EducationStatus,
    DependentInsuranceStatus InsuranceStatus,
    string? Notes);
