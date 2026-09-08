namespace ProductUI.Components;

/// Semantic status categories for AppStatusCard's ring/label color (CLAUDE.md
/// §4 — colors go through a semantic token per status, never a raw hex per
/// card). Deliberately separate from the StatusLabel string parameter: the
/// same label text can carry different semantics on different cards (e.g.
/// "تمت" appears at 80% as Info-blue on one card and at 100% as Success-green
/// on another, per the Figma reference — confirmed by comparing both cards'
/// ring colors directly, not assumed from the shared word).
public enum AppStatusType
{
    Success,
    Warning,
    Info,
    Neutral,
}
