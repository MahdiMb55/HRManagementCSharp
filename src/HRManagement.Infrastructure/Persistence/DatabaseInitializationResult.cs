namespace HRManagement.Infrastructure.Persistence;

public sealed record DatabaseInitializationResult(bool IsSuccess, string? ErrorCode = null)
{
    public static DatabaseInitializationResult Success() => new(true);
}
