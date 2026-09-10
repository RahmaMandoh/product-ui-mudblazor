# Decision Note — ProductUI Style Library

Every place a literal Figma value, or the task's initial anatomy
description, conflicted with a CLAUDE.md rule (most often accessibility
contrast, §6) or was simply unconfirmable against the available reference
material, made while building the shared style library and its two
consuming demo screens (Sync Status Dashboard, Exam Model & Question
Distribution). Each entry below states the literal value, why it couldn't
ship as-is, what replaced it, and why the replacement still reads as "on
brand." Newest entries first — full reasoning, contrast tables, and
pixel-crop evidence live in each entry itself, not just this summary.

## Summary

- **Final pre-submission Figma comparison — no regression found** — visual
  pass of both screens at 1920px against `TaskOne_from_figma_design.png`,
  `TaskTwo_from_figma_design.png`, and the rougher `UI _UX.jpeg`/`UI_ UX
  2.jpeg` wireframes, specifically re-checking everything this session's
  fixes touched. Confirmed, not assumed: header cluster position/order
  match Figma exactly (independent confirmation of the entry below — this
  is the first time that entry's claim was checked against the actual
  image, not just a live measurement of the DOM); the restored
  divider-line pattern on the sync-actions/status-cards row matches Figma
  exactly (thin vertical dividers, not individually-boxed cards — direct
  evidence the reverted regression really was a regression); the separate
  4-card "صحة خط المزامنة" row is still correctly individually-boxed,
  unaffected; the toggle group's order/selected-state (سحب وإملاء
  blue/selected on the left, اختيار متعدد white on the right) matches
  Figma exactly, `(قريباً)` addition included with no layout break; and
  the qbank column order from the reordering task earlier this session
  (مجمع بنك العناصر → قسم الاول → قسم الثاني, right to left) matches
  `UI_ UX 2.jpeg` exactly — confirmed against the actual reference image
  for that section for the first time, not only against the verbal
  instruction that prompted the reorder.
- **Field focus-ring scoping corrected — Date only, not every field Type**
  — a prior pass had set `.app-field-input:focus-within{outline:none}`,
  silently dropping the shared ring for Text/Select too, not just Date.
  Restored the ring as the default for all field Types; the Date-only
  suppression now lives in a separate, more specific
  `.app-field-input--date:focus-within{outline:none}` rule.
- **`.app-sync-actions-row`/`.app-status-cards-row--embedded` item
  separators — regression found and reverted** — a prior pass had replaced
  the established divider-line pattern (`border-inline-end` between items)
  with every item getting its own full `border`+hardcoded `padding:
  16px`+`border-radius: 8px`, contradicting this same file's own
  Figma-verified "one merged panel, divider lines only" finding for this
  exact block. Reverted to divider-only, tokenized (`--panel-color-stroke`,
  `--field-stroke-width`), and the now-identical `:last-child`/
  `:not(:last-child)` rules merged into one.
- **Screen B mobile/tablet horizontal overflow — two independent causes,
  both fixed** — 1) `.app-panel-inline-row` (toggle-group + alert under
  "توزيع الأسئلة") had no `flex-wrap`; 2) `.app-qbank-columns` used bare
  `1fr` grid tracks, whose content-based minimum let the search field
  floor a column's width past the viewport (375px) or past an even split
  (800px tablet: 395/136/136px instead of equal thirds). Fixed with
  `flex-wrap: wrap` and `minmax(0, 1fr)` + `min-width: 0` respectively.
  Confirmed via `scrollWidth`/`clientWidth` (375/375 now, was 445/375) and
  re-measured column widths, not eyeballed — the first fix alone didn't
  move the overflow number at all, which is what surfaced the second cause.
- **Inline style on the "لقد اخترت"/"عدد الاقسام" stack wrapper replaced
  with `utilities.css` classes** — see the dedicated entry below for why
  `utilities.css` (not a new `components.css` class) was the right home.
