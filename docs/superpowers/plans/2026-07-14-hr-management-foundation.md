# HRManagement Version-One Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Deliver a production-quality offline Persian HR application, beginning with a verified foundation and working employee list/create/edit slice.

**Architecture:** Four projects separate pure domain rules, application use cases, EF Core/file implementations, and Passive View WinForms. SQLite queries are projected and paginated; managed files remain outside the database; startup integrity distinguishes first run from data loss.

**Tech Stack:** C# 14, .NET 10 LTS, Windows Forms, EF Core 10, SQLite, Microsoft.Extensions.Hosting/Logging/DependencyInjection, Serilog file sink, xUnit, ClosedXML, Open XML, Inno Setup.

## Global Constraints

- Root namespace is `HRManagement`; user-facing language is Persian and layouts are RTL.
- Target `net10.0` except WinForms, which targets `net10.0-windows` with `<UseWindowsForms>true</UseWindowsForms>`.
- Domain references no other project; Application references Domain; Infrastructure references Domain and Application; WinForms references Application and Infrastructure.
- Store business dates as Gregorian `DateOnly` and SQLite ISO `yyyy-MM-dd`; display/input Jalali through WinForms adapters; technical timestamps are UTC.
- Resolve `Data` from `AppContext.BaseDirectory`; store only validated relative managed-file paths; never store file bytes in SQLite.
- Use one DbContext per operation, projected/paginated SQLite queries, cancellation, and stale-result protection.
- Use red-green-refactor for domain and application behavior and isolated temporary paths/databases for infrastructure tests.
- No payroll, attendance, leave, authentication, server, network, cloud, branches, heavy UI framework, Office Interop, fake employees, or external publication.

---

## Milestone 1 — Foundation and employee vertical slice (execute now)

### Task 1: Normalize solution and package system

**Files:**
- Create: `HRManagement.sln`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
- Create: `src/HRManagement.Domain/HRManagement.Domain.csproj`
- Create: `src/HRManagement.Application/HRManagement.Application.csproj`
- Create: `src/HRManagement.Infrastructure/HRManagement.Infrastructure.csproj`
- Move/modify: `src/HRManagement.WinForms/HRManagement.WinForms.csproj`
- Create: `tests/HRManagement.Domain.Tests/HRManagement.Domain.Tests.csproj`
- Create: `tests/HRManagement.Application.Tests/HRManagement.Application.Tests.csproj`
- Create: `tests/HRManagement.Infrastructure.Tests/HRManagement.Infrastructure.Tests.csproj`
- Modify: `.gitignore`

**Interfaces:**
- Consumes: .NET SDK `10.0.301` and SDK-style projects.
- Produces: build graph `Domain <- Application <- Infrastructure <- WinForms` and test references to their target layers.

- [ ] Generate the seven projects and `HRManagement.sln`; add projects and references with `dotnet sln` and `dotnet add reference`.
- [ ] Centralize versions for EF Core SQLite/Design `10.0.9`, Microsoft.Extensions `10.0.1`, xUnit `2.9.3`, test SDK `18.0.1`, coverlet `6.0.4`, Serilog.Extensions.Hosting `10.0.0`, Serilog.Sinks.File `7.0.0`, ClosedXML `0.105.0`, and DocumentFormat.OpenXml `3.4.1`.
- [ ] Run `dotnet restore HRManagement.sln`; expect exit 0.
- [ ] Run `dotnet build HRManagement.sln`; expect 0 errors.
- [ ] Run `dotnet test HRManagement.sln`; expect all discovered tests pass.
- [ ] Commit with `chore: scaffold layered .NET 10 solution`.

### Task 2: Add identity and text primitives test-first

**Files:**
- Create: `tests/HRManagement.Domain.Tests/Identity/NationalCodeTests.cs`
- Create: `tests/HRManagement.Domain.Tests/Identity/PersonnelNumberTests.cs`
- Create: `tests/HRManagement.Domain.Tests/Text/PersianTextNormalizerTests.cs`
- Create: `src/HRManagement.Domain/Identity/NationalCode.cs`
- Create: `src/HRManagement.Domain/Identity/PersonnelNumber.cs`
- Create: `src/HRManagement.Domain/Text/PersianTextNormalizer.cs`

