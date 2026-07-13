namespace HRManagement.Application.Employees;

public interface IEmployeeEditorService
{
    Task<EmployeeEditDto?> GetAsync(long employeeId, CancellationToken cancellationToken);
    Task<SaveEmployeeResult> SaveAsync(SaveEmployeeRequest request, CancellationToken cancellationToken);
}
