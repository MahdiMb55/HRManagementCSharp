# HRManagement Approved Design

## Product boundary

Build an offline, single-user, Persian RTL HR desktop application for one company. The foundation delivers the complete persistence shape plus working dashboard, employee list and basic employee add/edit. Subsequent milestones fill employment, organization, records, files, reporting, backup, letters and installer workflows without changing the approved boundaries.

## Architecture

Use .NET 10 Windows Forms, SQLite, EF Core 10, Microsoft hosting/DI/logging and xUnit in four layers. Domain is pure; Application defines use cases and ports; Infrastructure implements persistence/files/logging; WinForms uses Passive View presenters. `IDbContextFactory` supplies one context per operation. Potentially blocking SQLite/file work crosses `IBackgroundExecutor`; presenter request generations prevent stale UI updates.

## Identity and data

`Person` is shared by employees and dependents, making normalized Iranian national code globally unique. Business histories use `DateOnly` Gregorian storage with nullable end dates and application-enforced overlap rules. UTC timestamps record technical events. Files live in a managed `Data` tree and SQLite stores relative paths and metadata only. The complete 25-table model and enforcement split are defined in `docs/05-database-design.md`.

## Startup and reliability

The installer chooses a writable directory. Startup resolves `Data` beside the executable, creates required directories on a legitimate first run, tests writability, checks an initialization marker, and refuses silent empty-database replacement after initialization. It then configures rolling logs, applies migrations, seeds fixed categories, checks SQLite integrity/WAL, loads private Vazirmatn and shows the shell. A named mutex enforces a single instance.

## User experience

`MainForm` has a fixed RTL Persian navigation rail and cached content controls. Dashboard uses real projected counts. Employee list is read-only, paginated, sorted and filtered in SQLite with 300 ms debounce and cancellation. One reusable non-modal employee details form supports explicit Save, ErrorProvider, Jalali adapter, unsaved-change confirmation and refreshes the list after a successful transactional save with audit event.

## Error, privacy and performance

Expected failures are typed and localized. Technical exceptions go to 30-day rolling logs with operation identity and masked identifiers. UI never shows stack traces or storage paths. Queries are small, projected and indexed; constructors contain no I/O; operations are measured against `docs/07-performance-guidelines.md` budgets.

## Verification

Behavioral rules are developed red-green-refactor. Infrastructure tests isolate temporary paths and SQLite files. Completion requires format verification, Release build and tests, migration script review, applying the migration in a fresh temporary directory, launching the app, and recording each manual scenario truthfully in `docs/foundation-verification.md`.

## Self-review

This design uses the approved type names consistently, assigns every concern to one layer, retains all product exclusions, and contains no unresolved implementation choice. Full milestone decomposition and exact interfaces are in the foundation plan.