**Interfaces:**
- Consumes: no production interfaces.
- Produces: `NationalCode.Create(string?) : ValidationResult<NationalCode>`, `PersonnelNumber.Create(string?) : ValidationResult<PersonnelNumber>`, and `PersianTextNormalizer.Normalize(string?) : string`.

- [ ] Write tests proving mandatory/ten-digit/check-digit/repeated-digit national-code behavior and Persian/Arabic digit normalization.
- [ ] Run `dotnet test tests/HRManagement.Domain.Tests --filter FullyQualifiedName~Identity`; expect failures because value types do not exist.
- [ ] Add immutable value records and `ValidationResult<T>` in `src/HRManagement.Domain/Common/ValidationResult.cs` with Persian error codes/messages.
- [ ] Run the identity tests; expect pass.
- [ ] Write text tests for trim, repeated spaces, half-space, Yeh and Kaf normalization; run and observe missing helper failure.
- [ ] Add the pure normalizer, rerun the focused tests, then run the Domain suite.
- [ ] Commit with `feat: add domain identity validation`.

### Task 3: Add domain entities, enums and temporal rules test-first

**Files:**
- Create: `src/HRManagement.Domain/Enums/*.cs`
- Create: `src/HRManagement.Domain/Entities/Person.cs`, `Employee.cs`, `EmploymentPeriod.cs`, `Department.cs`, `Responsibility.cs`
- Create: `src/HRManagement.Domain/Common/Entity.cs`, `AuditableEntity.cs`, `DateRange.cs`
- Create: `tests/HRManagement.Domain.Tests/Common/DateRangeTests.cs`
- Create: `tests/HRManagement.Domain.Tests/Entities/PersonTests.cs`, `EmploymentPeriodTests.cs`, `DepartmentTests.cs`

**Interfaces:**
- Consumes: `NationalCode`, `PersonnelNumber`, `ValidationResult<T>`.
- Produces: stable explicit enums and focused entity constructors/mutators; `DateRange.Overlaps(DateRange) : bool`; `Department.WouldCreateCycle(long departmentId, long candidateParentId, IReadOnlyDictionary<long,long?> parents) : bool`.

- [ ] Add failing tests for reversed date range, inclusive overlap, future birth date, one open employment period, and hierarchy cycles.
- [ ] Run `dotnet test tests/HRManagement.Domain.Tests --filter FullyQualifiedName~DateRange|FullyQualifiedName~Entities`; expect compile/failing assertions for missing behavior.
- [ ] Add minimal enums/entities/rules with private setters suitable for EF Core and focused behavior methods.
- [ ] Rerun focused tests, refactor duplication while green, then run the full Domain suite.
- [ ] Commit with `feat: add core HR domain model`.

### Task 4: Complete domain persistence model and EF configurations

**Files:**
- Create: remaining entity files under `src/HRManagement.Domain/Entities/`
- Create: `src/HRManagement.Infrastructure/Persistence/HrManagementDbContext.cs`
- Create: one `*Configuration.cs` per entity under `src/HRManagement.Infrastructure/Persistence/Configurations/`
- Create: `src/HRManagement.Infrastructure/Persistence/DateOnlyTextConverter.cs`
- Create: `tests/HRManagement.Infrastructure.Tests/Persistence/ModelConstraintTests.cs`

**Interfaces:**
- Consumes: all Domain entities/enums.
- Produces: `HrManagementDbContext(DbContextOptions<HrManagementDbContext>)`; DbSets for all 25 approved tables; SQLite keys, relationships, converters and indexes from `docs/05-database-design.md`.

- [ ] Write model tests that inspect EF metadata for every table, relationship, DateOnly converter, required unique index and partial-index filter.
- [ ] Run `dotnet test tests/HRManagement.Infrastructure.Tests --filter FullyQualifiedName~ModelConstraintTests`; expect failure because context/configurations are absent.
- [ ] Add remaining small data entities and explicit configurations; keep overlap/conditional rules out of mapping when SQLite cannot enforce them.
- [ ] Rerun model tests and `dotnet build HRManagement.sln`.
- [ ] Commit with `feat: add complete SQLite persistence model`.

