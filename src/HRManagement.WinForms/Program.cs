using HRManagement.WinForms.Startup;

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
            MessageBox.Show(
                "برنامه هم‌اکنون در حال اجرا است.",
                "مدیریت منابع انسانی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return;
        }

        using var context = new StartupApplicationContext();
        System.Windows.Forms.Application.Run(context);
    }
}
