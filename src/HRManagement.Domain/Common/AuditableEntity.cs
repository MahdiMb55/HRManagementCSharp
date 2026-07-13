namespace HRManagement.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; protected set; }

    public DateTime UpdatedAtUtc { get; protected set; }

    protected void InitializeTimestamps(DateTime nowUtc)
    {
        CreatedAtUtc = EnsureUtc(nowUtc);
        UpdatedAtUtc = CreatedAtUtc;
    }

    protected void Touch(DateTime nowUtc) => UpdatedAtUtc = EnsureUtc(nowUtc);

    private static DateTime EnsureUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
}