### Task 5: Create migration, initializer and fixed seed

**Files:**
- Create: `src/HRManagement.Infrastructure/Persistence/DesignTimeHrManagementDbContextFactory.cs`
- Create: `src/HRManagement.Infrastructure/Persistence/DatabaseInitializer.cs`
- Create: `src/HRManagement.Infrastructure/Persistence/SystemDocumentCategories.cs`
- Generate: `src/HRManagement.Infrastructure/Persistence/Migrations/*_InitialCreate.cs`, model snapshot
- Create: `tests/HRManagement.Infrastructure.Tests/Persistence/MigrationTests.cs`
- Create: `artifacts/sql/initial.sql`

**Interfaces:**
- Consumes: `HrManagementDbContext`, `IApplicationPaths`.
- Produces: `IDatabaseInitializer.InitializeAsync(CancellationToken) : Task<DatabaseInitializationResult>` and idempotent seed of seven fixed document categories.

- [ ] Write a failing test that migrates a fresh temporary SQLite file and asserts 25 application tables, WAL/foreign keys, indexes and zero employees.
- [ ] Run the migration test; expect failure because no migration exists.
- [ ] Generate `InitialCreate` with `dotnet ef migrations add InitialCreate --project src/HRManagement.Infrastructure --startup-project src/HRManagement.WinForms --output-dir Persistence/Migrations`.
- [ ] Implement initializer and idempotent system-category seed; rerun the migration test twice against the same database.
- [ ] Generate `artifacts/sql/initial.sql` using `dotnet ef migrations script --idempotent ...` and review for all partial indexes and no fake employees.
- [ ] Commit with `feat: add initial database migration`.

### Task 6: Implement application paths and startup integrity test-first

**Files:**
- Create: `src/HRManagement.Application/Abstractions/IApplicationPaths.cs`, `IClock.cs`, `IBackgroundExecutor.cs`
- Create: `src/HRManagement.Application/Startup/StartupIntegrityResult.cs`, `StartupFailureCode.cs`
- Create: `src/HRManagement.Infrastructure/Paths/ApplicationPaths.cs`
- Create: `src/HRManagement.Infrastructure/Startup/StartupIntegrityService.cs`
- Create: `tests/HRManagement.Infrastructure.Tests/Startup/StartupIntegrityServiceTests.cs`

**Interfaces:**
- Consumes: base directory string, `IApplicationPaths`, file-system operations.
- Produces: `IStartupIntegrityService.CheckAsync(CancellationToken) : Task<StartupIntegrityResult>`; paths for Database, Documents, Photos, LetterTemplates, GeneratedLetters, Backups, Trash, Logs and Temp.

- [ ] Write isolated tests for legitimate first run, directory creation, marker creation after successful initialization, unwritable directory, initialized missing database, and existing healthy database.
- [ ] Run the focused tests and observe missing service failures.
- [ ] Implement canonical path resolution, write/delete probe, marker semantics and friendly failure mapping without fallback directories.
- [ ] Rerun focused tests and the Infrastructure suite.
- [ ] Commit with `feat: add startup data integrity checks`.

### Task 7: Add application contracts and employee save use case test-first

**Files:**
- Create: `src/HRManagement.Application/Employees/SaveEmployeeRequest.cs`, `SaveEmployeeResult.cs`, `IEmployeeEditorService.cs`, `EmployeeEditorService.cs`
- Create: `src/HRManagement.Application/Abstractions/IEmployeeRepository.cs`, `IAuditWriter.cs`, `IUnitOfWork.cs`
- Create: `tests/HRManagement.Application.Tests/Employees/EmployeeEditorServiceTests.cs`
- Create: `src/HRManagement.Infrastructure/Employees/EfEmployeeRepository.cs`, `EfAuditWriter.cs`, `EfUnitOfWork.cs`
- Create: `tests/HRManagement.Infrastructure.Tests/Employees/EfEmployeeEditorIntegrationTests.cs`

