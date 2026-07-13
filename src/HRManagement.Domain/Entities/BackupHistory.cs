using HRManagement.Domain.Common;
using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Entities;

public sealed class BackupHistory : Entity
{
    public BackupType BackupType { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public long? FileSizeBytes { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public bool WasSuccessful { get; private set; }
    public string? ErrorMessage { get; private set; }
}
