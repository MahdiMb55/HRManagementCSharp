using HRManagement.Application.Employees.Search;
using HRManagement.Domain.Enums;

namespace HRManagement.Application.Tests.Employees;

public sealed class EmployeeSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_NormalizesPersianQueryAndPreservesDefaultDepartedFilter()
    {
        var repository = new CapturingRepository();
        var service = new EmployeeSearchService(repository);
        var request = new EmployeeSearchRequest(
            "  علي   رضايي  ", 1, 25,
            new EmployeeSort(EmployeeSortField.PersonnelNumber, SortDirection.Ascending),
            EmployeeFilter.Default);

        await service.SearchAsync(request, CancellationToken.None);

        Assert.Equal("علی رضایی", repository.Request!.Query);
        Assert.False(repository.Request.Filter.IncludeDeparted);
    }

    [Theory]
    [InlineData(24)]
    [InlineData(26)]
    [InlineData(101)]
    public async Task SearchAsync_RejectsUnsupportedPageSize(int pageSize)
    {
        var service = new EmployeeSearchService(new CapturingRepository());
        var request = new EmployeeSearchRequest(
            null, 1, pageSize,
            new EmployeeSort(EmployeeSortField.PersonnelNumber, SortDirection.Ascending),
            EmployeeFilter.Default);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_NormalizesPageNumberToOne()
    {
        var repository = new CapturingRepository();
        var service = new EmployeeSearchService(repository);
        var request = new EmployeeSearchRequest(
            null, 0, 50,
            new EmployeeSort(EmployeeSortField.LastName, SortDirection.Descending),
            new EmployeeFilter([EmploymentStatus.Active], [], [], false));

        await service.SearchAsync(request, CancellationToken.None);

        Assert.Equal(1, repository.Request!.PageNumber);
    }

    private sealed class CapturingRepository : IEmployeeSearchRepository
    {
        public EmployeeSearchRequest? Request { get; private set; }

        public Task<PagedResult<EmployeeListItemDto>> SearchAsync(
            EmployeeSearchRequest request,
            CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new PagedResult<EmployeeListItemDto>([], 0, request.PageNumber, request.PageSize));
        }
    }
}
