# Production verification

Final verification is intentionally deferred until all milestones are complete.

## Publish

```powershell
scripts\Publish.ps1
scripts\Verify-Publish.ps1
```

Expected evidence:

- Release `win-x64` self-contained publish exists under `artifacts\publish\win-x64`.
- `HRManagement.WinForms.exe` launches from the publish directory.
- bundled `Vazirmatn-Variable.ttf` and `OFL.txt` exist in the publish output.
- no development SQLite database is published.
- first run creates writable `Data` subdirectories next to the executable.

## Installer

Compile `installer\HRManagement.iss` with Inno Setup after publish verification.

Expected evidence:

- setup executable exists under `artifacts\installer`.
- install destination is writable by the target operator.
- installer creates/preserves `{app}\Data`.
- uninstall warns that operational data is preserved.
- no Windows service is installed.

## Installed-machine acceptance

Run these checks on a clean Windows machine or VM:

1. Install into a writable directory.
2. Launch first run and confirm dashboard opens.
3. Confirm `Data\Database\hr-management.db` is created after initialization.
4. Add an employee, close, reopen, and confirm data remains.
5. Run a backup from the administration page.
6. Upgrade in place with a newer build and confirm `Data` is preserved.
7. Delete or rename the database after `.initialized`, relaunch, and confirm startup integrity handling is shown.
8. Attempt a second app instance and confirm single-instance behavior.
9. Uninstall and confirm the uninstall warning preserves operational data.

Record exact dates, machine name, installer path, publish path, and observed results in this document during final verification.