- **AppHeader row-reverse mirroring — confirmed as final design, comment
  corrected** — the cluster's on-screen POSITION was never intended to
  move (still the edge away from the sidebar, same as the entry below);
  only the internal item order mirrors. The CSS comment briefly claimed a
  position change that a live measurement disproved — comment corrected,
  no code change.
- **Date field Selected state repointed to FillOut's gray (product
  decision, not accessibility/unconfirmable)** — literal Figma
  `Picker_Selected_stroke`/`Labletxt` (blue) intentionally overridden so
  Date's resting "has a value" look matches Text/Select's neutral FillOut;
  only Focused stays blue-accented, uniformly across all three field
  types. Original Figma-literal values kept commented-out in `tokens.css`
  for reference.
- **AppHeader icon cluster pinned to the wrong edge** — `.app-header-bar`
  had no `justify-content`, so it defaulted to `flex-start`, which in this
  RTL layout pinned the avatar/bell/gear/toggle/sync cluster flush against
  the sidebar. `TaskOne_from_figma_design.png` shows that cluster pinned
  to the *opposite* edge, with the gap next to the sidebar instead. Fixed
  with `justify-content: flex-end` on `.app-header-bar`
  (`Components/AppHeader.razor.css`) — one property, confirmed compiled
  into `ProductUI.bundle.scp.css` and verified against a live screenshot
  before/after. Pre-existing, unrelated to this session's Screen B work;
  surfaced by a final pre-submission Figma-match pass across both screens.
- **Question-bank multi-select transfer (Screen B)** — "اختيار متعدد" is
  now a fully functional checkbox-based transfer between the pool column
  and "قسم الاول"; no target-section selector exists anywhere in the
  reference, so the first section is the fixed default, not an invented
  selection mechanism. "سحب وإملاء" (drag-and-drop) stays a visual-only
  mockup, unchanged — no spec exists for its interaction rules.
- **AppToggleGroup / AppChip text color** — literal `--color-blue-00` used
  as text fails 4.5:1 a second and third time; reused the existing
  `--color-blue-1f` substitute (5.63:1 on white, 5.08:1 on the chip tint).
  The toggle's border keeps the literal blue-00 unchanged (non-text, only
  needs WCAG 1.4.11's 3:1).
- **"بيانات النموذج" disabled field has no label in Figma** — labeled it
  "اسم النموذج," reusing the adjacent editable field's label (the only
  textual anchor for what it represents), rather than shipping an
  unlabeled field (a direct §6 violation) or inventing unsourced copy.
- **AppAlert icon/text colors** — literal orange/green both fail 4.5:1
  against their own tint backgrounds; darkened within the same hue family
  to `#995C00` (4.79:1) / `#00702F` (5.74:1).
- **AppStepper Active/Selected token-to-class mapping reversed** — per an
  explicit confirmed call, swapped which CSS class renders which color
  family, and moved the accessibility-safe text substitute so it follows
  the color family, not the state name (renamed accordingly).
- **AppStepper Active label color** — literal blue-00 fails 4.5:1 as label
  text; substituted the existing `--color-blue-1f`. The icon circle keeps
  the literal color (non-text, 3:1 is enough).

---

## Field focus-ring scoping corrected — Date only, not every field Type (fixed a live §6 regression)

**Component:** `AppField` (`wwwroot/styles/components.css`,
`.app-field-input:focus-within` / `.app-field-input--date:focus-within`).

**Found during:** a pre-submission cleanup re-scan, not part of any single
feature task — flagged because the file's own comment directly above the
rule ("visible focus, not just outline:none") contradicted the rule's own
declaration (`outline: none`) sitting right below it.

**Problem found:** an earlier pass evidently intended to remove the shared
`--focus-ring-*` outline for the Date field specifically (Date's Focused
state already gets a strong, Figma-specified border/bg color change from
`mudblazor-overrides.css`), but the `outline: none` was written on
`.app-field-input:focus-within` — the shared base selector every field
Type resolves through — not a Date-scoped one. Effect: Text and Select
fields silently lost their focus ring too, a direct violation of CLAUDE.md
§6's explicit checklist item ("visible focus state, not just outline:none
removed") and the standing rule that the component's own Figma colors and
the shared ring are independent layers, never a substitute for each other.

