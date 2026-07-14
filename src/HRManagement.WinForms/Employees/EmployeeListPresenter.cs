using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class EmployeeListPresenter(
    IEmployeeListView view,
    IEmployeeSearchService searchService,
    IDelay delay,
    ILogger<EmployeeListPresenter>? logger = null,
    IBackgroundExecutor? backgroundExecutor = null) : IDisposable
{
    private CancellationTokenSource? activeSearch;
    private long requestVersion;
    private bool disposed;

    public async Task SearchChangedAsync(CancellationToken cancellationToken)
    {
        await delay.DelayAsync(TimeSpan.FromMilliseconds(300), cancellationToken);
        await RefreshAsync(cancellationToken);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        activeSearch?.Cancel();
        activeSearch?.Dispose();
        activeSearch = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = activeSearch.Token;
        var version = Interlocked.Increment(ref requestVersion);
        view.SetLoading(true);

        try
        {
            var request = new EmployeeSearchRequest(
                view.SearchText,
                view.PageNumber,
                view.PageSize,
                view.Sort,
                view.Filter);
            var executor = backgroundExecutor ?? new InlineBackgroundExecutor();
            var page = await executor.ExecuteAsync(
                backgroundToken => searchService.SearchAsync(request, backgroundToken),
                token);
            if (version == Volatile.Read(ref requestVersion) && !token.IsCancellationRequested)
            {
                view.ShowPage(page);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            return;
        }
        catch (Exception) when (version != Volatile.Read(ref requestVersion))
        {
            return;
        }
        catch (Exception exception)
        {
            if (version == Volatile.Read(ref requestVersion))
            {
                (logger ?? NullLogger<EmployeeListPresenter>.Instance)
                    .LogError(exception, "Employee list search failed");
                view.ShowError("بارگذاری فهرست کارکنان انجام نشد.");
            }
        }
        finally
        {
            if (version == Volatile.Read(ref requestVersion))
            {
                view.SetLoading(false);
            }
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        activeSearch?.Cancel();
        activeSearch?.Dispose();
    }

    private sealed class InlineBackgroundExecutor : IBackgroundExecutor
    {
        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken) =>
            operation(cancellationToken);
    }
}
