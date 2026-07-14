using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class AppSetting : Entity
{
    private AppSetting()
    {
    }

    private AppSetting(string key, string value, string valueType, DateTime updatedAtUtc)
    {
        Key = key.Trim();
        Value = value;
        ValueType = valueType.Trim();
        UpdatedAtUtc = updatedAtUtc.Kind == DateTimeKind.Utc ? updatedAtUtc : updatedAtUtc.ToUniversalTime();
    }

    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string ValueType { get; private set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; private set; }

    public static AppSetting Create(string key, string value, string valueType, DateTime updatedAtUtc) =>
        new(key, value, valueType, updatedAtUtc);

    public void Update(string value, string valueType, DateTime updatedAtUtc)
    {
        Value = value;
        ValueType = valueType.Trim();
        UpdatedAtUtc = updatedAtUtc.Kind == DateTimeKind.Utc ? updatedAtUtc : updatedAtUtc.ToUniversalTime();
    }
}
