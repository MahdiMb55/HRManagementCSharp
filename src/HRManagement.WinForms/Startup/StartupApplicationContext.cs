using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Startup;

public sealed class StartupApplicationContext : ApplicationContext
{
    private readonly CancellationTokenSource lifetime = new();
    private IHost? host;
    private Form? mainForm;
    private bool shuttingDown;
    private bool disposed;

    public StartupApplicationContext()
    {
        System.Windows.Forms.Application.Idle += OnFirstIdle;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            System.Windows.Forms.Application.Idle -= OnFirstIdle;
            lifetime.Cancel();
            lifetime.Dispose();
            host?.Dispose();
            host = null;
        }

        base.Dispose(disposing);
    }

    private async void OnFirstIdle(object? sender, EventArgs eventArgs)
    {
        System.Windows.Forms.Application.Idle -= OnFirstIdle;
        try
        {
            var bootstrap = await ApplicationBootstrapper.BuildAsync(lifetime.Token);
            if (!bootstrap.IsSuccess || bootstrap.Host is null || bootstrap.MainForm is null)
            {
                ShowStartupError(bootstrap.UserMessage ?? "راه‌اندازی برنامه انجام نشد.");
                ExitThread();
                return;
            }

            host = bootstrap.Host;
            mainForm = bootstrap.MainForm;
            mainForm.FormClosed += OnMainFormClosed;
            mainForm.Show();
        }
        catch (OperationCanceledException) when (lifetime.IsCancellationRequested)
        {
            ExitThread();
        }
        catch (Exception)
        {
            ShowStartupError("راه‌اندازی برنامه انجام نشد. جزئیات در گزارش برنامه ثبت شد.");
            ExitThread();
        }
    }

    private async void OnMainFormClosed(object? sender, FormClosedEventArgs eventArgs)
    {
        if (shuttingDown)
        {
            return;
        }

        shuttingDown = true;
        if (mainForm is not null)
        {
            mainForm.FormClosed -= OnMainFormClosed;
        }

        try
        {
            if (host is not null)
            {
                var logger = host.Services.GetService<ILoggerFactory>()?.CreateLogger<StartupApplicationContext>()
                    ?? NullLogger<StartupApplicationContext>.Instance;
                await HostLifecycle.StopAndDisposeAsync(host, logger, CancellationToken.None);
                host = null;
            }
        }
        finally
        {
            ExitThread();
        }
    }

    private static void ShowStartupError(string message) => MessageBox.Show(
        message,
        "مدیریت منابع انسانی",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error,
        MessageBoxDefaultButton.Button1,
        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
}
