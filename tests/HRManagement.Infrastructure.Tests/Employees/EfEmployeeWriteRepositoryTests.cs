using HRManagement.Application.Employees;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Employees;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Tests.Employees;

public sealed class EfEmployeeWriteRepositoryTests
{
    [Fact]
    public async Task SaveAsync_PersistsEmployeePersonAndAuditInOneDatabase()
    {
        using var database = new TemporaryDatabase();
        await database.InitializeAsync();
        var repository = new EfEmployeeWriteRepository(database.Factory);
        var save = new ValidatedEmployeeSave(
            null, "علی", "رضایی", "0012", "0013548581", "حسن", Gender.Male,
            new DateOnly(1990, 1, 1), "09121234567", database.NowUtc, "employee.created");

        var employeeId = await repository.SaveAsync(save, CancellationToken.None);

        await using var context = database.Factory.CreateDbContext();
        Assert.Equal(1, await context.Persons.CountAsync());
        Assert.Equal(1, await context.Employees.CountAsync());
        var audit = Assert.Single(await context.AuditLogs.ToListAsync());
        Assert.Equal(employeeId, audit.EntityId);
        Assert.Equal("employee.created", audit.Action);
        Assert.True(await repository.NationalCodeExistsAsync("0013548581", null, CancellationToken.None));
        Assert.True(await repository.PersonnelNumberExistsAsync("0012", null, CancellationToken.None));
    }

    [Fact]
    public async Task SaveAsync_UpdatesBasicFieldsWithoutCreatingSecondIdentity()
    {
        using var database = new TemporaryDatabase();
        await database.InitializeAsync();
        var repository = new EfEmployeeWriteRepository(database.Factory);
        var original = new ValidatedEmployeeSave(
            null, "علی", "رضایی", "0012", "0013548581", null, Gender.Male,
            null, null, database.NowUtc, "employee.created");
        var employeeId = await repository.SaveAsync(original, CancellationToken.None);
        var updated = original with
        {
            EmployeeId = employeeId,
            FirstName = "امیرعلی",
            FatherName = "حسن",
            MobileNumber = "09121234567",
            AuditAction = "employee.updated",
        };

        await repository.SaveAsync(updated, CancellationToken.None);
        var edit = await repository.FindForEditAsync(employeeId, CancellationToken.None);

        Assert.NotNull(edit);
        Assert.Equal("امیرعلی", edit.FirstName);
        Assert.Equal("حسن", edit.FatherName);
        Assert.Equal("09121234567", edit.MobileNumber);
        await using var context = database.Factory.CreateDbContext();
        Assert.Equal(1, await context.Persons.CountAsync());
        Assert.Equal(2, await context.AuditLogs.CountAsync());
    }

    private sealed class TemporaryDatabase : IDisposable
    {
        private readonly string directory;

        public TemporaryDatabase()
        {
            directory = Path.Combine(Path.GetTempPath(), "HRManagement.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "employee.db");
            var options = new DbContextOptionsBuilder<HrManagementDbContext>()
                .UseSqlite($"Data Source={path};Foreign Keys=True;Pooling=False")
                .Options;
            Factory = new TestDbContextFactory(options);
        }

        public DateTime NowUtc { get; } = new(2026, 7, 14, 8, 0, 0, DateTimeKind.Utc);
        public TestDbContextFactory Factory { get; }

        public async Task InitializeAsync() =>
            await new DatabaseInitializer(Factory).InitializeAsync(CancellationToken.None);

        public void Dispose() => Directory.Delete(directory, recursive: true);
    }

    private sealed class TestDbContextFactory(DbContextOptions<HrManagementDbContext> options)
        : IDbContextFactory<HrManagementDbContext>
    {
        public HrManagementDbContext CreateDbContext() => new(options);
    }
}
