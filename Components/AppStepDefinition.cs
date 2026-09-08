namespace ProductUI.Components;

/// One entry in AppStepper's Steps list (CLAUDE.md §3.1 — shared component).
/// A record, not the prior (string Icon, string Label) tuple: AlwaysShowLabel
/// is optional per-step config a caller only sets for the rare step that
/// needs it (see below) — a named, defaulted property lets every existing
/// call site (StepperStatesGallery) add it later without being forced to
/// name a third positional tuple field everywhere, forever.
///
/// AlwaysShowLabel (default false) opts ONE step out of AppStepItem's
/// otherwise-uniform icon-only mobile treatment (responsive-rtl.css,
/// `.app-stepper__label` visually-hidden below 600px). Added for Screen B's
/// real mobile export (demo/ScreenB/TaskTwo_Mobile.png): "ملخص الاختبار"
/// (the final/summary step) keeps its label visible there while the other 5
/// steps go icon-only — confirmed intentional, not an export artifact, so
/// this is a per-step flag rather than a change to the shared mobile rule
/// every other stepper usage still relies on.
public record AppStepDefinition(string Icon, string Label, bool AlwaysShowLabel = false);
