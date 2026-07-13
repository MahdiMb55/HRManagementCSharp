using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Tests.Persistence;

public sealed class MigrationTests
{
    [Fact]
    public async Task InitializeAsync_CreatesCompleteDatabaseAndFixedCategoriesOnly()
    {
        var directory = Path.Combine(Path.GetTempPath(), "HRManagement.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var databasePath = Path.Combine(directory, "hr-management.db");

        try
        {
            var options = new DbContextOptionsBuilder<HrManagementDbContext>()
                .UseSqlite($"Data Source={databasePath};Foreign Keys=True;Pooling=False")
                .Options;
            var initializer = new DatabaseInitializer(new TestDbContextFactory(options));

            var first = await initializer.InitializeAsync(CancellationToken.None);
            var second = await initializer.InitializeAsync(CancellationToken.None);

            Assert.True(first.IsSuccess);
            Assert.True(second.IsSuccess);
            await using var context = new HrManagementDbContext(options);
            Assert.Equal(7, await context.DocumentCategories.CountAsync());
            Assert.Empty(await context.Employees.ToListAsync());
            var journalMode = await ReadPragmaAsync(context, "journal_mode");
            var foreignKeys = await ReadPragmaAsync(context, "foreign_keys");
            Assert.Equal("wal", journalMode);
            Assert.Equal("1", foreignKeys);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static async Task<string> ReadPragmaAsync(HrManagementDbContext context, string pragma)
    {
        await context.Database.OpenConnectionAsync();
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = $"PRAGMA {pragma};";
        var value = await command.ExecuteScalarAsync();
        return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private sealed class TestDbContextFactory(DbContextOptions<HrManagementDbContext> options)
        : IDbContextFactory<HrManagementDbContext>
    {
        public HrManagementDbContext CreateDbContext() => new(options);
    }
}
