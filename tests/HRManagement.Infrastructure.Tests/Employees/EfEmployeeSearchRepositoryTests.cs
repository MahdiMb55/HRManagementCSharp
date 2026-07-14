using System.Globalization;
using HRManagement.Application.Employees;
using HRManagement.Application.Employees.Search;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Employees;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Tests.Employees;

public sealed class EfEmployeeSearchRepositoryTests
{
    [Fact]
    public async Task SearchAsync_FiltersDepartedAndPagesAndSortsInSqlite()
    {
        using var database = new TemporarySearchDatabase();
        await database.InitializeAsync();
        var writer = new EfEmployeeWriteRepository(database.Factory);
        var employeeIds = new List<long>();
        for (var index = 1; index <= 26; index++)
        {
            var save = new ValidatedEmployeeSave(
                null,
                index == 1 ? "علی" : $"نام {index}",
                $"نام خانوادگی {index:D2}",
                index.ToString("D4", CultureInfo.InvariantCulture),
                CreateNationalCode(100_000_000 + index),
                null,
                Gender.Male,
                null,
                index == 1 ? "09121234567" : null,
                database.NowUtc,
                "employee.created");
            employeeIds.Add(await writer.SaveAsync(save, CancellationToken.None));
        }

        await database.MarkDepartedAsync(employeeIds[^1]);
        var repository = new EfEmployeeSearchRepository(database.Factory);
        var sort = new EmployeeSort(EmployeeSortField.PersonnelNumber, SortDirection.Ascending);

        var defaultPage = await repository.SearchAsync(
            new EmployeeSearchRequest(null, 1, 25, sort, EmployeeFilter.Default),
            CancellationToken.None);
        var includedPageTwo = await repository.SearchAsync(
            new EmployeeSearchRequest(null, 2, 25, sort, new EmployeeFilter([], [], [], true)),
            CancellationToken.None);
        var normalizedSearch = await repository.SearchAsync(
            new EmployeeSearchRequest("علی", 1, 25, sort, EmployeeFilter.Default),
            CancellationToken.None);

        Assert.Equal(25, defaultPage.TotalCount);
        Assert.Equal(25, defaultPage.Items.Count);
        Assert.Equal("0001", defaultPage.Items[0].PersonnelNumber);
        Assert.Equal(26, includedPageTwo.TotalCount);
        Assert.Single(includedPageTwo.Items);
        Assert.Equal("0026", includedPageTwo.Items[0].PersonnelNumber);
        Assert.Single(normalizedSearch.Items);
        Assert.Equal("علی", normalizedSearch.Items[0].FirstName);
    }

    private static string CreateNationalCode(int nineDigitPrefix)
    {
        var prefix = nineDigitPrefix.ToString("D9", CultureInfo.InvariantCulture);
        var sum = prefix.Select((character, index) => (character - '0') * (10 - index)).Sum();
        var remainder = sum % 11;
        var checkDigit = remainder < 2 ? remainder : 11 - remainder;
        return prefix + checkDigit.ToString(CultureInfo.InvariantCulture);
    }

    private sealed class TemporarySearchDatabase : IDisposable
    {
        private readonly string directory;

        public TemporarySearchDatabase()
        {
            directory = Path.Combine(Path.GetTempPath(), "HRManagement.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "search.db");
            var options = new DbContextOptionsBuilder<HrManagementDbContext>()
                .UseSqlite($"Data Source={path};Foreign Keys=True;Pooling=False")
                .Options;
            Factory = new TestDbContextFactory(options);
        }

        public DateTime NowUtc { get; } = new(2026, 7, 14, 8, 0, 0, DateTimeKind.Utc);
        public TestDbContextFactory Factory { get; }

        public async Task InitializeAsync() =>
            await new DatabaseInitializer(Factory).InitializeAsync(CancellationToken.None);

        public async Task MarkDepartedAsync(long employeeId)
        {
            await using var context = Factory.CreateDbContext();
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                INSERT INTO EmploymentPeriods (EmployeeId, HireDate, CreatedAtUtc, UpdatedAtUtc)
                VALUES ({employeeId}, {"2020-01-01"}, {NowUtc}, {NowUtc});
                """);
            var periodId = await context.EmploymentPeriods
                .Where(period => period.EmployeeId == employeeId)
                .Select(period => period.Id)
                .SingleAsync();
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                INSERT INTO EmployeeStatusHistories (EmployeeId, EmploymentPeriodId, Status, StartDate, EndDate, CreatedAtUtc)
                VALUES ({employeeId}, {periodId}, {(int)EmploymentStatus.Resigned}, {"2026-01-01"}, NULL, {NowUtc});
                """);
        }

        public void Dispose() => Directory.Delete(directory, recursive: true);
    }

    private sealed class TestDbContextFactory(DbContextOptions<HrManagementDbContext> options)
        : IDbContextFactory<HrManagementDbContext>
    {
        public HrManagementDbContext CreateDbContext() => new(options);
    }
}
