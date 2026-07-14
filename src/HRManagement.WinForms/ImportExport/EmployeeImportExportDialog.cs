using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees.Search;
using HRManagement.Application.ImportExport;
using HRManagement.Application.Reports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.ImportExport;

public sealed class EmployeeImportExportDialog : Form
{
    private readonly IEmployeeWorkbookService workbookService;
    private readonly IEmployeeSummaryService summaryService;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly EmployeeFilter filter;
    private readonly EmployeeSort sort;
    private readonly string? query;
    private readonly IReadOnlyCollection<long> selectedEmployeeIds;
    private readonly ILogger<EmployeeImportExportDialog> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly TextBox pathTextBox = new();
    private readonly Label messageLabel = new();
    private bool disposed;

    public EmployeeImportExportDialog(
        IEmployeeWorkbookService workbookService,
        IEmployeeSummaryService summaryService,
        IBackgroundExecutor backgroundExecutor,
        EmployeeFilter filter,
        EmployeeSort sort,
        string? query,
        IReadOnlyCollection<long> selectedEmployeeIds,
        ILogger<EmployeeImportExportDialog>? logger = null)
    {
        this.workbookService = workbookService;
        this.summaryService = summaryService;
        this.backgroundExecutor = backgroundExecutor;
        this.filter = filter;
        this.sort = sort;
        this.query = query;
        this.selectedEmployeeIds = selectedEmployeeIds;
        this.logger = logger ?? NullLogger<EmployeeImportExportDialog>.Instance;

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
        Text = "ورود، خروجی و گزارش";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(640, 300);
        Size = new Size(760, 360);

        pathTextBox.Dock = DockStyle.Top;
        pathTextBox.PlaceholderText = "مسیر فایل خروجی یا ورودی";
        pathTextBox.Margin = new Padding(16);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 58,
            Padding = new Padding(16, 8, 16, 8),
        };
        actions.Controls.Add(CreateActionButton("ساخت قالب CSV", CreateTemplateAsync));
        actions.Controls.Add(CreateActionButton("پیش‌نمایش ورود", PreviewImportAsync));
        actions.Controls.Add(CreateActionButton("خروجی CSV", ExportAsync));
        actions.Controls.Add(CreateActionButton("گزارش HTML", CreateSummaryAsync));

        messageLabel.Dock = DockStyle.Fill;
        messageLabel.Padding = new Padding(16);
        messageLabel.ForeColor = Color.DimGray;
        messageLabel.Text = "برای خروجی CSV یا گزارش HTML مسیر خروجی را وارد کنید. برای پیش‌نمایش ورود مسیر فایل CSV را وارد کنید.";

        Controls.Add(messageLabel);
        Controls.Add(actions);
        Controls.Add(pathTextBox);
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
            logger.LogError(exception, "Import/export operation failed");
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.";
        }
    }

    private async Task CreateTemplateAsync()
    {
        var result = await backgroundExecutor.ExecuteAsync(
            token => workbookService.CreateTemplateAsync(pathTextBox.Text, token),
            lifetime.Token);
        ShowResult(result, "قالب ساخته شد.");
    }

    private async Task PreviewImportAsync()
    {
        var preview = await backgroundExecutor.ExecuteAsync(
            token => workbookService.PreviewImportAsync(pathTextBox.Text, token),
            lifetime.Token);
        messageLabel.ForeColor = preview.Errors.Count == 0 ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = preview.Errors.Count == 0
            ? $"{preview.ValidRows.Count} ردیف معتبر آماده ورود است."
            : $"{preview.Errors.Count} خطا در فایل پیدا شد: {preview.Errors[0].Message}";
    }

    private async Task ExportAsync()
    {
        var request = new EmployeeExportRequest(
            pathTextBox.Text,
            query,
            filter,
            sort,
            selectedEmployeeIds);
        var result = await backgroundExecutor.ExecuteAsync(
            token => workbookService.ExportAsync(request, token),
            lifetime.Token);
        ShowResult(result, $"{result.AffectedRows} ردیف خروجی گرفته شد.");
    }

    private async Task CreateSummaryAsync()
    {
        var request = new EmployeeSummaryRequest(selectedEmployeeIds, pathTextBox.Text);
        var result = await backgroundExecutor.ExecuteAsync(
            token => summaryService.CreateAsync(request, token),
            lifetime.Token);
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? "گزارش HTML ساخته شد." : result.UserMessage;
    }

    private void ShowResult(ImportExportResult result, string successMessage)
    {
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? successMessage : result.UserMessage;
    }
}