**Interfaces:**
- Consumes: `NationalCode.Create`, `PersonnelNumber.Create`, repository uniqueness checks, `IClock`.
- Produces: `IEmployeeEditorService.GetAsync(long, CancellationToken)`, `SaveAsync(SaveEmployeeRequest, CancellationToken)` with typed `EmployeeSaveFailureCode`; transactional Person/Employee upsert plus audit event.

- [ ] Write application tests for required values, invalid national code, duplicate national/personnel identifiers, female military rule, create, edit without personnel-number mutation, and audit creation.
- [ ] Run the focused tests and observe missing use-case failures.
- [ ] Implement the minimal orchestration against narrow repository methods; rerun Application tests.
- [ ] Write integration tests proving SQLite unique constraints, transaction rollback and persisted audit; observe failure before EF implementations.
- [ ] Add EF implementations, rerun integration and complete test suites.
- [ ] Commit with `feat: add employee create and edit use case`.

### Task 8: Add employee search service and stale-result presenter tests

**Files:**
- Create: `src/HRManagement.Application/Common/PagedResult.cs`
- Create: `src/HRManagement.Application/Employees/Search/EmployeeSearchRequest.cs`, `EmployeeFilter.cs`, `EmployeeSort.cs`, `EmployeeListItemDto.cs`, `IEmployeeSearchService.cs`, `EmployeeSearchService.cs`
- Create: `src/HRManagement.Application/Abstractions/IEmployeeSearchRepository.cs`, `IDelay.cs`
- Create: `src/HRManagement.Infrastructure/Employees/EfEmployeeSearchRepository.cs`
- Create: `tests/HRManagement.Application.Tests/Employees/EmployeeSearchServiceTests.cs`
- Create: `tests/HRManagement.Infrastructure.Tests/Employees/EfEmployeeSearchRepositoryTests.cs`

**Interfaces:**
- Consumes: `PersianTextNormalizer`, `IEmployeeSearchRepository.SearchAsync`, cancellation token.
- Produces: `IEmployeeSearchService.SearchAsync(EmployeeSearchRequest, CancellationToken) : Task<PagedResult<EmployeeListItemDto>>`; supported page sizes 25/50/100 and stable sort keys.

- [ ] Write failing service tests for page-size validation, normalized query, departed-hidden default and filter OR-within/AND-between semantics.
- [ ] Implement request validation and service coordination; rerun Application tests.
- [ ] Write failing SQLite tests that prove projection, ordering before Skip/Take, total count, search fields and no tracking.
- [ ] Implement EF expressions without client evaluation and rerun Infrastructure tests.
- [ ] Commit with `feat: add paged employee search`.

### Task 9: Configure hosting, logging, singleton and startup

**Files:**
- Create: `src/HRManagement.WinForms/Program.cs`, `Startup/ApplicationBootstrapper.cs`, `Startup/ServiceRegistration.cs`, `Startup/GlobalExceptionHandler.cs`, `Startup/SingleInstanceGuard.cs`
- Create: `src/HRManagement.Infrastructure/Logging/LoggingRegistration.cs`, `SensitiveValueMasker.cs`, `LogRetentionService.cs`
- Create: `src/HRManagement.WinForms/Resources/Fonts/Vazirmatn-Regular.ttf`
- Create: `src/HRManagement.WinForms/Formatting/PrivateFontService.cs`, `DevelopmentPerformanceScope.cs`
- Create: `tests/HRManagement.Infrastructure.Tests/Logging/SensitiveValueMaskerTests.cs`

**Interfaces:**
- Consumes: startup integrity, database initializer, paths and Microsoft Host.
- Produces: `ApplicationBootstrapper.BuildAsync`, `SingleInstanceGuard.TryAcquire("Local\\HRManagement.SingleInstance.v1")`, rolling daily files retained 30 days, global Persian error handling, private font family.

