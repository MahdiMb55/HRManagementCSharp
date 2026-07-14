using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using HRManagement.Application.Abstractions;
using HRManagement.Application.Backups;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
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

        string? tempManifest = null;
        string? tempDatabaseSnapshot = null;

        try
        {
            var manifestEntries = new List<BackupManifestEntry>();
            Directory.CreateDirectory(paths.TempDirectory);

            tempManifest = Path.Combine(paths.TempDirectory, $"backup-manifest-{Guid.NewGuid():N}.json");
            tempDatabaseSnapshot = Path.Combine(paths.TempDirectory, $"hr-management-{Guid.NewGuid():N}.db");

            using (var archive = ZipFile.Open(backupFile, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.EnumerateFiles(paths.DataDirectory, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (ShouldSkipDataFile(file))
                    {
                        continue;
                    }

                    var relative = Path.GetRelativePath(paths.DataDirectory, file).Replace('\\', '/');
                    var sourceFile = file;

                    if (string.Equals(file, paths.DatabaseFile, StringComparison.OrdinalIgnoreCase))
                    {
                        CreateDatabaseSnapshot(tempDatabaseSnapshot);
                        sourceFile = tempDatabaseSnapshot;
                    }

                    await AddEntryFromFileAsync(archive, sourceFile, relative, cancellationToken);
                    var info = new FileInfo(sourceFile);
                    manifestEntries.Add(new BackupManifestEntry(relative, info.Length, await Sha256Async(sourceFile, cancellationToken)));
                }

                var manifest = new BackupManifest("HRManagement", startedAt.ToString("O"), manifestEntries);
                await File.WriteAllTextAsync(
                    tempManifest,
                    JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }),
                    cancellationToken);
                await AddEntryFromFileAsync(archive, tempManifest, "backup-manifest.json", cancellationToken);
            }

            history.Complete(new FileInfo(backupFile).Length, clock.UtcNow);
            await context.SaveChangesAsync(cancellationToken);
            return BackupResult.Success(backupFile);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            history.Fail(ex.Message, clock.UtcNow);
            await context.SaveChangesAsync(cancellationToken);
            return BackupResult.Failure("پشتیبان‌گیری انجام نشد.");
        }
        finally
        {
            DeleteTemporaryFile(tempManifest);
            DeleteTemporaryFile(tempDatabaseSnapshot);
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
            if (!File.Exists(source) || await Sha256Async(source, cancellationToken) != entry.Sha256)
            {
                return BackupResult.Failure("فایل‌های پشتیبان معتبر نیستند.");
            }

            var target = Path.Combine(paths.DataDirectory, entry.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(source, target, overwrite: true);
        }

        await Task.CompletedTask;
        return BackupResult.Success(request.BackupFilePath);
    }

    private bool ShouldSkipDataFile(string file)
    {
        if (file.StartsWith(paths.BackupsDirectory, StringComparison.OrdinalIgnoreCase)
            || file.StartsWith(paths.TempDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return file.StartsWith(paths.DatabaseDirectory, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(file, paths.DatabaseFile, StringComparison.OrdinalIgnoreCase);
    }

    private void CreateDatabaseSnapshot(string snapshotFile)
    {
        DeleteTemporaryFile(snapshotFile);

        using var source = new SqliteConnection($"Data Source={paths.DatabaseFile};Mode=ReadOnly");
        using var destination = new SqliteConnection($"Data Source={snapshotFile}");
        source.Open();
        destination.Open();
        source.BackupDatabase(destination);
        SqliteConnection.ClearAllPools();
    }

    private static void DeleteTemporaryFile(string? path)
    {
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static async Task AddEntryFromFileAsync(
        ZipArchive archive,
        string sourceFile,
        string entryName,
        CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        await using var input = new FileStream(
            sourceFile,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
        await using var output = entry.Open();
        await input.CopyToAsync(output, cancellationToken);
    }

    private static async Task<string> Sha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
