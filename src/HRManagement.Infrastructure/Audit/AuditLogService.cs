using HRManagement.Application.Audit;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Audit;

public sealed class AuditLogService(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IAuditLogService
{
    public async Task<IReadOnlyList<AuditLogDto>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var take = Math.Clamp(count, 1, 500);
        return await context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAtUtc)
            .Take(take)
            .Select(log => new AuditLogDto(
                log.Id,
                log.EntityType,
                log.EntityId,
                log.Action,
                log.Description,
                log.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
