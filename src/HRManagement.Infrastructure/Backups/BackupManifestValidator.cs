using System.Text.Json;
using HRManagement.Application.Backups;

namespace HRManagement.Infrastructure.Backups;

public static class BackupManifestValidator
{
    public static BackupManifest? Read(string manifestPath)
    {
        if (!File.Exists(manifestPath))
        {
            return null;
        }

        var json = File.ReadAllText(manifestPath);
        return JsonSerializer.Deserialize<BackupManifest>(json);
    }

    public static bool ContainsDatabase(BackupManifest manifest) =>
        manifest.Entries.Any(entry => string.Equals(entry.RelativePath, "Database/hr-management.db", StringComparison.OrdinalIgnoreCase));
}
