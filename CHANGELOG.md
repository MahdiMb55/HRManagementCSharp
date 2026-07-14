# Changelog

All notable changes follow Keep a Changelog conventions.

## [Unreleased]

### Added

- Product, architecture, persistence, UI, performance, storage, testing, and roadmap documentation.
- Foundation plan for the .NET 10 HR management application.
- Persian WinForms shell, dashboard zero state, employee list/editor slice, startup integrity checks, SQLite initialization, file logging, and runtime verification notes.

### Fixed

- Loaded bundled Vazirmatn variable font without assuming a single font family.
- Kept SQLite-backed dashboard, list, and editor service work off the UI thread.
- Kept employee search input enabled while loading so newer searches can supersede in-flight work.
- Prevented blank edit forms when employee loading fails.
- Made shutdown disposal paths idempotent and logged host shutdown failures.
- Replaced count-based log retention with deletion of `hrmanagement-*.log` files older than 30 days.
- Added initial employment lifecycle application/domain/infrastructure contracts for starting employment, termination, return-to-work, and status changes.
- Started organization history services for department transfer and responsibility assignment rules.
- Added Windows publish scripts, Inno Setup installer definition, and production verification checklist.
