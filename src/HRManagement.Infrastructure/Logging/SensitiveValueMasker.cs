namespace HRManagement.Infrastructure.Logging;

public static class SensitiveValueMasker
{
    public static string? MaskNationalCode(string? value) => MaskLastFour(value);

    public static string? MaskFinancialIdentifier(string? value) => MaskLastFour(value);

    private static string? MaskLastFour(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var visibleLength = Math.Min(4, value.Length);
        return new string('*', value.Length - visibleLength) + value[^visibleLength..];
    }
}
