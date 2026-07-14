using HRManagement.Infrastructure.Logging;
using HRManagement.Infrastructure.Paths;
using HRManagement.Infrastructure.Persistence;
using HRManagement.Infrastructure.Startup;
using HRManagement.WinForms.Formatting;
using HRManagement.WinForms.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Startup;

public static class ApplicationBootstrapper
{
    public static async Task<BootstrapResult> BuildAsync(CancellationToken cancellationToken)
    {
        IHost? host = null;
        ILogger? startupLogger = null;
        try
        {
            var paths = ApplicationPaths.FromExecutableDirectory();
            Directory.CreateDirectory(paths.LogsDirectory);
            LogRetentionService.DeleteExpiredLogs(
                paths.LogsDirectory,
                TimeSpan.FromDays(30),
                DateTimeOffset.UtcNow);
            var builder = Host.CreateApplicationBuilder();
            builder.AddFileLogging(paths);
            builder.Services.AddHrManagement(paths);
            host = builder.Build();
            await host.StartAsync(cancellationToken);
            startupLogger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

            var integrity = host.Services.GetRequiredService<IStartupIntegrityService>();
            var integrityResult = await integrity.CheckAsync(cancellationToken);
            if (!integrityResult.CanContinue)
            {
                await HostLifecycle.StopAndDisposeAsync(
                    host,
                    startupLogger ?? NullLogger.Instance,
                    cancellationToken);
                return BootstrapResult.Failure(integrityResult.UserMessage);
            }

            var initializer = host.Services.GetRequiredService<IDatabaseInitializer>();
            await initializer.InitializeAsync(cancellationToken);
            if (integrityResult.Status == Application.Startup.StartupIntegrityStatus.ReadyFirstRun)
            {
                var marker = await integrity.MarkInitializedAsync(cancellationToken);
                if (!marker.IsSuccess)
                {
                    await HostLifecycle.StopAndDisposeAsync(
                        host,
                        startupLogger ?? NullLogger.Instance,
                        cancellationToken);
                    return BootstrapResult.Failure("راه‌اندازی اولیه کامل نشد. جزئیات در گزارش برنامه ثبت شد.");
                }
            }

            host.Services.GetRequiredService<GlobalExceptionHandler>().Register();
            var mainForm = host.Services.GetRequiredService<MainForm>();
            var font = host.Services.GetRequiredService<PrivateFontService>().CreateDefaultFont();
            ControlFontApplier.Apply(mainForm, font);
            startupLogger.LogInformation("Application startup completed");
            return BootstrapResult.Success(host, mainForm);
        }
        catch (Exception exception)
        {
            startupLogger?.LogCritical(exception, "Application bootstrap failed");
            if (host is not null)
            {
                await HostLifecycle.StopAndDisposeAsync(
                    host,
                    startupLogger ?? NullLogger.Instance,
                    cancellationToken);
            }

            return BootstrapResult.Failure("راه‌اندازی برنامه انجام نشد. مسیر نصب و پوشه گزارش‌ها را بررسی کنید.");
        }
    }

}
