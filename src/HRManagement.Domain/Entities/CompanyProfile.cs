using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class CompanyProfile : Entity
{
    private CompanyProfile()
    {
    }

    public string CompanyName { get; private set; } = string.Empty;
    public string? NationalIdentifier { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public long? LogoFileId { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static CompanyProfile CreateOrUpdate(
        CompanyProfile? existing,
        string companyName,
        string? nationalIdentifier,
        string? registrationNumber,
        string? phoneNumber,
        string? address,
        long? logoFileId,
        DateTime updatedAtUtc)
    {
        var profile = existing ?? new CompanyProfile { Id = 1 };
        profile.CompanyName = companyName.Trim();
        profile.NationalIdentifier = string.IsNullOrWhiteSpace(nationalIdentifier) ? null : nationalIdentifier.Trim();
        profile.RegistrationNumber = string.IsNullOrWhiteSpace(registrationNumber) ? null : registrationNumber.Trim();
        profile.PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        profile.Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        profile.LogoFileId = logoFileId;
        profile.UpdatedAtUtc = updatedAtUtc.Kind == DateTimeKind.Utc ? updatedAtUtc : updatedAtUtc.ToUniversalTime();
        return profile;
    }
}