- [ ] Add failing masking tests for national code, card and IBAN; implement masker and rerun.
- [ ] Register `IDbContextFactory`, repositories, use cases, presenters, forms, file logging and startup services.
- [ ] Keep `Program.Main` limited to WinForms initialization, guard acquisition, bootstrap and controlled run/shutdown.
- [ ] Build the WinForms project and run all tests.
- [ ] Commit with `feat: add hosted WinForms startup`.

### Task 10: Build Persian shell and real dashboard

**Files:**
- Create: `src/HRManagement.Application/Dashboard/IDashboardService.cs`, `DashboardSnapshot.cs`
- Create: `src/HRManagement.Infrastructure/Dashboard/EfDashboardService.cs`
- Create: `src/HRManagement.WinForms/Forms/MainForm.cs`, `MainForm.Designer.cs`
- Create: `src/HRManagement.WinForms/Controls/DashboardControl.cs`, `DashboardControl.Designer.cs`
- Create: `tests/HRManagement.Infrastructure.Tests/Dashboard/EfDashboardServiceTests.cs`

**Interfaces:**
- Consumes: projected counts from `HrManagementDbContext`.
- Produces: `IDashboardService.GetSnapshotAsync(CancellationToken)` and cached navigation pages for dashboard, employees, departments, responsibilities, archive, backups and settings.

- [ ] Write failing temporary-database tests asserting real zero counts and counts from inserted records.
- [ ] Implement dashboard query and rerun focused tests.
- [ ] Build RTL shell with fixed side navigation, content cache and Persian zero-state dashboard; no fake data or heavy chart.
- [ ] Build and launch to inspect layout and first render.
- [ ] Commit with `feat: add Persian application shell`.

### Task 11: Build employee list Passive View slice

**Files:**
- Create: `src/HRManagement.WinForms/Employees/IEmployeeListView.cs`, `EmployeeListPresenter.cs`, `EmployeeListControl.cs`, `EmployeeListControl.Designer.cs`
- Create: `src/HRManagement.WinForms/Threading/WinFormsDelay.cs`, `ControlDispatcher.cs`
- Create: `tests/HRManagement.Application.Tests/Employees/EmployeeListPresenterTests.cs`

**Interfaces:**
- Consumes: `IEmployeeSearchService`, `IDelay`, view search/page/sort events.
- Produces: `EmployeeListPresenter.InitializeAsync`, `RefreshAsync`, `Dispose`; view methods `SetLoading`, `ShowPage`, `ShowEmpty`, `ShowError`, `ShowSummary`, `SetCommandsEnabled`.

- [ ] Write presenter tests proving 300 ms debounce, cancellation, stale-result rejection, paging, sorting, error/empty/loading states and refresh after edit.
- [ ] Run focused tests and observe missing presenter failures.
- [ ] Implement presenter independent of WinForms controls; rerun tests.
- [ ] Implement read-only RTL DataGridView, page-size picker, search, paging, summary panel, multi-select and shortcuts without photos in grid.
- [ ] Build and inspect search, sort and paging against SQLite.
- [ ] Commit with `feat: add employee list vertical slice`.

### Task 12: Build reusable employee add/edit form

**Files:**
- Create: `src/HRManagement.WinForms/Employees/IEmployeeEditorView.cs`, `EmployeeEditorPresenter.cs`, `EmployeeEditorForm.cs`, `EmployeeEditorForm.Designer.cs`
- Create: `src/HRManagement.WinForms/Formatting/IPersianDateAdapter.cs`, `PersianDateAdapter.cs`
- Create: `tests/HRManagement.Application.Tests/Employees/EmployeeEditorPresenterTests.cs`
- Create: `tests/HRManagement.Domain.Tests/Dates/PersianDateAdapterContractTests.cs`

**Interfaces:**
- Consumes: `IEmployeeEditorService`, `SaveEmployeeRequest`, editor view events.
- Produces: presenter `LoadAsync(long?)`, `SaveAsync`, `CanClose`; form result event `EmployeeSaved(long employeeId)` and Jalali adapter `TryParse(string, out DateOnly)`/`Format(DateOnly?)`.

