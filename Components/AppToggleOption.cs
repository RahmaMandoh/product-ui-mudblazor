using MudBlazor;

namespace ProductUI.Components;

/// One option in an AppToggleGroup&lt;T&gt;. IconPosition reuses MudBlazor's
/// own Start/End Adornment enum — the exact same parameter AppButton.razor
/// already uses for its own Icon/IconPosition — rather than inventing a
/// second Start/End enum for this component.
public sealed record AppToggleOption<T>(T Value, string Label, string Icon, Adornment IconPosition = Adornment.End);
