using HRManagement.Infrastructure.Paths;

namespace HRManagement.Infrastructure.Tests.Startup;

public sealed class ApplicationPathsTests
{
    [Fact]
    public void Constructor_ResolvesEveryPathUnderProvidedBaseDirectory()
    {
        var baseDirectory = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "hr-base"));

        var paths = new ApplicationPaths(baseDirectory);

        Assert.Equal(Path.Combine(baseDirectory, "Data"), paths.DataDirectory);
        Assert.Equal(Path.Combine(baseDirectory, "Data", "Database", "hr-management.db"), paths.DatabaseFile);
        Assert.Equal(Path.Combine(baseDirectory, "Data", "Documents"), paths.DocumentsDirectory);
        Assert.Equal(Path.Combine(baseDirectory, "Data", "Photos"), paths.PhotosDirectory);
        Assert.Equal(Path.Combine(baseDirectory, "Data", "Logs"), paths.LogsDirectory);
        Assert.All(paths.RequiredDirectories, path => Assert.StartsWith(paths.DataDirectory, path, StringComparison.OrdinalIgnoreCase));
    }
}
