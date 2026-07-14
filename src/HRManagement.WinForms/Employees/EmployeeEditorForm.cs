using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees;
using HRManagement.Application.Employment;
using HRManagement.Application.Organization;
using HRManagement.Application.PersonnelRecords;
using HRManagement.Domain.Enums;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Employees;

public partial class EmployeeEditorForm : Form, IEmployeeEditorView
{
    private readonly EmployeeEditorPresenter presenter;
    private readonly IEmploymentLifecycleService lifecycleService;
    private readonly IAssignmentService assignmentService;
    private readonly IPersonnelRecordService personnelRecordService;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly ILogger<EmploymentLifecycleForm> lifecycleLogger;
    private readonly ILogger<OrganizationAssignmentsForm> organizationLogger;
    private readonly ILogger<PersonnelRecordsForm> personnelRecordsLogger;
    private readonly CancellationTokenSource lifetime = new();
    private long? employeeId;
    private bool isDirty;
    private bool applyingData;
    private bool disposed;

    public EmployeeEditorForm(
        IEmployeeEditorService editorService,
        IEmploymentLifecycleService lifecycleService,
        IAssignmentService assignmentService,
        IPersonnelRecordService personnelRecordService,
        IPersianDateAdapter dateAdapter,
        IBackgroundExecutor backgroundExecutor,
        ILogger<EmployeeEditorPresenter> logger,
        ILogger<EmploymentLifecycleForm> lifecycleLogger,
        ILogger<OrganizationAssignmentsForm> organizationLogger,
        ILogger<PersonnelRecordsForm> personnelRecordsLogger)
    {
        this.lifecycleService = lifecycleService;
        this.assignmentService = assignmentService;
        this.personnelRecordService = personnelRecordService;
        this.dateAdapter = dateAdapter;
        this.backgroundExecutor = backgroundExecutor;
        this.lifecycleLogger = lifecycleLogger;
        this.organizationLogger = organizationLogger;
        this.personnelRecordsLogger = personnelRecordsLogger;
        InitializeComponent();
        presenter = new EmployeeEditorPresenter(this, editorService, logger, backgroundExecutor);
        WireEvents();
    }

    public event EventHandler<long>? EmployeeSaved;

    public long? EmployeeId => employeeId;
    public string FirstName => firstNameTextBox.Text;
    public string LastName => lastNameTextBox.Text;
    public string PersonnelNumber => personnelNumberTextBox.Text;
    public string NationalCode => nationalCodeTextBox.Text;
    public string? FatherName => fatherNameTextBox.Text;
    public Gender Gender => genderComboBox.SelectedIndex == 1 ? Gender.Female : Gender.Male;
    public DateOnly? BirthDate =>
        string.IsNullOrWhiteSpace(birthDateTextBox.Text)
            ? null
            : dateAdapter.TryParse(birthDateTextBox.Text, out var date) ? date : null;
    public string? MobileNumber => mobileTextBox.Text;

    public async Task<bool> TryLoadAsync(long? selectedEmployeeId, CancellationToken cancellationToken)
    {
        if (isDirty && !ConfirmDiscardChanges())
        {
            return false;
        }

        applyingData = true;
        try
        {
            ClearForm();
            employeeId = selectedEmployeeId;
            personnelNumberTextBox.ReadOnly = selectedEmployeeId is not null;
            employmentLifecycleButton.Enabled = selectedEmployeeId is not null;
            organizationAssignmentsButton.Enabled = selectedEmployeeId is not null;
            personnelRecordsButton.Enabled = selectedEmployeeId is not null;
            Text = selectedEmployeeId is null ? "افزودن کارمند" : "ویرایش اطلاعات کارمند";
            if (selectedEmployeeId is long id)
            {
                if (!await presenter.LoadAsync(id, cancellationToken))
                {
                    return false;
                }
            }

            isDirty = false;
            return true;
        }
        finally
        {
            applyingData = false;
        }
    }

    public void Apply(EmployeeEditDto employee)
    {
        employeeId = employee.EmployeeId;
        firstNameTextBox.Text = employee.FirstName;
        lastNameTextBox.Text = employee.LastName;
        personnelNumberTextBox.Text = employee.PersonnelNumber;
        nationalCodeTextBox.Text = employee.NationalCode;
        fatherNameTextBox.Text = employee.FatherName;
        genderComboBox.SelectedIndex = employee.Gender == Gender.Female ? 1 : 0;
        birthDateTextBox.Text = dateAdapter.Format(employee.BirthDate);
        mobileTextBox.Text = employee.MobileNumber;
    }

    public void ClearErrors() => errorProvider.Clear();

    public void SetBusy(bool isBusy)
    {
        saveButton.Enabled = !isBusy;
        fieldsPanel.Enabled = !isBusy;
        busyLabel.Visible = isBusy;
    }

