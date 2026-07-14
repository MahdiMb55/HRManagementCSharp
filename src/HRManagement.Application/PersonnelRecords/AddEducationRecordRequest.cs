using HRManagement.Domain.Enums;

namespace HRManagement.Application.PersonnelRecords;

public sealed record AddEducationRecordRequest(
    long EmployeeId,
    EducationDegree Degree,
    string? FieldOfStudy,
    string? InstitutionName,
    int? GraduationYear,
    bool IsPrimary,
    string? Notes);
