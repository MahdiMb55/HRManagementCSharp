namespace HRManagement.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardSnapshot> GetSnapshotAsync(CancellationToken cancellationToken);
}
