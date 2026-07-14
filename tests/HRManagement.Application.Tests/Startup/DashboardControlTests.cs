using HRManagement.Application.Abstractions;
using HRManagement.Application.Dashboard;
using HRManagement.WinForms.Controls;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.Application.Tests.Startup;

public sealed class DashboardControlTests
{
    [Fact]
    public async Task InitializeAsync_LoadsSnapshotThroughBackgroundExecutor()
    {
        var executor = new CapturingBackgroundExecutor();
        using var control = new DashboardControl(
            new StubDashboardService(),
            NullLogger<DashboardControl>.Instance,
            executor);

        await control.InitializeAsync(CancellationToken.None);

        Assert.True(executor.WasUsed);
    }

    private sealed class StubDashboardService : IDashboardService
    {
        public Task<DashboardSnapshot> GetSnapshotAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new DashboardSnapshot(0, 0, 0, []));
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
}
