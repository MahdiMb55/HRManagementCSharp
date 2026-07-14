namespace HRManagement.Application.Abstractions;

public interface IBackgroundExecutor
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);
}
