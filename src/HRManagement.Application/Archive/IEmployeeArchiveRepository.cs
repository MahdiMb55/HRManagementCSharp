namespace HRManagement.Application.Archive;

public interface IEmployeeArchiveRepository
{
    Task<EmployeeArchiveSnapshot?> FindAsync(long employeeId, CancellationToken cancellationToken);

    Task<ArchiveResult> ArchiveAsync(
        long employeeId,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<ArchiveResult> RestoreAsync(
        long employeeId,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<ArchiveResult> DeletePermanentlyAsync(
        long employeeId,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
