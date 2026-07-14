namespace HRManagement.Application.Settings;

public interface IAdministrationSettingsService
{
    Task<IReadOnlyList<AppSettingDto>> GetSettingsAsync(CancellationToken cancellationToken);

    Task<SettingsResult> SaveSettingAsync(
        string key,
        string value,
        string valueType,
        CancellationToken cancellationToken);

    Task<CompanyProfileDto?> GetCompanyProfileAsync(CancellationToken cancellationToken);

    Task<SettingsResult> SaveCompanyProfileAsync(
        CompanyProfileDto profile,
        CancellationToken cancellationToken);
}
