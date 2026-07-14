namespace HRManagement.Application.Files;

public sealed record FileRecordResult(
    bool IsSuccess,
    FileRecordFailureCode FailureCode,
    string UserMessage,
    long? EntityId = null)
{
    public static FileRecordResult Success(long? entityId = null) =>
        new(true, FileRecordFailureCode.None, string.Empty, entityId);

    public static FileRecordResult Failure(FileRecordFailureCode failureCode, string userMessage) =>
        new(false, failureCode, userMessage);
}
