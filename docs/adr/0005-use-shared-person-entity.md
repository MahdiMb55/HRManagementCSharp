# ADR 0005: Shared Person identity

- Status: Accepted
- Date: 2026-07-14

## Decision

Represent human identity once in `Person`; both `Employee` and dependent relationships reference it. `NationalCode` is required and globally unique.

## Consequences

A person may be an employee, a dependent or both without duplicated identity. Adding a dependent first searches by normalized national code and links the existing person. Self-dependency and duplicate relationships are rejected.
