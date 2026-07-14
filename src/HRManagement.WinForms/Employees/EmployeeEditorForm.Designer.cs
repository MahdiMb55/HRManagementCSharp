namespace HRManagement.WinForms.Employees;

partial class EmployeeEditorForm
{
    private TableLayoutPanel fieldsPanel = null!;
    private TextBox firstNameTextBox = null!;
    private TextBox lastNameTextBox = null!;
    private TextBox personnelNumberTextBox = null!;
    private TextBox nationalCodeTextBox = null!;
    private TextBox fatherNameTextBox = null!;
    private ComboBox genderComboBox = null!;
    private TextBox birthDateTextBox = null!;
    private TextBox mobileTextBox = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Button employmentLifecycleButton = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label busyLabel = null!;
    private ErrorProvider errorProvider = null!;

    private void InitializeComponent()
    {
        fieldsPanel = new TableLayoutPanel();
        firstNameTextBox = new TextBox();
        lastNameTextBox = new TextBox();
        personnelNumberTextBox = new TextBox();
        nationalCodeTextBox = new TextBox();
        fatherNameTextBox = new TextBox();
        genderComboBox = new ComboBox();
        birthDateTextBox = new TextBox();
        mobileTextBox = new TextBox();
        actionsPanel = new FlowLayoutPanel();
        employmentLifecycleButton = new Button();
        saveButton = new Button();
        cancelButton = new Button();
        busyLabel = new Label();
        errorProvider = new ErrorProvider();
        SuspendLayout();
        fieldsPanel.ColumnCount = 2;
        fieldsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        fieldsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        fieldsPanel.RowCount = 8;
        fieldsPanel.Dock = DockStyle.Fill;
        fieldsPanel.Padding = new Padding(20);
        AddField(0, "نام *", firstNameTextBox);
        AddField(1, "نام خانوادگی *", lastNameTextBox);
        AddField(2, "شماره پرسنلی *", personnelNumberTextBox);
        AddField(3, "کد ملی *", nationalCodeTextBox);
        AddField(4, "نام پدر", fatherNameTextBox);
        genderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        genderComboBox.Items.AddRange(["مرد", "زن"]);
        genderComboBox.SelectedIndex = 0;
        AddField(5, "جنسیت", genderComboBox);
        birthDateTextBox.PlaceholderText = "نمونه: 1405/01/01";
        AddField(6, "تاریخ تولد", birthDateTextBox);
        AddField(7, "تلفن همراه", mobileTextBox);
        actionsPanel.Dock = DockStyle.Bottom;
        actionsPanel.FlowDirection = FlowDirection.RightToLeft;
        actionsPanel.Height = 58;
        actionsPanel.Padding = new Padding(20, 10, 20, 8);
        employmentLifecycleButton.AutoSize = true;
        employmentLifecycleButton.Enabled = false;
        employmentLifecycleButton.Text = "سوابق استخدام";
        saveButton.Text = "ذخیره";
        saveButton.Width = 100;
        cancelButton.Text = "بستن";
        cancelButton.Width = 100;
        busyLabel.Text = "در حال انجام عملیات...";
        busyLabel.AutoSize = true;
        busyLabel.Padding = new Padding(8);
        busyLabel.Visible = false;
        actionsPanel.Controls.Add(employmentLifecycleButton);
        actionsPanel.Controls.Add(saveButton);
        actionsPanel.Controls.Add(cancelButton);
        actionsPanel.Controls.Add(busyLabel);
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        errorProvider.RightToLeft = true;
        Controls.Add(fieldsPanel);
        Controls.Add(actionsPanel);
        AcceptButton = saveButton;
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(560, 570);
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(520, 520);
        Name = "employeeEditorForm";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        Text = "اطلاعات کارمند";
        ResumeLayout(false);
    }

    private void AddField(int row, string title, Control input)
    {
        fieldsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        var label = new Label
        {
            Dock = DockStyle.Fill,
            Text = title,
            TextAlign = ContentAlignment.MiddleRight,
        };
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(8);
        input.TabIndex = row;
        fieldsPanel.Controls.Add(label, 0, row);
        fieldsPanel.Controls.Add(input, 1, row);
    }
}
