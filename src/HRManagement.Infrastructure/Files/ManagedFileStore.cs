using System.Security.Cryptography;
using HRManagement.Application.Abstractions;
using HRManagement.Application.Files;
using HRManagement.Domain.Entities;

namespace HRManagement.Infrastructure.Files;

public sealed class ManagedFileStore(IApplicationPaths paths) : IManagedFileStore
{
    private const long MaxFileSizeBytes = 20L * 1024 * 1024;

    public async Task<ManagedFileSaveResult> SaveAsync(
        ManagedFileSaveRequest request,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SourcePath) || !File.Exists(request.SourcePath))
        {
            return ManagedFileSaveResult.Failure("فایل انتخاب‌شده پیدا نشد.");
        }

        var sourceFullPath = Path.GetFullPath(request.SourcePath);
        var sourceInfo = new FileInfo(sourceFullPath);
        if (sourceInfo.Length <= 0 || sourceInfo.Length > MaxFileSizeBytes)
        {
            return ManagedFileSaveResult.Failure("اندازه فایل باید بیشتر از صفر و حداکثر ۲۰ مگابایت باشد.");
        }

        var extension = sourceInfo.Extension.ToLowerInvariant();
        if (!request.AllowedExtensions.Contains(extension))
        {
            return ManagedFileSaveResult.Failure("نوع فایل انتخاب‌شده مجاز نیست.");
        }

        var header = new byte[Math.Min(16, sourceInfo.Length)];
        await using (var headerStream = File.OpenRead(sourceFullPath))
        {
            _ = await headerStream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
        }

        if (!FileSignatureValidator.IsKnownSafeSignature(extension, header))
        {
            return ManagedFileSaveResult.Failure("امضای فایل با نوع آن سازگار نیست.");
        }

        var targetRoot = ResolveKindRoot(request.Kind);
        Directory.CreateDirectory(targetRoot);
        var storedFileName = $"{nowUtc:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}{extension}";
        var targetFullPath = Path.Combine(targetRoot, storedFileName);
        if (!IsInside(paths.DataDirectory, targetFullPath))
        {
            return ManagedFileSaveResult.Failure("مسیر ذخیره فایل معتبر نیست.");
        }

        await using (var source = File.OpenRead(sourceFullPath))
        await using (var target = File.Create(targetFullPath))
        {
            await source.CopyToAsync(target, cancellationToken);
        }

        var hash = await ComputeSha256Async(targetFullPath, cancellationToken);
        var relativePath = Path.GetRelativePath(paths.DataDirectory, targetFullPath).Replace('\\', '/');
        var managedFile = ManagedFile.Create(
            sourceInfo.Name,
            storedFileName,
            relativePath,
            extension,
            FileSignatureValidator.MimeTypeFor(extension),
            sourceInfo.Length,
            hash,
            nowUtc);
        if (!managedFile.IsSuccess)
        {
            return ManagedFileSaveResult.Failure(managedFile.Errors[0].Message);
        }

        return ManagedFileSaveResult.Success(managedFile.Value!);
    }

    private string ResolveKindRoot(ManagedFileKind kind) =>
        kind switch
        {
            ManagedFileKind.Photo => paths.PhotosDirectory,
            ManagedFileKind.ContractAttachment => Path.Combine(paths.DocumentsDirectory, "Contracts"),
            _ => paths.DocumentsDirectory,
        };

    private static bool IsInside(string root, string candidate)
    {
        var normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var normalizedCandidate = Path.GetFullPath(candidate);
        return normalizedCandidate.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string> ComputeSha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