- [ ] Write failing presenter tests for add/edit mapping, ErrorProvider messages, busy state, success notification, refresh event and dirty-close confirmation.
- [ ] Implement presenter and rerun tests.
- [ ] Write date-adapter tests for valid/invalid Jalali input and round trip; implement via `PersianCalendar` without culture mutation.
- [ ] Build the shared RTL form with the eight approved fields, explicit Save and unsaved-change warning.
- [ ] Connect list add/open actions and refresh only after successful commit.
- [ ] Run complete Debug tests/build and commit with `feat: add basic employee editor`.

### Task 13: Foundation verification and evidence

**Files:**
- Create: `docs/foundation-verification.md`
- Modify: `CHANGELOG.md`

**Interfaces:**
- Consumes: the built WinForms executable, initial migration and all foundation acceptance scenarios.
- Produces: timestamped evidence table with command, exit code/result and manual scenario status.

- [ ] Run `dotnet format HRManagement.sln --verify-no-changes`; format and repeat until exit 0.
- [ ] Run `dotnet restore HRManagement.sln`, `dotnet build HRManagement.sln --configuration Release`, and `dotnet test HRManagement.sln --configuration Release` and record exact counts.
- [ ] Generate/review the migration SQL and apply it with the executable rooted in a fresh temporary directory.
- [ ] Launch and inspect first run, second run, initialized missing database, two-instance attempt, RTL/font/zero state, add, duplicate national code, duplicate personnel number, search, sort, pagination, edit, dirty close and application close.
- [ ] Record only observed results and explicit blockers in `docs/foundation-verification.md`.
- [ ] Commit with `docs: record foundation verification`.

---

## Milestone 2 — Employment lifecycle

### Task 14: Employment periods, status, termination and return

**Files:**
- Create: `src/HRManagement.Application/Employment/IEmploymentLifecycleService.cs`, `EmploymentLifecycleService.cs`, request/result records
- Create: `src/HRManagement.Infrastructure/Employment/EfEmploymentLifecycleRepository.cs`
- Create: `src/HRManagement.WinForms/Employees/Tabs/EmploymentTabControl.cs`, `EmploymentTabPresenter.cs`
- Create: `tests/HRManagement.Application.Tests/Employment/EmploymentLifecycleServiceTests.cs`

**Interfaces:**
- Consumes: employee/period repository, `IClock`, `IAuditWriter`, transaction.
- Produces: `StartEmploymentAsync`, `TerminateAsync`, `ReturnToWorkAsync`, `ChangeStatusAsync` with one-open-period/current-status enforcement.

- [ ] Add failing tests for hire boundary, termination-before-hire, one open period, one termination, audit and return creating a new period.
- [ ] Implement use case and EF repository, rerun focused and full tests.
- [ ] Add lazy employment tab through the details-window tab factory and verify Persian interaction.
- [ ] Commit with `feat: add employment lifecycle`.

## Milestone 3 — Organization history

### Task 15: Department and responsibility management

**Files:**
- Create: `src/HRManagement.Application/Organization/IDepartmentService.cs`, `IResponsibilityService.cs`, `IAssignmentService.cs`
- Create: `src/HRManagement.Infrastructure/Organization/EfOrganizationRepository.cs`
- Create: `src/HRManagement.WinForms/Organization/DepartmentsControl.cs`, `ResponsibilitiesControl.cs`
- Create: `src/HRManagement.WinForms/Employees/Tabs/AssignmentsTabControl.cs`
- Create: `tests/HRManagement.Application.Tests/Organization/AssignmentServiceTests.cs`

**Interfaces:**
- Consumes: hierarchy and history repositories, `DateRange`, audit/transaction.
- Produces: cycle-safe department commands, deactivate-used lookup commands, `AssignDepartmentAsync` with prior-end confirmation, and responsibility primary transition operations.

- [ ] Add failing tests for cycles, overlap, automatic previous end date, multiple active responsibilities and exactly one primary.
- [ ] Implement services and persistence; rerun tests.
- [ ] Add management pages and lazy employee assignment tab; verify RTL workflows.
- [ ] Commit with `feat: add organization histories`.

