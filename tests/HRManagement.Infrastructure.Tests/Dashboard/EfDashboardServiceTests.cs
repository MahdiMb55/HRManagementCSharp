using HRManagement.Application.Employees;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Dashboard;
using HRManagement.Infrastructure.Employees;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Tests.Dashboard;

public sealed class EfDashboardServiceTests
{
    [Fact]
    public async Task GetSnapshotAsync_UsesRealDatabaseCounts()
    {
        using var database = new TemporaryDatabase();
        await database.InitializeAsync();
        var service = new EfDashboardService(database.Factory);

        var empty = await service.GetSnapshotAsync(CancellationToken.None);
        await new EfEmployeeWriteRepository(database.Factory).SaveAsync(
            new ValidatedEmployeeSave(
                null, "علی", "رضایی", "0012", "0013548581", null, Gender.Male,
                null, null, database.NowUtc, "employee.created"),
            CancellationToken.None);
        var populated = await service.GetSnapshotAsync(CancellationToken.None);

        Assert.Equal(0, empty.ActiveEmployees);
        Assert.Equal(0, empty.ArchivedOrDepartedEmployees);
        Assert.Equal(1, populated.ActiveEmployees);
        Assert.Equal(0, populated.ActiveContracts);
    }

    private sealed class TemporaryDatabase : IDisposable
    {
        private readonly string directory;

        public TemporaryDatabase()
        {
            directory = Path.Combine(Path.GetTempPath(), "HRManagement.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var options = new DbContextOptionsBuilder<HrManagementDbContext>()
                .UseSqlite($"Data Source={Path.Combine(directory, "dashboard.db")};Foreign Keys=True;Pooling=False")
                .Options;
            Factory = new TestDbContextFactory(options);
        }

        public DateTime NowUtc { get; } = new(2026, 7, 14, 8, 0, 0, DateTimeKind.Utc);
        public TestDbContextFactory Factory { get; }

        public Task<DatabaseInitializationResult> InitializeAsync() =>
            new DatabaseInitializer(Factory).InitializeAsync(CancellationToken.None);

        public void Dispose() => Directory.Delete(directory, recursive: true);
    }

    private sealed class TestDbContextFactory(DbContextOptions<HrManagementDbContext> options)
        : IDbContextFactory<HrManagementDbContext>
    {
        public HrManagementDbContext CreateDbContext() => new(options);
    }
}
