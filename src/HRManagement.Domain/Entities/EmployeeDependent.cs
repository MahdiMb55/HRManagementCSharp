using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDependent : AuditableEntity
{
    private EmployeeDependent()
    {
    }

    private EmployeeDependent(
        long employeeId,
        long dependentPersonId,
        RelationshipType relationshipType,
        DependentEducationStatus educationStatus,
        DependentInsuranceStatus insuranceStatus,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        DependentPersonId = dependentPersonId;
        RelationshipType = relationshipType;
        EducationStatus = educationStatus;
        InsuranceStatus = insuranceStatus;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        InitializeTimestamps(nowUtc);
    }

    public long EmployeeId { get; private set; }
    public long DependentPersonId { get; private set; }
    public RelationshipType RelationshipType { get; private set; }
    public DependentEducationStatus EducationStatus { get; private set; }
    public DependentInsuranceStatus InsuranceStatus { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public static ValidationResult<EmployeeDependent> Create(
        long employeeId,
        long dependentPersonId,
        RelationshipType relationshipType,
        DependentEducationStatus educationStatus,
        DependentInsuranceStatus insuranceStatus,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || dependentPersonId <= 0)
        {
            return ValidationResult<EmployeeDependent>.Failure(
                "dependent.required",
                "کارمند و هویت فرد تحت تکفل الزامی است.");
        }

        return ValidationResult<EmployeeDependent>.Success(
            new EmployeeDependent(
                employeeId,
                dependentPersonId,
                relationshipType,
                educationStatus,
                insuranceStatus,
                notes,
                nowUtc));
    }
}
