# Foundation Verification

Date: 2026-07-14

## Automated Gates

- `rtk proxy dotnet build HRManagement.sln --no-restore -m:1 --disable-build-servers -v minimal`
  - Result: passed, 0 warnings, 0 errors.
- `rtk proxy dotnet test HRManagement.sln --no-build --no-restore -m:1 -v minimal`
  - Result: passed, 97 tests.
  - Domain: 30 passed.
  - Application: 39 passed.
  - Infrastructure: 28 passed.
- `rtk git diff --check`
  - Result: passed.

## Runtime Checks

Runtime path:

`D:\Programs\C#\HRManagementCSharp\.tmp-runtime\debug-clean\HRManagement.WinForms.exe`

Observed:

- Fresh first run created `Data`, required subdirectories, `.initialized`, and `Data\Database\hr-management.db`.
- Main window opened with the Persian title `مدیریت منابع انسانی`.
- Dashboard rendered RTL Persian navigation and zero-state dashboard.
- Dashboard showed 0 active employees, 0 archived/terminated employees, and 0 active contracts.
- Bundled Vazirmatn font loaded without the previous multi-family startup crash.
- Closing the app through the main window exited the process.
- Second run opened against the existing initialized database.
- Attempting a second instance did not leave a second running `HRManagement.WinForms.exe` process.

Not yet manually verified:

- Add employee, duplicate national code, duplicate personnel number, search, sort, pagination, edit, dirty-close warning, and missing-database-after-initialization flows.

## Deferred Verification

Additional refinements after the recorded runtime smoke check:

- Employee-list supported-column visibility menu.
- Employee editor Escape close shortcut.
- Idempotent employee-editor disposal.

Verification for these refinements is intentionally deferred to the final verification step with the remaining milestone checks.
