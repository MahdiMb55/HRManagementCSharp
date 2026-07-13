using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HRManagement.Infrastructure.Tests.Persistence;

public sealed class ModelConstraintTests
{
    private static readonly string[] ExpectedTables =
    [
        "AppSettings", "AuditLogs", "BackupHistories", "CompanyProfiles",
        "ContractAttachments", "Contracts", "Departments", "DocumentCategories",
        "EducationRecords", "EmployeeAccessCards", "EmployeeBankAccounts",
        "EmployeeDepartmentHistories", "EmployeeDependents", "EmployeeDocumentVersions",
        "EmployeeDocuments", "EmployeeResponsibilityHistories", "EmployeeStatusHistories",
        "Employees", "EmploymentPeriods", "EmploymentTerminations",
        "IssuedLetters", "LetterTemplates", "ManagedFiles", "Persons", "Responsibilities",
    ];

    [Fact]
    public void Model_ContainsEveryApprovedTable()
    {
        using var context = CreateContext();

        var tables = context.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(table => table is not null)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ExpectedTables, tables);
    }

    [Theory]
    [InlineData("Person", "NationalCode", null)]
    [InlineData("Employee", "PersonId", null)]
    [InlineData("Employee", "PersonnelNumber", null)]
    [InlineData("AppSetting", "Key", null)]
    [InlineData("EmploymentPeriod", "EmployeeId", "EndDate IS NULL")]
    [InlineData("EmployeeDepartmentHistory", "EmployeeId", "EndDate IS NULL")]
    [InlineData("EmployeeResponsibilityHistory", "EmployeeId", "EndDate IS NULL AND IsPrimary = 1")]
    [InlineData("EmployeeBankAccount", "EmployeeId", "IsActive = 1 AND IsPrimary = 1 AND IsDeleted = 0")]
    [InlineData("EmployeeAccessCard", "EmployeeId", "EndDate IS NULL")]
    [InlineData("EmployeeDocumentVersion", "EmployeeDocumentId", "IsCurrent = 1")]
    public void Model_HasRequiredUniqueIndex(string entityName, string propertyName, string? filter)
    {
        using var context = CreateContext();
        var entity = context.Model.GetEntityTypes().Single(type => type.ClrType.Name == entityName);

        var index = entity.GetIndexes().Single(candidate =>
            candidate.IsUnique &&
            candidate.Properties.Select(property => property.Name).Contains(propertyName) &&
            candidate.GetFilter() == filter);

        Assert.Equal(filter, index.GetFilter());
    }

    [Fact]
    public void Model_StoresDateOnlyAsIsoText()
    {
        using var context = CreateContext();
        var property = context.Model.FindEntityType("HRManagement.Domain.Entities.EmploymentPeriod")!
            .FindProperty("HireDate")!;

        Assert.Equal(typeof(string), property.GetTypeMapping().Converter!.ProviderClrType);
    }

    private static HrManagementDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HrManagementDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        return new HrManagementDbContext(options);
    }
}
