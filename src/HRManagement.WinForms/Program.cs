using HRManagement.WinForms.Startup;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HRManagement.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        using var instanceGuard = SingleInstanceGuard.TryAcquire();
        if (!instanceGuard.IsPrimaryInstance)
        {
            ActivateExistingInstance();
            return;
        }

        using var context = new StartupApplicationContext();
        System.Windows.Forms.Application.Run(context);
    }

    private static void ActivateExistingInstance()
    {
        using var current = Process.GetCurrentProcess();
        foreach (var process in Process.GetProcessesByName(current.ProcessName))
        {
            using (process)
            {
                if (process.Id == current.Id || !string.Equals(process.MainModule?.FileName, current.MainModule?.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    ShowWindow(process.MainWindowHandle, ShowWindowCommand.Restore);
                    SetForegroundWindow(process.MainWindowHandle);
                }

                return;
            }
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

    private enum ShowWindowCommand
    {
        Restore = 9,
    }
}
