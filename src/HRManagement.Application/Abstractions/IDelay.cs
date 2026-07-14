namespace HRManagement.Application.Abstractions;

public interface IDelay
{
    Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken);
}
