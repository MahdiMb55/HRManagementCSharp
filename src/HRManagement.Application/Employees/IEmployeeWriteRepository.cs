namespace HRManagement.Application.Employees;

public interface IEmployeeWriteRepository
{
    Task<EmployeeEditDto?> FindForEditAsync(long employeeId, CancellationToken cancellationToken);
    Task<bool> NationalCodeExistsAsync(string nationalCode, long? excludingEmployeeId, CancellationToken cancellationToken);
    Task<bool> PersonnelNumberExistsAsync(string personnelNumber, long? excludingEmployeeId, CancellationToken cancellationToken);
    Task<long> SaveAsync(ValidatedEmployeeSave employeeSave, CancellationToken cancellationToken);
}
