using System.Drawing.Text;
using HRManagement.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Formatting;

public sealed class PrivateFontService(
    IApplicationPaths paths,
    ILogger<PrivateFontService> logger) : IDisposable
{
    private readonly PrivateFontCollection fontCollection = new();
    private FontFamily? fontFamily;

    public Font CreateDefaultFont(float size = 9.5F)
    {
        if (fontFamily is null)
        {
            var fontPath = Path.Combine(paths.BaseDirectory, "Resources", "Fonts", "Vazirmatn-Variable.ttf");
            try
            {
                fontCollection.AddFontFile(fontPath);
                fontFamily = fontCollection.Families
                    .FirstOrDefault(family => family.Name.Contains("Vazirmatn", StringComparison.OrdinalIgnoreCase))
                    ?? fontCollection.Families.FirstOrDefault()
                    ?? throw new InvalidOperationException("Bundled font did not expose any font families.");
                logger.LogInformation("Private Vazirmatn font loaded from bundled resources");
            }
            catch (Exception exception) when (exception is ArgumentException or FileNotFoundException)
            {
                logger.LogWarning(exception, "Bundled Vazirmatn font could not be loaded; using the Windows message font");
                return SystemFonts.MessageBoxFont?.Clone() as Font
                    ?? new Font(FontFamily.GenericSansSerif, size, FontStyle.Regular, GraphicsUnit.Point);
            }
        }

        return new Font(fontFamily, size, FontStyle.Regular, GraphicsUnit.Point);
    }

    public void Dispose() => fontCollection.Dispose();
}
