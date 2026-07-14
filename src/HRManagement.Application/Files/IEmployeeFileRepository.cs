using HRManagement.Domain.Entities;

namespace HRManagement.Application.Files;

public interface IEmployeeFileRepository
{
    Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken);

    Task<long?> GetOpenEmploymentPeriodIdAsync(long employeeId, CancellationToken cancellationToken);

    Task<FileRecordResult> CreateContractAsync(
        Contract contract,
        ManagedFile? attachment,
        string? attachmentDescription,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<FileRecordResult> AddEmployeeDocumentVersionAsync(
        EmployeeDocument document,
        ManagedFile file,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<FileRecordResult> SetProfilePhotoAsync(
        long employeeId,
        ManagedFile file,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
