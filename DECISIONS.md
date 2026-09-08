# Decision Notes

Running log of UX/accessibility trade-off decisions made while building the
style library — the source material for the task brief's eventual Decision
Note deliverable. Newest entries first. Each entry: what the literal Figma
value was, why it couldn't be used as-is, what replaced it, and why that
replacement is still "on brand."

---

## AppToggleGroup (unselected) and AppChip — literal blue-00 fails 4.5:1 as text again

**Components:** `AppToggleGroup` (`Components/AppToggleGroup.razor`) and
`AppChip` (`Components/AppChip.razor`), tokens in `wwwroot/styles/tokens.css`,
styled in `wwwroot/styles/components.css`. Screen B's "توزيع الأسئلة" mode
switch and the "نموذج 1" chip.

**Literal Figma values, pixel-sampled off `TaskTwo_from_figma_design.png`
(CLAUDE.md rule 1):**
- Unselected toggle item ("اختيار متعدد"): border AND text/icon both sample
  to `#009DDC` (`--color-blue-00`) — border at x=845,y=593-638; text/icon at
  x=1080-1180,y=617.
- Chip ("نموذج 1"): tint background `#E6F6FC` (new `--color-lightblue-e6`);
  text/icon also `#009DDC`.

**Problem found:** this is the SAME recurring failure already logged twice
below (AppAlert, AppStepper) — `--color-blue-00` as TEXT, not as a
decorative/non-text element, falls under WCAG AA's 4.5:1 normal-text
minimum, not the 3:1 non-text-UI-component minimum.

