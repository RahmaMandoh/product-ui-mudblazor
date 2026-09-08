namespace ProductUI.Components;

/// Which MudBlazor control AppField&lt;T&gt; renders internally. Same visual
/// state system/tokens/CSS classes apply regardless of Type — only the
/// underlying control and its type-specific parameters differ.
///
/// Named AppFieldType, not FieldType: MudBlazor itself already ships a
/// public `MudBlazor.FieldType` enum (used for input-mask field types,
/// unrelated to this) and `_Imports.razor` has `@using MudBlazor` globally,
/// so `FieldType` alone is ambiguous everywhere in this project — confirmed
/// by an actual build (CS0104), not guessed.
public enum AppFieldType
{
    Text,
    Select,
    Date,
}
