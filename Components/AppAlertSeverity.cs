namespace ProductUI.Components;

/// Severity variants for AppAlert (CLAUDE.md §4 — colors go through a
/// semantic token per severity, never a raw hex per instance). Warning and
/// Success only for now, matching the two real instances confirmed against
/// TaskTwo_from_figma_design.png — Error/Info deferred until a real use case
/// shows up (CLAUDE.md: don't build unused variants speculatively).
public enum AppAlertSeverity
{
    Warning,
    Success,
}
