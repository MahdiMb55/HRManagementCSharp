namespace HRManagement.Application.Backups;

public interface IBackupService
{
    Task<BackupResult> CreateAsync(
        BackupRequest request,
        CancellationToken cancellationToken);

    Task<BackupResult> RestoreAsync(
        RestoreRequest request,
        CancellationToken cancellationToken);
}
