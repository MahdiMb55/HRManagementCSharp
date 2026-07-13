namespace HRManagement.Infrastructure.Startup;

public interface IWritableDirectoryProbe
{
    Task<bool> CanWriteAsync(string directory, CancellationToken cancellationToken);
}
