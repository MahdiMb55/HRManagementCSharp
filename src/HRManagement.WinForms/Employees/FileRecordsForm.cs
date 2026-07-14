using HRManagement.Application.Abstractions;
using HRManagement.Application.Files;
using HRManagement.Domain.Enums;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class FileRecordsForm : Form
{
    private readonly long employeeId;
    private readonly IEmployeeFileService fileService;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<FileRecordsForm> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly Label messageLabel = new();
    private bool disposed;

    private readonly TextBox contractNumberTextBox = new();
    private readonly ComboBox contractTypeComboBox = new();
    private readonly TextBox contractStartDateTextBox = new();
    private readonly TextBox contractEndDateTextBox = new();
    private readonly TextBox contractAttachmentPathTextBox = new();
    private readonly TextBox contractNotesTextBox = new();

    private readonly NumericUpDown documentCategoryIdInput = new();
    private readonly TextBox documentTitleTextBox = new();
    private readonly TextBox documentDescriptionTextBox = new();
    private readonly TextBox documentPathTextBox = new();

    private readonly TextBox photoPathTextBox = new();

    public FileRecordsForm(
        long employeeId,
        IEmployeeFileService fileService,
        IPersianDateAdapter dateAdapter,
        IBackgroundExecutor backgroundExecutor,
        ILogger<FileRecordsForm>? logger = null)
    {
        this.employeeId = employeeId;
        this.fileService = fileService;
        this.dateAdapter = dateAdapter;
        this.backgroundExecutor = backgroundExecutor;
        this.logger = logger ?? NullLogger<FileRecordsForm>.Instance;

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
        Text = "قرارداد و فایل‌ها";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 560);
        Size = new Size(860, 640);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            RightToLeft = RightToLeft.Yes,
            RightToLeftLayout = true,
        };
        tabs.TabPages.Add(CreateContractPage());
        tabs.TabPages.Add(CreateDocumentPage());
        tabs.TabPages.Add(CreatePhotoPage());

        messageLabel.Dock = DockStyle.Bottom;
        messageLabel.AutoSize = false;
        messageLabel.Height = 36;
        messageLabel.Padding = new Padding(16, 8, 16, 8);

        Controls.Add(tabs);
        Controls.Add(messageLabel);
    }

    private TabPage CreateContractPage()
    {
        contractTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        contractTypeComboBox.DataSource = Enum.GetValues<ContractType>();
        contractStartDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        contractEndDateTextBox.PlaceholderText = "اختیاری";
        contractNotesTextBox.Multiline = true;
        contractNotesTextBox.Height = 72;

        var page = CreatePage("قرارداد", out var layout);
        AddField(layout, 0, "شماره قرارداد", contractNumberTextBox);
        AddField(layout, 1, "نوع قرارداد", contractTypeComboBox);
        AddField(layout, 2, "تاریخ شروع", contractStartDateTextBox);
        AddField(layout, 3, "تاریخ پایان", contractEndDateTextBox);
        AddField(layout, 4, "مسیر پیوست", contractAttachmentPathTextBox);
        AddField(layout, 5, "یادداشت", contractNotesTextBox);
        AddAction(page, "ثبت قرارداد", SaveContractAsync);
        return page;
    }

    private TabPage CreateDocumentPage()
    {
        ConfigureIdInput(documentCategoryIdInput);
        documentDescriptionTextBox.Multiline = true;
        documentDescriptionTextBox.Height = 72;

        var page = CreatePage("اسناد", out var layout);
        AddField(layout, 0, "شناسه دسته سند", documentCategoryIdInput);
        AddField(layout, 1, "عنوان", documentTitleTextBox);
        AddField(layout, 2, "مسیر فایل", documentPathTextBox);
        AddField(layout, 3, "توضیح", documentDescriptionTextBox);
        AddAction(page, "ثبت سند", SaveDocumentAsync);
        return page;
    }

    private TabPage CreatePhotoPage()
    {
        var page = CreatePage("تصویر پروفایل", out var layout);
        AddField(layout, 0, "مسیر تصویر", photoPathTextBox);
        AddAction(page, "ثبت تصویر", SavePhotoAsync);
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        page.Controls.Add(layout);
        return page;
    }

    private static void AddField(TableLayoutPanel layout, int row, string labelText, Control input)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, input is TextBox { Multiline: true } ? 96 : 48));
        var label = new Label
        {
            Dock = DockStyle.Fill,
            Text = labelText,
            TextAlign = ContentAlignment.MiddleRight,
        };
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(8);
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(input, 1, row);
    }

    private static void ConfigureIdInput(NumericUpDown input)
    {
        input.Minimum = 1;
        input.Maximum = long.MaxValue;
        input.Dock = DockStyle.Fill;
    }

    private void AddAction(TabPage page, string text, Func<Task> action)
    {
        var button = new Button
        {
            AutoSize = true,
            Dock = DockStyle.Bottom,
            Text = text,
        };
        button.Click += async (_, _) => await RunActionAsync(action);
        page.Controls.Add(button);
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
            logger.LogError(exception, "File record operation failed");
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.";
        }
    }

    private async Task SaveContractAsync()
    {
        if (!TryReadRequiredDate(contractStartDateTextBox, out var startDate) ||
            !TryReadOptionalDate(contractEndDateTextBox, out var endDate))
        {
            return;
        }

        var request = new CreateContractRequest(
            employeeId,
            contractNumberTextBox.Text,
            (ContractType)contractTypeComboBox.SelectedItem!,
            startDate,
            endDate,
            contractNotesTextBox.Text,
            contractAttachmentPathTextBox.Text);
        var result = await ExecuteAsync(token => fileService.CreateContractAsync(request, token));
        ShowResult(result, "قرارداد ثبت شد.");
    }

    private async Task SaveDocumentAsync()
    {
        var request = new AddEmployeeDocumentRequest(
            employeeId,
            (long)documentCategoryIdInput.Value,
            documentTitleTextBox.Text,
            documentDescriptionTextBox.Text,
            documentPathTextBox.Text);
        var result = await ExecuteAsync(token => fileService.AddEmployeeDocumentAsync(request, token));
        ShowResult(result, "سند ثبت شد.");
    }

    private async Task SavePhotoAsync()
    {
        var request = new SetProfilePhotoRequest(employeeId, photoPathTextBox.Text);
        var result = await ExecuteAsync(token => fileService.SetProfilePhotoAsync(request, token));
        ShowResult(result, "تصویر پروفایل ثبت شد.");
    }

    private Task<FileRecordResult> ExecuteAsync(Func<CancellationToken, Task<FileRecordResult>> operation) =>
        backgroundExecutor.ExecuteAsync(operation, lifetime.Token);

    private bool TryReadRequiredDate(TextBox input, out DateOnly date)
    {
        if (dateAdapter.TryParse(input.Text, out date))
        {
            return true;
        }

        messageLabel.ForeColor = Color.Firebrick;
        messageLabel.Text = "تاریخ واردشده معتبر نیست.";
        input.Focus();
        return false;
    }

    private bool TryReadOptionalDate(TextBox input, out DateOnly? date)
    {
        date = null;
        if (string.IsNullOrWhiteSpace(input.Text))
        {
            return true;
        }

        if (dateAdapter.TryParse(input.Text, out var parsed))
        {
            date = parsed;
            return true;
        }

        messageLabel.ForeColor = Color.Firebrick;
        messageLabel.Text = "تاریخ واردشده معتبر نیست.";
        input.Focus();
        return false;
    }

    private void ShowResult(FileRecordResult result, string successMessage)
    {
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? successMessage : result.UserMessage;
    }
}
