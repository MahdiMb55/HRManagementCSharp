using HRManagement.WinForms.Controls;
using HRManagement.WinForms.Employees;

namespace HRManagement.WinForms.Forms;

public partial class MainForm : Form
{
    private readonly DashboardControl dashboardControl;
    private readonly EmployeeListControl employeeListControl;
    private readonly Dictionary<string, Control> placeholderPages = [];
    private readonly CancellationTokenSource lifetime = new();
    private bool employeesInitialized;

    public MainForm(DashboardControl dashboardControl, EmployeeListControl employeeListControl)
    {
        this.dashboardControl = dashboardControl;
        this.employeeListControl = employeeListControl;
        InitializeComponent();
        WireNavigation();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        lifetime.Cancel();
        lifetime.Dispose();
        foreach (var page in placeholderPages.Values)
        {
            page.Dispose();
        }

        base.OnFormClosed(e);
    }

    private void WireNavigation()
    {
        dashboardButton.Click += async (_, _) =>
        {
            ShowPage(dashboardControl, "داشبورد");
            await dashboardControl.InitializeAsync(lifetime.Token);
        };
        employeesButton.Click += async (_, _) =>
        {
            ShowPage(employeeListControl, "کارکنان");
            if (!employeesInitialized)
            {
                employeesInitialized = true;
                await employeeListControl.InitializeAsync(lifetime.Token);
            }
        };
        departmentsButton.Click += (_, _) => ShowMessagePage("واحدهای سازمانی", "مدیریت واحدهای سازمانی در مرحله سازماندهی سوابق فعال می‌شود.");
        responsibilitiesButton.Click += (_, _) => ShowMessagePage("مسئولیت‌ها", "مدیریت مسئولیت‌ها در مرحله سازماندهی سوابق فعال می‌شود.");
        archiveButton.Click += (_, _) => ShowMessagePage("بایگانی", "بایگانی کارکنان در مرحله بایگانی و بازیابی فعال می‌شود.");
        backupsButton.Click += (_, _) => ShowMessagePage("پشتیبان‌گیری", "پشتیبان‌گیری و بازیابی در مرحله مدیریت داده فعال می‌شود.");
        settingsButton.Click += (_, _) => ShowMessagePage("تنظیمات", "تنظیمات شرکت و برنامه در مرحله مدیریت داده فعال می‌شود.");
        Shown += async (_, _) =>
        {
            ShowPage(dashboardControl, "داشبورد");
            await dashboardControl.InitializeAsync(lifetime.Token);
        };
    }

    private void ShowPage(Control control, string title)
    {
        pageTitleLabel.Text = title;
        contentPanel.SuspendLayout();
        contentPanel.Controls.Clear();
        control.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(control);
        contentPanel.ResumeLayout();
    }

    private void ShowMessagePage(string title, string message)
    {
        var key = $"{title}\n{message}";
        if (!placeholderPages.TryGetValue(key, out var label))
        {
            label = new Label
            {
                Dock = DockStyle.Fill,
                Text = message,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DimGray,
            };
            placeholderPages.Add(key, label);
        }

        ShowPage(label, title);
    }
}
