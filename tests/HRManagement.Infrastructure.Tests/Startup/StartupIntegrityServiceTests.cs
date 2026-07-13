using HRManagement.Application.Startup;
using HRManagement.Infrastructure.Paths;
using HRManagement.Infrastructure.Startup;

namespace HRManagement.Infrastructure.Tests.Startup;

public sealed class StartupIntegrityServiceTests
{
    [Fact]
    public async Task CheckAsync_PreparesDirectoriesForLegitimateFirstRunWithoutMarker()
    {
        using var directory = new TemporaryDirectory();
        var paths = new ApplicationPaths(directory.Path);
        var service = new StartupIntegrityService(paths, new FixedWriteProbe(canWrite: true));

        var result = await service.CheckAsync(CancellationToken.None);

        Assert.Equal(StartupIntegrityStatus.ReadyFirstRun, result.Status);
        Assert.All(paths.RequiredDirectories, path => Assert.True(Directory.Exists(path)));
        Assert.False(File.Exists(paths.InitializationMarkerFile));
    }

    [Fact]
    public async Task CheckAsync_RefusesSilentReplacementWhenInitializedDatabaseIsMissing()
    {
        using var directory = new TemporaryDirectory();
        var paths = new ApplicationPaths(directory.Path);
        Directory.CreateDirectory(paths.DatabaseDirectory);
        await File.WriteAllTextAsync(paths.InitializationMarkerFile, "initialized");
        var service = new StartupIntegrityService(paths, new FixedWriteProbe(canWrite: true));

        var result = await service.CheckAsync(CancellationToken.None);

        Assert.Equal(StartupIntegrityStatus.MissingDatabase, result.Status);
        Assert.Contains("پایگاه داده", result.UserMessage);
        Assert.False(File.Exists(paths.DatabaseFile));
    }

    [Fact]
    public async Task MarkInitializedAsync_RequiresDatabaseAndCreatesMarker()
    {
        using var directory = new TemporaryDirectory();
        var paths = new ApplicationPaths(directory.Path);
        var service = new StartupIntegrityService(paths, new FixedWriteProbe(canWrite: true));
        await service.CheckAsync(CancellationToken.None);

        var withoutDatabase = await service.MarkInitializedAsync(CancellationToken.None);
        await File.WriteAllBytesAsync(paths.DatabaseFile, [1]);
        var withDatabase = await service.MarkInitializedAsync(CancellationToken.None);

        Assert.False(withoutDatabase.IsSuccess);
        Assert.True(withDatabase.IsSuccess);
        Assert.True(File.Exists(paths.InitializationMarkerFile));
    }

    [Fact]
    public async Task CheckAsync_MapsUnwritableDirectoryToFriendlyFailure()
    {
        using var directory = new TemporaryDirectory();
        var paths = new ApplicationPaths(directory.Path);
        var service = new StartupIntegrityService(paths, new FixedWriteProbe(canWrite: false));

        var result = await service.CheckAsync(CancellationToken.None);

        Assert.Equal(StartupIntegrityStatus.NotWritable, result.Status);
        Assert.Contains("نوشتن", result.UserMessage);
    }

    private sealed class FixedWriteProbe(bool canWrite) : IWritableDirectoryProbe
    {
        public Task<bool> CanWriteAsync(string directory, CancellationToken cancellationToken) =>
            Task.FromResult(canWrite);
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "HRManagement.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
