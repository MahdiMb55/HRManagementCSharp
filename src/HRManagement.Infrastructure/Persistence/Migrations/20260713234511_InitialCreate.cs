using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    ValueType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<long>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OldValuesJson = table.Column<string>(type: "TEXT", nullable: true),
                    NewValuesJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BackupType = table.Column<int>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WasSuccessful = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ParentDepartmentId = table.Column<long>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Departments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsSystemCategory = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagedFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RelativePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    FileHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsInTrash = table.Column<bool>(type: "INTEGER", nullable: false),
                    MovedToTrashAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TrashRelativePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedFirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedLastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NationalCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    BirthDate = table.Column<string>(type: "TEXT", nullable: true),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Responsibilities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsibilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NationalIdentifier = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    LogoFileId = table.Column<long>(type: "INTEGER", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                    table.CheckConstraint("CK_CompanyProfiles_Singleton", "Id = 1");
                    table.ForeignKey(
                        name: "FK_CompanyProfiles_ManagedFiles_LogoFileId",
                        column: x => x.LogoFileId,
                        principalTable: "ManagedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LetterTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ManagedFileId = table.Column<long>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterTemplates_ManagedFiles_ManagedFileId",
                        column: x => x.ManagedFileId,
                        principalTable: "ManagedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonId = table.Column<long>(type: "INTEGER", nullable: false),
                    PersonnelNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NormalizedPersonnelNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FatherName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BirthCertificateNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BirthCertificateIssuePlace = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    MobileNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NormalizedMobileNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: true),
                    EmergencyContactName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    EmergencyContactRelation = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    HomeAddress = table.Column<string>(type: "TEXT", nullable: true),
                    MaritalStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    BloodType = table.Column<int>(type: "INTEGER", nullable: true),
                    MilitaryServiceStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    InsuranceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SpecialNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ProfilePhotoFileId = table.Column<long>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_ManagedFiles_ProfilePhotoFileId",
                        column: x => x.ProfilePhotoFileId,
                        principalTable: "ManagedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Employees_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EducationRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    Degree = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    InstitutionName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GraduationYear = table.Column<int>(type: "INTEGER", nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationRecords_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBankAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    BankName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CardNumber = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Iban = table.Column<string>(type: "TEXT", maxLength: 26, nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBankAccounts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDependents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    DependentPersonId = table.Column<long>(type: "INTEGER", nullable: false),
                    RelationshipType = table.Column<int>(type: "INTEGER", nullable: false),
                    EducationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    InsuranceStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDependents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDependents_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDependents_Persons_DependentPersonId",
                        column: x => x.DependentPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<long>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDocuments_DocumentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DocumentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeDocuments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentPeriods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    HireDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentPeriods_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssuedLetters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    LetterTemplateId = table.Column<long>(type: "INTEGER", nullable: false),
                    LetterNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IssueDate = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OutputFileId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuedLetters_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuedLetters_LetterTemplates_LetterTemplateId",
                        column: x => x.LetterTemplateId,
                        principalTable: "LetterTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuedLetters_ManagedFiles_OutputFileId",
                        column: x => x.OutputFileId,
                        principalTable: "ManagedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDocumentVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeDocumentId = table.Column<long>(type: "INTEGER", nullable: false),
                    ManagedFileId = table.Column<long>(type: "INTEGER", nullable: false),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDocumentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDocumentVersions_EmployeeDocuments_EmployeeDocumentId",
                        column: x => x.EmployeeDocumentId,
                        principalTable: "EmployeeDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDocumentVersions_ManagedFiles_ManagedFileId",
                        column: x => x.ManagedFileId,
                        principalTable: "ManagedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    EmploymentPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    ContractNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContractType = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_EmploymentPeriods_EmploymentPeriodId",
                        column: x => x.EmploymentPeriodId,
                        principalTable: "EmploymentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAccessCards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    EmploymentPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    CardNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAccessCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAccessCards_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeAccessCards_EmploymentPeriods_EmploymentPeriodId",
                        column: x => x.EmploymentPeriodId,
                        principalTable: "EmploymentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDepartmentHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    EmploymentPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    DepartmentId = table.Column<long>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    TransferDescription = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDepartmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDepartmentHistories_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeDepartmentHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDepartmentHistories_EmploymentPeriods_EmploymentPeriodId",
                        column: x => x.EmploymentPeriodId,
                        principalTable: "EmploymentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeResponsibilityHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    EmploymentPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    ResponsibilityId = table.Column<long>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeResponsibilityHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibilityHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibilityHistories_EmploymentPeriods_EmploymentPeriodId",
                        column: x => x.EmploymentPeriodId,
                        principalTable: "EmploymentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeResponsibilityHistories_Responsibilities_ResponsibilityId",
                        column: x => x.ResponsibilityId,
                        principalTable: "Responsibilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeStatusHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<long>(type: "INTEGER", nullable: false),
                    EmploymentPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeStatusHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeStatusHistories_EmploymentPeriods_EmploymentPeriodId",
                        column: x => x.EmploymentPeriodId,
                        principalTable: "EmploymentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTerminations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmploymentPeriodId = table.Column<long>(type: "INTEGER", nullable: false),
                    TerminationType = table.Column<int>(type: "INTEGER", nullable: false),
                    TerminationDate = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTerminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentTerminations_EmploymentPeriods_EmploymentPeriodId",
                        column: x => x.EmploymentPeriodId,
                        principalTable: "EmploymentPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractAttachments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractId = table.Column<long>(type: "INTEGER", nullable: false),
                    ManagedFileId = table.Column<long>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractAttachments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractAttachments_ManagedFiles_ManagedFileId",
                        column: x => x.ManagedFileId,
                        principalTable: "ManagedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DocumentCategories",
                columns: new[] { "Id", "CreatedAtUtc", "Description", "IsActive", "IsSystemCategory", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "مدارک کارت ملی", true, true, "کارت ملی", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "مدارک شناسنامه", true, true, "شناسنامه", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "مدارک تحصیلی", true, true, "تحصیلات", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "مدارک قرارداد", true, true, "قرارداد", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "مدارک بیمه", true, true, "بیمه", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "مدارک نظام وظیفه", true, true, "نظام وظیفه", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7L, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "سایر مدارک", true, true, "سایر", new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAtUtc",
                table: "AuditLogs",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId_CreatedAtUtc",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BackupHistories_BackupType_WasSuccessful",
                table: "BackupHistories",
                columns: new[] { "BackupType", "WasSuccessful" });

            migrationBuilder.CreateIndex(
                name: "IX_BackupHistories_StartedAtUtc",
                table: "BackupHistories",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_LogoFileId",
                table: "CompanyProfiles",
                column: "LogoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAttachments_ContractId_IsDeleted",
                table: "ContractAttachments",
                columns: new[] { "ContractId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractAttachments_ManagedFileId",
                table: "ContractAttachments",
                column: "ManagedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractNumber",
                table: "Contracts",
                column: "ContractNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractType_IsDeleted",
                table: "Contracts",
                columns: new[] { "ContractType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_EmployeeId",
                table: "Contracts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_EmploymentPeriodId_StartDate_EndDate",
                table: "Contracts",
                columns: new[] { "EmploymentPeriodId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsActive",
                table: "Departments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_NormalizedName",
                table: "Departments",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategories_IsActive",
                table: "DocumentCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategories_Name",
                table: "DocumentCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationRecords_Degree_IsDeleted",
                table: "EducationRecords",
                columns: new[] { "Degree", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EducationRecords_EmployeeId",
                table: "EducationRecords",
                column: "EmployeeId",
                unique: true,
                filter: "IsPrimary = 1 AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAccessCards_CardNumber",
                table: "EmployeeAccessCards",
                column: "CardNumber",
                unique: true,
                filter: "EndDate IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAccessCards_EmployeeId",
                table: "EmployeeAccessCards",
                column: "EmployeeId",
                unique: true,
                filter: "EndDate IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAccessCards_EmployeeId_StartDate_EndDate",
                table: "EmployeeAccessCards",
                columns: new[] { "EmployeeId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAccessCards_EmploymentPeriodId",
                table: "EmployeeAccessCards",
                column: "EmploymentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_CardNumber",
                table: "EmployeeBankAccounts",
                column: "CardNumber",
                unique: true,
                filter: "CardNumber IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_EmployeeId",
                table: "EmployeeBankAccounts",
                column: "EmployeeId",
                unique: true,
                filter: "IsActive = 1 AND IsPrimary = 1 AND IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_EmployeeId_IsDeleted_IsActive",
                table: "EmployeeBankAccounts",
                columns: new[] { "EmployeeId", "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_Iban",
                table: "EmployeeBankAccounts",
                column: "Iban",
                unique: true,
                filter: "Iban IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentHistories_DepartmentId_StartDate_EndDate",
                table: "EmployeeDepartmentHistories",
                columns: new[] { "DepartmentId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentHistories_EmployeeId",
                table: "EmployeeDepartmentHistories",
                column: "EmployeeId",
                unique: true,
                filter: "EndDate IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentHistories_EmployeeId_StartDate_EndDate",
                table: "EmployeeDepartmentHistories",
                columns: new[] { "EmployeeId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDepartmentHistories_EmploymentPeriodId",
                table: "EmployeeDepartmentHistories",
                column: "EmploymentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDependents_DependentPersonId",
                table: "EmployeeDependents",
                column: "DependentPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDependents_EmployeeId_DependentPersonId_RelationshipType",
                table: "EmployeeDependents",
                columns: new[] { "EmployeeId", "DependentPersonId", "RelationshipType" },
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDependents_IsDeleted",
                table: "EmployeeDependents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_CategoryId",
                table: "EmployeeDocuments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocuments_EmployeeId_CategoryId_IsDeleted",
                table: "EmployeeDocuments",
                columns: new[] { "EmployeeId", "CategoryId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentVersions_EmployeeDocumentId",
                table: "EmployeeDocumentVersions",
                column: "EmployeeDocumentId",
                unique: true,
                filter: "IsCurrent = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentVersions_EmployeeDocumentId_VersionNumber",
                table: "EmployeeDocumentVersions",
                columns: new[] { "EmployeeDocumentId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDocumentVersions_ManagedFileId",
                table: "EmployeeDocumentVersions",
                column: "ManagedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibilityHistories_EmployeeId",
                table: "EmployeeResponsibilityHistories",
                column: "EmployeeId",
                unique: true,
                filter: "EndDate IS NULL AND IsPrimary = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibilityHistories_EmployeeId_StartDate_EndDate",
                table: "EmployeeResponsibilityHistories",
                columns: new[] { "EmployeeId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibilityHistories_EmploymentPeriodId",
                table: "EmployeeResponsibilityHistories",
                column: "EmploymentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeResponsibilityHistories_ResponsibilityId_StartDate_EndDate",
                table: "EmployeeResponsibilityHistories",
                columns: new[] { "ResponsibilityId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsDeleted",
                table: "Employees",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NormalizedMobileNumber",
                table: "Employees",
                column: "NormalizedMobileNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NormalizedPersonnelNumber",
                table: "Employees",
                column: "NormalizedPersonnelNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PersonId",
                table: "Employees",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PersonnelNumber",
                table: "Employees",
                column: "PersonnelNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ProfilePhotoFileId",
                table: "Employees",
                column: "ProfilePhotoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatusHistories_EmployeeId",
                table: "EmployeeStatusHistories",
                column: "EmployeeId",
                unique: true,
                filter: "EndDate IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatusHistories_EmploymentPeriodId",
                table: "EmployeeStatusHistories",
                column: "EmploymentPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatusHistories_Status_StartDate_EndDate",
                table: "EmployeeStatusHistories",
                columns: new[] { "Status", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentPeriods_EmployeeId",
                table: "EmploymentPeriods",
                column: "EmployeeId",
                unique: true,
                filter: "EndDate IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentPeriods_EmployeeId_HireDate_EndDate",
                table: "EmploymentPeriods",
                columns: new[] { "EmployeeId", "HireDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentTerminations_EmploymentPeriodId",
                table: "EmploymentTerminations",
                column: "EmploymentPeriodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssuedLetters_EmployeeId_IssueDate",
                table: "IssuedLetters",
                columns: new[] { "EmployeeId", "IssueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_IssuedLetters_LetterNumber",
                table: "IssuedLetters",
                column: "LetterNumber");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedLetters_LetterTemplateId",
                table: "IssuedLetters",
                column: "LetterTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedLetters_OutputFileId",
                table: "IssuedLetters",
                column: "OutputFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterTemplates_IsActive",
                table: "LetterTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_LetterTemplates_ManagedFileId",
                table: "LetterTemplates",
                column: "ManagedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagedFiles_FileHash",
                table: "ManagedFiles",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_ManagedFiles_IsInTrash",
                table: "ManagedFiles",
                column: "IsInTrash");

            migrationBuilder.CreateIndex(
                name: "IX_ManagedFiles_RelativePath",
                table: "ManagedFiles",
                column: "RelativePath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NationalCode",
                table: "Persons",
                column: "NationalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NormalizedLastName_NormalizedFirstName",
                table: "Persons",
                columns: new[] { "NormalizedLastName", "NormalizedFirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Responsibilities_IsActive",
                table: "Responsibilities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibilities_NormalizedTitle",
                table: "Responsibilities",
                column: "NormalizedTitle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BackupHistories");

            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "ContractAttachments");

            migrationBuilder.DropTable(
                name: "EducationRecords");

            migrationBuilder.DropTable(
                name: "EmployeeAccessCards");

            migrationBuilder.DropTable(
                name: "EmployeeBankAccounts");

            migrationBuilder.DropTable(
                name: "EmployeeDepartmentHistories");

            migrationBuilder.DropTable(
                name: "EmployeeDependents");

            migrationBuilder.DropTable(
                name: "EmployeeDocumentVersions");

            migrationBuilder.DropTable(
                name: "EmployeeResponsibilityHistories");

            migrationBuilder.DropTable(
                name: "EmployeeStatusHistories");

            migrationBuilder.DropTable(
                name: "EmploymentTerminations");

            migrationBuilder.DropTable(
                name: "IssuedLetters");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "EmployeeDocuments");

            migrationBuilder.DropTable(
                name: "Responsibilities");

            migrationBuilder.DropTable(
                name: "LetterTemplates");

            migrationBuilder.DropTable(
                name: "EmploymentPeriods");

            migrationBuilder.DropTable(
                name: "DocumentCategories");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ManagedFiles");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
