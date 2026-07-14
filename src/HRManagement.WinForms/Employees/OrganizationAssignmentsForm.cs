using HRManagement.Application.Abstractions;
using HRManagement.Application.Organization;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class OrganizationAssignmentsForm : Form
{
    private readonly long employeeId;
    private readonly IAssignmentService assignmentService;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<OrganizationAssignmentsForm> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly NumericUpDown departmentIdInput = new();
    private readonly NumericUpDown responsibilityIdInput = new();
    private readonly NumericUpDown responsibilityAssignmentIdInput = new();
    private readonly NumericUpDown newPrimaryAssignmentIdInput = new();
    private readonly CheckBox makePrimaryCheckBox = new();
    private readonly TextBox effectiveDateTextBox = new();
    private readonly TextBox notesTextBox = new();
    private readonly Label messageLabel = new();
    private bool disposed;

    public OrganizationAssignmentsForm(
        long employeeId,
        IAssignmentService assignmentService,
        IPersianDateAdapter dateAdapter,
        IBackgroundExecutor backgroundExecutor,
        ILogger<OrganizationAssignmentsForm>? logger = null)
    {
        this.employeeId = employeeId;
        this.assignmentService = assignmentService;
        this.dateAdapter = dateAdapter;
        this.backgroundExecutor = backgroundExecutor;
        this.logger = logger ?? NullLogger<OrganizationAssignmentsForm>.Instance;

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
        Text = "واحد و مسئولیت";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(620, 470);
        Size = new Size(700, 520);

        ConfigureIdInput(departmentIdInput);
        ConfigureIdInput(responsibilityIdInput);
        ConfigureIdInput(responsibilityAssignmentIdInput);
        ConfigureIdInput(newPrimaryAssignmentIdInput);
        effectiveDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        makePrimaryCheckBox.Text = "مسئولیت اصلی";
        notesTextBox.Multiline = true;
        notesTextBox.Height = 72;
        messageLabel.AutoSize = true;
        messageLabel.ForeColor = Color.DimGray;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(20),
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddField(layout, 0, "تاریخ اثر", effectiveDateTextBox);
        AddField(layout, 1, "شناسه واحد", departmentIdInput);
        AddField(layout, 2, "شناسه مسئولیت", responsibilityIdInput);
        AddField(layout, 3, "شناسه انتساب مسئولیت", responsibilityAssignmentIdInput);
        AddField(layout, 4, "شناسه مسئولیت اصلی جدید", newPrimaryAssignmentIdInput);
        AddField(layout, 5, string.Empty, makePrimaryCheckBox);
        AddField(layout, 6, "یادداشت", notesTextBox);
        AddField(layout, 7, string.Empty, messageLabel);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(20, 8, 20, 8),
        };
        actions.Controls.Add(CreateActionButton("ثبت واحد", AssignDepartmentAsync));
        actions.Controls.Add(CreateActionButton("ثبت مسئولیت", AssignResponsibilityAsync));
        actions.Controls.Add(CreateActionButton("پایان مسئولیت", EndResponsibilityAsync));

        Controls.Add(layout);
        Controls.Add(actions);
    }

    private static void ConfigureIdInput(NumericUpDown input)
    {
        input.Minimum = 0;
        input.Maximum = long.MaxValue;
        input.ThousandsSeparator = false;
        input.Dock = DockStyle.Fill;
    }

    private static void AddField(TableLayoutPanel layout, int row, string labelText, Control input)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, row == 6 ? 96 : 48));
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
            logger.LogError(exception, "Organization assignment operation failed");
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.";
        }
    }

    private async Task AssignDepartmentAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var result = await backgroundExecutor.ExecuteAsync(
            token => assignmentService.AssignDepartmentAsync(
                employeeId,
                (long)departmentIdInput.Value,
                date,
                notesTextBox.Text,
                token),
            lifetime.Token);
        ShowResult(result, "واحد سازمانی ثبت شد.");
    }

    private async Task AssignResponsibilityAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var result = await backgroundExecutor.ExecuteAsync(
            token => assignmentService.AssignResponsibilityAsync(
                employeeId,
                (long)responsibilityIdInput.Value,
                date,
                makePrimaryCheckBox.Checked,
                notesTextBox.Text,
                token),
            lifetime.Token);
        ShowResult(result, "مسئولیت ثبت شد.");
    }

    private async Task EndResponsibilityAsync()
    {
        if (!TryReadDate(out var date))
        {
            return;
        }

        var newPrimaryId = newPrimaryAssignmentIdInput.Value <= 0
            ? null
            : (long?)newPrimaryAssignmentIdInput.Value;
        var result = await backgroundExecutor.ExecuteAsync(
            token => assignmentService.EndResponsibilityAsync(
                (long)responsibilityAssignmentIdInput.Value,
                date,
                newPrimaryId,
                token),
            lifetime.Token);
        ShowResult(result, "مسئولیت پایان یافت.");
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

    private void ShowResult(OrganizationResult result, string successMessage)
    {
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? successMessage : result.UserMessage;
    }
}
