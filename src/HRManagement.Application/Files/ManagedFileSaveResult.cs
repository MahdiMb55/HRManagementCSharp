using HRManagement.Domain.Entities;

namespace HRManagement.Application.Files;

public sealed record ManagedFileSaveResult(
    bool IsSuccess,
    string UserMessage,
    ManagedFile? File = null)
{
    public static ManagedFileSaveResult Success(ManagedFile file) =>
        new(true, string.Empty, file);

    public static ManagedFileSaveResult Failure(string userMessage) =>
        new(false, userMessage);
}
