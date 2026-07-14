using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Identity;
using HRManagement.Domain.Text;

namespace HRManagement.Domain.Entities;

public sealed class Employee : AuditableEntity
{
    private Employee()
    {
    }

    private Employee(long personId, PersonnelNumber personnelNumber, DateTime nowUtc)
    {
        PersonId = personId;
        PersonnelNumber = personnelNumber.Value;
        NormalizedPersonnelNumber = PersianTextNormalizer.Normalize(personnelNumber.Value);
        InitializeTimestamps(nowUtc);
    }

    public long PersonId { get; private set; }

    public string PersonnelNumber { get; private set; } = string.Empty;

    public string NormalizedPersonnelNumber { get; private set; } = string.Empty;

    public string? FatherName { get; private set; }

    public string? BirthCertificateNumber { get; private set; }

    public string? BirthCertificateIssuePlace { get; private set; }

    public string? MobileNumber { get; private set; }

    public string? NormalizedMobileNumber { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string? Email { get; private set; }

    public string? EmergencyContactName { get; private set; }

    public string? EmergencyContactPhone { get; private set; }

    public string? EmergencyContactRelation { get; private set; }

    public string? HomeAddress { get; private set; }

    public MaritalStatus? MaritalStatus { get; private set; }

    public BloodType? BloodType { get; private set; }

    public MilitaryServiceStatus? MilitaryServiceStatus { get; private set; }

    public string? InsuranceNumber { get; private set; }

    public string? SpecialNotes { get; private set; }

    public long? ProfilePhotoFileId { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }

    public static ValidationResult<Employee> Create(
        long personId,
        PersonnelNumber personnelNumber,
        DateTime nowUtc)
    {
        ArgumentNullException.ThrowIfNull(personnelNumber);

        if (personId <= 0)
        {
            return ValidationResult<Employee>.Failure(
                "employee.person.required",
                "هویت شخص برای ایجاد کارمند الزامی است.");
        }

        return ValidationResult<Employee>.Success(new Employee(personId, personnelNumber, nowUtc));
    }

    public void UpdateBasicDetails(string? fatherName, string? mobileNumber, DateTime nowUtc)
    {
        FatherName = string.IsNullOrWhiteSpace(fatherName) ? null : fatherName.Trim();
        MobileNumber = string.IsNullOrWhiteSpace(mobileNumber) ? null : mobileNumber.Trim();
        NormalizedMobileNumber = MobileNumber is null ? null : PersianTextNormalizer.Normalize(MobileNumber);
        Touch(nowUtc);
    }

    public void SetProfilePhoto(long managedFileId, DateTime nowUtc)
    {
        if (managedFileId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(managedFileId));
        }

        ProfilePhotoFileId = managedFileId;
        Touch(nowUtc);
    }
}