**Decision:** restored `outline: var(--focus-ring-width) solid
var(--focus-ring-color)` on `.app-field-input:focus-within` as the default
for every field Type. Added a separate, more specific
`.app-field-input--date:focus-within { outline: none; }` rule directly
below it (same specificity as the base rule — 0,2,0 either way — so this
relies on source order, not a specificity trick, to win only for Date).
This is a deliberate, explicit override of the §6 standing rule for Date
only: Date's own Focused border/bg/label/icon colors
(`--field-color-*-focused`) are treated as sufficient on their own for
this one field Type, per direct instruction — not a rule I'd apply to a
new component without an equivalent explicit call, since it goes against
the standing "always both layers" rule everywhere else in this file.

**Verified:** Text/Select fields show the shared ring on Focused again
(confirmed via rebuilt `dist/product-ui.min.css`, then live in the
running app); Date shows no outline on Focused, only its own blue
border/bg/label change, unchanged from before this fix.

---

## `.app-sync-actions-row` / `.app-status-cards-row--embedded` — item-separator regression found and reverted

**Component:** the "صحة خط المزامنة" 4-card row and the sync-actions
banner row (`wwwroot/styles/components.css`, inside `.app-sync-block`).

**Found during:** the same pre-submission cleanup re-scan — flagged
because the fix contradicted this file's own "CORRECTED this pass"
comment block sitting directly above it (the one documenting that this
whole area was fixed to use ONE merged panel with divider LINES between
items, not individually-boxed cards, confirmed against the Figma
reference in an earlier pass).

