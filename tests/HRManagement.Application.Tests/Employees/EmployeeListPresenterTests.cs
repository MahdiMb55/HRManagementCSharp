using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees.Search;
using HRManagement.WinForms.Employees;
using Microsoft.Extensions.Logging;

namespace HRManagement.Application.Tests.Employees;

public sealed class EmployeeListPresenterTests
{
    [Fact]
    public async Task SearchChangedAsync_DebouncesForThreeHundredMilliseconds()
    {
        var view = new FakeView { SearchText = "علی" };
        var delay = new CapturingDelay();
        var service = new ControlledSearchService();
        service.Complete("علی", Result(1));
        using var presenter = new EmployeeListPresenter(view, service, delay);

        await presenter.SearchChangedAsync(CancellationToken.None);

        Assert.Equal(TimeSpan.FromMilliseconds(300), delay.LastDelay);
        Assert.Single(view.LastPage!.Items);
    }

    [Fact]
    public async Task RefreshAsync_DoesNotDisplayStaleOlderResult()
    {
        var view = new FakeView { SearchText = "اول" };
        var service = new ControlledSearchService();
        using var presenter = new EmployeeListPresenter(view, service, new CapturingDelay());

        var first = presenter.RefreshAsync(CancellationToken.None);
        view.SearchText = "دوم";
        var second = presenter.RefreshAsync(CancellationToken.None);
        service.Complete("دوم", Result(2));
        await second;
        service.Complete("اول", Result(1));
        await first;

        Assert.Equal(2, view.LastPage!.Items.Single().EmployeeId);
    }

    [Fact]
    public async Task RefreshAsync_LogsUnexpectedSearchFailure()
    {
        var view = new FakeView();
        var logger = new CapturingLogger<EmployeeListPresenter>();
        using var presenter = new EmployeeListPresenter(
            view,
            new ThrowingSearchService(),
            new CapturingDelay(),
            logger);

        await presenter.RefreshAsync(CancellationToken.None);

        Assert.Single(logger.Exceptions);
        Assert.Equal("بارگذاری فهرست کارکنان انجام نشد.", view.ErrorMessage);
    }

    [Fact]
    public async Task RefreshAsync_RunsSearchThroughBackgroundExecutor()
    {
        var view = new FakeView { SearchText = "علی" };
        var executor = new CapturingBackgroundExecutor();
        using var presenter = new EmployeeListPresenter(
            view,
            new ImmediateSearchService(),
            new CapturingDelay(),
            backgroundExecutor: executor);

        await presenter.RefreshAsync(CancellationToken.None);

        Assert.True(executor.WasUsed);
        Assert.Single(view.LastPage!.Items);
    }

    private static PagedResult<EmployeeListItemDto> Result(long employeeId) =>
        new(
            [new EmployeeListItemDto(employeeId, "1", "نام", "نام خانوادگی", "0013548581", null, null, null, null, null, HRManagement.Domain.Enums.EmploymentStatus.Active)],
            1,
            1,
            25);

    private sealed class FakeView : IEmployeeListView
    {
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public EmployeeSort Sort { get; set; } = new(EmployeeSortField.PersonnelNumber, SortDirection.Ascending);
        public EmployeeFilter Filter { get; set; } = EmployeeFilter.Default;
        public PagedResult<EmployeeListItemDto>? LastPage { get; private set; }
        public string? ErrorMessage { get; private set; }

        public void SetLoading(bool isLoading) { }
        public void ShowPage(PagedResult<EmployeeListItemDto> page) => LastPage = page;
        public void ShowError(string message) => ErrorMessage = message;
    }

    private sealed class CapturingDelay : IDelay
    {
        public TimeSpan LastDelay { get; private set; }

        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            LastDelay = delay;
            return Task.CompletedTask;
        }
    }

    private sealed class ControlledSearchService : IEmployeeSearchService
    {
        private readonly Dictionary<string, TaskCompletionSource<PagedResult<EmployeeListItemDto>>> pending = [];
        private readonly Dictionary<string, PagedResult<EmployeeListItemDto>> completed = [];

        public Task<PagedResult<EmployeeListItemDto>> SearchAsync(EmployeeSearchRequest request, CancellationToken cancellationToken)
        {
            var query = request.Query ?? string.Empty;
            if (completed.TryGetValue(query, out var result))
            {
                return Task.FromResult(result);
            }

            var source = new TaskCompletionSource<PagedResult<EmployeeListItemDto>>(TaskCreationOptions.RunContinuationsAsynchronously);
            pending[query] = source;
            return source.Task;
        }

        public void Complete(string query, PagedResult<EmployeeListItemDto> result)
        {
            completed[query] = result;
            if (pending.Remove(query, out var source))
            {
                source.SetResult(result);
            }
        }
    }

    private sealed class ThrowingSearchService : IEmployeeSearchService
    {
        public Task<PagedResult<EmployeeListItemDto>> SearchAsync(
            EmployeeSearchRequest request,
            CancellationToken cancellationToken) =>
            Task.FromException<PagedResult<EmployeeListItemDto>>(new InvalidOperationException("database failure"));
    }

    private sealed class ImmediateSearchService : IEmployeeSearchService
    {
        public Task<PagedResult<EmployeeListItemDto>> SearchAsync(
            EmployeeSearchRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(Result(1));
    }

    private sealed class CapturingBackgroundExecutor : IBackgroundExecutor
    {
        public bool WasUsed { get; private set; }

        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
        {
            WasUsed = true;
            return operation(cancellationToken);
        }
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<Exception?> Exceptions { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => EmptyScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) => Exceptions.Add(exception);

        private sealed class EmptyScope : IDisposable
        {
            public static EmptyScope Instance { get; } = new();
            public void Dispose() { }
        }
    }
}
