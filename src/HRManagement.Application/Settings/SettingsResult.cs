namespace HRManagement.Application.Settings;

public sealed record SettingsResult(bool IsSuccess, string UserMessage)
{
    public static SettingsResult Success() => new(true, string.Empty);
    public static SettingsResult Failure(string userMessage) => new(false, userMessage);
}
