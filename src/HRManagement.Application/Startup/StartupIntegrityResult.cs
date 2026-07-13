namespace HRManagement.Application.Startup;

public sealed record StartupIntegrityResult(
    StartupIntegrityStatus Status,
    string UserMessage)
{
    public bool CanContinue =>
        Status is StartupIntegrityStatus.ReadyFirstRun or StartupIntegrityStatus.ReadyExistingInstallation;
}
