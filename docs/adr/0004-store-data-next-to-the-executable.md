# ADR 0004: Store data next to the executable

- Status: Accepted
- Date: 2026-07-14

## Decision

Resolve a fixed `Data` tree from `AppContext.BaseDirectory`; do not fall back to profile or machine data folders.

## Consequences

Inno Setup must default to and validate a writable install destination. Startup tests writability and uses an initialization marker. Once initialized, a missing database is a recovery condition, never a signal to create an empty replacement.
