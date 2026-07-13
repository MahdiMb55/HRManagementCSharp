using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDependent : AuditableEntity
{
    public long EmployeeId { get; private set; }
    public long DependentPersonId { get; private set; }
    public RelationshipType RelationshipType { get; private set; }
    public DependentEducationStatus EducationStatus { get; private set; }
    public DependentInsuranceStatus InsuranceStatus { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
}
