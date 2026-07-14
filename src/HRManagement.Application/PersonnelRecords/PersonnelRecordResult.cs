namespace HRManagement.Application.PersonnelRecords;

public sealed record PersonnelRecordResult(
    bool IsSuccess,
    PersonnelRecordFailureCode FailureCode,
    string UserMessage,
    long? EntityId = null)
{
    public static PersonnelRecordResult Success(long? entityId = null) =>
        new(true, PersonnelRecordFailureCode.None, string.Empty, entityId);

    public static PersonnelRecordResult Failure(
        PersonnelRecordFailureCode failureCode,
        string userMessage) =>
        new(false, failureCode, userMessage);
}
