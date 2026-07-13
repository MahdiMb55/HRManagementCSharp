using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class ManagedFile : Entity
{
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StoredFileName { get; private set; } = string.Empty;
    public string RelativePath { get; private set; } = string.Empty;
    public string Extension { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public string FileHash { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsInTrash { get; private set; }
    public DateTime? MovedToTrashAtUtc { get; private set; }
    public string? TrashRelativePath { get; private set; }
}
