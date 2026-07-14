namespace HRManagement.Application.PersonnelRecords;

public sealed record AddBankAccountRequest(
    long EmployeeId,
    string? BankName,
    string? AccountNumber,
    string? CardNumber,
    string? Iban,
    bool IsPrimary,
    string? Notes);
