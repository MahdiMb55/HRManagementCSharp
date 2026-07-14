using HRManagement.Application.Employees.Search;

namespace HRManagement.WinForms.Employees;

public sealed class AdvancedFilterDialog : Form
{
    private readonly CheckBox includeArchivedCheckBox = new();

    public AdvancedFilterDialog(EmployeeFilter currentFilter)
    {
        Filter = currentFilter;
        InitializeComponent(currentFilter);
    }

    public EmployeeFilter Filter { get; private set; }

    private void InitializeComponent(EmployeeFilter currentFilter)
    {
        Text = "فیلتر پیشرفته";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(420, 220);
        Size = new Size(480, 260);

        includeArchivedCheckBox.Text = "نمایش کارمندان بایگانی‌شده";
        includeArchivedCheckBox.Checked = currentFilter.IncludeArchived;
        includeArchivedCheckBox.AutoSize = true;
        includeArchivedCheckBox.Dock = DockStyle.Top;
        includeArchivedCheckBox.Padding = new Padding(20);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 56,
            Padding = new Padding(20, 8, 20, 8),
        };
        var applyButton = new Button { Text = "اعمال", AutoSize = true };
        applyButton.Click += (_, _) =>
        {
            Filter = currentFilter with { IncludeArchived = includeArchivedCheckBox.Checked };
            DialogResult = DialogResult.OK;
            Close();
        };
        var cancelButton = new Button { Text = "بستن", AutoSize = true };
        cancelButton.Click += (_, _) => Close();
        actions.Controls.Add(applyButton);
        actions.Controls.Add(cancelButton);

        Controls.Add(includeArchivedCheckBox);
        Controls.Add(actions);
    }
}
