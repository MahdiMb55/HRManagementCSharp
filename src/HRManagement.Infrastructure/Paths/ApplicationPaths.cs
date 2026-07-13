using HRManagement.Application.Abstractions;

namespace HRManagement.Infrastructure.Paths;

public sealed class ApplicationPaths : IApplicationPaths
{
    public ApplicationPaths(string baseDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);

        BaseDirectory = Path.GetFullPath(baseDirectory);
        DataDirectory = Path.Combine(BaseDirectory, "Data");
        DatabaseDirectory = Path.Combine(DataDirectory, "Database");
        DatabaseFile = Path.Combine(DatabaseDirectory, "hr-management.db");
        DocumentsDirectory = Path.Combine(DataDirectory, "Documents");
        PhotosDirectory = Path.Combine(DataDirectory, "Photos");
        LetterTemplatesDirectory = Path.Combine(DataDirectory, "LetterTemplates");
        GeneratedLettersDirectory = Path.Combine(DataDirectory, "GeneratedLetters");
        BackupsDirectory = Path.Combine(DataDirectory, "Backups");
        TrashDirectory = Path.Combine(DataDirectory, "Trash");
        LogsDirectory = Path.Combine(DataDirectory, "Logs");
        TempDirectory = Path.Combine(DataDirectory, "Temp");
        InitializationMarkerFile = Path.Combine(DataDirectory, ".initialized");
        RequiredDirectories =
        [
            DataDirectory,
            DatabaseDirectory,
            DocumentsDirectory,
            PhotosDirectory,
            LetterTemplatesDirectory,
            GeneratedLettersDirectory,
            BackupsDirectory,
            TrashDirectory,
            LogsDirectory,
            TempDirectory,
        ];
    }

    public string BaseDirectory { get; }
    public string DataDirectory { get; }
    public string DatabaseDirectory { get; }
    public string DatabaseFile { get; }
    public string DocumentsDirectory { get; }
    public string PhotosDirectory { get; }
    public string LetterTemplatesDirectory { get; }
    public string GeneratedLettersDirectory { get; }
    public string BackupsDirectory { get; }
    public string TrashDirectory { get; }
    public string LogsDirectory { get; }
    public string TempDirectory { get; }
    public string InitializationMarkerFile { get; }
    public IReadOnlyList<string> RequiredDirectories { get; }

    public static ApplicationPaths FromExecutableDirectory() => new(AppContext.BaseDirectory);
}
