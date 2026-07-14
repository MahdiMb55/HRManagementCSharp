# Production verification

Date: 2026-07-14

## Automated verification

| Gate | Result |
| --- | --- |
| `rtk proxy dotnet format HRManagement.sln --verify-no-changes --no-restore` | Passed. |
| `rtk git diff --check` | Passed. |
| `rtk dotnet build HRManagement.sln -c Debug --no-restore` | Passed: 8 projects, 0 errors, 7 analyzer warnings. |
| `rtk dotnet build HRManagement.sln -c Release --no-restore` | Passed: 8 projects, 0 errors, 7 analyzer warnings. |
| `rtk dotnet test tests\HRManagement.Domain.Tests\HRManagement.Domain.Tests.csproj -c Debug --no-build` | Passed: 30 tests. |
| `rtk dotnet test tests\HRManagement.Application.Tests\HRManagement.Application.Tests.csproj -c Debug --no-build` | Passed: 39 tests. |
| `rtk dotnet test tests\HRManagement.Infrastructure.Tests\HRManagement.Infrastructure.Tests.csproj -c Debug --no-build` | Passed: 28 tests. |
| `rtk dotnet test tests\HRManagement.Domain.Tests\HRManagement.Domain.Tests.csproj -c Release --no-build` | Passed: 30 tests. |
| `rtk dotnet test tests\HRManagement.Application.Tests\HRManagement.Application.Tests.csproj -c Release --no-build` | Passed: 39 tests. |
| `rtk dotnet test tests\HRManagement.Infrastructure.Tests\HRManagement.Infrastructure.Tests.csproj -c Release --no-build` | Passed: 28 tests. |
| `rtk proxy dotnet test HRManagement.sln -c Debug --no-build --no-restore --disable-build-servers -m:1 -v minimal` | Passed: 97 tests across 3 test assemblies. |
| `rtk proxy dotnet test HRManagement.sln -c Release --no-build --no-restore --disable-build-servers -m:1 -v minimal` | Passed: 97 tests across 3 test assemblies. |

Note: solution-level `dotnet test` without `-m:1` timed out after the VSTest solution target printed no actionable test failures. Serializing MSBuild/VSTest execution with `-m:1` avoids that orchestration hang and runs all three test assemblies.

## Publish

| Gate | Result |
| --- | --- |
| `rtk powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Publish.ps1` | Passed. Published to `artifacts\publish\win-x64`. |
| `rtk powershell -NoProfile -ExecutionPolicy Bypass -File scripts\Verify-Publish.ps1` | Passed for `D:\Programs\C#\HRManagementCSharp\artifacts\publish\win-x64`. |

Observed publish evidence:

- `artifacts\publish\win-x64\HRManagement.WinForms.exe` exists.
- Bundled font files are included under `Resources\Fonts`.
- No `Data` directory is present in the publish output.

## Installer

| Gate | Result |
| --- | --- |
| `rtk proxy "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\HRManagement.iss` | Passed with Inno Setup 6.7.3. |

Observed installer evidence:

- Setup executable exists at `D:\Programs\C#\HRManagementCSharp\artifacts\installer\HRManagementSetup.exe`.
- Setup executable size: 39,931,101 bytes.
- Installer script creates `{app}\Data` and required subdirectories with user modify permissions.
- Installer excludes `Data\*` from application file copy.
- Uninstall code displays an operational-data-preservation warning.

## Installed-machine acceptance

Not yet verified on a clean Windows machine or VM:

1. Install into a writable directory.
2. Launch first run and confirm dashboard opens.
3. Confirm `Data\Database\hr-management.db` is created during initialization.
4. Add an employee, close, reopen, and confirm data remains.
5. Run the backup administration page.
6. Upgrade in place with a newer build and confirm `Data` is preserved.
7. Rename or delete the database after `.initialized`, relaunch, and confirm startup integrity handling is shown.
8. Attempt a second app instance and confirm single-instance behavior.
9. Uninstall and confirm the uninstall warning preserves operational data.
