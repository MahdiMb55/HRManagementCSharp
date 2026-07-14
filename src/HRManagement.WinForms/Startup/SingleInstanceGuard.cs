namespace HRManagement.WinForms.Startup;

public sealed class SingleInstanceGuard : IDisposable
{
    public const string MutexName = "Local\\HRManagement.SingleInstance.v1";

    private readonly Mutex mutex;
    private bool disposed;

    private SingleInstanceGuard(Mutex mutex, bool isPrimaryInstance)
    {
        this.mutex = mutex;
        IsPrimaryInstance = isPrimaryInstance;
    }

    public bool IsPrimaryInstance { get; }

    public static SingleInstanceGuard TryAcquire(string mutexName = MutexName)
    {
        var mutex = new Mutex(initiallyOwned: false, mutexName, out var createdNew);
        return new SingleInstanceGuard(mutex, createdNew);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        mutex.Dispose();
    }
}
