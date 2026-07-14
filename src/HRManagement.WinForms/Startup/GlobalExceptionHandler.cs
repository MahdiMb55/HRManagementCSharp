using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Startup;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
{
    public void Register()
    {
        System.Windows.Forms.Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private void OnThreadException(object sender, ThreadExceptionEventArgs eventArgs) =>
        Report(eventArgs.Exception, "ui.thread_exception");

    private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs eventArgs)
    {
        if (eventArgs.ExceptionObject is Exception exception)
        {
            Report(exception, "appdomain.unhandled_exception");
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs eventArgs)
    {
        logger.LogError(eventArgs.Exception, "Unobserved task exception");
        eventArgs.SetObserved();
    }

    private void Report(Exception exception, string operationName)
    {
        var operationId = Guid.NewGuid();
        logger.LogCritical(exception, "Fatal application error in {OperationName}; OperationId={OperationId}", operationName, operationId);
        MessageBox.Show(
            $"عملیات انجام نشد. جزئیات خطا با شناسه {operationId} در گزارش برنامه ثبت شد.",
            "خطای برنامه",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
    }
}
