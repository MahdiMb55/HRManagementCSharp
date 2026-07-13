namespace HRManagement.Application.Startup;

public enum StartupIntegrityStatus
{
    ReadyFirstRun = 1,
    ReadyExistingInstallation = 2,
    MissingDatabase = 3,
    NotWritable = 4,
    DirectoryCreationFailed = 5,
}
