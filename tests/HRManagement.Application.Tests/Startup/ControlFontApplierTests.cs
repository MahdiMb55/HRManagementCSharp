using HRManagement.WinForms.Formatting;

namespace HRManagement.Application.Tests.Startup;

public sealed class ControlFontApplierTests
{
    [Fact]
    public void Apply_SetsFontOnControlAndChildren()
    {
        using var font = new Font(FontFamily.GenericSansSerif, 11);
        using var parent = new Panel();
        using var child = new Label();
        parent.Controls.Add(child);

        ControlFontApplier.Apply(parent, font);

        Assert.Equal(font.FontFamily.Name, parent.Font.FontFamily.Name);
        Assert.Equal(font.Size, parent.Font.Size);
        Assert.Equal(font.FontFamily.Name, child.Font.FontFamily.Name);
        Assert.Equal(font.Size, child.Font.Size);
    }
}
