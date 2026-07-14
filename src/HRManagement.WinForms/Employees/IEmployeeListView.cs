using HRManagement.Application.Employees.Search;

namespace HRManagement.WinForms.Employees;

public interface IEmployeeListView
{
    string? SearchText { get; }
    int PageNumber { get; }
    int PageSize { get; }
    EmployeeSort Sort { get; }
    EmployeeFilter Filter { get; }

    void SetLoading(bool isLoading);
    void ShowPage(PagedResult<EmployeeListItemDto> page);
    void ShowError(string message);
}
