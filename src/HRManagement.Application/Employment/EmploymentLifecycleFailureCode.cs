namespace HRManagement.Application.Employment;

public enum EmploymentLifecycleFailureCode
{
    None = 0,
    EmployeeNotFound = 1,
    EmploymentPeriodNotFound = 2,
    OpenEmploymentPeriodExists = 3,
    NoOpenEmploymentPeriod = 4,
    InvalidDateRange = 5,
    TerminationAlreadyExists = 6,
    InvalidStatus = 7,
}
