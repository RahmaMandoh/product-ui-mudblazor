# ProductUI style library

A reusable MudBlazor 8.7 style library + component set for two Arabic-RTL
screens (Sync Status Dashboard, Exam Model & Question Distribution). Per
`CLAUDE.md`'s task rules, both screens style themselves through this one
shared package — tokens, utilities, shared component classes, and a
controlled MudBlazor override layer — never through page-specific CSS.

## Package structure

```
Components/            Shared Blazor components (AppButton, AppField,
                        AppStatusCard, AppStepper, AppQuestionCard, ...).
                        A few ship a ComponentName.razor.css isolation file
                        for MudBlazor-internal wiring only (CLAUDE.md §3.1)
                        — never for a new reusable visual pattern; those
                        always live in wwwroot/styles/components.css or
                        utilities.css instead.
wwwroot/styles/
  tokens.css            Color, typography, spacing, radius, elevation,
                        borders, focus ring, breakpoints, motion — the
                        two-layer Variable/Token convention (see below).
  utilities.css          Bootstrap-like spacing/display/flex/text helpers.
  components.css          Shared visual patterns: buttons, fields, cards,
                        panels, status badges, alerts, tabs/steps, the
                        question-bank column pattern, etc.
  mudblazor-overrides.css  The ONLY place MudBlazor's own generated classes
                        are targeted globally (CLAUDE.md §5) — every
                        selector here carries a comment naming the
                        MudBlazor version/component it targets.
  responsive-rtl.css     RTL-aware rules using logical properties
                        (inline-start/end), not left/right.
wwwroot/dist/
  product-ui.min.css     GENERATED — the single compiled reference file
                        (see Build step below). Never edit by hand.
demo/
  ScreenA/                Sync Status Dashboard demo, mock data only,
                        consumes the shared library exclusively.
  ScreenB/                Exam Model / Question Distribution demo, same
                        rule.
build-styles.ps1        Concatenates + minifies the 5 wwwroot/styles/*.css
                        layers into wwwroot/dist/product-ui.min.css.
DECISIONS.md            The task brief's Decision Note deliverable — a
                        summary of every Figma-vs-accessibility (or
                        Figma-vs-spec) trade-off made while building this,
                        each backed by its full reasoning/contrast math —
                        read this before "fixing" something back to a
                        literal Figma value.
```

No component- or page-specific CSS files exist outside this structure. If
both screens need the same visual pattern, it's a class in
`components.css`/`utilities.css`, used identically by both — never
duplicated per screen.

## How the demo screens consume this

`wwwroot/index.html` loads stylesheets in the order CLAUDE.md §4 requires
— library defaults first, our overrides last, so ours always wins on
ties:

```
css/bootstrap/bootstrap.min.css
_content/MudBlazor/MudBlazor.min.css
ProductUI.styles.css          (Blazor's own isolated-CSS bundle)
dist/product-ui.min.css       (this library — tokens/overrides/components/
                                utilities/responsive-rtl, concatenated)
css/app.css                   (last — page-shell-level overrides only)
```

`demo/ScreenA/SyncStatus.razor` and `demo/ScreenB/QuestionDistribution.razor`
reference only shared components and shared CSS classes — no inline
styles, no page-specific stylesheet. That's the enforcement mechanism for
"one shared package, two consuming screens": if a screen needs a new
visual pattern, the change has to land in `wwwroot/styles/`, not in the
demo page itself.

## Build step for `dist/product-ui.min.css`

`build-styles.ps1` concatenates the 5 `wwwroot/styles/*.css` files (in the
CLAUDE.md §4 order: tokens → mudblazor-overrides → components → utilities
→ responsive-rtl), strips comments, and collapses whitespace into
`wwwroot/dist/product-ui.min.css`.

This runs automatically — `ProductUI.csproj` has a `BuildStyles` MSBuild
target (`BeforeTargets="Build"`) that invokes the script on every
`dotnet build` (and therefore every `dotnet clean` + rebuild, which is the
verification workflow CLAUDE.md §7 requires). There is no separate manual
step to remember: edit the source files under `wwwroot/styles/`, then
build as usual.

