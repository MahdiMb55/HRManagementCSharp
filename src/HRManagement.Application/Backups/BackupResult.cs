namespace HRManagement.Application.Backups;

public sealed record BackupResult(
    bool IsSuccess,
    string UserMessage,
    string? BackupFilePath = null)
{
    public static BackupResult Success(string backupFilePath) =>
        new(true, string.Empty, backupFilePath);

    public static BackupResult Failure(string userMessage) =>
        new(false, userMessage);
}
