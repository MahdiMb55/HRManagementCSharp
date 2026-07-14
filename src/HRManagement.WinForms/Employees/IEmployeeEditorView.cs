using HRManagement.Application.Employees;
using HRManagement.Domain.Enums;

namespace HRManagement.WinForms.Employees;

public enum EmployeeEditorField
{
    FirstName = 1,
    LastName = 2,
    PersonnelNumber = 3,
    NationalCode = 4,
    BirthDate = 5,
    General = 6,
}

public interface IEmployeeEditorView
{
    long? EmployeeId { get; }
    string FirstName { get; }
    string LastName { get; }
    string PersonnelNumber { get; }
    string NationalCode { get; }
    string? FatherName { get; }
    Gender Gender { get; }
    DateOnly? BirthDate { get; }
    string? MobileNumber { get; }

    void Apply(EmployeeEditDto employee);
    void ClearErrors();
    void SetBusy(bool isBusy);
    void ShowFieldError(EmployeeEditorField field, string message);
    void ShowGeneralError(string message);
    void NotifySaved(long employeeId);
}
