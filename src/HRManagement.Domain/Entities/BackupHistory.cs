using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class BackupHistory : Entity
{
    private BackupHistory()
    {
    }

    private BackupHistory(BackupType backupType, string filePath, DateTime startedAtUtc)
    {
        BackupType = backupType;
        FilePath = filePath;
        StartedAtUtc = startedAtUtc.Kind == DateTimeKind.Utc ? startedAtUtc : startedAtUtc.ToUniversalTime();
    }

    public BackupType BackupType { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public long? FileSizeBytes { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public bool WasSuccessful { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static BackupHistory Start(BackupType backupType, string filePath, DateTime startedAtUtc) =>
        new(backupType, filePath, startedAtUtc);

    public void Complete(long fileSizeBytes, DateTime completedAtUtc)
    {
        FileSizeBytes = fileSizeBytes;
        CompletedAtUtc = completedAtUtc.Kind == DateTimeKind.Utc ? completedAtUtc : completedAtUtc.ToUniversalTime();
        WasSuccessful = true;
        ErrorMessage = null;
    }

    public void Fail(string errorMessage, DateTime completedAtUtc)
    {
        CompletedAtUtc = completedAtUtc.Kind == DateTimeKind.Utc ? completedAtUtc : completedAtUtc.ToUniversalTime();
        WasSuccessful = false;
        ErrorMessage = errorMessage;
    }
}
