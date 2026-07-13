# ADR 0006: Gregorian storage and Jalali presentation

- Status: Accepted
- Date: 2026-07-14

## Decision

Use `DateOnly` for business dates, store ISO `yyyy-MM-dd` Gregorian text in SQLite, use UTC for technical timestamps, and convert to/from Jalali only through presentation adapters.

## Consequences

Database ordering and range comparisons are stable. Jalali strings never enter persistence and process-wide culture is not changed. UI validation must distinguish invalid Persian dates from valid Gregorian domain dates.
