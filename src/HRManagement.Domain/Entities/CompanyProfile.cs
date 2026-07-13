using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class CompanyProfile : Entity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string? NationalIdentifier { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public long? LogoFileId { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
}
