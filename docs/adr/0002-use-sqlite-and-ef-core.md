# ADR 0002: SQLite and Entity Framework Core 10

- Status: Accepted
- Date: 2026-07-14

## Decision

Use SQLite with EF Core 10 migrations, explicit per-entity configurations, foreign keys, WAL, partial indexes and one DbContext per operation.

## Consequences

The app remains offline and deployment is file-based. Queries must be indexed, projected and paginated; Async-named SQLite calls are not treated as true asynchronous I/O. Cross-row rules unsupported by SQLite are enforced in application services and tests.
