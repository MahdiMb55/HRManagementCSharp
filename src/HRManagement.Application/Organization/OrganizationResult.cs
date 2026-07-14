namespace HRManagement.Application.Organization;

public sealed record OrganizationResult(
    bool IsSuccess,
    OrganizationFailureCode FailureCode,
    string UserMessage,
    long? EntityId = null)
{
    public static OrganizationResult Success(long? entityId = null) =>
        new(true, OrganizationFailureCode.None, string.Empty, entityId);

    public static OrganizationResult Failure(
        OrganizationFailureCode failureCode,
        string userMessage) =>
        new(false, failureCode, userMessage);
}