To run it standalone (e.g. to inspect the output without a full build):

```powershell
.\build-styles.ps1
```

**Never hand-edit `wwwroot/dist/product-ui.min.css`** — it's regenerated
from the source layers on every build and any manual edit is silently
overwritten. The minifier is regex-based (comment-stripping +
whitespace-collapsing), not a full CSS parser — safe for this file set
because none of the 5 layers currently contain `data:` URIs or quoted
string values with embedded punctuation (verified before writing the
script); if either is ever added to `wwwroot/styles/`, revisit
`build-styles.ps1`'s minification regexes first.

## Token conventions

Two layers, same pattern (see `wwwroot/styles/tokens.css`):

```css
/* Variables — raw/primitive values, never used directly in a component */
--color-blue-00: #009DDC;
--font-weight-medium: 500;

/* Tokens — semantic aliases referencing a variable, scoped to their use */
--{component}-color-{role}: var(--color-...);
--{component}-font-{property}: var(--font-...);
```

Every rule in `components.css`, `utilities.css`, `mudblazor-overrides.css`,
and every `ComponentName.razor.css` isolation file references a token —
never a raw hex value, a bare pixel number, or the same value hardcoded
twice. If a component's Figma spec needs a value with no existing token,
add the token to `tokens.css` first, then consume it — see CLAUDE.md §4/§5.

Several tokens intentionally deviate from their literal Figma value for
contrast/accessibility reasons (e.g. `--stepper-color-text-on-brand-safe`,
`--alert-color-text-warning`) — each deviation is logged in `DECISIONS.md`
with the contrast math behind it. Don't "correct" these back to the
literal Figma color without reading that entry first.

## RTL setup

`Layout/MainLayout.razor` wraps the whole app in `<MudRTLProvider
RightToLeft="true">` once, at the top level — never set `dir="rtl"` on
individual components. **Deviation from a generic MudBlazor RTL snippet,
flagged in that file's own comment:** `MudThemeProvider` has no
`RightToLeft` parameter in the installed MudBlazor 8.7.0 (confirmed
against the package's XML docs, and against an actual runtime check —
setting it fails at runtime, not at compile time, in this version). Only
`MudRTLProvider.RightToLeft="true"` is set; `MudThemeProvider` picks up
RTL automatically via the cascading value `MudRTLProvider` provides. If
you're following an older MudBlazor RTL guide that sets both, that guide
predates 8.7.0.

Numeric/date values embedded in RTL text are wrapped (`<span
dir="ltr">...</span>` or the CSS/unicode-bidi equivalent) so they don't
reorder.

## Running the demo screens

```powershell
dotnet build     # also regenerates dist/product-ui.min.css
dotnet run --launch-profile http
```

Then open (all routes are additive demo routes — the real nav-linked
pages they'll eventually replace are still empty `@page` stubs):

| Screen | Route |
|---|---|
| Screen A — Sync Status Dashboard | `/demo/sync-status` |
| Screen B — Exam Model / Question Distribution | `/demo/question-distribution` |
| Component state galleries | `/demo/button-states`, `/demo/field-states`, `/demo/alert-states`, `/demo/stepper-states`, `/demo/action-banner-states` |

After any CSS-isolation-related change, `dotnet clean` before rebuilding —
per CLAUDE.md §7, hot-reload isn't trustworthy for isolated-CSS changes —
and hard-refresh (`Ctrl+Shift+R`) before judging the result.

## Accessibility

Every component in `components.css` is expected to satisfy the CLAUDE.md
§6 checklist (contrast, visible focus ring, full keyboard operability,
persistent field labels, 44×44 touch targets, status conveyed via text +
icon not color alone, `prefers-reduced-motion`, native semantic HTML
first). Where a component's own Figma-specified focus colors exist, both
those AND the shared `--focus-ring-*` outline are applied — the two are
independent layers, not alternatives (see CLAUDE.md §6's standing rule).
Trade-offs made to satisfy this checklist against a literal Figma value
are logged in `DECISIONS.md`, not just as code comments.