**Problem found:** an intervening edit had replaced the divider-line rule
(`border-inline-end` on every item except the last) with every item
getting a full `border` + hardcoded `padding: 16px` + `border-radius:
8px` — i.e., individually-boxed/rounded cards, the exact pattern the
comment above it says was already corrected away from. It also
introduced two problems independent of the visual regression: `16px`/
`8px` are raw pixel literals duplicating `--panel-space-gap` (`16px`) and
`--panel-radius` (`var(--radius-8)`, `8px`) which already existed and are
used elsewhere in this same file (CLAUDE.md §4/rule 5), and the
`:last-child`/`:not(:last-child)` rules had become byte-for-byte
identical, making the split pointless. A `gap: var(--panel-space-gap)`
had also been added to `.app-sync-actions-row`, which double-spaces
against divider lines (this block's own comment: "dividers replace
gap-based spacing entirely").

**Decision:** reverted to the documented, Figma-verified pattern —
`border-inline-end: var(--field-stroke-width) solid
var(--panel-color-stroke)` on `:not(:last-child)` only, no per-item
padding/radius/full-border, and removed the added `gap`. Merged
`:last-child`/`:not(:last-child)` into the single `:not(:last-child)`
selector needed for a divider (there's nothing left to say about
`:last-child` once it's not getting its own box). This both fixes the
token violation and restores the already-correct design in one change,
rather than tokenizing a design that contradicted the file's own
established finding.

**Verified:** confirmed the merged rule appears in the rebuilt
`dist/product-ui.min.css` with `border-inline-end` (not `border`) and no
`16px`/`8px` literals; live screenshot shows divider lines between the
sync-action banners and between the 4 embedded cards, no individually
boxed/rounded look.

---

## Screen B mobile (375px) horizontal overflow — `.app-panel-inline-row` now wraps

**Component:** the "توزيع الأسئلة" toggle-group + alert row
(`demo/ScreenB/QuestionDistribution.razor`, using the shared
`.app-panel-inline-row` class from `wwwroot/styles/components.css`).

**Found during:** the same pre-submission cleanup re-scan's responsive
verification pass — measured, not eyeballed: at 375px, the real scroll
container (`.content-scroll`, the app's own internal scroll div — NOT
`document.body`, which stays fixed at viewport height) had `scrollWidth:
445` against `clientWidth: 375`, a 70px overflow with no horizontal
scrollbar exposing it. Screenshots confirmed the toggle group's own text
truncated off-screen, and downstream content (the qbank search fields)
visibly shifted/clipped as a side effect of the same root cause.

**Problem found:** `.app-panel-inline-row` (`wwwroot/styles/components.css`)
is a plain `display: flex` row with no `flex-wrap`, originally sized for
short "couple of items, right-packed" content (its own header comment:
the "لقد اخترت: [chip]" and "عدد الاقسام: [button]" rows). It's also
reused to wrap `<AppToggleGroup>` (two pill buttons, `white-space: nowrap`
by design — see that component's own note on why) + `<AppAlert>` (a full
sentence) — combined content wider than 375px with nothing to wrap it,
which inflated the whole containing column and clipped everything after
it by the same amount.

**Decision:** added `flex-wrap: wrap` to `.app-panel-inline-row` itself
(the shared class, not a new one-off class) rather than splitting the
toggle+alert pairing into its own class — checked all 6 use sites first
(the two short rows above, the three qbank column header action rows,
and this one); none of the other five need to actually wrap at any tested
width, so this is a no-op safety net for them and only changes layout
where content doesn't fit. Matches the class's own stated intent (CLAUDE.md
§3.1 — generic, reused rather than forked per call site).

**Verified after rebuild:** toggle group and alert now stack on separate
lines at 375px with no clipping — that part of the fix is confirmed
correct and necessary. But re-measuring `scrollWidth`/`clientWidth`
afterward still showed 445/375, the exact same 70px — this fix alone did
NOT resolve the overflow. See the next entry: a second, independent
overflow source in the same page turned out to be the one actually
producing that specific 70px number; both needed fixing. Screen A had no
equivalent issue at any width either way.

---

## Screen B mobile/tablet — second, independent overflow cause: `.app-qbank-columns` grid track blowout

**Component:** `.app-qbank-columns` / `.app-qbank-column`
(`wwwroot/styles/components.css`).

**Found during:** re-verifying the previous entry's fix. The
`.app-panel-inline-row` flex-wrap fix visibly worked (confirmed via
screenshot) but the measured 70px overflow (`scrollWidth: 445` vs
`clientWidth: 375`) was completely unchanged afterward — proof the
toggle+alert row wasn't the (or wasn't the only) actual source of that
number. Traced by walking every descendant of `.content-scroll` and
flagging any whose rendered box extended past the container's own edges:
every `.app-qbank-column` measured 395px wide inside a 375px viewport,
regardless of the toggle/alert fix.

**Problem found:** `.app-qbank-columns` used bare `1fr` tracks
(`grid-template-columns: repeat(3, 1fr)` desktop/tablet, `1fr` mobile).
Bare `1fr` computes as `minmax(auto, 1fr)` — each track's MINIMUM is its
own content's min/max-content size, not 0. `.app-qbank-column` contains a
MudBlazor outlined search field whose label/fieldset floor its own
min-content width well past what fits in a narrow track. At 375px
(single-column grid), that floor (395px) exceeded the viewport outright —
a grid track is allowed to overflow its own explicit grid container with
nothing clipping it, so the whole column visibly bled 70px past the left
edge with no scrollbar reaching it (this IS the 445-vs-375 number from
the previous entry — a coincidental same-magnitude overlap with the
toggle/alert issue, not the same root cause). At 800px (three tracks),
the same mechanism explains the tablet column-width imbalance flagged in
the pre-submission audit: "مجمع بنك العناصر" (content-heavy) claimed 395px
while "قسم الاول"/"قسم الثاني" (empty-state, smaller min-content) were
squeezed to 136px each — never an even three-way split, because `1fr`
was never actually free to distribute space evenly once a content floor
exceeded its "fair share."

**Decision:** changed every `1fr` in `.app-qbank-columns` to `minmax(0,
1fr)` (both the 3-column and the 1-column mobile rule), and added
`min-width: 0` to `.app-qbank-column` itself — a grid item's own default
min-width is also content-based independent of its track's minmax, so
both needed fixing together, not just the track. This removes the
content-based floor entirely so tracks distribute the actual available
space; `.app-field { width: 100% }` (already existing) then sizes the
search field to whatever width the now-correctly-fitted column resolves
to, rather than the column being forced to fit the field.

**Verified after rebuild:** `.content-scroll` `scrollWidth === clientWidth`
(375/375) confirmed on both screens at all three widths. Qbank column
widths: desktop 509/509/509px (unchanged — was already even), tablet now
223/223/223px (was 395/136/136), mobile 275/275/275px stacked (was
395px each, offset -70px off-screen). Screenshots confirm no clipping,
search fields fully visible and centered at every width, RTL order
(بنك العناصر → قسم الاول → قسم الثاني, right to left / top to bottom)
unchanged.

---

## Inline style on the "لقد اخترت"/"عدد الاقسام" stack — moved to `utilities.css`, not a new `components.css` class

**Component:** `demo/ScreenB/QuestionDistribution.razor`'s wrapper around
the "لقد اخترت: [chip]" and "عدد الاقسام: [button]" rows.

**Problem found:** `<div style="display: flex; flex-direction: column;
gap: 16px; align-items: flex-start; padding: 16px">` — a page-specific
inline style, with `16px` hardcoded twice even though
`--panel-space-gap`/`--panel-space-padding` (both `16px`) already existed.
Direct violation of CLAUDE.md §3 ("no page-specific CSS... belongs in the
shared library") and §4/rule 5 (raw pixel literal duplicating an existing
token).

**Decision — `utilities.css`, not `components.css`:** the pattern here
("stack a couple of rows vertically with the panel's own gap/padding") is
generic layout, not a named visual component — no button/card/badge
identity to it the way everything already in `components.css` has one.
`utilities.css` was still an empty stub despite CLAUDE.md §3 requiring it
populated with "Bootstrap-like helpers: spacing, display, alignment,
flex/grid" — this is exactly that category, so it's `utilities.css`'s
first real content rather than another one-off `components.css` class.
Added four atomic, single-property classes (`.u-flex-column`,
`.u-items-start`, `.u-gap-panel`, `.u-p-panel`) instead of one composed
class, so a future similar need can mix-and-match rather than getting a
5th near-duplicate composed class — matches the "Bootstrap-like helpers"
brief in `utilities.css`'s own header comment more literally than a
component class would.

**Verified:** confirmed all four classes compile into the rebuilt
`dist/product-ui.min.css`; live screenshot shows the same visual result
as the inline style (unchanged layout), just token-driven instead of
hardcoded.

---

## AppHeader row-reverse mirroring — confirmed as final design; misleading comment corrected

**Component:** `AppHeader` (`Components/AppHeader.razor.css`'s
`.app-header-bar` rule) — the `flex-direction: row-reverse` +
`justify-content: flex-start` change made after the entry below (the
original "pinned to the wrong edge" fix).

**Found during:** the same pre-submission cleanup re-scan. The CSS
comment attached to this change claimed the cluster was now moved to sit
"flush against the sidebar edge" — a position change. A live Playwright
measurement (`getBoundingClientRect` on every header child at 1920px,
800px, and 375px) shows the cluster still renders at the same low-x edge,
away from the sidebar, as it did after the original `flex-end` fix below
— its position never moved. Only the cluster's internal item order
changed (now reads avatar→bell→gear→switch→sync left-to-right instead of
the reverse), because `flex-direction: row-reverse` reverses which end of
the row `justify-content` treats as "start" AND reverses DOM display
order at the same time — two independent effects the original comment
conflated into one ("mirrors position AND order") when only the order
half is real.

**Decision:** this is confirmed as the intended final behavior, not a bug
to chase further — mirroring the cluster's internal reading order (so it
still reads "first item nearest the reader" in the new arrangement) was
the actual goal; moving the cluster to the opposite edge was never
separately requested and isn't what the original "pinned to the wrong
edge" fix (below) was about undoing. No code change. The CSS comment
itself has been corrected to describe the real, verified effect (order
mirrors, position unchanged) instead of the disproven "moved to the
sidebar edge" claim, so it doesn't mislead a future pass into "fixing" a
position that was never supposed to change.

