namespace HRManagement.Application.Abstractions;

public interface IApplicationPaths
{
    string BaseDirectory { get; }
    string DataDirectory { get; }
    string DatabaseDirectory { get; }
    string DatabaseFile { get; }
    string DocumentsDirectory { get; }
    string PhotosDirectory { get; }
    string LetterTemplatesDirectory { get; }
    string GeneratedLettersDirectory { get; }
    string BackupsDirectory { get; }
    string TrashDirectory { get; }
    string LogsDirectory { get; }
    string TempDirectory { get; }
    string InitializationMarkerFile { get; }
    IReadOnlyList<string> RequiredDirectories { get; }
}
