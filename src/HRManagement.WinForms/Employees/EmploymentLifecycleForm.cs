using HRManagement.Application.Abstractions;
using HRManagement.Application.Employment;
using HRManagement.Domain.Enums;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class EmploymentLifecycleForm : Form
{
    private readonly long employeeId;
    private readonly IEmploymentLifecycleService lifecycleService;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<EmploymentLifecycleForm> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly TextBox effectiveDateTextBox = new();
    private readonly ComboBox statusComboBox = new();
    private readonly ComboBox terminationTypeComboBox = new();
    private readonly TextBox reasonTextBox = new();
    private readonly TextBox notesTextBox = new();
    private readonly Label messageLabel = new();
    private bool disposed;

    public EmploymentLifecycleForm(
        long employeeId,
        IEmploymentLifecycleService lifecycleService,
        IPersianDateAdapter dateAdapter,
        IBackgroundExecutor backgroundExecutor,
        ILogger<EmploymentLifecycleForm>? logger = null)
    {
        this.employeeId = employeeId;
        this.lifecycleService = lifecycleService;
        this.dateAdapter = dateAdapter;
        this.backgroundExecutor = backgroundExecutor;
        this.logger = logger ?? NullLogger<EmploymentLifecycleForm>.Instance;

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
        Text = "سوابق استخدام";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(560, 420);
        Size = new Size(640, 460);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(20),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        effectiveDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        statusComboBox.DataSource = Enum.GetValues<EmploymentStatus>();
        terminationTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        terminationTypeComboBox.DataSource = Enum.GetValues<TerminationType>();
        reasonTextBox.MaxLength = 500;
        notesTextBox.Multiline = true;
        notesTextBox.Height = 72;
        messageLabel.AutoSize = true;
        messageLabel.ForeColor = Color.DimGray;

        AddField(layout, 0, "تاریخ اثر", effectiveDateTextBox);
        AddField(layout, 1, "وضعیت", statusComboBox);
        AddField(layout, 2, "نوع خاتمه", terminationTypeComboBox);
        AddField(layout, 3, "دلیل", reasonTextBox);
        AddField(layout, 4, "یادداشت", notesTextBox);
        AddField(layout, 5, string.Empty, messageLabel);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(20, 8, 20, 8),
        };
        var startButton = CreateActionButton("شروع استخدام", StartEmploymentAsync);
        var statusButton = CreateActionButton("تغییر وضعیت", ChangeStatusAsync);
        var terminateButton = CreateActionButton("خاتمه همکاری", TerminateAsync);
        var returnButton = CreateActionButton("بازگشت به کار", ReturnToWorkAsync);
        actions.Controls.Add(startButton);
        actions.Controls.Add(statusButton);
        actions.Controls.Add(terminateButton);
        actions.Controls.Add(returnButton);

        Controls.Add(layout);
        Controls.Add(actions);
    }

    private static void AddField(TableLayoutPanel layout, int row, string labelText, Control input)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, row == 4 ? 96 : 48));
        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
        };
        input.Dock = DockStyle.Fill;
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(input, 1, row);
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
            logger.LogError(exception, "Employment lifecycle operation failed");
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.";
        }
    }

    private async Task StartEmploymentAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var result = await backgroundExecutor.ExecuteAsync(
            token => lifecycleService.StartEmploymentAsync(employeeId, date, notesTextBox.Text, token),
            lifetime.Token);
        ShowResult(result, "دوره استخدام ثبت شد.");
    }

    private async Task ChangeStatusAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var status = (EmploymentStatus)statusComboBox.SelectedItem!;
        var result = await backgroundExecutor.ExecuteAsync(
            token => lifecycleService.ChangeStatusAsync(employeeId, status, date, notesTextBox.Text, token),
            lifetime.Token);
        ShowResult(result, "وضعیت استخدام تغییر کرد.");
    }

    private async Task TerminateAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var terminationType = (TerminationType)terminationTypeComboBox.SelectedItem!;
        var result = await backgroundExecutor.ExecuteAsync(
            token => lifecycleService.TerminateAsync(employeeId, date, terminationType, reasonTextBox.Text, notesTextBox.Text, token),
            lifetime.Token);
        ShowResult(result, "خاتمه همکاری ثبت شد.");
    }

    private async Task ReturnToWorkAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var result = await backgroundExecutor.ExecuteAsync(
            token => lifecycleService.ReturnToWorkAsync(employeeId, date, notesTextBox.Text, token),
            lifetime.Token);
        ShowResult(result, "بازگشت به کار ثبت شد.");
    }

    private bool TryReadDate(out DateOnly date)
    {
        if (!dateAdapter.TryParse(effectiveDateTextBox.Text, out date))
        {
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "تاریخ واردشده معتبر نیست.";
            effectiveDateTextBox.Focus();
            return false;
        }

        return true;
    }

    private void ShowResult(EmploymentLifecycleResult result, string successMessage)
    {
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? successMessage : result.UserMessage;
    }
}
