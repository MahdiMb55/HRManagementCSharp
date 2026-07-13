# ADR 0003: Pragmatic four-layer architecture

- Status: Accepted
- Date: 2026-07-14

## Decision

Separate Domain, Application, Infrastructure and WinForms, with Passive View/MVP at the presentation boundary.

## Consequences

Domain stays pure, Application owns use-case contracts, Infrastructure owns EF/file implementations, and controls interact through presenters. This adds explicit mapping but keeps rules testable and prevents database concerns from entering Forms.