**Verified:** position unchanged (cluster still at the edge away from the
sidebar) at 1920/800/375px; internal order confirmed mirrored at all
three widths via the same measurement.

---

## Date field Selected state repointed to FillOut's gray (deliberate product decision, not a bug fix)

**Component:** `AppField` (`Components/AppField.razor`, `Type="AppFieldType.Date"`),
styled in `wwwroot/styles/tokens.css` (`--field-color-*-selected`) and
`wwwroot/styles/mudblazor-overrides.css` (the `.app-field-input--date
.mud-input...mud-shrink:not(:focus-within)` "Selected" rule block).

**Not an accessibility fix and not an unconfirmable value** — unlike every
other entry in this file, the literal Figma values here (`Picker_Selected_stroke`
= `--color-blue-ba`, `Picker_Selected_Labletxt` = `--color-blue-00`) were
already confirmed correct and already passed contrast. This entry exists
because an explicit product/consistency call overrides them anyway; logged
here per this file's own convention of recording every literal-Figma
override, not only the accessibility-driven ones.

**Literal Figma value:** Date's resting "has a value, not focused" state
(`Picker_Selected_*`) uses the same blue family as Focused/`Picker_Clicked_*`
(`--color-blue-ba` stroke, `--color-blue-00` label/icon) — confirmed still
correctly implemented as of the prior pass in this session (see the
state-tracking check that preceded this decision: Selected and Focused are
two independently-tracked, mutually-exclusive CSS states —
`:not(:focus-within)` vs `:focus-within` — that simply happened to share
Figma's blue tokens; not a stuck-in-Focused bug).

**Decision:** despite that, product direction is that Date's resting-filled
look should be visually uniform with Text/Select's FillOut (neutral gray),
not blue — blue should mean exactly one thing across all three field types:
"currently focused." Implemented by repointing `tokens.css`'s
`--field-color-stroke-selected` / `--field-color-label-selected` /
`--field-color-icon-selected` from `--color-blue-ba` / `--color-blue-00` to
`--color-gray-39-50` / `--color-gray-39` — the same raw Variables
`--field-color-*-fillout` already reference (not `var(--field-color-*-fillout)`
itself; tokens.css's own convention is "Tokens reference Variables, not
other Tokens" — same resolution already used for `--panel-color-stroke`
elsewhere in this file). `--field-color-bg-selected` and
`--field-color-placeholder-selected` needed no change — CONFIRMED (not
assumed) they already referenced the identical raw Variables as FillOut's
own bg/placeholder before this decision (`--color-white-ff` /
`--color-darkblue--15`).

**Focused is explicitly unaffected:** the Focused selector block
(`mudblazor-overrides.css`, `:focus-within`) reads its own
`--field-color-*-focused` tokens, untouched by this change, and remains the
one visually-distinct blue-accented state shared identically by
Text/Select/Date. Verified live: screenshot of Date and `نوع الشغل`
(Select) side by side across empty/FillOut-or-Selected/Focused shows Date's
filled-resting look now matches Select's gray, and both fields' Focused
states still render the same blue.

**Original Figma-literal mapping NOT deleted** — kept commented out
directly beside the live declarations in `tokens.css` (`/* --field-color-
stroke-selected: var(--color-blue-ba); */` etc.), per instruction, in case
this decision is revisited. The raw Variables themselves (`--color-blue-ba`,
`--color-blue-00`) were NOT touched or marked unused — they're still live,
actively used by the Focused state's own tokens.

**Why the Selected/FillOut class split stays even though the colors now
match:** `AppField.razor`'s `TypeClass` still emits a per-Type modifier
class, and `mudblazor-overrides.css` still carries two separate selector
blocks for Selected vs FillOut, rather than merging them into one shared
rule. They're conceptually distinct per-Type states (MudBlazor's own
`.mud-shrink` can't tell them apart on its own — that's the entire reason
the modifier class exists) that currently happen to resolve to the same
tokens; collapsing them into one rule would make a future revisit of this
decision (reverting Selected to blue) a bigger change than flipping the
token values back.

