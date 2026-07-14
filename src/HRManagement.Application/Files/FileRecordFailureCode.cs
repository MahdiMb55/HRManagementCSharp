namespace HRManagement.Application.Files;

public enum FileRecordFailureCode
{
    None = 0,
    InvalidInput,
    EmployeeNotFound,
    EmploymentPeriodRequired,
    FileRejected,
    DuplicateRecord,
}
