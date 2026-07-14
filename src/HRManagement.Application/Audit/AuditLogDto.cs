namespace HRManagement.Application.Audit;

public sealed record AuditLogDto(
    long Id,
    string EntityType,
    long EntityId,
    string Action,
    string Description,
    DateTime CreatedAtUtc);