## Milestone 4 — Personnel records

### Task 16: Education, dependents, banks and access cards

**Files:**
- Create: services under `src/HRManagement.Application/PersonnelRecords/`
- Create: EF repositories under `src/HRManagement.Infrastructure/PersonnelRecords/`
- Create: tab controls under `src/HRManagement.WinForms/Employees/Tabs/`
- Create: tests under `tests/HRManagement.Application.Tests/PersonnelRecords/`

**Interfaces:**
- Consumes: shared Person lookup, national code/IBAN/card/account validators, employment period.
- Produces: education single-primary, dependent link-existing-person/no-self/no-duplicate, bank single-active-primary, and one-active-access-card commands.

- [ ] Add failing tests for every produced invariant and identifier algorithm.
- [ ] Implement validators, services and repositories; rerun narrow and full suites.
- [ ] Add lazy RTL tabs with explicit saves and typed Persian errors.
- [ ] Commit with `feat: add personnel records`.

## Milestone 5 — Contracts and managed files

### Task 17: Contracts, documents and profile photo

**Files:**
- Create: `src/HRManagement.Application/Files/IManagedFileStore.cs`, contract/document/photo services
- Create: `src/HRManagement.Infrastructure/Files/ManagedFileStore.cs`, `FileSignatureValidator.cs`, `ProfilePhotoProcessor.cs`
- Create: contract/document/photo controls under `src/HRManagement.WinForms/Employees/Tabs/`
- Create: tests under `tests/HRManagement.Domain.Tests/Files/`, `tests/HRManagement.Infrastructure.Tests/Files/`

**Interfaces:**
- Consumes: application paths, file stream metadata, transactions and `DateRange`.
- Produces: safe generated name, validated relative path, 20 MiB enforcement, atomic copy/trash, image JPEG max 1200 px at quality 85 plus 160 px thumbnail, contract non-overlap and one-current-document-version operations.

- [ ] Add failing tests for signature, extension, size, traversal, hashes, overlap and current-version uniqueness.
- [ ] Implement file and contract services; rerun isolated tests.
- [ ] Add lazy tabs and non-locking image display; verify GDI disposal and source preservation.
- [ ] Commit with `feat: add contracts and managed files`.

## Milestone 6 — Filters, archive and deletion

### Task 18: Advanced filtering and archive workflows

**Files:**
- Create: `src/HRManagement.Application/Employees/Search/AdvancedEmployeeFilter.cs`, expression specification records
- Create: `src/HRManagement.Application/Archive/IEmployeeArchiveService.cs`, `EmployeeArchiveService.cs`
- Create: `src/HRManagement.WinForms/Employees/AdvancedFilterDialog.cs`, `ArchiveControl.cs`
- Create: tests under `tests/HRManagement.Application.Tests/Employees/AdvancedFilterTests.cs`, `ArchiveTests.cs`

**Interfaces:**
- Consumes: search repository and employee/file archive repositories.
- Produces: OR-within/AND-between filter tree, exact/range/before/after dates, soft delete/restore, and permanent delete requiring exact personnel-number confirmation.

- [ ] Add failing filter and archive tests including stale searches and serious confirmation.
- [ ] Implement query translation and transactional archive services; rerun suites.
- [ ] Add Persian filter dialog without boolean terminology and archive page.
- [ ] Commit with `feat: add advanced filters and archive`.

## Milestone 7 — Import, export and reports

### Task 19: Excel and printable employee summary

**Files:**
- Create: `src/HRManagement.Application/ImportExport/` request/result/service contracts
- Create: `src/HRManagement.Infrastructure/ImportExport/ClosedXmlEmployeeWorkbookService.cs`
- Create: `src/HRManagement.WinForms/ImportExport/ExcelImportControl.cs`, `EmployeeExportDialog.cs`
- Create: `src/HRManagement.WinForms/Reports/EmployeeSummaryDocument.cs`
- Create: tests under `tests/HRManagement.Application.Tests/ImportExport/`, `tests/HRManagement.Infrastructure.Tests/ImportExport/`

