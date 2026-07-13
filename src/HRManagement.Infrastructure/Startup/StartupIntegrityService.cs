using HRManagement.Application.Abstractions;
using HRManagement.Application.Startup;

namespace HRManagement.Infrastructure.Startup;

public interface IStartupIntegrityService
{
    Task<StartupIntegrityResult> CheckAsync(CancellationToken cancellationToken);
    Task<StartupMarkerResult> MarkInitializedAsync(CancellationToken cancellationToken);
}

public sealed class StartupIntegrityService(
    IApplicationPaths paths,
    IWritableDirectoryProbe writeProbe) : IStartupIntegrityService
{
    public async Task<StartupIntegrityResult> CheckAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var directory in paths.RequiredDirectories)
            {
                Directory.CreateDirectory(directory);
            }
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or IOException)
        {
            return new StartupIntegrityResult(
                StartupIntegrityStatus.DirectoryCreationFailed,
                "پوشه‌های داده برنامه ایجاد نشدند. مسیر نصب و مجوز نوشتن را بررسی کنید.");
        }

        if (!await writeProbe.CanWriteAsync(paths.DataDirectory, cancellationToken))
        {
            return new StartupIntegrityResult(
                StartupIntegrityStatus.NotWritable,
                "برنامه اجازه نوشتن در پوشه داده را ندارد. مسیر نصب را بررسی کنید.");
        }

        var hasMarker = File.Exists(paths.InitializationMarkerFile);
        var hasDatabase = File.Exists(paths.DatabaseFile);
        if (hasMarker && !hasDatabase)
        {
            return new StartupIntegrityResult(
                StartupIntegrityStatus.MissingDatabase,
                "پایگاه داده این نصب پیدا نشد. از ایجاد اطلاعات جدید خودداری کنید و نسخه پشتیبان یا مسیر نصب را بررسی کنید.");
        }

        return new StartupIntegrityResult(
            hasDatabase ? StartupIntegrityStatus.ReadyExistingInstallation : StartupIntegrityStatus.ReadyFirstRun,
            string.Empty);
    }

    public async Task<StartupMarkerResult> MarkInitializedAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(paths.DatabaseFile))
        {
            return StartupMarkerResult.Failure("startup.database_missing");
        }

        var temporaryMarker = paths.InitializationMarkerFile + ".tmp";
        try
        {
            await File.WriteAllTextAsync(
                temporaryMarker,
                "HRManagement initialization v1",
                cancellationToken);
            File.Move(temporaryMarker, paths.InitializationMarkerFile, overwrite: true);
            return StartupMarkerResult.Success();
        }
        finally
        {
            if (File.Exists(temporaryMarker))
            {
                File.Delete(temporaryMarker);
            }
        }
    }
}
