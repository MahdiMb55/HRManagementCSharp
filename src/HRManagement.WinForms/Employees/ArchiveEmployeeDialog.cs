using HRManagement.Application.Abstractions;
using HRManagement.Application.Archive;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class ArchiveEmployeeDialog : Form
{
    private readonly long employeeId;
    private readonly string personnelNumber;
    private readonly IEmployeeArchiveService archiveService;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<ArchiveEmployeeDialog> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly TextBox confirmationTextBox = new();
    private readonly Label messageLabel = new();
    private bool disposed;

    public ArchiveEmployeeDialog(
        long employeeId,
        string personnelNumber,
        IEmployeeArchiveService archiveService,
        IBackgroundExecutor backgroundExecutor,
        ILogger<ArchiveEmployeeDialog>? logger = null)
    {
        this.employeeId = employeeId;
        this.personnelNumber = personnelNumber;
        this.archiveService = archiveService;
        this.backgroundExecutor = backgroundExecutor;
        this.logger = logger ?? NullLogger<ArchiveEmployeeDialog>.Instance;

        InitializeComponent();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            Close();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
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
            lifetime.Cancel();
            lifetime.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        Text = "بایگانی و حذف";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(520, 300);
        Size = new Size(600, 340);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            Padding = new Padding(20),
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            AutoSize = true,
            Text = $"شماره پرسنلی: {personnelNumber}",
        }, 0, 0);
        confirmationTextBox.PlaceholderText = "برای حذف دائمی شماره پرسنلی را دقیقاً وارد کنید";
        layout.Controls.Add(confirmationTextBox, 0, 1);
        messageLabel.AutoSize = true;
        messageLabel.ForeColor = Color.DimGray;
        layout.Controls.Add(messageLabel, 0, 2);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(20, 8, 20, 8),
        };
        actions.Controls.Add(CreateActionButton("بایگانی", ArchiveAsync));
        actions.Controls.Add(CreateActionButton("بازگردانی", RestoreAsync));
        actions.Controls.Add(CreateActionButton("حذف دائمی", DeletePermanentlyAsync));

        Controls.Add(layout);
        Controls.Add(actions);
    }

    private Button CreateActionButton(string text, Func<Task> action)
    {
        var button = new Button
        {
            AutoSize = true,
            Text = text,
        };
        button.Click += async (_, _) => await RunActionAsync(action);
        return button;
    }

    private async Task RunActionAsync(Func<Task> action)
    {
        messageLabel.ForeColor = Color.DimGray;
        messageLabel.Text = "در حال انجام...";
        try
        {
            await action();
        }
        catch (OperationCanceledException) when (lifetime.IsCancellationRequested)
        {
            messageLabel.Text = string.Empty;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Archive operation failed");
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.";
        }
    }

    private async Task ArchiveAsync()
    {
        var result = await backgroundExecutor.ExecuteAsync(
            token => archiveService.ArchiveAsync(employeeId, token),
            lifetime.Token);
        ShowResult(result, "کارمند بایگانی شد.");
    }

    private async Task RestoreAsync()
    {
        var result = await backgroundExecutor.ExecuteAsync(
            token => archiveService.RestoreAsync(employeeId, token),
            lifetime.Token);
        ShowResult(result, "کارمند از بایگانی بازگردانده شد.");
    }

    private async Task DeletePermanentlyAsync()
    {
        var result = await backgroundExecutor.ExecuteAsync(
            token => archiveService.DeletePermanentlyAsync(employeeId, confirmationTextBox.Text, token),
            lifetime.Token);
        ShowResult(result, "کارمند به‌صورت دائمی حذف شد.");
    }

    private void ShowResult(ArchiveResult result, string successMessage)
    {
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? successMessage : result.UserMessage;
    }
}
