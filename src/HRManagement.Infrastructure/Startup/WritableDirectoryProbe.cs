namespace HRManagement.Infrastructure.Startup;

public sealed class WritableDirectoryProbe : IWritableDirectoryProbe
{
    public async Task<bool> CanWriteAsync(string directory, CancellationToken cancellationToken)
    {
        var probeFile = Path.Combine(directory, $".write-probe-{Guid.NewGuid():N}.tmp");
        try
        {
            await using var stream = new FileStream(
                probeFile,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 1,
                FileOptions.Asynchronous | FileOptions.WriteThrough);
            await stream.WriteAsync(new byte[] { 0 }, cancellationToken);
            await stream.FlushAsync(cancellationToken);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        finally
        {
            if (File.Exists(probeFile))
            {
                File.Delete(probeFile);
            }
        }
    }
}
