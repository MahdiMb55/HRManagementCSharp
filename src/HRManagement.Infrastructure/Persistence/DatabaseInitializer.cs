using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Persistence;

public interface IDatabaseInitializer
{
    Task<DatabaseInitializationResult> InitializeAsync(CancellationToken cancellationToken);
}

public sealed class DatabaseInitializer(IDbContextFactory<HrManagementDbContext> contextFactory)
    : IDatabaseInitializer
{
    public async Task<DatabaseInitializationResult> InitializeAsync(CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.OpenConnectionAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken);

        await using (var journalCommand = context.Database.GetDbConnection().CreateCommand())
        {
            journalCommand.CommandText = "PRAGMA journal_mode=WAL;";
            await journalCommand.ExecuteScalarAsync(cancellationToken);
        }

        await using (var foreignKeysCommand = context.Database.GetDbConnection().CreateCommand())
        {
            foreignKeysCommand.CommandText = "PRAGMA foreign_keys=ON;";
            await foreignKeysCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        return DatabaseInitializationResult.Success();
    }
}
