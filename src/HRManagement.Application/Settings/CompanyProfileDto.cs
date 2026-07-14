namespace HRManagement.Application.Settings;

public sealed record CompanyProfileDto(
    string CompanyName,
    string? NationalIdentifier,
    string? RegistrationNumber,
    string? PhoneNumber,
    string? Address,
    long? LogoFileId);
