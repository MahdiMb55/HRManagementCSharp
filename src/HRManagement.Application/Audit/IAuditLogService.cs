namespace HRManagement.Application.Audit;

public interface IAuditLogService
{
    Task<IReadOnlyList<AuditLogDto>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken);
}
