using HRManagement.Application.Abstractions;
using HRManagement.Domain.Identity;
using HRManagement.Domain.Text;

namespace HRManagement.Application.Employees;

public sealed class EmployeeEditorService(
    IEmployeeWriteRepository repository,
    IClock clock) : IEmployeeEditorService
{
    public Task<EmployeeEditDto?> GetAsync(long employeeId, CancellationToken cancellationToken) =>
        repository.FindForEditAsync(employeeId, cancellationToken);

    public async Task<SaveEmployeeResult> SaveAsync(
        SaveEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return SaveEmployeeResult.Failure(
                EmployeeSaveFailureCode.RequiredName,
                "نام و نام خانوادگی الزامی است.");
        }

        var nationalCode = NationalCode.Create(request.NationalCode);
        if (!nationalCode.IsSuccess)
        {
            return SaveEmployeeResult.Failure(
                EmployeeSaveFailureCode.InvalidNationalCode,
                nationalCode.Errors[0].Message);
        }

        var personnelNumber = PersonnelNumber.Create(request.PersonnelNumber);
        if (!personnelNumber.IsSuccess)
        {
            return SaveEmployeeResult.Failure(
                EmployeeSaveFailureCode.InvalidPersonnelNumber,
                personnelNumber.Errors[0].Message);
        }

        if (request.BirthDate > clock.Today)
        {
            return SaveEmployeeResult.Failure(
                EmployeeSaveFailureCode.FutureBirthDate,
                "تاریخ تولد نمی‌تواند در آینده باشد.");
        }

        if (request.EmployeeId is long employeeId)
        {
            var existing = await repository.FindForEditAsync(employeeId, cancellationToken);
            if (existing is null)
            {
                return SaveEmployeeResult.Failure(
                    EmployeeSaveFailureCode.EmployeeNotFound,
                    "کارمند موردنظر پیدا نشد.");
            }

            if (!string.Equals(existing.PersonnelNumber, personnelNumber.Value!.Value, StringComparison.Ordinal))
            {
                return SaveEmployeeResult.Failure(
                    EmployeeSaveFailureCode.PersonnelNumberChangeRequiresReason,
                    "تغییر شماره پرسنلی فقط از عملیات اختصاصی و با ثبت دلیل انجام می‌شود.");
            }
        }

        if (await repository.NationalCodeExistsAsync(
                nationalCode.Value!.Value,
                request.EmployeeId,
                cancellationToken))
        {
            return SaveEmployeeResult.Failure(
                EmployeeSaveFailureCode.DuplicateNationalCode,
                "کد ملی قبلاً ثبت شده است.");
        }

        if (await repository.PersonnelNumberExistsAsync(
                personnelNumber.Value!.Value,
                request.EmployeeId,
                cancellationToken))
        {
            return SaveEmployeeResult.Failure(
                EmployeeSaveFailureCode.DuplicatePersonnelNumber,
                "شماره پرسنلی قبلاً ثبت شده است.");
        }

        var validated = new ValidatedEmployeeSave(
            request.EmployeeId,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            personnelNumber.Value.Value,
            nationalCode.Value.Value,
            NormalizeOptional(request.FatherName),
            request.Gender,
            request.BirthDate,
            NormalizePhone(request.MobileNumber),
            clock.UtcNow,
            request.EmployeeId is null ? "employee.created" : "employee.updated");

        var savedEmployeeId = await repository.SaveAsync(validated, cancellationToken);
        return SaveEmployeeResult.Success(savedEmployeeId);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizePhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = PersianTextNormalizer.NormalizeDigits(value);
        var characters = normalized.Where(character => char.IsAsciiDigit(character)).ToArray();
        return characters.Length == 0 ? null : new string(characters);
    }
}