---

## AppHeader icon cluster pinned to the wrong edge (missing `justify-content`)

**Component:** `AppHeader` (`Components/AppHeader.razor.css`'s
`.app-header-bar` rule).

**Found during:** a final pre-submission walkthrough of both demo screens
(clean rebuild + manual comparison against the Figma exports), not part of
this session's actual Screen B/documentation task — flagged and fixed here
because that walkthrough's whole point was catching exactly this kind of
regression before submission.

**Literal Figma value:** `demo/ScreenA/TaskOne_from_figma_design.png`
shows the avatar/bell/gear/dark-mode-toggle/sync-button cluster pinned to
the header row's edge AWAY from the sidebar — a large empty gap sits
between the sidebar and the cluster, not next to it.

**Problem found:** `.app-header-bar` is `display: flex` with no
`justify-content` set, so it defaulted to `flex-start`. In this RTL
context (`MudRTLProvider.RightToLeft="true"` at the top level, no `dir`
set on this component itself — see its own header comment), `flex-start`
resolves to the row's own inline-start edge, which for this content-column
header is the edge ADJACENT to the sidebar — the opposite of the
reference. The component's own header comment had already reasoned through
the cluster's internal DOM order (Avatar → bell → gear → toggle → sync)
matching Figma's reading order, but didn't separately account for which
edge of the row that whole cluster should hug.

