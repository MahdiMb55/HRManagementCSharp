using System.Text;
using System.Globalization;
using HRManagement.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace HRManagement.Infrastructure.Logging;

public static class LoggingRegistration
{
    public static void AddFileLogging(this HostApplicationBuilder builder, IApplicationPaths paths)
    {
        builder.Services.AddSerilog((_, configuration) => configuration
            .MinimumLevel.Is(GetMinimumLevel())
            .Enrich.FromLogContext()
            .WriteTo.File(
                Path.Combine(paths.LogsDirectory, "hrmanagement-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                shared: true,
                encoding: Encoding.UTF8,
                formatProvider: CultureInfo.InvariantCulture,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"));
    }

    private static LogEventLevel GetMinimumLevel()
    {
#if DEBUG
        return LogEventLevel.Debug;
#else
        return LogEventLevel.Information;
#endif
    }
}
