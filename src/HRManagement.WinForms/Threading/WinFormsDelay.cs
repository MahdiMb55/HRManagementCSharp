using HRManagement.Application.Abstractions;

namespace HRManagement.WinForms.Threading;

public sealed class WinFormsDelay : IDelay
{
    public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) =>
        Task.Delay(delay, cancellationToken);
}
