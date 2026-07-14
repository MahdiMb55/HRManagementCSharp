using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Diagnostics;

public sealed class DevelopmentPerformanceScope : IDisposable
{
    private readonly string operationName;
    private readonly ILogger logger;
    private readonly Stopwatch stopwatch;

    private DevelopmentPerformanceScope(string operationName, ILogger logger)
    {
        this.operationName = operationName;
        this.logger = logger;
        stopwatch = Stopwatch.StartNew();
    }

    public static IDisposable Start(string operationName, ILogger logger)
    {
#if DEBUG
        return new DevelopmentPerformanceScope(operationName, logger);
#else
        return NullScope.Instance;
#endif
    }

    public void Dispose()
    {
        stopwatch.Stop();
        if (stopwatch.ElapsedMilliseconds >= 50)
        {
            logger.LogDebug("Operation {OperationName} took {ElapsedMilliseconds} ms", operationName, stopwatch.ElapsedMilliseconds);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose() { }
    }
}
