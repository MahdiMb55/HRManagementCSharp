using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Persistence;

public sealed class HrManagementDbContext(DbContextOptions<HrManagementDbContext> options)
    : DbContext(options)
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeDependent> EmployeeDependents => Set<EmployeeDependent>();
    public DbSet<EmploymentPeriod> EmploymentPeriods => Set<EmploymentPeriod>();
    public DbSet<EmploymentTermination> EmploymentTerminations => Set<EmploymentTermination>();
    public DbSet<EmployeeStatusHistory> EmployeeStatusHistories => Set<EmployeeStatusHistory>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistories => Set<EmployeeDepartmentHistory>();
    public DbSet<Responsibility> Responsibilities => Set<Responsibility>();
    public DbSet<EmployeeResponsibilityHistory> EmployeeResponsibilityHistories => Set<EmployeeResponsibilityHistory>();
    public DbSet<EducationRecord> EducationRecords => Set<EducationRecord>();
    public DbSet<EmployeeBankAccount> EmployeeBankAccounts => Set<EmployeeBankAccount>();
    public DbSet<EmployeeAccessCard> EmployeeAccessCards => Set<EmployeeAccessCard>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ManagedFile> ManagedFiles => Set<ManagedFile>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<EmployeeDocumentVersion> EmployeeDocumentVersions => Set<EmployeeDocumentVersion>();
    public DbSet<ContractAttachment> ContractAttachments => Set<ContractAttachment>();
    public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<BackupHistory> BackupHistories => Set<BackupHistory>();
    public DbSet<LetterTemplate> LetterTemplates => Set<LetterTemplate>();
    public DbSet<IssuedLetter> IssuedLetters => Set<IssuedLetter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrManagementDbContext).Assembly);
    }
}