**Interfaces:**
- Consumes: standard Persian workbook schema, employee editor validation, filtered/selected employee queries and Jalali formatter.
- Produces: template, preview rows/errors, reject-duplicate import transaction, error workbook, text-safe export and printable/PDF-capable summary.

- [ ] Add failing workbook round-trip, invalid-row, duplicate and identifier-format tests.
- [ ] Implement ClosedXML services without Excel dependency; rerun tests.
- [ ] Add preview/confirm UI, export selection and print workflow; verify codes are not scientific notation.
- [ ] Commit with `feat: add employee import export and reports`.

## Milestone 8 — Backup, settings and audit

### Task 20: Consistent backup/restore and administration

**Files:**
- Create: `src/HRManagement.Application/Backups/IBackupService.cs`, manifest/progress/result models
- Create: `src/HRManagement.Infrastructure/Backups/BackupService.cs`, `BackupManifestValidator.cs`
- Create: `src/HRManagement.WinForms/Backups/BackupsControl.cs`, settings/audit controls
- Create: tests under `tests/HRManagement.Infrastructure.Tests/Backups/`

**Interfaces:**
- Consumes: coordinated DbContext gate, application paths, clock and background executor.
- Produces: manual/daily-on-close/emergency backup, SHA-256 manifest, retention, validation, atomic restore with rollback and history records.

- [ ] Add failing tests for manifest hashes, missing entries, WAL snapshot, external destination, retention and restore rollback.
- [ ] Implement coordinated backup/restore and rerun isolated tests.
- [ ] Add progress UI, destination/retention/company settings and important audit view.
- [ ] Commit with `feat: add backup restore and settings`.

## Milestone 9 — Word letters

### Task 21: Open XML letter generation

**Files:**
- Create: `src/HRManagement.Application/Letters/ILetterService.cs`, placeholder models
- Create: `src/HRManagement.Infrastructure/Letters/OpenXmlLetterService.cs`
- Create: `src/HRManagement.WinForms/Letters/LetterIssuanceControl.cs`
- Create: tests under `tests/HRManagement.Infrastructure.Tests/Letters/`

**Interfaces:**
- Consumes: `.docx` managed templates, employee/company projection, letter number/date.
- Produces: formatting-preserving simple placeholder replacement, individual/group output, managed output file and issued-letter history without employee snapshot.

- [ ] Add failing Open XML tests for all approved placeholders, split text runs, missing placeholder report and preserved formatting.
- [ ] Implement constrained replacement and history transaction; rerun tests.
- [ ] Add issuance/print UI and document template-engine limitations.
- [ ] Commit with `feat: add Word letter issuance`.

## Milestone 10 — Packaging and production acceptance

### Task 22: Inno Setup and installed-machine validation

**Files:**
- Create: `installer/HRManagement.iss`, `scripts/Publish.ps1`, `scripts/Verify-Publish.ps1`
- Create: `docs/production-verification.md`
- Modify: `README.md`, `CHANGELOG.md`

**Interfaces:**
- Consumes: self-contained/published WinForms output and bundled resources.
- Produces: upgrade-capable per-machine installer that prompts for a writable directory, preserves `Data`, and installs no runtime service.

- [ ] Publish Release win-x64 and verify executable, dependencies, font, migrations and no development database.
- [ ] Compile Inno Setup and install into a writable non-Program-Files directory.
- [ ] Verify first run, upgrade-in-place, data preservation, missing database recovery, backup/restore, single instance and uninstall data warning.
- [ ] Record exact installed-machine evidence and commit with `build: add Windows installer`.

## Plan self-review

- [x] Every approved version-one subsystem maps to a milestone and independently testable task.
- [x] Foundation tasks name exact files, interfaces, red/green commands and focused commits.
- [x] Type names stay stable from Application contracts through Infrastructure implementations and WinForms presenters.
- [x] Product exclusions, dependency direction, data location, date semantics, SQLite limitations and privacy rules are explicit.
- [x] Placeholder scan found no unresolved marker or ambiguous “appropriate handling” instruction.
