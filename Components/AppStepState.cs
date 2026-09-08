namespace ProductUI.Components;

/// Semantic states for AppStepper/AppStepItem (CLAUDE.md §4 — colors go
/// through a semantic token per state, never inline). Not assigned directly
/// by the consumer: AppStepper computes this per step from CurrentIndex (and
/// the optional SelectedIndex override — see AppStepper.razor's GetState()
/// for the actual, current formula; Selected is not always CurrentIndex + 1,
/// confirmed by pixel-sampling Screen B's real screen, where Selected sits
/// BEFORE Active).
/// Disabled is the only non-interactive state — Pass, Active, AND Selected
/// are all clickable/navigable (confirmed; Selected being interactive was
/// not the original assumption for this component).
public enum AppStepState
{
    Disabled,
    Selected,
    Pass,
    Active,
}
