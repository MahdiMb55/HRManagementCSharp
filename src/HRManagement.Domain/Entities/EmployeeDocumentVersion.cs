using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDocumentVersion : Entity
{
    private EmployeeDocumentVersion()
    {
    }

    private EmployeeDocumentVersion(
        long employeeDocumentId,
        long managedFileId,
        int versionNumber,
        bool isCurrent,
        DateTime createdAtUtc)
    {
        EmployeeDocumentId = employeeDocumentId;
        ManagedFileId = managedFileId;
        VersionNumber = versionNumber;
        IsCurrent = isCurrent;
        CreatedAtUtc = createdAtUtc.Kind == DateTimeKind.Utc ? createdAtUtc : createdAtUtc.ToUniversalTime();
    }

    public long EmployeeDocumentId { get; private set; }
    public long ManagedFileId { get; private set; }
    public int VersionNumber { get; private set; }
    public bool IsCurrent { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<EmployeeDocumentVersion> Create(
        long employeeDocumentId,
        long managedFileId,
        int versionNumber,
        bool isCurrent,
        DateTime createdAtUtc)
    {
        if (employeeDocumentId <= 0 || managedFileId <= 0 || versionNumber <= 0)
        {
            return ValidationResult<EmployeeDocumentVersion>.Failure(
                "employee_document_version.required",
                "اطلاعات نسخه سند کامل نیست.");
        }

        return ValidationResult<EmployeeDocumentVersion>.Success(
            new EmployeeDocumentVersion(employeeDocumentId, managedFileId, versionNumber, isCurrent, createdAtUtc));
    }

    public void ClearCurrent() => IsCurrent = false;
}