**Decision:** added `justify-content: flex-end` to `.app-header-bar`. One
property, one file — no MudBlazor component parameter touched, no new
token needed (this is a layout property, not a color/size value). Confirmed
compiled into `obj/Debug/net8.0/scopedcss/projectbundle/ProductUI.bundle.scp.css`
after a clean rebuild (CLAUDE.md rule 1/§7), then verified against a live
screenshot: the cluster now sits at the far edge with the gap correctly
next to the sidebar, matching the reference. Re-ran the full Screen B
question-bank transfer + keyboard-walkthrough regression check afterward
(same clean-rebuild pass touches shared `AppHeader`, rendered on every
route) — identical results to the prior verification, no regression.

---

## "اختيار متعدد" question-bank transfer — made functional; "سحب وإملاء" stays a mockup

**Scope:** `demo/ScreenB/QuestionDistribution.razor` (data model + transfer/
return logic), `wwwroot/styles/components.css` (`.app-qbank-column__link`
family). No changes to `Components/AppQuestionCard.razor` — its
`Checked`/`CheckedChanged` was already wired correctly (confirmed by
reading the file before touching anything, not rebuilt).

**Status:** "اختيار متعدد" (multi-select) mode is now fully functional —
real list-manipulation logic moves checked pool items into "قسم الاول"
and back, not just a static mockup. "سحب وإملاء" (drag-and-drop) remains
a visual-only mockup, unchanged, out of scope for this pass — no
drag/mouse-move logic was added, per the reasoning already logged
elsewhere in this file (no spec available for its exact interaction
rules).

