namespace HRManagement.Application.Employees.Search;

public interface IEmployeeSearchService
{
    Task<PagedResult<EmployeeListItemDto>> SearchAsync(
        EmployeeSearchRequest request,
        CancellationToken cancellationToken);
}
