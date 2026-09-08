using MudBlazor;

namespace ProductUI;

/// <summary>
/// Central MudTheme instance consumed by MainLayout's MudThemeProvider.
///
/// SOURCE-OF-TRUTH SYNC (flagged per CLAUDE.md §4 "one source of truth per value"):
/// MudTheme's Palette/Typography/LayoutProperties are plain C# objects compiled
/// into the WASM binary — they cannot read a CSS custom property (--color-*,
/// --font-*, ...) at runtime, and CSS in wwwroot/styles/tokens.css cannot read
/// a C# value either. These are two genuinely separate representations of the
/// same design values, so there is no way to make one "directly reference" the
/// other at runtime.
///
/// Decision for this project: keep them in sync MANUALLY. tokens.css remains
/// the source of truth (it's what Figma maps to); every value set below must
/// have a matching token in wwwroot/styles/tokens.css and a comment naming it.
/// This file should never introduce a color/size that doesn't already exist
/// as a token. If duplication drifts in practice, the fallback is a small
/// build-time generator (parse tokens.css -> emit this file, or vice versa) —
/// not attempted yet since there's nothing but placeholders to generate from.
/// </summary>
public static class AppTheme
{
    public static readonly MudTheme Default = new()
    {
        // TODO: every property below must be replaced with a value that has
        // a corresponding token in wwwroot/styles/tokens.css, once the real
        // Figma palette/type-scale is supplied. Left at MudBlazor's built-in
        // defaults for now so the app renders something other than blank.
        PaletteLight = new PaletteLight
        {
            // Button (AppButton, Primary/Filled variant) — verified against
            // MudBlazor 8.7.0's shipped MudBlazor.min.css before wiring these:
            //   .mud-button-filled-primary{background-color:var(--mud-palette-primary)}
            //   .mud-button-filled-primary:hover{background-color:var(--mud-palette-primary-darken)}
            // i.e. Default bg + Hover bg are fully driven by Primary/PrimaryDarken
            // once set here — no CSS override needed for either state.
            Primary = "#009DDC",             // tokens.css --btn-pri-color-bg / --color-blue-00
            PrimaryContrastText = "#FFFFFF", // tokens.css --btn-pri-color-text / --color-white-ff
            PrimaryDarken = "#0386BA",       // tokens.css --btn-pri-color-bg-hover / --color-blue-03

            // FLAGGED: PrimaryLighten intentionally left unset (no lighter
            // brand shade given by Figma yet) — MudBlazor auto-calculates it
            // if omitted. Also flagged: there is NO separate "pressed"
            // Palette property — MudBlazor's own CSS (see above) reuses
            // PrimaryDarken for BOTH :hover AND :active/:focus-visible on
            // filled-primary buttons. Figma's "onClick" state
            // (--btn-pri-color-bg-active, --color-blue-75) is a different
            // color than hover, so it can't be expressed through Palette
            // alone — handled with a scoped :active CSS override in
            // components.css's .app-btn-primary instead (documented there).

            // Button (AppButton) disabled state. FLAGGED — these two Palette
            // properties are GLOBAL: they back --mud-palette-action-disabled
            // and --mud-palette-action-disabled-background, which MudButton's
            // own shipped CSS reads via `!important` rules
            // (.mud-button-root:disabled, .mud-button-filled:disabled).
            // Setting them here means EVERY disabled MudBlazor control that
            // uses those two variables gets this look, not just
            // .app-btn-primary — verified this does NOT include MudTextField/
            // AppField (its disabled state uses the separate --mud-palette-
            // text-disabled + generic .mud-disabled{opacity:.5} mechanism, a
            // different pair of variables), so no clash with the Field work
            // already done. But it does mean any *other* future disabled
            // MudButton/MudChip/MudFab elsewhere in the app will inherit this
            // same gray unless it defines its own override. Accepted as a
            // reasonable app-wide disabled default for now since nothing else
            // sets these yet — flag if per-component scoping is wanted later.
            ActionDisabled = "#9E9E9E",           // tokens.css --btn-pri-color-text-disabled / --color-gray-9e (PLACEHOLDER, not yet confirmed in Figma)
            ActionDisabledBackground = "#E7E7E7", // tokens.css --btn-pri-color-bg-disabled / --color-lightgray-e7

            // Secondary        -> TODO: map to --{component}-color-secondary
            // Background       -> TODO: map to --color-... background token
            // Surface          -> TODO: map to --color-... surface token
            // AppbarBackground -> TODO: map to --color-... appbar token
            // TextPrimary      -> TODO: map to --color-... text token
            // TextSecondary    -> TODO: map to --color-... text-secondary token
            // Divider          -> TODO: map to --color-... border/divider token
        },

        PaletteDark = new PaletteDark
        {
            // TODO: dark-mode equivalents, once/if dark mode is in scope.
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                // Cairo confirmed as product font in Step 2 (index.html link).
                // TODO: keep this literal string in sync with --font-family-cairo
                // in tokens.css — another manual-sync pair, same reason as above.
                FontFamily = new[] { "Cairo", "sans-serif" },
            },
            // H1..H6 / Body1 / Body2 / Button / Caption / Overline
            // TODO: each must mirror a --font-size-* / --font-weight-* token
            // once the Figma type scale is supplied.
            //
            // Button specifically: deliberately left untouched here rather
            // than set to --btn-pri-font-size/--btn-pri-font-weight, because
            // Typography.Button is GLOBAL (every MudButton in the app,
            // including the header's existing "المزامنة" sync button) —
            // MudBlazor's stock default is 14px/weight-500/uppercase
            // (Material spec), which would apply everywhere the moment this
            // is set. AppButton's font-size/weight/text-transform are scoped
            // instead to .app-btn-primary in components.css so only the new
            // Button component changes. Flag if a global Typography.Button
            // value is actually what's wanted once other buttons exist.
        },

        LayoutProperties = new LayoutProperties
        {
            // TODO: e.g. DrawerWidthLeft should reference the same value as
            // any --sidebar-width token, not a second hardcoded number
            // (CLAUDE.md §1 rule 5 — one source of truth per value).
        },
    };
}
