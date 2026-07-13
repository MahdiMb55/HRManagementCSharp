# Repository Instructions

## Architecture and dependencies

The solution uses four projects under `src`: Domain, Application, Infrastructure, and WinForms. Domain has no project dependency. Application references only Domain. Infrastructure references Application and Domain. WinForms references Application and Infrastructure. Test projects mirror these seams. Forms and controls never create, retain, or query a DbContext.

## Commands

```powershell
dotnet restore HRManagement.sln
dotnet build HRManagement.sln
dotnet test HRManagement.sln
dotnet format HRManagement.sln
dotnet format HRManagement.sln --verify-no-changes
dotnet ef migrations add <MigrationName> --project src/HRManagement.Infrastructure --startup-project src/HRManagement.WinForms
dotnet ef migrations script --idempotent --project src/HRManagement.Infrastructure --startup-project src/HRManagement.WinForms
dotnet ef database update --project src/HRManagement.Infrastructure --startup-project src/HRManagement.WinForms
```

Prefix shell commands with `rtk` in Codex sessions.

## C# conventions

Use file-scoped namespaces, nullable reference types, implicit usings, explicit enum values, `long` identifiers, `DateOnly` for business dates, and UTC `DateTime` technical timestamps. Prefer immutable request/result records and focused files. Comments explain reasoning. Expected business failures use typed results; domain exceptions are reserved for broken invariants.

## WinForms rules

All user-facing text is Persian. Set `RightToLeft.Yes` and `RightToLeftLayout = true` where supported. Use the privately loaded Vazirmatn font, standard controls, high-DPI layout, meaningful control names, tab order, keyboard access, `ErrorProvider`, and explicit Save actions. Constructors stay cheap. Presenters coordinate use cases and cancellation. Designer files contain layout only.

## Async, threading, and SQLite

Microsoft.Data.Sqlite does not provide true asynchronous file I/O. Keep queries projected, indexed, paginated, and short. Use one DbContext per operation through `IDbContextFactory`; never share one across threads. Potentially blocking work goes through `IBackgroundExecutor`, obsolete searches are cancelled, and stale results are ignored. Never use `.Result`, `.Wait()`, nested `Task.Run`, or synchronous long work in UI event handlers.

## Files and backups

Resolve data paths from `AppContext.BaseDirectory`. Store managed files beneath `Data`, use generated internal names, retain original names in SQLite, store only validated relative paths, cap files at 20 MB, and never delete the source file. Database and managed files are backed up and restored together. Never copy only the SQLite main file without coordinating WAL state.

## Tests

Use test-first red-green-refactor for domain and application behavior. Infrastructure tests use isolated temporary directories and databases and never the real `Data` directory. Verify a failing test for the intended reason before production code. Run the narrow test during development and the complete Release suite before completion.

## Git

Make focused local commits after green verification. Do not push, publish, or create pull requests from this work. Preserve unrelated user changes.

## Prohibited shortcuts

No WPF/Avalonia/web UI, server database, authentication, fake production employees, direct SQL concatenation, `IQueryable` exposure to UI, service locator, static mutable state, file bytes in SQLite, Jalali strings in storage, process-wide culture mutation, business logic in Forms, swallowed exceptions, raw sensitive values in logs, or silent database recreation after initialization.
