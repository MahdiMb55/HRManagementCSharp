namespace HRManagement.Application.Backups;

public sealed record BackupManifest(
    string Application,
    string CreatedAtUtc,
    IReadOnlyList<BackupManifestEntry> Entries);
