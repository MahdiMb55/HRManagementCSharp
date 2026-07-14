namespace HRManagement.Application.Files;

public interface IManagedFileStore
{
    Task<ManagedFileSaveResult> SaveAsync(
        ManagedFileSaveRequest request,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
