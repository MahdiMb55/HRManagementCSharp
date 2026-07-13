# ADR 0001: Windows Forms on .NET 10

- Status: Accepted
- Date: 2026-07-14

## Decision

Use C# on .NET 10 LTS with modern SDK-style Windows Forms targeting `net10.0-windows`.

## Consequences

The product is Windows-only, uses native desktop controls and high-DPI support, and requires the .NET 10 SDK for development. WPF, Avalonia, WinUI, MAUI, web shells and .NET Framework are excluded.
