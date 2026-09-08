# Project Rules — MudBlazor UI/UX Evaluation Task

You are working inside a .NET Blazor WASM project using **MudBlazor 8.7**.
The goal: redesign two existing Arabic-RTL screens (Sync Status Dashboard,
Exam Model & Question Distribution) and ship a **reusable style library**
that both screens consume — not page-specific duplicated CSS.
Deadline: 10 Sep 2026, 12:00 PM.

Read this file fully before writing any code. If a request conflicts with
a rule below, say so explicitly instead of silently picking one side.

---

## 1. Non-negotiable behavior rules

1. **Never guess the rendered DOM. Verify it.**
   MudBlazor components do not always render the class/element structure you'd
   expect. Before writing a CSS selector that targets a MudBlazor-generated
   element (`.mud-nav-link`, `.mud-nav-group`, etc.), do ONE of:
   - Read the actual generated scoped-CSS bundle after building
     (`obj/Debug/net8.0/scopedcss/projectbundle/*.bundle.scp.css`) to confirm
     the selector you wrote compiled the way you expect, **and**
   - Ask to see the DevTools "Elements" panel (or a screenshot of it) for the
     real rendered HTML before assuming nesting/class placement.
   Do not present a fix as "done" until one of these has actually confirmed it.

2. **Check for inline styles before fighting with CSS specificity.**
   MudBlazor components often set colors via `Color`/`IconColor` parameters,
   which some versions render as an **inline `style="color:..."` attribute**.
   Inline styles beat any CSS rule regardless of specificity or `!important`
   placement tricks. If a color/visual override "isn't applying" after a
   correct, well-specified CSS rule, the first hypothesis is an inline style
   from a component parameter — fix it by passing `Color.Inherit` (or the
   component's equivalent "no fixed color" option), not by adding more CSS.

3. **`!important` is a last resort, not a fix.**
   Only acceptable to override a library's own inline style, and only with a
   one-line comment explaining exactly why. Never use it to route around an
   unverified specificity/ordering guess.

4. **CSS load order matters — get it right once, in `index.html`:**
   ```
   bootstrap.min.css
   _content/MudBlazor/MudBlazor.min.css   <- library defaults FIRST
   {ProjectName}.styles.css               <- our scoped/isolated overrides
   css/tokens.css                         <- design tokens (custom properties)
   css/app.css / other global overrides   <- LAST, so ours always wins on ties
   ```
   Never place your own stylesheet or the isolated-CSS bundle before
   MudBlazor's own stylesheet.

5. **One source of truth per value.** If a token exists
   (`--sidebar-width`, `--color-blue-00`, etc.), every place that needs that
   value must reference the token — never hardcode the same value again in a
   component attribute (`Width="260px"`) or a second CSS rule. If you notice
   a hardcoded duplicate, flag it and replace it.

6. **Show the diff before applying it**, for any change touching more than
   one file or a MudBlazor component parameter. State the assumption you're
   making about MudBlazor's internals if you're not 100% sure, so it can be
   checked instead of discovered broken later.

7. **Don't restructure MudBlazor components.** Per the task brief: style
   existing components via `MudTheme`, documented CSS classes, and a
   controlled override layer. Minor wrapper/class attributes are fine; do
   not replace or reimplement MudBlazor components wholesale.

---

## 2. RTL setup (do this once, correctly, at the top level)

```razor
@* MainLayout.razor *@
<MudThemeProvider Theme="AppTheme.Default" RightToLeft="true" />
<MudRTLProvider RightToLeft="true">
    <MudLayout>
        ...
    </MudLayout>
</MudRTLProvider>
```
Both `MudThemeProvider.RightToLeft` and `MudRTLProvider.RightToLeft` must be
set together and kept in sync — one drives MudBlazor's internal RTL-aware
logic, the other actually sets the `dir` attribute on the DOM. Do not set
`dir="rtl"` manually on individual components (e.g. just the drawer); it
causes inconsistent RTL behavior across the rest of the app (dialogs, menus).

Numeric/date values embedded in RTL text must be wrapped so they don't
reorder: `<span dir="ltr">2026/09/01</span>` or the CSS/unicode-bidi
equivalent.

---

## 3. Required package structure (per task §4.2 / §6)

```
styles/
  tokens.css              — color, typography, spacing, radius, elevation,
                             borders, focus ring, breakpoints, motion
  utilities.css            — Bootstrap-like helpers: spacing, display,
                             alignment, flex/grid, text, visibility,
                             responsive helpers
  components.css            — buttons, icon buttons, fields, cards, panels,
                             status badges, alerts, progress, tabs/steps,
                             toolbars, lists
  mudblazor-overrides.css  — the ONLY place MudBlazor's own classes are
                             targeted; scoped safely (see §4)
  responsive-rtl.css       — RTL-aware rules using logical CSS properties
                             (inline-start/end, not left/right)
dist/
  product-ui.min.css       — compiled/minified, single reference file
demo/
  ScreenA/                 — sync dashboard demo, mock data, uses the
                             shared library only
  ScreenB/                 — exam model/question distribution demo
README.md
```

No component-specific or page-specific CSS files outside this structure.
If Screen A and Screen B need the same visual pattern (a status badge, a
card, a filter field), it belongs in `components.css`/`utilities.css`, used
by both — never copy-pasted per screen.

---

## 3.1 — CSS Isolation vs shared library (resolves §3/§5 tension)

Component-scoped CSS isolation files (`ComponentName.razor.css`) ARE allowed,
per §5, for MudBlazor-internal wiring a component needs to render correctly
(targeting library-generated classes with `::deep`, structural layout specific
to that component's markup).

They are NOT allowed to define a new reusable visual pattern (a button style,
badge style, card style, spacing scale, etc.) — that always belongs in
`styles/components.css` or `styles/utilities.css` as a class name, used
identically by both screens.

Rule of thumb: if a second, unrelated component would ever plausibly want
the same rule, it belongs in the shared library, not in that component's
isolated CSS. If it's purely about making THIS component's specific MudBlazor
markup behave, isolation is fine.

Every value inside a `.razor.css` file must be `var(--token-name)` — never a
new hardcoded color/size. If a needed token doesn't exist in `tokens.css` yet,
add it there first.

---

## 4. Design tokens — naming convention

Two layers, same pattern used for the sidebar already:

```css
/* Variables = raw/primitive values */
--color-blue-00: #009DDC;
--font-weight-medium: 500;

/* Tokens = semantic aliases referencing a variable, scoped to their use */
--{component}-color-{role}: var(--color-...);
--{component}-font-{property}: var(--font-...);
```
Never reference a raw hex value or a bare pixel number directly in a
component's CSS — always go through a semantic token. If a needed token
doesn't exist yet, add it to `tokens.css` first, then use it.

---

## 5. MudBlazor override layer rules (`mudblazor-overrides.css`)

- Prefer **Blazor CSS Isolation** (`ComponentName.razor.css` with `::deep`)
  scoped to the component that owns the markup, over global selectors in
  `mudblazor-overrides.css`. Use the global override file only for things
  that must apply app-wide (e.g. global focus-ring style).
- Every selector that reaches into MudBlazor internals needs a one-line
  comment saying which MudBlazor version/component it targets, since
  MudBlazor's internal class names are not a stable public API and may
  change between versions.
- Before assuming a `Class` parameter lands on a specific wrapper element,
  verify it (see rule 1).

---

## 6. Accessibility requirements (task §5 — 25% of the grade)

Every component in `components.css` must satisfy, before it's considered done:

- [ ] Text contrast ≥ 4.5:1 (normal text) / 3:1 (large text, meaningful
      borders) — check against the actual token colors, not by eye.
- [ ] Visible focus state (not just `outline: none` removed) on every
      interactive element — buttons, links, fields, tabs, drawer items.

  **Standing rule:** a component's own Figma-specified focused-state colors
  (border/bg/label/icon) and the shared `--focus-ring-*` outline are two
  independent layers, not alternatives — apply both, always. Neither one
  substitutes for the other: the component color satisfies the visual design,
  the shared ring is what guarantees every interactive element in the app has
  a consistent, unmissable focus indicator regardless of whether that
  component's own Figma spec happens to give focus a strongly-contrasting
  color. If a future component's Figma data includes its own focused-state
  colors, add them on top of the ring — don't treat their presence as a
  reason to drop the ring, and don't treat the ring as a reason to skip
  wiring the component's own colors.
- [ ] Fully keyboard-operable, logical tab order. Any drag-and-drop
      interaction (Screen B) needs a non-drag keyboard alternative.
- [ ] Persistent visible label on every field — placeholders never serve as
      the only label.
- [ ] Touch targets ≥ 44×44 CSS px.
- [ ] Status/error/progress communicated via text + icon, never color alone
      (see the red dashed-border empty-state issue found in Screen B audit —
      don't repeat that pattern).
- [ ] `prefers-reduced-motion` respected; no animation that can't be
      disabled.
- [ ] Native semantic HTML first; ARIA only to fill a real gap.

---

## 7. Verification workflow (do this before saying something is fixed)

1. `dotnet clean` then rebuild — don't trust a hot-reload result for
   anything CSS-isolation-related.
2. Confirm the relevant rule actually appears in the rebuilt
   `{ProjectName}.styles.css` / `dist/product-ui.min.css`.
3. Hard-refresh the browser (`Ctrl+Shift+R`) before judging the result.
4. If a visual result still doesn't match the Figma reference after that,
   stop and ask for a DevTools Elements screenshot rather than trying a
   second blind CSS change.

---

## 8. Reference

- Task brief: `UI_UX_Evaluation_Task_MudBlazor_.docx`
- UX/accessibility audit of the two current screens: `ux_audit.md`
- Figma tokens source of truth for the sidebar component family
  (Regular / Child / Parent variants, all states) — extend this same
  variant/token pattern to every new component added for Screen A and B.
