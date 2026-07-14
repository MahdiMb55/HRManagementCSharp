namespace HRManagement.Application.PersonnelRecords;

public enum PersonnelRecordFailureCode
{
    None = 0,
    InvalidInput,
    EmployeeNotFound,
    EmploymentPeriodRequired,
    DuplicateRecord,
    SelfDependent,
}
