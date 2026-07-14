using HRManagement.Infrastructure.Logging;

namespace HRManagement.Infrastructure.Tests.Logging;

public sealed class LogRetentionServiceTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    [Fact]
    public void DeleteExpiredLogs_RemovesOnlyHrManagementLogsOlderThanRetentionWindow()
    {
        Directory.CreateDirectory(directory);
        var oldLog = CreateFile("hrmanagement-20260601.log", DateTimeOffset.UtcNow.AddDays(-31));
        var recentLog = CreateFile("hrmanagement-20260701.log", DateTimeOffset.UtcNow.AddDays(-29));
        var unrelated = CreateFile("other.log", DateTimeOffset.UtcNow.AddDays(-60));

        LogRetentionService.DeleteExpiredLogs(directory, TimeSpan.FromDays(30), DateTimeOffset.UtcNow);

        Assert.False(File.Exists(oldLog));
        Assert.True(File.Exists(recentLog));
        Assert.True(File.Exists(unrelated));
    }

    public void Dispose()
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private string CreateFile(string fileName, DateTimeOffset lastWriteTime)
    {
        var path = Path.Combine(directory, fileName);
        File.WriteAllText(path, "log");
        File.SetLastWriteTimeUtc(path, lastWriteTime.UtcDateTime);
        return path;
    }
}
