namespace HRManagement.Infrastructure.Logging;

public static class LogRetentionService
{
    public static void DeleteExpiredLogs(string logsDirectory, TimeSpan retentionWindow, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logsDirectory);

        if (!Directory.Exists(logsDirectory))
        {
            return;
        }

        var cutoff = now.UtcDateTime - retentionWindow;
        foreach (var file in Directory.EnumerateFiles(logsDirectory, "hrmanagement-*.log"))
        {
            if (File.GetLastWriteTimeUtc(file) < cutoff)
            {
                File.Delete(file);
            }
        }
    }
}
