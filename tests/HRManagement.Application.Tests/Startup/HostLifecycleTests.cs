using HRManagement.WinForms.Startup;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.Application.Tests.Startup;

public sealed class HostLifecycleTests
{
    [Fact]
    public async Task StopAndDisposeAsync_DisposesHostWhenStopFails()
    {
        var host = new ThrowingHost();

        await HostLifecycle.StopAndDisposeAsync(host, NullLogger.Instance, CancellationToken.None);

        Assert.True(host.IsDisposed);
    }

    [Fact]
    public void StartupApplicationContext_DisposeCanBeCalledMoreThanOnce()
    {
        using var context = new StartupApplicationContext();

        context.Dispose();
        context.Dispose();
    }

    private sealed class ThrowingHost : IHost
    {
        public IServiceProvider Services { get; } = new EmptyServiceProvider();
        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken = default) =>
            Task.FromException(new InvalidOperationException("shutdown failure"));

        private sealed class EmptyServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType) => null;
        }
    }
}
