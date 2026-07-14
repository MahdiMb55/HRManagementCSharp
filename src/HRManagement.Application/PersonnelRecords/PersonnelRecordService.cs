using HRManagement.Application.Abstractions;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Identity;
using HRManagement.Domain.Text;

namespace HRManagement.Application.PersonnelRecords;

public sealed class PersonnelRecordService(
    IPersonnelRecordRepository repository,
    IClock clock) : IPersonnelRecordService
{
    public async Task<PersonnelRecordResult> AddEducationRecordAsync(
        AddEducationRecordRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        var record = EducationRecord.Create(
            request.EmployeeId,
            request.Degree,
            request.FieldOfStudy,
            request.InstitutionName,
            request.GraduationYear,
            request.IsPrimary,
            request.Notes,
            clock.UtcNow);
        if (!record.IsSuccess)
        {
            return Invalid(record.Errors[0].Message);
        }

        return await repository.AddEducationRecordAsync(record.Value!, clock.UtcNow, cancellationToken);
    }

    public async Task<PersonnelRecordResult> AddDependentAsync(
        AddDependentRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        var nationalCode = NationalCode.Create(request.NationalCode);
        if (!nationalCode.IsSuccess)
        {
            return Invalid(nationalCode.Errors[0].Message);
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return Invalid("نام و نام خانوادگی فرد تحت تکفل الزامی است.");
        }

        return await repository.AddDependentAsync(
            request.EmployeeId,
            nationalCode.Value!,
            (personId, nowUtc) => CreatePerson(request, nationalCode.Value!, nowUtc),
            (personId, nowUtc) => CreateDependent(request, personId, nowUtc),
            clock.UtcNow,
            cancellationToken);
    }

    public async Task<PersonnelRecordResult> AddBankAccountAsync(
        AddBankAccountRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        var cardNumber = NormalizeDigitsOrNull(request.CardNumber);
        if (cardNumber is not null && (cardNumber.Length is < 12 or > 19 || cardNumber.Any(character => character is < '0' or > '9')))
        {
            return Invalid("شماره کارت بانکی معتبر نیست.");
        }

        var iban = NormalizeIban(request.Iban);
        if (iban is not null && (iban.Length is < 24 or > 26 || !iban.StartsWith("IR", StringComparison.OrdinalIgnoreCase)))
        {
            return Invalid("شماره شبا معتبر نیست.");
        }

        var account = EmployeeBankAccount.Create(
            request.EmployeeId,
            request.BankName,
            NormalizeDigitsOrNull(request.AccountNumber),
            cardNumber,
            iban,
            request.IsPrimary,
            request.Notes,
            clock.UtcNow);
        if (!account.IsSuccess)
        {
            return Invalid(account.Errors[0].Message);
        }

        return await repository.AddBankAccountAsync(account.Value!, clock.UtcNow, cancellationToken);
    }

    public async Task<PersonnelRecordResult> IssueAccessCardAsync(
        IssueAccessCardRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        var cardNumber = NormalizeDigitsOrNull(request.CardNumber);
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return Invalid("شماره کارت تردد الزامی است.");
        }

        return await repository.IssueAccessCardAsync(
            request.EmployeeId,
            cardNumber,
            request.StartDate,
            request.Notes,
            clock.UtcNow,
            cancellationToken);
    }

    private static Person CreatePerson(AddDependentRequest request, NationalCode nationalCode, DateTime nowUtc)
    {
        var person = Person.Create(
            request.FirstName,
            request.LastName,
            nationalCode,
            request.Gender,
            request.BirthDate,
            DateOnly.FromDateTime(nowUtc),
            nowUtc);
        if (!person.IsSuccess)
        {
            throw new InvalidOperationException(person.Errors[0].Message);
        }

        return person.Value!;
    }

    private static EmployeeDependent CreateDependent(AddDependentRequest request, long personId, DateTime nowUtc)
    {
        var dependent = EmployeeDependent.Create(
            request.EmployeeId,
            personId,
            request.RelationshipType,
            request.EducationStatus,
            request.InsuranceStatus,
            request.Notes,
            nowUtc);
        if (!dependent.IsSuccess)
        {
            throw new InvalidOperationException(dependent.Errors[0].Message);
        }

        return dependent.Value!;
    }

    private static PersonnelRecordResult Invalid(string message) =>
        PersonnelRecordResult.Failure(PersonnelRecordFailureCode.InvalidInput, message);

    private static string? NormalizeDigitsOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return PersianTextNormalizer.NormalizeDigits(value.Trim()).Replace(" ", string.Empty, StringComparison.Ordinal);
    }

    private static string? NormalizeIban(string? value)
    {
        var normalized = NormalizeDigitsOrNull(value);
        return normalized?.Replace("-", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
    }
}
