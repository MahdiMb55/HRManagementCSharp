namespace HRManagement.Application.Employees;

public sealed record SaveEmployeeResult(
    bool IsSuccess,
    long? EmployeeId,
    EmployeeSaveFailureCode FailureCode,
    string UserMessage)
{
    public static SaveEmployeeResult Success(long employeeId) =>
        new(true, employeeId, EmployeeSaveFailureCode.None, "اطلاعات کارمند ذخیره شد.");

    public static SaveEmployeeResult Failure(EmployeeSaveFailureCode code, string message) =>
        new(false, null, code, message);
}
