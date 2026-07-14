using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeBankAccount : AuditableEntity
{
    private EmployeeBankAccount()
    {
    }

    private EmployeeBankAccount(
        long employeeId,
        string bankName,
        string accountNumber,
        string? cardNumber,
        string? iban,
        bool isPrimary,
        string? notes,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        BankName = bankName.Trim();
        AccountNumber = accountNumber.Trim();
        CardNumber = string.IsNullOrWhiteSpace(cardNumber) ? null : cardNumber.Trim();
        Iban = string.IsNullOrWhiteSpace(iban) ? null : iban.Trim().ToUpperInvariant();
        IsPrimary = isPrimary;
        IsActive = true;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        InitializeTimestamps(nowUtc);
    }

    public long EmployeeId { get; private set; }
    public string BankName { get; private set; } = string.Empty;
    public string AccountNumber { get; private set; } = string.Empty;
    public string? CardNumber { get; private set; }
    public string? Iban { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public static ValidationResult<EmployeeBankAccount> Create(
        long employeeId,
        string? bankName,
        string? accountNumber,
        string? cardNumber,
        string? iban,
        bool isPrimary,
        string? notes,
        DateTime nowUtc)
    {
        if (employeeId <= 0)
        {
            return ValidationResult<EmployeeBankAccount>.Failure(
                "bank.employee.required",
                "کارمند برای ثبت حساب بانکی الزامی است.");
        }

        if (string.IsNullOrWhiteSpace(bankName) || string.IsNullOrWhiteSpace(accountNumber))
        {
            return ValidationResult<EmployeeBankAccount>.Failure(
                "bank.required",
                "نام بانک و شماره حساب الزامی است.");
        }

        return ValidationResult<EmployeeBankAccount>.Success(
            new EmployeeBankAccount(employeeId, bankName, accountNumber, cardNumber, iban, isPrimary, notes, nowUtc));
    }

    public void ClearPrimary(DateTime nowUtc)
    {
        IsPrimary = false;
        Touch(nowUtc);
    }
}
