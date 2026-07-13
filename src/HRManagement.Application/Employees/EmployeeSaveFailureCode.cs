namespace HRManagement.Application.Employees;

public enum EmployeeSaveFailureCode
{
    None = 0,
    RequiredName = 1,
    InvalidNationalCode = 2,
    InvalidPersonnelNumber = 3,
    DuplicateNationalCode = 4,
    DuplicatePersonnelNumber = 5,
    FutureBirthDate = 6,
    EmployeeNotFound = 7,
    PersonnelNumberChangeRequiresReason = 8,
}
