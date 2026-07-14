using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class ManagedFile : Entity
{
    private ManagedFile()
    {
    }

    private ManagedFile(
        string originalFileName,
        string storedFileName,
        string relativePath,
        string extension,
        string mimeType,
        long sizeBytes,
        string fileHash,
        DateTime createdAtUtc)
    {
        OriginalFileName = originalFileName.Trim();
        StoredFileName = storedFileName.Trim();
        RelativePath = relativePath.Trim();
        Extension = extension.Trim().ToLowerInvariant();
        MimeType = mimeType.Trim();
        SizeBytes = sizeBytes;
        FileHash = fileHash.Trim();
        CreatedAtUtc = createdAtUtc.Kind == DateTimeKind.Utc ? createdAtUtc : createdAtUtc.ToUniversalTime();
    }

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

    public static ValidationResult<ManagedFile> Create(
        string? originalFileName,
        string? storedFileName,
        string? relativePath,
        string? extension,
        string? mimeType,
        long sizeBytes,
        string? fileHash,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(originalFileName) ||
            string.IsNullOrWhiteSpace(storedFileName) ||
            string.IsNullOrWhiteSpace(relativePath) ||
            string.IsNullOrWhiteSpace(extension) ||
            string.IsNullOrWhiteSpace(mimeType) ||
            string.IsNullOrWhiteSpace(fileHash))
        {
            return ValidationResult<ManagedFile>.Failure(
                "managed_file.required",
                "اطلاعات فایل مدیریت‌شده کامل نیست.");
        }

        if (relativePath.Contains("..", StringComparison.Ordinal) ||
            Path.IsPathRooted(relativePath))
        {
            return ValidationResult<ManagedFile>.Failure(
                "managed_file.path.invalid",
                "مسیر فایل معتبر نیست.");
        }

        if (sizeBytes <= 0)
        {
            return ValidationResult<ManagedFile>.Failure(
                "managed_file.size.invalid",
                "اندازه فایل معتبر نیست.");
        }

        return ValidationResult<ManagedFile>.Success(
            new ManagedFile(originalFileName, storedFileName, relativePath, extension, mimeType, sizeBytes, fileHash, createdAtUtc));
    }

    public void MoveToTrash(string trashRelativePath, DateTime nowUtc)
    {
        IsInTrash = true;
        TrashRelativePath = trashRelativePath;
        MovedToTrashAtUtc = nowUtc.Kind == DateTimeKind.Utc ? nowUtc : nowUtc.ToUniversalTime();
    }
}
