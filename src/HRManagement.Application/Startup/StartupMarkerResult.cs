namespace HRManagement.Application.Startup;

public sealed record StartupMarkerResult(bool IsSuccess, string? ErrorCode = null)
{
    public static StartupMarkerResult Success() => new(true);
    public static StartupMarkerResult Failure(string errorCode) => new(false, errorCode);
}
