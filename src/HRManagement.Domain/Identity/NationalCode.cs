using HRManagement.Domain.Common;
using HRManagement.Domain.Text;

namespace HRManagement.Domain.Identity;

public sealed record NationalCode
{
    private NationalCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ValidationResult<NationalCode> Create(string? input)
    {
        var normalized = PersianTextNormalizer.NormalizeDigits(input?.Trim() ?? string.Empty);
        if (normalized.Length != 10 || normalized.Any(character => character is < '0' or > '9'))
        {
            return Invalid();
        }

        if (normalized.All(character => character == normalized[0]))
        {
            return Invalid();
        }

        var sum = 0;
        for (var index = 0; index < 9; index++)
        {
            sum += (normalized[index] - '0') * (10 - index);
        }

        var remainder = sum % 11;
        var expectedCheckDigit = remainder < 2 ? remainder : 11 - remainder;
        if (normalized[9] - '0' != expectedCheckDigit)
        {
            return Invalid();
        }

        return ValidationResult<NationalCode>.Success(new NationalCode(normalized));
    }

    public override string ToString() => Value;

    private static ValidationResult<NationalCode> Invalid() =>
        ValidationResult<NationalCode>.Failure(
            "national_code.invalid",
            "کد ملی واردشده معتبر نیست.");
}
