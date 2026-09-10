using MudBlazor;

namespace ProductUI.Components;

/// One option in an AppToggleGroup&lt;T&gt;. IconPosition reuses MudBlazor's
/// own Start/End Adornment enum — the exact same parameter AppButton.razor
/// already uses for its own Icon/IconPosition — rather than inventing a
/// second Start/End enum for this component.
///
/// Note: optional short status text rendered next to the label (e.g.
/// "قريباً") for an option that's selectable/visible but not yet
/// functionally wired up — CLAUDE.md §6 requires status communicated via
/// text, not color/dimming alone, so an inert option needs this rather
/// than silently doing nothing when picked.
public sealed record AppToggleOption<T>(T Value, string Label, string Icon, Adornment IconPosition = Adornment.End, string? Note = null);
