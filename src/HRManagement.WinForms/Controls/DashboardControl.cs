using HRManagement.Application.Abstractions;
using HRManagement.Application.Dashboard;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Controls;

public partial class DashboardControl : UserControl
{
    private readonly IDashboardService dashboardService;
    private readonly ILogger<DashboardControl> logger;
    private readonly IBackgroundExecutor backgroundExecutor;

    public DashboardControl(
        IDashboardService dashboardService,
        ILogger<DashboardControl> logger,
        IBackgroundExecutor backgroundExecutor)
    {
        this.dashboardService = dashboardService;
        this.logger = logger;
        this.backgroundExecutor = backgroundExecutor;
        InitializeComponent();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        statusLabel.Text = "در حال بارگذاری...";
        try
        {
            var snapshot = await backgroundExecutor.ExecuteAsync(
                backgroundToken => dashboardService.GetSnapshotAsync(backgroundToken),
                cancellationToken);
            activeEmployeesValueLabel.Text = snapshot.ActiveEmployees.ToString(System.Globalization.CultureInfo.CurrentCulture);
            archivedEmployeesValueLabel.Text = snapshot.ArchivedOrDepartedEmployees.ToString(System.Globalization.CultureInfo.CurrentCulture);
            activeContractsValueLabel.Text = snapshot.ActiveContracts.ToString(System.Globalization.CultureInfo.CurrentCulture);
            statusLabel.Text = snapshot.ActiveEmployees == 0 ? "هنوز کارمندی ثبت نشده است." : string.Empty;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            statusLabel.Text = string.Empty;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Dashboard query failed");
            statusLabel.Text = "نمایش اطلاعات داشبورد انجام نشد.";
        }
    }
}
