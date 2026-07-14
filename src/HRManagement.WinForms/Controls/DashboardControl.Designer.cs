namespace HRManagement.WinForms.Controls;

partial class DashboardControl
{
    private TableLayoutPanel cardsPanel = null!;
    private Label activeEmployeesValueLabel = null!;
    private Label archivedEmployeesValueLabel = null!;
    private Label activeContractsValueLabel = null!;
    private Label statusLabel = null!;

    private void InitializeComponent()
    {
        cardsPanel = new TableLayoutPanel();
        activeEmployeesValueLabel = CreateCard("کارکنان فعال", 0);
        archivedEmployeesValueLabel = CreateCard("بایگانی و خاتمه‌یافته", 1);
        activeContractsValueLabel = CreateCard("قراردادهای فعال", 2);
        statusLabel = new Label();
        SuspendLayout();
        cardsPanel.ColumnCount = 3;
        cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        cardsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
        cardsPanel.Dock = DockStyle.Top;
        cardsPanel.Height = 130;
        cardsPanel.Padding = new Padding(8);
        cardsPanel.RightToLeft = RightToLeft.Yes;
        statusLabel.Dock = DockStyle.Top;
        statusLabel.Height = 44;
        statusLabel.Padding = new Padding(12);
        statusLabel.TextAlign = ContentAlignment.MiddleRight;
        statusLabel.ForeColor = Color.DimGray;
        Controls.Add(statusLabel);
        Controls.Add(cardsPanel);
        BackColor = Color.White;
        Dock = DockStyle.Fill;
        Name = "dashboardControl";
        RightToLeft = RightToLeft.Yes;
        ResumeLayout(false);
    }

    private Label CreateCard(string title, int column)
    {
        var panel = new TableLayoutPanel
        {
            BackColor = Color.FromArgb(245, 247, 250),
            Dock = DockStyle.Fill,
            Margin = new Padding(8),
            RowCount = 2,
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        var titleLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = title,
            TextAlign = ContentAlignment.BottomCenter,
            ForeColor = Color.FromArgb(70, 78, 90),
        };
        var valueLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "۰",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font(Font, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 92, 150),
        };
        panel.Controls.Add(titleLabel, 0, 0);
        panel.Controls.Add(valueLabel, 0, 1);
        cardsPanel.Controls.Add(panel, column, 0);
        return valueLabel;
    }
}
