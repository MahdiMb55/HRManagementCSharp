namespace HRManagement.Application.Backups;

public sealed record BackupManifestEntry(
    string RelativePath,
    long SizeBytes,
    string Sha256);
