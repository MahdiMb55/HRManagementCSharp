namespace HRManagement.WinForms.Employees;

partial class EmployeeListControl
{
    private TableLayoutPanel rootLayout = null!;
    private FlowLayoutPanel toolbarPanel = null!;
    private TextBox searchTextBox = null!;
    private ComboBox pageSizeComboBox = null!;
    private Button columnVisibilityButton = null!;
    private ContextMenuStrip columnVisibilityMenu = null!;
    private Button advancedFilterButton = null!;
    private Button archiveEmployeeButton = null!;
    private Button addEmployeeButton = null!;
    private Button editEmployeeButton = null!;
    private DataGridView employeesGrid = null!;
    private Label loadingLabel = null!;
    private Label emptyStateLabel = null!;
    private Label employeeSummaryLabel = null!;
    private FlowLayoutPanel pagingPanel = null!;
    private Button previousPageButton = null!;
    private Button nextPageButton = null!;
    private Label pageStatusLabel = null!;

    private void InitializeComponent()
    {
        rootLayout = new TableLayoutPanel();
        toolbarPanel = new FlowLayoutPanel();
        searchTextBox = new TextBox();
        pageSizeComboBox = new ComboBox();
        columnVisibilityButton = new Button();
        columnVisibilityMenu = new ContextMenuStrip();
        advancedFilterButton = new Button();
        archiveEmployeeButton = new Button();
        addEmployeeButton = new Button();
        editEmployeeButton = new Button();
        employeesGrid = new DataGridView();
        loadingLabel = new Label();
        emptyStateLabel = new Label();
        employeeSummaryLabel = new Label();
        pagingPanel = new FlowLayoutPanel();
        previousPageButton = new Button();
        nextPageButton = new Button();
        pageStatusLabel = new Label();
        SuspendLayout();
        rootLayout.ColumnCount = 2;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        rootLayout.RowCount = 3;
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        rootLayout.Dock = DockStyle.Fill;
        toolbarPanel.Dock = DockStyle.Fill;
        toolbarPanel.FlowDirection = FlowDirection.RightToLeft;
        toolbarPanel.Padding = new Padding(4, 8, 4, 4);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.PlaceholderText = "جست‌وجوی نام، کد ملی، شماره پرسنلی یا تلفن";
        searchTextBox.Width = 340;
        pageSizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        pageSizeComboBox.Items.AddRange([25, 50, 100]);
        columnVisibilityButton.AutoSize = true;
        columnVisibilityButton.Text = "ستون‌ها";
        advancedFilterButton.AutoSize = true;
        advancedFilterButton.Text = "فیلتر پیشرفته";
        archiveEmployeeButton.AutoSize = true;
        archiveEmployeeButton.Enabled = false;
        archiveEmployeeButton.Text = "بایگانی";
        pageSizeComboBox.SelectedIndex = 0;
        pageSizeComboBox.Width = 72;
        addEmployeeButton.Text = "افزودن کارمند";
        addEmployeeButton.AutoSize = true;
        editEmployeeButton.Text = "ویرایش";
        editEmployeeButton.AutoSize = true;
        editEmployeeButton.Enabled = false;
        toolbarPanel.Controls.Add(addEmployeeButton);
        toolbarPanel.Controls.Add(editEmployeeButton);
        toolbarPanel.Controls.Add(archiveEmployeeButton);
        toolbarPanel.Controls.Add(advancedFilterButton);
        toolbarPanel.Controls.Add(columnVisibilityButton);
        toolbarPanel.Controls.Add(searchTextBox);
        toolbarPanel.Controls.Add(new Label { Text = "تعداد در صفحه:", AutoSize = true, Padding = new Padding(4, 7, 4, 0) });
        toolbarPanel.Controls.Add(pageSizeComboBox);
        employeesGrid.AllowUserToAddRows = false;
        employeesGrid.AllowUserToDeleteRows = false;
        employeesGrid.AllowUserToOrderColumns = true;
        employeesGrid.AutoGenerateColumns = false;
        employeesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        employeesGrid.BackgroundColor = Color.White;
        employeesGrid.BorderStyle = BorderStyle.Fixed3D;
        employeesGrid.Dock = DockStyle.Fill;
        employeesGrid.MultiSelect = true;
        employeesGrid.ReadOnly = true;
        employeesGrid.RowHeadersVisible = false;
        employeesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        employeesGrid.Columns.AddRange(
            CreateColumn("personnelNumberColumn", "شماره پرسنلی", "PersonnelNumber", 85),
            CreateColumn("firstNameColumn", "نام", "FirstName", 80),
            CreateColumn("lastNameColumn", "نام خانوادگی", "LastName", 100),
            CreateColumn("nationalCodeColumn", "کد ملی", "NationalCode", 90),
            CreateColumn("mobileColumn", "تلفن همراه", "MobileNumber", 90),
            CreateColumn("departmentColumn", "واحد", "DepartmentName", 100),
            CreateColumn("responsibilityColumn", "مسئولیت", "PrimaryResponsibility", 110),
            CreateColumn("statusColumn", "وضعیت", "EmploymentStatus", 75));
        employeeSummaryLabel.Dock = DockStyle.Fill;
        employeeSummaryLabel.BackColor = Color.White;
        employeeSummaryLabel.Padding = new Padding(16);
        employeeSummaryLabel.Text = "یک کارمند را انتخاب کنید.";
        employeeSummaryLabel.TextAlign = ContentAlignment.TopRight;
        pagingPanel.Dock = DockStyle.Fill;
        pagingPanel.FlowDirection = FlowDirection.RightToLeft;
        previousPageButton.Text = "قبلی";
        nextPageButton.Text = "بعدی";
        pageStatusLabel.AutoSize = true;
        pageStatusLabel.Padding = new Padding(12, 7, 12, 0);
        pagingPanel.Controls.Add(nextPageButton);
        pagingPanel.Controls.Add(previousPageButton);
        pagingPanel.Controls.Add(pageStatusLabel);
        loadingLabel.AutoSize = true;
        loadingLabel.Text = "در حال بارگذاری...";
        loadingLabel.Visible = false;
        emptyStateLabel.Dock = DockStyle.Fill;
        emptyStateLabel.Text = "رکوردی برای نمایش وجود ندارد.";
        emptyStateLabel.TextAlign = ContentAlignment.MiddleCenter;
        emptyStateLabel.Visible = false;
        rootLayout.Controls.Add(toolbarPanel, 0, 0);
        rootLayout.SetColumnSpan(toolbarPanel, 2);
        rootLayout.Controls.Add(employeesGrid, 0, 1);
        rootLayout.Controls.Add(employeeSummaryLabel, 1, 1);
        rootLayout.Controls.Add(pagingPanel, 0, 2);
        rootLayout.SetColumnSpan(pagingPanel, 2);
        Controls.Add(emptyStateLabel);
        Controls.Add(loadingLabel);
        Controls.Add(rootLayout);
        BackColor = Color.White;
        Dock = DockStyle.Fill;
        Name = "employeeListControl";
        RightToLeft = RightToLeft.Yes;
        ResumeLayout(false);
        PerformLayout();
    }

    private static DataGridViewTextBoxColumn CreateColumn(
        string name,
        string header,
        string property,
        float fillWeight) => new()
        {
            Name = name,
            HeaderText = header,
            DataPropertyName = property,
            FillWeight = fillWeight,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Programmatic,
        };
}