| Role | Background | Contrast | Verdict |
|---|---|---|---|
| Toggle border (non-text) | white | 3.06:1 vs 3:1 needed | passes — kept literal |
| Toggle unselected text/icon | white | 3.06:1 vs 4.5:1 needed | fails |
| Chip text/icon | `--color-lightblue-e6` (#E6F6FC) | 2.77:1 vs 4.5:1 needed | fails (worse than white — recomputed for this specific tint, not assumed from the white figure) |

**Decision:** both text/icon roles reuse the EXISTING `--color-blue-1f`
substitute (already in `tokens.css`, backing `--status-color-info` and the
Stepper's Selected-label fix below) rather than a new color:
- vs white (toggle): 5.63:1 — passes.
- vs `--color-lightblue-e6` (chip): 5.08:1 — passes (recomputed for this
  background specifically).

The toggle's BORDER keeps the literal `--color-blue-00` unchanged — a
non-text UI component boundary only needs WCAG 1.4.11's 3:1, which it
already clears, so only the text/icon role needed to move (same split
already applied to the Stepper's Active icon-circle vs. label below).

**Also found, not an accessibility fix but flagged the same way (CLAUDE.md
rule 1):** "بيانات النموذج" and "توزيع الأسئلة" are NOT two separate
`.app-panel`s. Pixel-scanning this whole area's left/right border columns
(x=24 / x=1634) top to bottom found the `#DCDCDC` border unbroken from
y≈395 to y≈1047 — one single merged panel, exactly like Screen A's
`.app-sync-block` finding, with a divider LINE (`.app-panel-divider`, new
shared class) as the only internal separation. Full reasoning lives as a
code comment in `demo/ScreenB/QuestionDistribution.razor` rather than
repeated here, since it's a structural reading, not a contrast trade-off.

---

## "بيانات النموذج" panel — one field has no label in Figma at all

**Component:** the 4-field row inside Screen B's "بيانات النموذج" panel
(`demo/ScreenB/QuestionDistribution.razor`, using the shared `AppField`).

**Literal Figma value:** pixel-cropped `TaskTwo_from_figma_design.png`
field-by-field (CLAUDE.md rule 1) — the rightmost of the 4 fields (disabled,
grey fill, value "نموذج 1") has no floating label, no border, and no
placeholder at all. Re-cropped with extra vertical margin above the field to
rule out the label simply being cut off by the crop bounds — confirmed
absent, not a crop artifact. The other 3 fields all have a normal MudBlazor
outlined-field label breaking their border ("اسم النموذج", "كود النموذج",
"وصف النموذج").

**Problem found:** CLAUDE.md §6 requires "a persistent visible label on
every field — placeholders never serve as the only label." A field with
literally no label and no placeholder fails this outright, independent of
contrast/focus/anything else.

**Options considered:**
- Ship it exactly as Figma shows (no label) — rejected outright, direct
  violation of a non-negotiable §6 requirement, not a borderline judgment
  call.
- Invent new label copy not present anywhere in this Figma export (e.g.
  "النموذج الحالي") — rejected: no product-spec text exists for this field's
  purpose, and inventing new Arabic copy is a worse guess than reusing text
  Figma actually shows elsewhere on the same panel.
- Reuse "اسم النموذج" (Model Name), the label already on the adjacent
  editable field that shares this field's exact value ("نموذج 1") — the only
  textual anchor available anywhere in the reference for what this read-only
  field represents.

**Decision:** labeled the disabled field "اسم النموذج", same as the
editable field beside it. This is an ASSUMPTION about what a read-only
duplicate of the model name is for (e.g. a "currently selected model" pinned
display vs. the real editable name field) — not a confirmed product-spec
name. Flagged in the component's own header comment; revisit if a different
label is intended.

**Also found, not silently corrected:** `TaskTwo_Mobile.png`'s stacked field
order swaps positions 2 and 3 ("كود النموذج" before "اسم النموذج") relative
to both `TaskTwo_from_figma_design.png` (desktop) and `TaskTwo_Tablet.png` —
verified by direct pixel crops on all three exports, not a misread. Not
reproduced: reordering only the mobile breakpoint via CSS `order` would
decouple visual order from DOM/tab order, itself a §6 violation ("logical
tab order"). Treated as a mobile-export inconsistency; DOM order stays
desktop/tablet's sequence at every breakpoint.

---

## AppAlert — both severities' literal Figma text/icon colors fail contrast against their own tint

**Component:** `AppAlert` (`Components/AppAlert.razor`), tokens in
`wwwroot/styles/tokens.css`'s "Alert" section, styled in
`wwwroot/styles/components.css`.

**Literal Figma values:** the warning pill's icon+text reuse the same raw
orange already in this file as `--color-orange-ff` (`#ff9800`, also
`--status-color-warning`'s literal value); the success pill's icon+text
reuse the same raw green as `--color-green-00` (`#00C853`, also
`--status-color-success`'s literal value). Both pixel-sampled directly off
`demo/ScreenB/TaskTwo_from_figma_design.png` at its native 1920px width —
not eyeballed.

**Problem found:** unlike AppStatusCard's ring stroke (a decorative accent
where the adjacent status-label word already carries the real information
in a safe-contrast color, so the ring itself was allowed to keep its
failing literal hue), AppAlert's icon+text color pair **is** the real
information-carrying text — there's no separate safe-contrast element next
to it. Computed contrast (standard relative-luminance formula) of each
literal hue against its own pill's tint background:

| Role | Literal color | Tint background | Contrast | Verdict |
|---|---|---|---|---|
| Warning icon/text | `--color-orange-ff` (`#ff9800`) | `--color-peach-fa` (`#FAF0E1`) | **1.91:1** | fails |
| Success icon/text | `--color-green-00` (`#00C853`) | `--color-mint-e6` (`#E6FAEE`) | **2.05:1** | fails |

Both fail WCAG AA's 4.5:1 normal-text minimum by a wide margin — the same
recurring orange/green-family gap already logged for AppStatusCard's ring
colors and the Stepper entries below, resurfacing on a new component.

**Options checked** — kept to the SAME hue angle as each literal color
(HSL ~36° for orange, ~145° for green), varying only lightness, so the
result still reads as "the same orange/green," not an unrelated swatch:

| Candidate (orange, on `--color-peach-fa`) | Contrast | Verdict |
|---|---|---|
| `#B36B00` | 3.71:1 | fails |
| `#A66200` | 4.26:1 | fails |
| `#995C00` | **4.79:1** | **passes — chosen** |
| `#8C5400` | 5.50:1 | passes, more margin |

| Candidate (green, on `--color-mint-e6`) | Contrast | Verdict |
|---|---|---|
| `#00893D` | 4.15:1 | fails |
| `#007C37` | 4.89:1 | passes |
| `#00702F` | **5.74:1** | **passes — chosen, more margin** |

**Decision:** added `--color-orange-99: #995C00` and `--color-green-70:
#00702F` as new raw Variables in `tokens.css` (same hue family as the
existing `--color-orange-ff`/`--color-green-00`, not independently
invented), and wired them as `--alert-color-icon-warning`/
`--alert-color-text-warning` and `--alert-color-icon-success`/
`--alert-color-text-success` respectively. The literal `--color-orange-ff`/
`--color-green-00` values are NOT reused anywhere in AppAlert — unlike
AppStatusCard's ring, there's no decorative-only role here that could
safely keep them. Picked `#995C00`/`#00702F` (not the first passing
candidate in each table) for a bit of margin above the 4.5:1 line rather
than sitting right at the edge.

**Also found and fixed while here (CLAUDE.md §5):** `--header-radius-pill`
was previously a hardcoded `999px` literal with no backing Variable.
AppAlert's own pill needs the identical fully-rounded shape, so rather than
hardcoding `999px` a second time, promoted it to a new `--radius-pill`
Variable that both `--header-radius-pill` and the new `--alert-radius` now
reference.

**Anatomy correction, not just a color fix:** the task's initial anatomy
description assumed "background tint + border." Pixel-scanning every edge
of both reference pills (warning: x=1140-1637/y=298-331; success:
x=1220-1608/y=648-679 at the PNG's native 1920px width) found a direct,
~1px-antialiased transition straight from the page/card background to the
tint color at every edge — no distinct border-stroke hue anywhere. AppAlert
is built with no border, per that pixel evidence, not the original
assumption.

---

## AppStepper — Active/Selected color-token mapping reversed (accessibility substitute follows the color, not the state name)

**Component:** `AppStepper` / `AppStepItem`, styled in
`wwwroot/styles/components.css`, tokens in `wwwroot/styles/tokens.css`.

**What changed:** per your explicit confirmed decision, `.app-stepper__btn--active`
now renders the `--color-darkblue--15` family (previously wired to
`.app-stepper__btn--selected`), and `.app-stepper__btn--selected` now
renders the `--color-blue-00`/`-03`/`-75` family (previously wired to
`.app-stepper__btn--active`). This is a CSS-class-to-token rewiring only —
every token's own definition in `tokens.css` is unchanged, still pointing to
the same raw color it always did, and still named after that color's
*original* Figma role (`--stepper-color-bg-active` is still the blue-00
family; `--stepper-color-bg-selected` is still the darkblue family) per
your instruction not to invent new token names for this.

**Contrast recomputed, not assumed, for both labels under the new wiring**
(standard relative-luminance formula vs. the white page background — the
label sits on the page, not inside the colored circle):

| Role after reversal | Background family | vs. white | Verdict |
|---|---|---|---|
| Active (now darkblue--15, `#152934`) | single fixed swatch | **15.01:1** | passes comfortably, no substitute needed |
| Selected (now blue-00 family, `#009DDC`) | 3-variant (default/hover/pressed) | **3.06:1** | fails WCAG AA 4.5:1 |

**Decision:** the accessibility-driven substitute this problem has always
needed (see the entry below — `--color-blue-1f`, 5.63:1 vs white, same blue
family/hue lineage as `--color-blue-00`, already used elsewhere in the
file) moves with the color, not the state name: it now backs
`.app-stepper__btn--selected`'s label instead of Active's. The token itself
was renamed `--stepper-color-text-active-safe` → `--stepper-color-text-on-brand-safe`
— a name tied to "the blue-00 family needs a safe substitute when used as
text," which stays true regardless of which state renders that family,
rather than a name that says "active" while a Selected element reads it.
Active's label needed no equivalent substitute added: `--color-darkblue--15`
already clears 4.5:1 by a wide margin (15.01:1) as a literal value, so its
existing `--stepper-color-text-selected` alias (unchanged, still exactly
`var(--stepper-color-bg-selected)`) is safe to use as-is.

**Genuine UX side effect, not just a color fix:** Selected went from a
single fixed swatch (Figma gave it no hover/onClick variant) to a real
3-variant hover/pressed family, since that's what came bundled with the
token family it inherited. Verified live (screenshot + computed
`background-color` on `:hover`/`:active`) that Selected now visibly responds
to hover and press, which it never did before this pass.

**Why this isn't a silent "revert" of the entry below:** the ORIGINAL
finding there (blue-00 fails 4.5:1 as text, blue-1f is the right substitute)
is still 100% correct and still applies — only WHICH CSS class needs that
finding applied to it changed. Kept as a separate, later entry rather than
edited in place so the history of what was decided, and why, at each point
stays intact.

---

## AppStepper — Active step label text color deviates from literal Figma value

**Component:** `AppStepper` / `AppStepItem` (Sync Status Dashboard's exam
model stepper, `Components/AppStepItem.razor`, styled in
`wwwroot/styles/components.css`).

**Literal Figma value:** `stepper_Active_txt_bg` = `--color-blue-00`
(`#009DDC`). Figma's own naming (`"_txt_bg"`) indicates this single value
is meant to serve as both the icon-circle background AND the label text
color for the Active state — the same pattern used for every other stepper
state (Pass, Selected).

**Problem found:** the step label is real informational text — the step's
name has no other always-legible element carrying the same meaning (unlike,
say, `AppStatusCard`'s ring stroke, where the status word next to it already
carries the information in a safe-contrast color, so the ring's own
decorative hue is allowed to fail contrast). At the label's actual rendered
size (`--stepper-font-size` = 16px, `--stepper-font-weight` = regular/400),
WCAG 2.1 AA requires 4.5:1 for normal text; the 3:1 large-text exception
only kicks in at ≥24px regular or ≥18.66px (14pt) bold, which this text
doesn't meet. Computed contrast (standard relative-luminance formula,
`--color-blue-00` vs white page background): **3.06:1 — fails**.

**Options checked** before picking a fix (exact contrast, not eyeballed),
restricted to colors already in `tokens.css` so no new hex was introduced:

| Candidate | vs white | Verdict |
|---|---|---|
| `--color-blue-00` (literal Figma value) | 3.06:1 | fails |
| `--color-blue-03` (existing hover blue) | 4.10:1 | still fails |
| `--color-blue-1f` (existing, used by `--status-color-info`) | 5.63:1 | **passes** |

**Decision:** added `--stepper-color-text-active-safe: var(--color-blue-1f);`
in `tokens.css` and used it for the Active label's text color only. The
icon circle's background stays on the literal `--stepper-color-bg-active`
(`--color-blue-00`) unchanged — that's a non-text UI component under WCAG
1.4.11 (3:1 minimum against its surroundings), which it already clears, so
only the text role needed to move. `--color-blue-1f` is the same blue
family/hue lineage as the literal value (a deeper, more teal-leaning blue,
not an unrelated substitute) and was already in use elsewhere in the file,
so this stays a token-reuse decision, not a new hardcoded color.

**Why this isn't a silent "correction back to Figma" risk:** both the
token's own comment in `tokens.css` and the CSS rule in `components.css`
spell out explicitly that this is an intentional, accessibility-driven
deviation from the literal Figma value for this one role — not an error,
not a placeholder — so a future pass shouldn't "fix" it back to
`--color-blue-00` to match Figma literally.
