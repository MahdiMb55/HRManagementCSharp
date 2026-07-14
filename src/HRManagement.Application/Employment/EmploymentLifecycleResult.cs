namespace HRManagement.Application.Employment;

public sealed record EmploymentLifecycleResult(
    bool IsSuccess,
    EmploymentLifecycleFailureCode FailureCode,
    string UserMessage,
    long? EntityId = null)
{
    public static EmploymentLifecycleResult Success(long? entityId = null) =>
        new(true, EmploymentLifecycleFailureCode.None, string.Empty, entityId);

    public static EmploymentLifecycleResult Failure(
        EmploymentLifecycleFailureCode failureCode,
        string userMessage) =>
        new(false, failureCode, userMessage);
}
