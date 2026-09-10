# Decision Note — ProductUI Style Library

This is where we explain the calls made while building this style library
and its two demo screens — especially the spots where Figma's literal
design and our accessibility/consistency rules didn't agree. Each entry
below says what the design showed, why we couldn't ship it exactly as-is,
what we did instead, and how we checked it. Entries are newest first.

## Summary

A few themes came up more than once:

- **Colors that were hard to read.** A handful of colors taken straight
  from the design — a bright blue used as text, an orange and green on
  alert messages, a step label — didn't stand out enough against their
  background for comfortable reading. We darkened them slightly, staying
  in the same color family, until they were easy to read, and kept a note
  of the original design color nearby in case it's ever needed again.
- **Right-to-left layout.** The version of MudBlazor we're using doesn't
  support right-to-left the way some older guides describe. We found the
  one setting that actually works, applied it once at the very top of the
  app, and every screen follows from there automatically — see the RTL
  section in README.md for exactly what we set.
- **Drag-and-drop.** Moving questions between sections by checking boxes
  and clicking "move" works fully today. Dragging and dropping them is
  still just a switch you can flip — nothing happens yet, because there's
  no design showing how that should actually behave. It's labeled "coming
  soon" so it's obviously intentional, not broken.
- **Small-screen layout.** One of the screens was scrolling sideways on
  phones and tablets, with content spilling past the edge. That turned out
  to be two separate small bugs sitting on top of each other. Both are
  fixed now, and nothing overflows anymore.

The rest of this file walks through the smaller decisions one at a time.
The technical detail behind each one — exact contrast numbers, pixel
measurements — is kept underneath, for anyone who wants to double-check
the work.

---

## Field focus ring — was accidentally removed from Text and Select fields, not just Date

**Where:** `AppField`, in `wwwroot/styles/components.css`
(`.app-field-input:focus-within` / `.app-field-input--date:focus-within`).

**What happened:** An earlier change meant to turn off the shared focus
ring for the Date field only (Date already gets its own strong blue
border/background when focused). But the `outline: none` rule ended up on
the base selector every field type uses, not a Date-only one — so Text and
Select fields silently lost their focus ring too. That's a real
accessibility problem: every interactive element needs a visible focus
state, and a component's own focus styling is never a substitute for the
shared ring.

**Fix:** Put the shared ring back as the default for every field type, and
added a separate, more specific rule
(`.app-field-input--date:focus-within { outline: none; }`) that only turns
it off for Date. This is a deliberate exception for Date specifically — not
a pattern to copy for other fields without the same kind of explicit
sign-off.

**Checked:** Rebuilt the CSS, confirmed Text and Select show the ring again
on focus, and Date still shows just its own blue border with no outline.

---

## Sync dashboard rows — accidentally turned into boxed cards, reverted back to dividers

**Where:** the "صحة خط المزامنة" (sync health) row and the sync actions
row, in `wwwroot/styles/components.css`.

**What happened:** This area had already been fixed once before to use one
merged panel with thin divider lines between items, matching Figma. A
later edit undid that by mistake — it gave every item its own full border,
rounded corners, and hardcoded 16px/8px spacing, which both looked wrong
(individually boxed cards instead of one panel) and reintroduced hardcoded
pixel values where tokens already existed for the same numbers.

**Fix:** Reverted to the divider-line pattern — a single border between
items, using the existing spacing/radius tokens instead of hardcoded
numbers — and cleaned up a couple of now-duplicate CSS rules along the way.

**Checked:** Confirmed the rebuilt CSS uses divider lines and no raw pixel
values, and a live screenshot shows thin dividers between items again, not
individual boxes.

---

## Screen B was scrolling sideways on phones — cause #1: a row that didn't wrap

**Where:** the toggle group + alert row under "توزيع الأسئلة", using the
shared `.app-panel-inline-row` class.

**What happened:** At 375px wide, the page was 70px wider than the screen,
with no scrollbar showing it — content was just getting cut off.
`.app-panel-inline-row` is a plain flex row that was built for short
content and never needed to wrap. On this page it holds a toggle button
pair plus a full sentence of alert text, which doesn't fit on one line at
phone width.

**Fix:** Added `flex-wrap: wrap` to the shared class (not a one-off fix
just for this page) — checked all six places that use this class first,
and confirmed none of the others need to wrap at any width we tested, so
this only changes behavior where it was actually needed.