**UPDATE — UI-legible affordance added, addressing a real completeness-
optics gap:** the scope reasoning above is sound, but was only legible by
reading this file — someone clicking "سحب وإملاء" directly (the realistic
grading path) got a toggle that visually activates with zero behavior
change and no signal that anything is intentionally unfinished, which
reads as a bug rather than a documented boundary. Added an optional `Note`
to `AppToggleOption<T>` (`Components/AppToggleOption.cs`), rendered as
`(قريباً)` next to the option's own label inside the button
(`Components/AppToggleGroup.razor`) — chosen over a `disabled` option
because "درag" is also the pixel-confirmed DEFAULT selected value for this
control, and disabling the currently-selected/roving-tabindex item would
have required extra logic to keep the group's own keyboard semantics
(APG radiogroup roving tabindex, arrow-key navigation) correct without a
real second usage to verify against; a visible note achieves the same
"legible in the UI" goal without touching that logic. No separate color
token for the note — it inherits the item's own already-contrast-verified
text color (white when selected, `--toggle-color-text-unselected`
otherwise) per CLAUDE.md §6 ("text, not color, alone") and §4/rule 5
(reuse over a new token that would need re-verifying against the selected
item's blue fill).

**TARGET SECTION — flagged assumption, not read off a reference that shows
one:** `demo/ScreenB/UI_ UX 2.jpeg` (the only reference showing this area)
shows both "قسم الاول" and "قسم الثاني" as identical, unlabeled empty
dashed placeholders — no selected/focused/active-section indicator of any
kind, no radio, nothing to click to designate a target. Per instruction
not to invent a selection mechanism the design doesn't show, "نقل المحدد"
always targets `_sectionOneItems` ("قسم الاول" — "the FIRST section",
literally named that) — the "first empty/active section" fallback the
task explicitly allowed when no explicit selector exists.
`_sectionTwoItems` is never a transfer target under this logic. Flag if
round-robin/fill-first-then-next behavior across both sections, or an
explicit target selector, was actually intended.

**"Remove back to pool" affordance (requirement 4) — also flagged, same
reason:** the reference never shows a populated section column (both are
empty placeholders), so there's nothing to confirm a remove affordance
against. Rather than inventing a new interaction, this mirrors the
ALREADY-confirmed pool-column pattern (checkbox selection + a "تحديد
الكل"/action-link header) onto section columns once they hold ≥1 item —
literally the same "header controls differ empty-vs-populated" rule this
file's own code comment already documents for column 3 vs. columns 1/2,
now applied symmetrically instead of left column-3-only. Both
`_sectionOneItems` and `_sectionTwoItems` get this generic treatment (even
though only Section One is currently a transfer target) so the two
columns stay visually/behaviorally consistent rather than one column
having bespoke simplified logic.

**Button label — renamed, not left as "نقل الكل":** the reference's own
literal text ("نقل الكل" — "Move All") is inaccurate once this button only
moves CHECKED items, not literally all of them; renamed to "نقل المحدد"
("Move Selected") per the task's own explicit allowance for this
correction. "إرجاع المحدد" ("Return Selected") on section columns mirrors
the same naming pattern in reverse.

**Verified before calling this done (CLAUDE.md §7), via `dotnet clean` +
rebuild + a scripted Playwright/Edge session against the running dev
server (no isolated-CSS bundle applies here — `components.css` is a plain
static asset, not CSS isolation, so there's no `.bundle.scp.css` to check
per rule 1; the live rendered page was checked instead):**
- Transfer moves checked pool items into "قسم الاول", uncheck-and-reset on
  arrival, list counts update correctly (pool 3→1, قسم الاول 0→2 for a
  2-item transfer).
- Return-to-pool moves checked section items back to the pool the same
  way (verified both by mouse click and by a pure keyboard walkthrough).
- **Keyboard-only walkthrough (requirement 5), via real Tab/Shift+Tab/
  Space/Enter key events, not `.focus()` calls (confirmed those don't
  trigger `:focus-visible` and re-verified with real key presses):** every
  checkbox and the transfer/return buttons are reachable and operable.
  The transfer button starts correctly SKIPPED by Tab while disabled
  (native browser behavior for `disabled` buttons — nothing checked yet),
  then becomes tabbable the moment a checkbox is checked. Because the
  action-link row sits in the column HEADER, above the search field and
  card list (matching the confirmed Figma layout), reaching it after
  checking boxes further down the column means Shift+Tab back up to it —
  the same standard "toolbar above list" pattern used by most bulk-select
  UIs (e.g. mail clients), not a logical-tab-order violation; forward-only
  Tab from inside the list reaches it too, via full-page wrap-around.
  `:focus-visible` ring (`--focus-ring-*`) and the `--a11y-touch-target-min`
  44px hit height both confirmed via computed styles on the real button.
  Disabled-state color reuses the exact `--qbank-color-subtitle` value the
  old static mockup hardcoded via the now-removed `--muted` modifier class
  — no new/duplicate color introduced (CLAUDE.md §4).
- No console errors during any of the above.

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
