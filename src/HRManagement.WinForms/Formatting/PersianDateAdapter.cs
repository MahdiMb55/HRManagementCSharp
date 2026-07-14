using System.Globalization;
using HRManagement.Domain.Text;

namespace HRManagement.WinForms.Formatting;

public sealed class PersianDateAdapter : IPersianDateAdapter
{
    private static readonly PersianCalendar Calendar = new();

    public string Format(DateOnly? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var gregorianDateTime = value.Value.ToDateTime(TimeOnly.MinValue);
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{Calendar.GetYear(gregorianDateTime):0000}/{Calendar.GetMonth(gregorianDateTime):00}/{Calendar.GetDayOfMonth(gregorianDateTime):00}");
    }

    public bool TryParse(string? value, out DateOnly result)
    {
        result = default;
        var normalized = PersianTextNormalizer.NormalizeDigits(value?.Trim() ?? string.Empty);
        var parts = normalized.Split('/');
        if (parts.Length != 3 ||
            !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var year) ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var month) ||
            !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var day))
        {
            return false;
        }

        try
        {
            result = DateOnly.FromDateTime(Calendar.ToDateTime(year, month, day, 0, 0, 0, 0));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
