namespace HRManagement.Application.Employees.Search;

public interface IEmployeeSearchRepository
{
    Task<PagedResult<EmployeeListItemDto>> SearchAsync(
        EmployeeSearchRequest request,
        CancellationToken cancellationToken);
}
