namespace HRManagement.WinForms.Formatting;

public interface IPersianDateAdapter
{
    string Format(DateOnly? value);
    bool TryParse(string? value, out DateOnly result);
}
