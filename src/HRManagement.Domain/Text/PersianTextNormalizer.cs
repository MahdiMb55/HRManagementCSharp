using System.Text;
using System.Text.RegularExpressions;

namespace HRManagement.Domain.Text;

public static partial class PersianTextNormalizer
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = NormalizeDigits(value)
            .Replace('ي', 'ی')
            .Replace('ى', 'ی')
            .Replace('ك', 'ک')
            .Replace('\u200c', ' ');

        return WhitespacePattern().Replace(normalized.Trim(), " ");
    }

    public static string NormalizeDigits(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            builder.Append(character switch
            {
                >= '۰' and <= '۹' => (char)('0' + character - '۰'),
                >= '٠' and <= '٩' => (char)('0' + character - '٠'),
                _ => character,
            });
        }

        return builder.ToString();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}
