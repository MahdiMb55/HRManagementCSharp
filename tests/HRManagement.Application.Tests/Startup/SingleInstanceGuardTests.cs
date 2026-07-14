using HRManagement.WinForms.Startup;
using System.Reflection;

namespace HRManagement.Application.Tests.Startup;

public sealed class SingleInstanceGuardTests
{
    [Fact]
    public void ProgramEntryPoint_IsSynchronousSoWinFormsStaysOnStaThread()
    {
        var programType = typeof(SingleInstanceGuard).Assembly.GetType("HRManagement.WinForms.Program");
        var main = programType?.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);

        Assert.NotNull(main);
        Assert.Equal(typeof(void), main.ReturnType);
    }

    [Fact]
    public void TryAcquire_AllowsOnlyOneLiveGuardWithoutThreadOwnedMutex()
    {
        var mutexName = $"Local\\HRManagement.Tests.{Guid.NewGuid():N}";

        using var first = SingleInstanceGuard.TryAcquire(mutexName);
        using var second = SingleInstanceGuard.TryAcquire(mutexName);

        Assert.True(first.IsPrimaryInstance);
        Assert.False(second.IsPrimaryInstance);
    }

    [Fact]
    public async Task Dispose_CanRunOnDifferentThreadThanAcquisition()
    {
        var mutexName = $"Local\\HRManagement.Tests.{Guid.NewGuid():N}";
        var guard = SingleInstanceGuard.TryAcquire(mutexName);

        await Task.Run(guard.Dispose);
    }
}
