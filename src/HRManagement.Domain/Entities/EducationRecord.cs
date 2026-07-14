using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class EducationRecord : AuditableEntity
{
    private EducationRecord()
    {
    }

    private EducationRecord(
        long employeeId,
        EducationDegree degree,
        string? fieldOfStudy,
        string? institutionName,
        int? graduationYear,
        bool isPrimary,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        Degree = degree;
        FieldOfStudy = string.IsNullOrWhiteSpace(fieldOfStudy) ? null : fieldOfStudy.Trim();
        InstitutionName = string.IsNullOrWhiteSpace(institutionName) ? null : institutionName.Trim();
        GraduationYear = graduationYear;
        IsPrimary = isPrimary;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        InitializeTimestamps(nowUtc);
    }

    public long EmployeeId { get; private set; }
    public EducationDegree Degree { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public string? InstitutionName { get; private set; }
    public int? GraduationYear { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public static ValidationResult<EducationRecord> Create(
        long employeeId,
        EducationDegree degree,
        string? fieldOfStudy,
        string? institutionName,
        int? graduationYear,
        bool isPrimary,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0)
        {
            return ValidationResult<EducationRecord>.Failure(
                "education.employee.required",
                "کارمند برای ثبت سابقه تحصیلی الزامی است.");
        }

        if (graduationYear is < 1200 or > 1600)
        {
            return ValidationResult<EducationRecord>.Failure(
                "education.graduation_year.invalid",
                "سال فارغ‌التحصیلی معتبر نیست.");
        }

        return ValidationResult<EducationRecord>.Success(
            new EducationRecord(employeeId, degree, fieldOfStudy, institutionName, graduationYear, isPrimary, notes, nowUtc));
    }

    public void ClearPrimary(DateTime nowUtc)
    {
        IsPrimary = false;
        Touch(nowUtc);
    }
}