**Checked:** The toggle group and alert now stack on separate lines with
nothing cut off. But re-measuring afterward showed the exact same 70px of
overflow — this fix was necessary but not sufficient. See the next entry
for the other cause.

---

## Screen B was scrolling sideways — cause #2: grid columns that wouldn't shrink

**Where:** `.app-qbank-columns` / `.app-qbank-column` in
`wwwroot/styles/components.css`.

**What happened:** Even after fixing the row above, the page was still
70px too wide. Turned out every question-bank column was rendering at
395px inside a 375px screen. The column grid used plain `1fr` tracks,
which in CSS have a hidden floor: a track won't shrink below its own
content's natural minimum width. Each column has a MudBlazor search field
whose label and border need a certain minimum width, and that minimum was
wider than the whole phone screen — so the grid just let the column
overflow instead of shrinking it.

The same issue explained something noticed earlier on tablet too: at
800px, the busier first column was claiming 395px while the other two got
squeezed to 136px each instead of splitting evenly — `1fr` was never free
to share space evenly once one column's content had a bigger minimum than
its "fair share."

**Fix:** Changed every `1fr` to `minmax(0, 1fr)`, and added `min-width: 0`
to the column itself (a grid item has its own separate content-based
minimum that needs resetting too). This removes the artificial floor so
the three columns actually share the available space evenly.

**Checked:** After rebuilding, the page no longer scrolls sideways at any
of the three widths we test (desktop/tablet/mobile), and the three columns
measure equal widths at each one.

---

## Inline style on the question-count summary — moved into utilities.css

**Where:** the "لقد اخترت"/"عدد الاقسام" block in
`demo/ScreenB/QuestionDistribution.razor`.

**What happened:** This block had a hand-written inline style
(`display: flex; flex-direction: column; gap: 16px; ...`) directly in the
page markup — a page-specific style, and one that hardcoded `16px` twice
even though tokens for that exact spacing already existed. Both go against
the project's own rules: no page-specific CSS, and no repeating a value
that already has a token.

**Fix:** Added four small, single-purpose classes to `utilities.css`
(`.u-flex-column`, `.u-items-start`, `.u-gap-panel`, `.u-p-panel`) instead
of one big combined class, so future similar needs can mix and match
rather than adding another near-duplicate. This is also `utilities.css`'s
first real content — it existed but was essentially empty before this.

**Checked:** Confirmed the new classes compile into the built CSS, and the
page looks identical to before — same layout, just token-driven instead of
hardcoded.

---

## Header icon order — confirmed this was intentional, just fixed a misleading comment

**Where:** `AppHeader`, in `Components/AppHeader.razor.css`.

**What happened:** A code comment claimed that reversing the header's icon
row also moved the whole cluster to a different edge of the header.
Measuring it directly in the browser showed that wasn't true — the
cluster's position never changed, only the order the icons appear in
changed (avatar, bell, gear, toggle, sync — now left to right instead of
the reverse).

**Decision:** No code change needed — the icon-order mirroring was always
the actual goal, moving the cluster was never part of it. Just corrected
the comment so it describes what the CSS actually does, instead of a claim
that turned out to be wrong.

---

## Date field's "filled" color — changed from Figma's blue to match the other fields' gray (product call, not an accessibility fix)

**Where:** `AppField` (Date type), tokens in `wwwroot/styles/tokens.css`,
styling in `wwwroot/styles/mudblazor-overrides.css`.

**Note:** unlike most entries here, this one isn't fixing a contrast
failure — Figma's blue value already passed contrast fine. It's a
deliberate product decision, logged here because this file tracks every
place we departed from the literal Figma value, not just the
accessibility-driven ones.

**What Figma shows:** once a Date field has a value but isn't focused,
Figma colors it the same blue as when it's actively focused. Text and
Select fields, by contrast, use a neutral gray for their equivalent
"has a value" state and save blue only for Focused.

**Decision:** made Date match Text and Select — gray when it has a value,
blue only when actually focused — so blue means one consistent thing
("this field is focused") across all three field types instead of two
different things depending on which field you're looking at. Implemented
by pointing the Date "has a value" tokens at the same gray Text/Select
already use. The original blue values are kept commented out in
`tokens.css` in case this gets revisited, and the Focused state itself is
untouched — all three field types still turn blue when you focus them.

---

