namespace HRManagement.Application.Settings;

public sealed record AppSettingDto(
    string Key,
    string Value,
    string ValueType,
    DateTime UpdatedAtUtc);
