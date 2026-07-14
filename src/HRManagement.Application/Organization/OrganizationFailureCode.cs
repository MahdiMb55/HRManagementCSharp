namespace HRManagement.Application.Organization;

public enum OrganizationFailureCode
{
    None = 0,
    EmployeeNotFound = 1,
    EmploymentPeriodNotFound = 2,
    DepartmentNotFound = 3,
    ResponsibilityNotFound = 4,
    InvalidDateRange = 5,
    DuplicateActiveDepartment = 6,
    DuplicateActiveResponsibility = 7,
    PrimaryResponsibilityRequired = 8,
}
