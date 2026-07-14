using HRManagement.Application.Abstractions;
using HRManagement.Application.Audit;
using HRManagement.Application.Backups;
using HRManagement.Application.Settings;
using HRManagement.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Backups;

public sealed class BackupsControl : UserControl
{
    private readonly IBackupService backupService;
    private readonly IAdministrationSettingsService settingsService;
    private readonly IAuditLogService auditLogService;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<BackupsControl> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly TextBox pathTextBox = new();
    private readonly TextBox companyNameTextBox = new();
    private readonly TextBox companyPhoneTextBox = new();
    private readonly TextBox companyAddressTextBox = new();
    private readonly TextBox settingKeyTextBox = new();
    private readonly TextBox settingValueTextBox = new();
    private readonly TextBox settingTypeTextBox = new();
    private readonly ListBox auditListBox = new();
    private readonly Label messageLabel = new();
    private bool initialized;

    public BackupsControl(
        IBackupService backupService,
        IAdministrationSettingsService settingsService,
        IAuditLogService auditLogService,
        IBackgroundExecutor backgroundExecutor,
        ILogger<BackupsControl> logger)
    {
        this.backupService = backupService;
        this.settingsService = settingsService;
        this.auditLogService = auditLogService;
        this.backgroundExecutor = backgroundExecutor;
        this.logger = logger;
        InitializeComponent();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        await LoadCompanyProfileAsync(cancellationToken);
        await LoadAuditAsync(cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            lifetime.Cancel();
            lifetime.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        Dock = DockStyle.Fill;
        RightToLeft = RightToLeft.Yes;

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            RightToLeft = RightToLeft.Yes,
            RightToLeftLayout = true,
        };
        tabs.TabPages.Add(CreateBackupPage());
        tabs.TabPages.Add(CreateSettingsPage());
        tabs.TabPages.Add(CreateAuditPage());

        messageLabel.Dock = DockStyle.Bottom;
        messageLabel.Height = 36;
        messageLabel.Padding = new Padding(16, 8, 16, 8);
        messageLabel.ForeColor = Color.DimGray;

        Controls.Add(tabs);
        Controls.Add(messageLabel);
    }

    private TabPage CreateBackupPage()
    {
        var page = CreatePage("پشتیبان‌گیری", out var layout);
        pathTextBox.PlaceholderText = "مسیر پوشه مقصد یا فایل پشتیبان برای بازیابی";
        AddField(layout, 0, "مسیر", pathTextBox);
        AddActions(page, [
            ("پشتیبان دستی", CreateBackupAsync),
            ("بازیابی", RestoreBackupAsync),
        ]);
        return page;
    }

    private TabPage CreateSettingsPage()
    {
        var page = CreatePage("تنظیمات", out var layout);
        AddField(layout, 0, "نام شرکت", companyNameTextBox);
        AddField(layout, 1, "تلفن", companyPhoneTextBox);
        companyAddressTextBox.Multiline = true;
        companyAddressTextBox.Height = 72;
        AddField(layout, 2, "آدرس", companyAddressTextBox);
        AddField(layout, 3, "کلید تنظیم", settingKeyTextBox);
        AddField(layout, 4, "مقدار", settingValueTextBox);
        settingTypeTextBox.Text = "String";
        AddField(layout, 5, "نوع مقدار", settingTypeTextBox);
        AddActions(page, [
            ("ذخیره شرکت", SaveCompanyAsync),
            ("ذخیره تنظیم", SaveSettingAsync),
        ]);
        return page;
    }

    private TabPage CreateAuditPage()
    {
        var page = new TabPage("ممیزی") { Padding = new Padding(16) };
        auditListBox.Dock = DockStyle.Fill;
        page.Controls.Add(auditListBox);
        AddActions(page, [("بازخوانی", RefreshAuditAsync)]);
        return page;
    }

    private static TabPage CreatePage(string title, out TableLayoutPanel layout)
    {
        var page = new TabPage(title) { Padding = new Padding(16) };
        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        page.Controls.Add(layout);
        return page;
    }

