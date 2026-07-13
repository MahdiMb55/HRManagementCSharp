namespace HRManagement.Application.Abstractions;

public interface IBackgroundExecutor
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, T> operation, CancellationToken cancellationToken);
}
