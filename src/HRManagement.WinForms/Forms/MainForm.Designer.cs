namespace HRManagement.WinForms.Forms;

partial class MainForm
{
    private Panel navigationPanel = null!;
    private Panel headerPanel = null!;
    private Panel contentPanel = null!;
    private Label appTitleLabel = null!;
    private Label pageTitleLabel = null!;
    private Button dashboardButton = null!;
    private Button employeesButton = null!;
    private Button departmentsButton = null!;
    private Button responsibilitiesButton = null!;
    private Button archiveButton = null!;
    private Button backupsButton = null!;
    private Button settingsButton = null!;

    private void InitializeComponent()
    {
        navigationPanel = new Panel();
        headerPanel = new Panel();
        contentPanel = new Panel();
        appTitleLabel = new Label();
        pageTitleLabel = new Label();
        dashboardButton = CreateNavigationButton("داشبورد");
        employeesButton = CreateNavigationButton("کارکنان");
        departmentsButton = CreateNavigationButton("واحدهای سازمانی");
        responsibilitiesButton = CreateNavigationButton("مسئولیت‌ها");
        archiveButton = CreateNavigationButton("بایگانی");
        backupsButton = CreateNavigationButton("پشتیبان‌گیری");
        settingsButton = CreateNavigationButton("تنظیمات");
        SuspendLayout();
        navigationPanel.BackColor = Color.FromArgb(35, 55, 75);
        navigationPanel.Dock = DockStyle.Right;
        navigationPanel.Width = 220;
        navigationPanel.Padding = new Padding(12);
        appTitleLabel.Dock = DockStyle.Top;
        appTitleLabel.Height = 72;
        appTitleLabel.Text = "مدیریت منابع انسانی";
        appTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
        appTitleLabel.ForeColor = Color.White;
        appTitleLabel.Font = new Font(Font, FontStyle.Bold);
        navigationPanel.Controls.Add(settingsButton);
        navigationPanel.Controls.Add(backupsButton);
        navigationPanel.Controls.Add(archiveButton);
        navigationPanel.Controls.Add(responsibilitiesButton);
        navigationPanel.Controls.Add(departmentsButton);
        navigationPanel.Controls.Add(employeesButton);
        navigationPanel.Controls.Add(dashboardButton);
        navigationPanel.Controls.Add(appTitleLabel);
        headerPanel.BackColor = Color.White;
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 64;
        pageTitleLabel.Dock = DockStyle.Fill;
        pageTitleLabel.Padding = new Padding(20, 0, 20, 0);
        pageTitleLabel.TextAlign = ContentAlignment.MiddleRight;
        pageTitleLabel.Font = new Font(Font, FontStyle.Bold);
        headerPanel.Controls.Add(pageTitleLabel);
        contentPanel.BackColor = Color.FromArgb(245, 247, 250);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Padding = new Padding(12);
        Controls.Add(contentPanel);
        Controls.Add(headerPanel);
        Controls.Add(navigationPanel);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(245, 247, 250);
        ClientSize = new Size(1180, 760);
        MinimumSize = new Size(960, 640);
        Name = "mainForm";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "مدیریت منابع انسانی";
        WindowState = FormWindowState.Maximized;
        ResumeLayout(false);
    }

    private static Button CreateNavigationButton(string text) => new()
    {
        Cursor = Cursors.Hand,
        Dock = DockStyle.Top,
        FlatStyle = FlatStyle.Flat,
        ForeColor = Color.White,
        Height = 48,
        Margin = new Padding(0, 3, 0, 3),
        Text = text,
        TextAlign = ContentAlignment.MiddleRight,
        UseVisualStyleBackColor = false,
    };
}