    private static void AddField(TableLayoutPanel layout, int row, string labelText, Control input)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, input is TextBox { Multiline: true } ? 96 : 48));
        layout.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = labelText,
            TextAlign = ContentAlignment.MiddleRight,
        }, 0, row);
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(8);
        layout.Controls.Add(input, 1, row);
    }

    private static void AddActions(TabPage page, IReadOnlyList<(string Text, Func<Task> Action)> actions)
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(16, 8, 16, 8),
        };
        foreach (var action in actions)
        {
            var button = new Button { AutoSize = true, Text = action.Text };
            button.Click += async (_, _) => await action.Action();
            panel.Controls.Add(button);
        }

        page.Controls.Add(panel);
    }

    private async Task CreateBackupAsync() =>
        await RunAsync(async token =>
        {
            var result = await backupService.CreateAsync(new BackupRequest(BackupType.Manual, pathTextBox.Text), token);
            ShowResult(result.IsSuccess, result.IsSuccess ? $"پشتیبان ساخته شد: {result.BackupFilePath}" : result.UserMessage);
        });

    private async Task RestoreBackupAsync() =>
        await RunAsync(async token =>
        {
            var result = await backupService.RestoreAsync(new RestoreRequest(pathTextBox.Text), token);
            ShowResult(result.IsSuccess, result.IsSuccess ? "بازیابی انجام شد." : result.UserMessage);
        });

    private async Task SaveCompanyAsync() =>
        await RunAsync(async token =>
        {
            var result = await settingsService.SaveCompanyProfileAsync(
                new CompanyProfileDto(companyNameTextBox.Text, null, null, companyPhoneTextBox.Text, companyAddressTextBox.Text, null),
                token);
            ShowResult(result.IsSuccess, result.IsSuccess ? "تنظیمات شرکت ذخیره شد." : result.UserMessage);
        });

    private async Task SaveSettingAsync() =>
        await RunAsync(async token =>
        {
            var result = await settingsService.SaveSettingAsync(settingKeyTextBox.Text, settingValueTextBox.Text, settingTypeTextBox.Text, token);
            ShowResult(result.IsSuccess, result.IsSuccess ? "تنظیم ذخیره شد." : result.UserMessage);
        });

    private async Task RefreshAuditAsync() =>
        await RunAsync(async token =>
        {
            await LoadAuditAsync(token);
            ShowResult(true, "ممیزی بازخوانی شد.");
        });

    private async Task LoadCompanyProfileAsync(CancellationToken cancellationToken)
    {
        var profile = await backgroundExecutor.ExecuteAsync(
            token => settingsService.GetCompanyProfileAsync(token),
            cancellationToken);
        if (profile is null)
        {
            return;
        }

        companyNameTextBox.Text = profile.CompanyName;
        companyPhoneTextBox.Text = profile.PhoneNumber;
        companyAddressTextBox.Text = profile.Address;
    }

    private async Task LoadAuditAsync(CancellationToken cancellationToken)
    {
        var rows = await backgroundExecutor.ExecuteAsync(
            token => auditLogService.GetRecentAsync(100, token),
            cancellationToken);
        auditListBox.Items.Clear();
        foreach (var row in rows)
        {
            auditListBox.Items.Add($"{row.CreatedAtUtc:u} | {row.EntityType} #{row.EntityId} | {row.Action} | {row.Description}");
        }
    }

    private async Task RunAsync(Func<CancellationToken, Task> operation)
    {
        messageLabel.ForeColor = Color.DimGray;
        messageLabel.Text = "در حال انجام...";
        try
        {
            await operation(lifetime.Token);
        }
        catch (OperationCanceledException) when (lifetime.IsCancellationRequested)
        {
            messageLabel.Text = string.Empty;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Administration operation failed");
            ShowResult(false, "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.");
        }
    }

    private void ShowResult(bool success, string message)
    {
        messageLabel.ForeColor = success ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = message;
    }
}
