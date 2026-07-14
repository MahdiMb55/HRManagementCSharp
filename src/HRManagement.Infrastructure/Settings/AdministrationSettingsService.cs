using HRManagement.Application.Abstractions;
using HRManagement.Application.Settings;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Settings;

public sealed class AdministrationSettingsService(
    IDbContextFactory<HrManagementDbContext> contextFactory,
    IClock clock) : IAdministrationSettingsService
{
    public async Task<IReadOnlyList<AppSettingDto>> GetSettingsAsync(CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.AppSettings
            .AsNoTracking()
            .OrderBy(setting => setting.Key)
            .Select(setting => new AppSettingDto(setting.Key, setting.Value, setting.ValueType, setting.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<SettingsResult> SaveSettingAsync(
        string key,
        string value,
        string valueType,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(valueType))
        {
            return SettingsResult.Failure("کلید و نوع مقدار الزامی است.");
        }

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var setting = await context.AppSettings.SingleOrDefaultAsync(candidate => candidate.Key == key.Trim(), cancellationToken);
        if (setting is null)
        {
            context.AppSettings.Add(AppSetting.Create(key, value, valueType, clock.UtcNow));
        }
        else
        {
            setting.Update(value, valueType, clock.UtcNow);
        }

        await context.SaveChangesAsync(cancellationToken);
        return SettingsResult.Success();
    }

    public async Task<CompanyProfileDto?> GetCompanyProfileAsync(CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.CompanyProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == 1)
            .Select(profile => new CompanyProfileDto(
                profile.CompanyName,
                profile.NationalIdentifier,
                profile.RegistrationNumber,
                profile.PhoneNumber,
                profile.Address,
                profile.LogoFileId))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<SettingsResult> SaveCompanyProfileAsync(
        CompanyProfileDto profile,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(profile.CompanyName))
        {
            return SettingsResult.Failure("نام شرکت الزامی است.");
        }

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await context.CompanyProfiles.SingleOrDefaultAsync(candidate => candidate.Id == 1, cancellationToken);
        var saved = CompanyProfile.CreateOrUpdate(
            existing,
            profile.CompanyName,
            profile.NationalIdentifier,
            profile.RegistrationNumber,
            profile.PhoneNumber,
            profile.Address,
            profile.LogoFileId,
            clock.UtcNow);
        if (existing is null)
        {
            context.CompanyProfiles.Add(saved);
        }

        await context.SaveChangesAsync(cancellationToken);
        return SettingsResult.Success();
    }
}
