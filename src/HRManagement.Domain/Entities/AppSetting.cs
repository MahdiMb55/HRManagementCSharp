using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class AppSetting : Entity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string ValueType { get; private set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; private set; }
}
