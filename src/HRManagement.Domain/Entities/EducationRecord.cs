using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EducationRecord : AuditableEntity
{
    public long EmployeeId { get; private set; }
    public EducationDegree Degree { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public string? InstitutionName { get; private set; }
    public int? GraduationYear { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
}