## Header icon cluster was pinned to the wrong side

**Where:** `AppHeader`, in `Components/AppHeader.razor.css`.

**What happened:** Found during a final side-by-side comparison against
the Figma export. The header's icon cluster (avatar, bell, gear,
dark-mode toggle, sync button) is supposed to sit at the edge away from
the sidebar, with the empty space next to the sidebar. Instead it was
pinned right next to the sidebar — the opposite of Figma. The row had no
`justify-content` set, so it fell back to a default that, combined with
our RTL layout, put the cluster on the wrong side.

**Fix:** One line — `justify-content: flex-end` on the header bar.

**Checked:** Confirmed the fix compiled into the built CSS, took a live
screenshot to confirm the cluster now sits at the correct edge, and re-ran
the Screen B keyboard/transfer checks afterward since this header is
shared across every page — no regressions.

---

## Question-bank transfer — "select multiple" is fully working, drag-and-drop stays a mockup

**Where:** `demo/ScreenB/QuestionDistribution.razor` and the related
classes in `wwwroot/styles/components.css`.

**What's real:** "اختيار متعدد" (select multiple) mode actually works —
check the boxes you want, click move, and the checked items really move
from the pool into "قسم الاول" (section one) and back. This isn't a static
mockup; it's real list logic.

**What's still a mockup:** "سحب وإملاء" (drag-and-drop) is still just a
toggle you can switch to — nothing happens when you try to drag anything.
There's no design spec anywhere for how drag-and-drop should behave here
(what's draggable, where it can drop, etc.), so building real behavior for
it would mean inventing an interaction the design never defined. Added a
small "(قريباً)" — "coming soon" — label next to that option so it's
obvious in the UI that it's intentionally unfinished, rather than looking
like a bug. It's a visible text label, not just a disabled/greyed-out
button, so it doesn't break the keyboard navigation of the toggle group.

**A couple of judgment calls, both because the reference design doesn't
show enough to confirm them directly:**
- There's no target-section picker anywhere in the design — both section
  columns show as empty, unlabeled placeholders. So "move selected" always
  moves into the first section, since nothing in the design shows how
  you'd pick a different one.
- The "remove from section" option (checkbox + a "select all" link in the
  column header) was only shown by the design for the pool column. We
  applied the same pattern to the section columns once they have items in
  them too, since that's the simplest, most consistent choice — the
  design's own layout already switches header controls between the empty
  and populated states elsewhere, we just used the same rule here.
- The move button was labeled "نقل الكل" ("move all") in the original
  design text, which is no longer accurate now that it only moves the
  checked items — renamed to "نقل المحدد" ("move selected").

**Checked:** Ran a full clean rebuild and tested the live app — moving
items updates both columns' counts correctly, moving back to the pool
works the same way, and a full keyboard-only pass (Tab/Shift+Tab/Space/
Enter, real key presses, not just `.focus()`) confirmed every checkbox and
button is reachable and usable, with a visible focus ring and a touch
target of at least 44px. No console errors.

---

## Toggle and chip text color — same blue-as-text contrast problem, twice more

**Where:** `AppToggleGroup` and `AppChip`, tokens in
`wwwroot/styles/tokens.css`, styling in `wwwroot/styles/components.css`.

**What happened:** The same blue (`#009DDC`) that Figma uses for the
unselected toggle button's border and text, and for the chip's text, fails
the 4.5:1 contrast minimum when used as text (it's fine as a border, which
only needs 3:1). On the toggle it measured 3.06:1 on white; on the chip's
tinted background it was even worse at 2.77:1.

**Fix:** Used the same substitute color already used elsewhere in the
project for this exact problem (`--color-blue-1f`) for the text/icon in
both places — 5.63:1 on white, 5.08:1 on the chip's tint, both comfortably
passing. The border color on the toggle stays the original blue, since as
a border it already passes.

**Also noticed while in this area:** the "بيانات النموذج" and "توزيع
الأسئلة" sections looked like two separate panels but are actually one
continuous panel with a divider line — confirmed by tracing the border
pixel-by-pixel in the Figma export. Fixed to match — one panel, one
divider, not two boxes.

---

## One field in "بيانات النموذج" has no label at all in Figma

**Where:** the disabled, greyed-out field showing "نموذج 1" in Screen B's
model-data panel.

**What happened:** Checked the Figma export carefully (including
re-cropping with extra margin, in case a label was just cut off in the
export) — this field genuinely has no label and no placeholder. Every
other field in that row has one. A field with no label at all fails the
project's accessibility rule that every field needs a persistent visible
label — placeholders don't count, and neither does nothing.

**What we considered:** shipping it exactly as Figma shows (rejected —
it's a real accessibility failure, not a style choice), or inventing new
label text that isn't anywhere in the design (rejected — guessing at
product copy is worse than reusing real text from elsewhere on the same
screen).

**Decision:** gave it the same label as the editable field right next to
it that shares the same value ("اسم النموذج" / "Model Name") — it's the
only text anywhere in the reference that's actually about what this field
represents. This is a guess about intent, flagged as such in the code, and
worth revisiting if there's a better answer.

**Also noticed, not changed:** the mobile export shows two of the fields
in a different order than desktop/tablet. Didn't reproduce that —
reordering just one breakpoint with CSS would make the visual order
disagree with the keyboard tab order, which is its own accessibility
problem. Treated it as a one-off mistake in that particular export
instead.

---

## Alert icon/text colors — both warning and success fail contrast against their own background

**Where:** `AppAlert`, tokens in `wwwroot/styles/tokens.css`, styling in
`wwwroot/styles/components.css`.

**What happened:** Figma's warning alert uses the same orange as
`--status-color-warning` for its icon and text, and the success alert uses
the same green as `--status-color-success` — both against their own light
tinted background. Measured contrast: the orange comes out at 1.91:1, the
green at 2.05:1. Both need to hit 4.5:1 since this is the actual
information text, not a decorative accent — nowhere else on the alert
repeats the same message in a safer color.

**Fix:** Darkened both colors while keeping the same hue (so they still
read as "the same orange" and "the same green," just legible) — tried a
few shades and picked ones with a bit of margin above the minimum rather
than sitting right on the line: `#995C00` for orange (4.79:1) and
`#00702F` for green (5.74:1). Added them as new tokens in the same color
family as the originals, rather than reusing the literal Figma value
anywhere in this component.

**Also noticed while here:** the design's alert pills turned out to have
no border at all (checked pixel-by-pixel) — the original build assumed a
border/tint combination that isn't actually there. Removed the border to
match.

---

## Stepper colors — swapped which state gets which color, per direct instruction

**Where:** `AppStepper` / `AppStepItem`, styling in
`wwwroot/styles/components.css`, tokens in `wwwroot/styles/tokens.css`.

**What changed:** at your direction, the Active step now uses the
dark-blue color family that used to belong to Selected, and Selected now
uses the brighter blue family that used to belong to Active. Nothing in
`tokens.css` itself changed — each token still points to the same color it
always did — this is only about which CSS class uses which token.

**Contrast, rechecked for the new pairing:** Active's dark blue
(`#152934`) is 15:1 against white — well clear of the minimum, no
adjustment needed. Selected's brighter blue (`#009DDC`) is only 3.06:1 —
fails as text — so the same accessible substitute already used for this
color elsewhere in the project now applies to Selected's label instead of
Active's (it moved with the color, since it's the color itself that has
the contrast problem, not either state name specifically).

**Side effect worth knowing about:** Selected went from a single flat
color to a color family with real hover/pressed states, since that's what
came with the token family it inherited — so Selected buttons now visibly
respond to hover and click, which they didn't before. Confirmed live.

---

## Stepper's active label text — darker blue than Figma's literal value, for contrast

**Where:** `AppStepper` / `AppStepItem`, styling in
`wwwroot/styles/components.css`.

**What happened:** Figma uses one blue (`#009DDC`) for both the Active
step's icon background and its label text. As a background behind a 40px
icon, that's fine (only needs 3:1 and clears it). As label text at normal
reading size, it only measures 3.06:1 — fails the 4.5:1 minimum for real
text.

**Fix:** Tried a couple of close alternatives first (a hover-blue at
4.10:1 still failed), landed on the same accessible substitute blue used
elsewhere in this project (`--color-blue-1f`, 5.63:1) for the label text
only. The icon circle's background keeps the original blue unchanged,
since as a non-text element it was never the problem.

**Why this stays intentional, not a bug to "fix" back to Figma:** both the
token comment and the CSS rule spell out that this is a deliberate
accessibility change, not an oversight — a future pass shouldn't revert it
to match Figma literally.
