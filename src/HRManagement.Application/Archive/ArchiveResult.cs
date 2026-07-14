namespace HRManagement.Application.Archive;

public sealed record ArchiveResult(
    bool IsSuccess,
    ArchiveFailureCode FailureCode,
    string UserMessage,
    long? EntityId = null)
{
    public static ArchiveResult Success(long? entityId = null) =>
        new(true, ArchiveFailureCode.None, string.Empty, entityId);

    public static ArchiveResult Failure(ArchiveFailureCode failureCode, string userMessage) =>
        new(false, failureCode, userMessage);
}
