using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Startup;

public static class HostLifecycle
{
    public static async Task StopAndDisposeAsync(
        IHost host,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        try
        {
            await host.StopAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Application host stop failed");
        }

        try
        {
            host.Dispose();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Application host disposal failed");
        }
    }
}