    public void ShowFieldError(EmployeeEditorField field, string message)
    {
        Control control = field switch
        {
            EmployeeEditorField.FirstName => firstNameTextBox,
            EmployeeEditorField.LastName => lastNameTextBox,
            EmployeeEditorField.PersonnelNumber => personnelNumberTextBox,
            EmployeeEditorField.NationalCode => nationalCodeTextBox,
            EmployeeEditorField.BirthDate => birthDateTextBox,
            _ => saveButton,
        };
        errorProvider.SetError(control, message);
        control.Focus();
    }

    public void ShowGeneralError(string message) => MessageBox.Show(
        this,
        message,
        "مدیریت منابع انسانی",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error,
        MessageBoxDefaultButton.Button1,
        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

    public void NotifySaved(long employeeId)
    {
        this.employeeId = employeeId;
        isDirty = false;
        personnelNumberTextBox.ReadOnly = true;
        employmentLifecycleButton.Enabled = true;
        organizationAssignmentsButton.Enabled = true;
        personnelRecordsButton.Enabled = true;
        EmployeeSaved?.Invoke(this, employeeId);
        MessageBox.Show(
            this,
            "اطلاعات کارمند ذخیره شد.",
            "مدیریت منابع انسانی",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (isDirty && !ConfirmDiscardChanges())
        {
            e.Cancel = true;
        }

        base.OnFormClosing(e);
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

    private void WireEvents()
    {
        foreach (var textBox in fieldsPanel.Controls.OfType<TextBox>())
        {
            textBox.TextChanged += (_, _) => MarkDirty();
        }

        genderComboBox.SelectedIndexChanged += (_, _) => MarkDirty();
        saveButton.Click += async (_, _) =>
        {
            errorProvider.Clear();
            if (!string.IsNullOrWhiteSpace(birthDateTextBox.Text) &&
                !dateAdapter.TryParse(birthDateTextBox.Text, out _))
            {
                errorProvider.SetError(birthDateTextBox, "تاریخ تولد جلالی معتبر نیست.");
                birthDateTextBox.Focus();
                return;
            }

            await presenter.SaveAsync(lifetime.Token);
        };
        employmentLifecycleButton.Click += (_, _) => ShowEmploymentLifecycle();
        organizationAssignmentsButton.Click += (_, _) => ShowOrganizationAssignments();
        personnelRecordsButton.Click += (_, _) => ShowPersonnelRecords();
        cancelButton.Click += (_, _) => Close();
    }

    private void MarkDirty()
    {
        if (!applyingData)
        {
            isDirty = true;
        }
    }

    private bool ConfirmDiscardChanges() => MessageBox.Show(
        this,
        "تغییرات ذخیره نشده‌اند. آیا از بستن یا تغییر پرونده مطمئن هستید؟",
        "تغییرات ذخیره‌نشده",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning,
        MessageBoxDefaultButton.Button2,
        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == DialogResult.Yes;

    private void ClearForm()
    {
        foreach (var textBox in fieldsPanel.Controls.OfType<TextBox>())
        {
            textBox.Clear();
        }

        genderComboBox.SelectedIndex = 0;
        errorProvider.Clear();
        employmentLifecycleButton.Enabled = false;
        organizationAssignmentsButton.Enabled = false;
        personnelRecordsButton.Enabled = false;
    }

    private void ShowEmploymentLifecycle()
    {
        if (employeeId is not long currentEmployeeId)
        {
            MessageBox.Show(
                this,
                "ابتدا اطلاعات پایه کارمند را ذخیره کنید.",
                "مدیریت منابع انسانی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return;
        }

        using var form = new EmploymentLifecycleForm(
            currentEmployeeId,
            lifecycleService,
            dateAdapter,
            backgroundExecutor,
            lifecycleLogger);
        form.ShowDialog(this);
    }

    private void ShowOrganizationAssignments()
    {
        if (employeeId is not long currentEmployeeId)
        {
            MessageBox.Show(
                this,
                "ابتدا اطلاعات پایه کارمند را ذخیره کنید.",
                "مدیریت منابع انسانی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return;
        }

        using var form = new OrganizationAssignmentsForm(
            currentEmployeeId,
            assignmentService,
            dateAdapter,
            backgroundExecutor,
            organizationLogger);
        form.ShowDialog(this);
    }

    private void ShowPersonnelRecords()
    {
        if (employeeId is not long currentEmployeeId)
        {
            MessageBox.Show(
                this,
                "ابتدا اطلاعات پایه کارمند را ذخیره کنید.",
                "مدیریت منابع انسانی",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
            return;
        }

        using var form = new PersonnelRecordsForm(
            currentEmployeeId,
            personnelRecordService,
            dateAdapter,
            backgroundExecutor,
            personnelRecordsLogger);
        form.ShowDialog(this);
    }
}
