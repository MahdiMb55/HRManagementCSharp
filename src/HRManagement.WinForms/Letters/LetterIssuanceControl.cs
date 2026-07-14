using HRManagement.Application.Abstractions;
using HRManagement.Application.Letters;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Letters;

public sealed class LetterIssuanceControl : UserControl
{
    private readonly ILetterService letterService;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<LetterIssuanceControl> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly ComboBox templatesComboBox = new();
    private readonly TextBox templateTitleTextBox = new();
    private readonly TextBox templateDescriptionTextBox = new();
    private readonly TextBox templatePathTextBox = new();
    private readonly TextBox employeeIdsTextBox = new();
    private readonly TextBox letterNumberTextBox = new();
    private readonly TextBox issueDateTextBox = new();
    private readonly TextBox subjectTextBox = new();
    private readonly Label messageLabel = new();
    private bool initialized;

    public LetterIssuanceControl(
        ILetterService letterService,
        IPersianDateAdapter dateAdapter,
        IBackgroundExecutor backgroundExecutor,
        ILogger<LetterIssuanceControl> logger)
    {
        this.letterService = letterService;
        this.dateAdapter = dateAdapter;
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
        await RefreshTemplatesAsync(cancellationToken);
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
        tabs.TabPages.Add(CreateTemplatePage());
        tabs.TabPages.Add(CreateIssuePage());
        messageLabel.Dock = DockStyle.Bottom;
        messageLabel.Height = 36;
        messageLabel.Padding = new Padding(16, 8, 16, 8);
        Controls.Add(tabs);
        Controls.Add(messageLabel);
    }

    private TabPage CreateTemplatePage()
    {
        var page = CreatePage("قالب", out var layout);
        AddField(layout, 0, "عنوان", templateTitleTextBox);
        AddField(layout, 1, "مسیر DOCX", templatePathTextBox);
        templateDescriptionTextBox.Multiline = true;
        templateDescriptionTextBox.Height = 72;
        AddField(layout, 2, "توضیح", templateDescriptionTextBox);
        AddActions(page, [("ثبت قالب", RegisterTemplateAsync)]);
        return page;
    }

    private TabPage CreateIssuePage()
    {
        templatesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        issueDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        employeeIdsTextBox.PlaceholderText = "مثال: 1,2,3";
        var page = CreatePage("صدور", out var layout);
        AddField(layout, 0, "قالب", templatesComboBox);
        AddField(layout, 1, "شناسه کارکنان", employeeIdsTextBox);
        AddField(layout, 2, "شماره نامه", letterNumberTextBox);
        AddField(layout, 3, "تاریخ نامه", issueDateTextBox);
        AddField(layout, 4, "موضوع", subjectTextBox);
        AddActions(page, [("صدور نامه", IssueAsync), ("بازخوانی قالب‌ها", RefreshTemplatesButtonAsync)]);
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

    private async Task RegisterTemplateAsync() =>
        await RunAsync(async token =>
        {
            var result = await letterService.RegisterTemplateAsync(
                new RegisterLetterTemplateRequest(templateTitleTextBox.Text, templateDescriptionTextBox.Text, templatePathTextBox.Text),
                token);
            ShowResult(result.IsSuccess, result.IsSuccess ? "قالب ثبت شد." : result.UserMessage);
            await RefreshTemplatesAsync(token);
        });

    private async Task IssueAsync() =>
        await RunAsync(async token =>
        {
            if (templatesComboBox.SelectedItem is not LetterTemplateDto template)
            {
                ShowResult(false, "ابتدا قالب را انتخاب کنید.");
                return;
            }

            if (!dateAdapter.TryParse(issueDateTextBox.Text, out var issueDate))
            {
                ShowResult(false, "تاریخ نامه معتبر نیست.");
                return;
            }

            var employeeIds = employeeIdsTextBox.Text
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => long.TryParse(value, out var id) ? id : 0)
                .Where(id => id > 0)
                .ToArray();
            var result = await letterService.IssueAsync(
                new IssueLetterRequest(template.TemplateId, employeeIds, letterNumberTextBox.Text, issueDate, subjectTextBox.Text),
                token);
            ShowResult(result.IsSuccess, result.IsSuccess ? $"{result.OutputPaths.Count} نامه صادر شد." : result.UserMessage);
        });

    private async Task RefreshTemplatesButtonAsync() =>
        await RunAsync(async token =>
        {
            await RefreshTemplatesAsync(token);
            ShowResult(true, "قالب‌ها بازخوانی شدند.");
        });

    private async Task RefreshTemplatesAsync(CancellationToken cancellationToken)
    {
        var templates = await backgroundExecutor.ExecuteAsync(
            token => letterService.GetTemplatesAsync(token),
            cancellationToken);
        templatesComboBox.DataSource = templates.ToList();
        templatesComboBox.DisplayMember = nameof(LetterTemplateDto.Title);
        templatesComboBox.ValueMember = nameof(LetterTemplateDto.TemplateId);
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
            logger.LogError(exception, "Letter operation failed");
            ShowResult(false, "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.");
        }
    }

    private void ShowResult(bool success, string message)
    {
        messageLabel.ForeColor = success ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = message;
    }
}
