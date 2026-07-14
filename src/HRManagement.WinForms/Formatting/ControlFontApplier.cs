namespace HRManagement.WinForms.Formatting;

public static class ControlFontApplier
{
    public static void Apply(Control control, Font font)
    {
        control.Font = font;
        foreach (Control child in control.Controls)
        {
            Apply(child, font);
        }
    }
}
