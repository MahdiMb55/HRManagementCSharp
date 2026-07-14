using HRManagement.Domain.Text;

namespace HRManagement.Application.Employees.Search;

public sealed class EmployeeSearchService(IEmployeeSearchRepository repository)
    : IEmployeeSearchService
{
    private static readonly HashSet<int> SupportedPageSizes = [25, 50, 100];

    public Task<PagedResult<EmployeeListItemDto>> SearchAsync(
        EmployeeSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (!SupportedPageSizes.Contains(request.PageSize))
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                request.PageSize,
                "Page size must be 25, 50, or 100.");
        }

        var normalized = request with
        {
            Query = PersianTextNormalizer.Normalize(request.Query),
            PageNumber = Math.Max(1, request.PageNumber),
        };
        return repository.SearchAsync(normalized, cancellationToken);
    }
}
