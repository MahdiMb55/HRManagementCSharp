using HRManagement.Application.Abstractions;
using HRManagement.Application.PersonnelRecords;
using HRManagement.Domain.Enums;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class PersonnelRecordsForm : Form
{
    private readonly long employeeId;
    private readonly IPersonnelRecordService personnelRecordService;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<PersonnelRecordsForm> logger;
    private readonly CancellationTokenSource lifetime = new();
    private readonly Label messageLabel = new();
    private bool disposed;

    private readonly ComboBox educationDegreeComboBox = new();
    private readonly TextBox educationFieldTextBox = new();
    private readonly TextBox educationInstitutionTextBox = new();
    private readonly NumericUpDown educationGraduationYearInput = new();
    private readonly CheckBox educationPrimaryCheckBox = new();
    private readonly TextBox educationNotesTextBox = new();

    private readonly TextBox dependentFirstNameTextBox = new();
    private readonly TextBox dependentLastNameTextBox = new();
    private readonly TextBox dependentNationalCodeTextBox = new();
    private readonly ComboBox dependentGenderComboBox = new();
    private readonly TextBox dependentBirthDateTextBox = new();
    private readonly ComboBox dependentRelationshipComboBox = new();
    private readonly ComboBox dependentEducationComboBox = new();
    private readonly ComboBox dependentInsuranceComboBox = new();
    private readonly TextBox dependentNotesTextBox = new();

    private readonly TextBox bankNameTextBox = new();
    private readonly TextBox bankAccountTextBox = new();
    private readonly TextBox bankCardTextBox = new();
    private readonly TextBox bankIbanTextBox = new();
    private readonly CheckBox bankPrimaryCheckBox = new();
    private readonly TextBox bankNotesTextBox = new();

    private readonly TextBox accessCardNumberTextBox = new();
    private readonly TextBox accessCardStartDateTextBox = new();
    private readonly TextBox accessCardNotesTextBox = new();

    public PersonnelRecordsForm(
        long employeeId,
        IPersonnelRecordService personnelRecordService,
        IPersianDateAdapter dateAdapter,
        IBackgroundExecutor backgroundExecutor,
        ILogger<PersonnelRecordsForm>? logger = null)
    {
        this.employeeId = employeeId;
        this.personnelRecordService = personnelRecordService;
        this.dateAdapter = dateAdapter;
        this.backgroundExecutor = backgroundExecutor;
        this.logger = logger ?? NullLogger<PersonnelRecordsForm>.Instance;

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
        Text = "سوابق پرسنلی";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(720, 560);
        Size = new Size(820, 640);

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            RightToLeft = RightToLeft.Yes,
            RightToLeftLayout = true,
        };
        tabs.TabPages.Add(CreateEducationPage());
        tabs.TabPages.Add(CreateDependentPage());
        tabs.TabPages.Add(CreateBankPage());
        tabs.TabPages.Add(CreateAccessCardPage());

        messageLabel.Dock = DockStyle.Bottom;
        messageLabel.AutoSize = false;
        messageLabel.Height = 36;
        messageLabel.Padding = new Padding(16, 8, 16, 8);
        messageLabel.ForeColor = Color.DimGray;

        Controls.Add(tabs);
        Controls.Add(messageLabel);
    }

    private TabPage CreateEducationPage()
    {
        educationDegreeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        educationDegreeComboBox.DataSource = Enum.GetValues<EducationDegree>();
        educationGraduationYearInput.Minimum = 0;
        educationGraduationYearInput.Maximum = 1600;
        educationGraduationYearInput.Value = 0;
        educationNotesTextBox.Multiline = true;
        educationNotesTextBox.Height = 72;

        var page = CreatePage("تحصیلات", out var layout);
        AddField(layout, 0, "مدرک", educationDegreeComboBox);
        AddField(layout, 1, "رشته", educationFieldTextBox);
        AddField(layout, 2, "مؤسسه", educationInstitutionTextBox);
        AddField(layout, 3, "سال فارغ‌التحصیلی", educationGraduationYearInput);
        AddField(layout, 4, string.Empty, educationPrimaryCheckBox);
        AddField(layout, 5, "یادداشت", educationNotesTextBox);
        educationPrimaryCheckBox.Text = "رکورد اصلی";
        AddAction(page, "ثبت تحصیلات", SaveEducationAsync);
        return page;
    }

    private TabPage CreateDependentPage()
    {
        dependentGenderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        dependentGenderComboBox.DataSource = Enum.GetValues<Gender>();
        dependentRelationshipComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        dependentRelationshipComboBox.DataSource = Enum.GetValues<RelationshipType>();
        dependentEducationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        dependentEducationComboBox.DataSource = Enum.GetValues<DependentEducationStatus>();
        dependentInsuranceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        dependentInsuranceComboBox.DataSource = Enum.GetValues<DependentInsuranceStatus>();
        dependentBirthDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        dependentNotesTextBox.Multiline = true;
        dependentNotesTextBox.Height = 72;

        var page = CreatePage("افراد تحت تکفل", out var layout);
        AddField(layout, 0, "نام", dependentFirstNameTextBox);
        AddField(layout, 1, "نام خانوادگی", dependentLastNameTextBox);
        AddField(layout, 2, "کد ملی", dependentNationalCodeTextBox);
        AddField(layout, 3, "جنسیت", dependentGenderComboBox);
        AddField(layout, 4, "تاریخ تولد", dependentBirthDateTextBox);
        AddField(layout, 5, "نسبت", dependentRelationshipComboBox);
        AddField(layout, 6, "وضعیت تحصیل", dependentEducationComboBox);
        AddField(layout, 7, "وضعیت بیمه", dependentInsuranceComboBox);
        AddField(layout, 8, "یادداشت", dependentNotesTextBox);
        AddAction(page, "ثبت فرد تحت تکفل", SaveDependentAsync);
        return page;
    }

    private TabPage CreateBankPage()
    {
        bankNotesTextBox.Multiline = true;
        bankNotesTextBox.Height = 72;
        var page = CreatePage("حساب بانکی", out var layout);
        AddField(layout, 0, "نام بانک", bankNameTextBox);
        AddField(layout, 1, "شماره حساب", bankAccountTextBox);
        AddField(layout, 2, "شماره کارت", bankCardTextBox);
        AddField(layout, 3, "شبا", bankIbanTextBox);
        AddField(layout, 4, string.Empty, bankPrimaryCheckBox);
        AddField(layout, 5, "یادداشت", bankNotesTextBox);
        bankPrimaryCheckBox.Text = "حساب اصلی حقوق";
        AddAction(page, "ثبت حساب", SaveBankAsync);
        return page;
    }

    private TabPage CreateAccessCardPage()
    {
        accessCardStartDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        accessCardNotesTextBox.Multiline = true;
        accessCardNotesTextBox.Height = 72;
        var page = CreatePage("کارت تردد", out var layout);
        AddField(layout, 0, "شماره کارت", accessCardNumberTextBox);
        AddField(layout, 1, "تاریخ شروع", accessCardStartDateTextBox);
        AddField(layout, 2, "یادداشت", accessCardNotesTextBox);
        AddAction(page, "صدور کارت", SaveAccessCardAsync);
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
            logger.LogError(exception, "Personnel record operation failed");
            messageLabel.ForeColor = Color.Firebrick;
            messageLabel.Text = "عملیات انجام نشد. جزئیات در گزارش برنامه ثبت شد.";
        }
    }

    private async Task SaveEducationAsync()
    {
        var request = new AddEducationRecordRequest(
            employeeId,
            (EducationDegree)educationDegreeComboBox.SelectedItem!,
            educationFieldTextBox.Text,
            educationInstitutionTextBox.Text,
            educationGraduationYearInput.Value <= 0 ? null : (int)educationGraduationYearInput.Value,
            educationPrimaryCheckBox.Checked,
            educationNotesTextBox.Text);
        var result = await ExecuteAsync(token => personnelRecordService.AddEducationRecordAsync(request, token));
        ShowResult(result, "سابقه تحصیلی ثبت شد.");
    }

    private async Task SaveDependentAsync()
    {
        if (!TryReadOptionalDate(dependentBirthDateTextBox, out var birthDate))
        {
            return;
        }

        var request = new AddDependentRequest(
            employeeId,
            dependentFirstNameTextBox.Text,
            dependentLastNameTextBox.Text,
            dependentNationalCodeTextBox.Text,
            (Gender)dependentGenderComboBox.SelectedItem!,
            birthDate,
            (RelationshipType)dependentRelationshipComboBox.SelectedItem!,
            (DependentEducationStatus)dependentEducationComboBox.SelectedItem!,
            (DependentInsuranceStatus)dependentInsuranceComboBox.SelectedItem!,
            dependentNotesTextBox.Text);
        var result = await ExecuteAsync(token => personnelRecordService.AddDependentAsync(request, token));
        ShowResult(result, "فرد تحت تکفل ثبت شد.");
    }

    private async Task SaveBankAsync()
    {
        var request = new AddBankAccountRequest(
            employeeId,
            bankNameTextBox.Text,
            bankAccountTextBox.Text,
            bankCardTextBox.Text,
            bankIbanTextBox.Text,
            bankPrimaryCheckBox.Checked,
            bankNotesTextBox.Text);
        var result = await ExecuteAsync(token => personnelRecordService.AddBankAccountAsync(request, token));
        ShowResult(result, "حساب بانکی ثبت شد.");
    }

    private async Task SaveAccessCardAsync()
    {
        if (!TryReadRequiredDate(accessCardStartDateTextBox, out var startDate))
        {
            return;
        }

        var request = new IssueAccessCardRequest(
            employeeId,
            accessCardNumberTextBox.Text,
            startDate,
            accessCardNotesTextBox.Text);
        var result = await ExecuteAsync(token => personnelRecordService.IssueAccessCardAsync(request, token));
        ShowResult(result, "کارت تردد ثبت شد.");
    }

    private Task<PersonnelRecordResult> ExecuteAsync(
        Func<CancellationToken, Task<PersonnelRecordResult>> operation) =>
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

    private void ShowResult(PersonnelRecordResult result, string successMessage)
    {
        messageLabel.ForeColor = result.IsSuccess ? Color.DarkGreen : Color.Firebrick;
        messageLabel.Text = result.IsSuccess ? successMessage : result.UserMessage;
    }
}
