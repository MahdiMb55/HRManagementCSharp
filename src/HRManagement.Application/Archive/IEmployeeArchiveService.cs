namespace HRManagement.Application.Archive;

public interface IEmployeeArchiveService
{
    Task<ArchiveResult> ArchiveAsync(long employeeId, CancellationToken cancellationToken);

    Task<ArchiveResult> RestoreAsync(long employeeId, CancellationToken cancellationToken);

    Task<ArchiveResult> DeletePermanentlyAsync(
        long employeeId,
        string? personnelNumberConfirmation,
        CancellationToken cancellationToken);
}
