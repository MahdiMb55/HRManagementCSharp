using HRManagement.Application.Abstractions;

namespace HRManagement.Infrastructure.Threading;

public sealed class ThreadPoolBackgroundExecutor : IBackgroundExecutor
{
    public Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return Task.Run(async () => await operation(cancellationToken), cancellationToken);
    }
}
