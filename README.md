# ProductUI style library

This is a shared style library for two Arabic, right-to-left screens — the
Sync Status Dashboard and the Exam Model / Question Distribution screen —
built on MudBlazor 8.7. Both screens are styled entirely from this one
shared set of tokens, CSS classes, and Blazor components. Neither screen
has any styling of its own.

## The pieces

- **`Components/`** — shared Blazor components you actually use in a page:
  `AppButton`, `AppField`, `AppChip`, `AppStatusCard`, `AppStepper`, and so
  on. A few of these carry their own `ComponentName.razor.css` file, but
  only for plumbing specific to how that one component renders — any
  reusable look or pattern lives in the shared CSS files below instead,
  never in a component's own isolated file.
- **`wwwroot/styles/`** — the actual shared CSS, split into five files by
  purpose:
  - `tokens.css` — every color, spacing value, font size, border radius,
    and so on, given a name.
  - `utilities.css` — small single-purpose helper classes (flex, spacing,
    alignment) in the Bootstrap style.
  - `components.css` — the real shared visual patterns: buttons, fields,
    cards, panels, badges, alerts, tabs, question-bank columns.
  - `mudblazor-overrides.css` — the only file allowed to target MudBlazor's
    own internal class names, and every rule in it says which MudBlazor
    version and component it's overriding.
  - `responsive-rtl.css` — right-to-left rules for specific screen sizes.
- **`wwwroot/dist/product-ui.min.css`** — generated automatically from the
  five files above every time you build. This is the one file the app
  actually loads. Don't edit it by hand — your changes will be overwritten
  on the next build.
- **`demo/ScreenA/`** and **`demo/ScreenB/`** — the two working demo
  screens, using mock data and nothing but the shared library above.
- **`DECISIONS.md`** — explains the judgment calls made while building
  this, especially anywhere the design and our accessibility rules didn't
  agree. Worth a read before "fixing" something back to match the design
  file exactly — it might have been changed on purpose.

If both screens ever need the same thing — a badge, a card, a spacing
pattern — it becomes one shared class in `components.css` or
`utilities.css`, used by both. Nothing gets copy-pasted per screen.

## How a page actually uses this

`wwwroot/index.html` loads styles in a specific order — Bootstrap, then
MudBlazor's own CSS, then Blazor's isolated-CSS bundle, then this library,
then any page-shell-level overrides last — so our styles always win if two
rules ever collide.

The two demo pages only ever reference shared components and shared CSS
classes. No inline styles, no page-specific stylesheet. That's what
actually keeps this "one shared library, two screens" instead of slowly
drifting apart: any new visual need has to go into the shared files, not
onto the page.

## Building the CSS

You don't need to do anything — the five files in `wwwroot/styles/` get
combined and minified into `wwwroot/dist/product-ui.min.css` automatically
every time you run `dotnet build`. If you want to run just that step on
its own:

```powershell
.\build-styles.ps1
```

## How the tokens work

Every value — a color, a spacing size, a font weight — exists in two
layers:

```css
/* A Variable: the raw value itself */
--color-blue-00: #009DDC;

/* A Token: a name for what that value is used for, pointing at a Variable */
--chip-color-text: var(--color-blue-00);
```

You should never see a raw color or pixel number typed directly into a
component's CSS — only a token. If you need a value that doesn't have a
token yet, add it to `tokens.css` first, then use it.

A few tokens deliberately don't match the design file's exact color,
almost always because the literal color was too hard to read as text —
`DECISIONS.md` explains each one. Don't change these back to match the
design file exactly without reading that explanation first.

**A real example — using the shared chip component:**

```razor
<AppChip Text="نموذج 1" Icon="@Icons.Material.Filled.Description" />
```

That's the whole thing — you don't write any CSS at all. `AppChip` already
pulls its color, padding, corner radius, and font from `tokens.css`
(`--chip-color-bg`, `--chip-color-text`, `--chip-radius`, and so on), so it
looks the same everywhere it's used. If a screen needs a chip that looks
different, the right move is to add a new token or a new variant class in
the shared files — never to style one instance by hand.

## Right-to-left setup

The whole app is wrapped once, at the very top, in
`<MudRTLProvider RightToLeft="true">` (in `Layout/MainLayout.razor`).
Nothing else sets `dir="rtl"` anywhere — one setting at the top is enough
for the whole app. One thing worth knowing: in the version of MudBlazor
we're on (8.7.0), `MudThemeProvider` doesn't actually have a
`RightToLeft` parameter, even though some guides assume it does — only
`MudRTLProvider` needs it, and everything else picks up RTL through that.

Numbers and dates inside Arabic text are wrapped in
`<span dir="ltr">...</span>` so they don't get visually reordered.

## Running it

```powershell
dotnet build     # also rebuilds the CSS
dotnet run --launch-profile http
```

Then open:

| Screen | Route |
|---|---|
| Screen A — Sync Status Dashboard | `/demo/sync-status` |
| Screen B — Exam Model / Question Distribution | `/demo/question-distribution` |
| Component state galleries | `/demo/button-states`, `/demo/field-states`, `/demo/alert-states`, `/demo/stepper-states`, `/demo/action-banner-states` |

If you change a component's own `.razor.css` file, run `dotnet clean`
before rebuilding — hot reload doesn't reliably pick those changes up —
and hard-refresh your browser afterward.

## Accessibility

Every shared component is expected to meet a basic bar: readable color
contrast, a focus state you can actually see (not just the browser
default), full keyboard support, a real visible label on every field,
touch targets big enough to tap comfortably, and status shown with text
and icons, not color alone. Where a component's own design also specifies
a focus color, that's applied on top of the shared focus outline, not
instead of it — both together. Any place we had to choose between matching
the design exactly and meeting this bar is explained in `DECISIONS.md`.
