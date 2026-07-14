using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using HRManagement.Application.Abstractions;
using HRManagement.Application.Backups;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Backups;

public sealed class BackupService(
    IApplicationPaths paths,
    IDbContextFactory<HrManagementDbContext> contextFactory,
    IClock clock) : IBackupService
{
    public async Task<BackupResult> CreateAsync(
        BackupRequest request,
        CancellationToken cancellationToken)
    {
        var destination = string.IsNullOrWhiteSpace(request.DestinationDirectory)
            ? paths.BackupsDirectory
            : Path.GetFullPath(request.DestinationDirectory);
        Directory.CreateDirectory(destination);
        var startedAt = clock.UtcNow;
        var backupFile = Path.Combine(destination, $"hr-management-{startedAt:yyyyMMddHHmmss}.zip");

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var history = BackupHistory.Start(request.BackupType, backupFile, startedAt);
        context.BackupHistories.Add(history);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            var manifestEntries = new List<BackupManifestEntry>();
            var tempManifest = Path.Combine(paths.TempDirectory, $"backup-manifest-{Guid.NewGuid():N}.json");
            Directory.CreateDirectory(paths.TempDirectory);

            using (var archive = ZipFile.Open(backupFile, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.EnumerateFiles(paths.DataDirectory, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (file.StartsWith(paths.BackupsDirectory, StringComparison.OrdinalIgnoreCase) ||
                        file.StartsWith(paths.TempDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var relative = Path.GetRelativePath(paths.DataDirectory, file).Replace('\\', '/');
                    archive.CreateEntryFromFile(file, relative, CompressionLevel.Optimal);
                    var info = new FileInfo(file);
                    manifestEntries.Add(new BackupManifestEntry(relative, info.Length, await Sha256Async(file, cancellationToken)));
                }

                var manifest = new BackupManifest("HRManagement", startedAt.ToString("O"), manifestEntries);
                await File.WriteAllTextAsync(tempManifest, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
                archive.CreateEntryFromFile(tempManifest, "backup-manifest.json", CompressionLevel.Optimal);
            }

            history.Complete(new FileInfo(backupFile).Length, clock.UtcNow);
            await context.SaveChangesAsync(cancellationToken);
            return BackupResult.Success(backupFile);
        }
        catch (Exception exception)
        {
            history.Fail(exception.Message, clock.UtcNow);
            await context.SaveChangesAsync(cancellationToken);
            return BackupResult.Failure("پشتیبان‌گیری انجام نشد.");
        }
    }

    public async Task<BackupResult> RestoreAsync(
        RestoreRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.BackupFilePath) || !File.Exists(request.BackupFilePath))
        {
            return BackupResult.Failure("فایل پشتیبان پیدا نشد.");
        }

        var restoreRoot = Path.Combine(paths.TempDirectory, $"restore-{Guid.NewGuid():N}");
        Directory.CreateDirectory(restoreRoot);
        ZipFile.ExtractToDirectory(request.BackupFilePath, restoreRoot);
        var manifest = BackupManifestValidator.Read(Path.Combine(restoreRoot, "backup-manifest.json"));
        if (manifest is null || !BackupManifestValidator.ContainsDatabase(manifest))
        {
            return BackupResult.Failure("مانیفست پشتیبان معتبر نیست.");
        }

        foreach (var entry in manifest.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var source = Path.Combine(restoreRoot, entry.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(source))
            {
                return BackupResult.Failure("فایل‌های پشتیبان کامل نیستند.");
            }

            var target = Path.Combine(paths.DataDirectory, entry.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(source, target, overwrite: true);
        }

        await Task.CompletedTask;
        return BackupResult.Success(request.BackupFilePath);
    }

    private static async Task<string> Sha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
