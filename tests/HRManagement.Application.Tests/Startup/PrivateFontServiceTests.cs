using HRManagement.Infrastructure.Paths;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.Application.Tests.Startup;

public sealed class PrivateFontServiceTests
{
    [Fact]
    public void CreateDefaultFont_LoadsBundledVazirmatnVariableFont()
    {
        var paths = new ApplicationPaths(AppContext.BaseDirectory);
        using var service = new PrivateFontService(paths, NullLogger<PrivateFontService>.Instance);

        using var font = service.CreateDefaultFont();

        Assert.Contains("Vazirmatn", font.FontFamily.Name, StringComparison.OrdinalIgnoreCase);
    }
}
