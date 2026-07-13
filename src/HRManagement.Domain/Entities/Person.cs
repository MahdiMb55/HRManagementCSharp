using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Identity;
using HRManagement.Domain.Text;

namespace HRManagement.Domain.Entities;

public sealed class Person : AuditableEntity
{
    private Person()
    {
    }

    private Person(
        string firstName,
        string lastName,
        NationalCode nationalCode,
        Gender gender,
        DateOnly? birthDate,
        DateTime nowUtc)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        NationalCode = nationalCode.Value;
        Gender = gender;
        BirthDate = birthDate;
        NormalizedFirstName = PersianTextNormalizer.Normalize(FirstName);
        NormalizedLastName = PersianTextNormalizer.Normalize(LastName);
        InitializeTimestamps(nowUtc);
    }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string NormalizedFirstName { get; private set; } = string.Empty;

    public string NormalizedLastName { get; private set; } = string.Empty;

    public string NationalCode { get; private set; } = string.Empty;

    public DateOnly? BirthDate { get; private set; }

    public Gender Gender { get; private set; }

    public static ValidationResult<Person> Create(
        string? firstName,
        string? lastName,
        NationalCode nationalCode,
        Gender gender,
        DateOnly? birthDate,
        DateOnly today,
        DateTime nowUtc)
    {
        ArgumentNullException.ThrowIfNull(nationalCode);

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return ValidationResult<Person>.Failure(
                "person.name.required",
                "نام و نام خانوادگی الزامی است.");
        }

        if (birthDate > today)
        {
            return ValidationResult<Person>.Failure(
                "person.birth_date.future",
                "تاریخ تولد نمی‌تواند در آینده باشد.");
        }

        return ValidationResult<Person>.Success(
            new Person(firstName, lastName, nationalCode, gender, birthDate, nowUtc));
    }

    public ValidationResult<bool> UpdateBasicIdentity(
        string? firstName,
        string? lastName,
        NationalCode nationalCode,
        Gender gender,
        DateOnly? birthDate,
        DateOnly today,
        DateTime nowUtc)
    {
        ArgumentNullException.ThrowIfNull(nationalCode);

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return ValidationResult<bool>.Failure("person.name.required", "نام و نام خانوادگی الزامی است.");
        }

        if (birthDate > today)
        {
            return ValidationResult<bool>.Failure("person.birth_date.future", "تاریخ تولد نمی‌تواند در آینده باشد.");
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        NormalizedFirstName = PersianTextNormalizer.Normalize(FirstName);
        NormalizedLastName = PersianTextNormalizer.Normalize(LastName);
        NationalCode = nationalCode.Value;
        Gender = gender;
        BirthDate = birthDate;
        Touch(nowUtc);
        return ValidationResult<bool>.Success(true);
    }
}
