using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeBankAccount : AuditableEntity
{
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
}
