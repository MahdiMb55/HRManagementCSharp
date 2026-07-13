using HRManagement.Domain.Common;
using HRManagement.Domain.Text;

namespace HRManagement.Domain.Identity;

public sealed record PersonnelNumber
{
    private PersonnelNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ValidationResult<PersonnelNumber> Create(string? input)
    {
        var normalized = PersianTextNormalizer.NormalizeDigits(input?.Trim() ?? string.Empty);
        if (normalized.Length == 0)
        {
            return ValidationResult<PersonnelNumber>.Failure(
                "personnel_number.required",
                "شماره پرسنلی الزامی است.");
        }

        if (normalized.Length > 50)
        {
            return ValidationResult<PersonnelNumber>.Failure(
                "personnel_number.too_long",
                "شماره پرسنلی نمی‌تواند بیش از ۵۰ نویسه باشد.");
        }

        if (normalized.Any(char.IsControl))
        {
            return ValidationResult<PersonnelNumber>.Failure(
                "personnel_number.invalid_character",
                "شماره پرسنلی شامل نویسه نامعتبر است.");
        }

        return ValidationResult<PersonnelNumber>.Success(new PersonnelNumber(normalized));
    }

    public override string ToString() => Value;
}
