# build-styles.ps1 — concatenates wwwroot/styles/*.css (CLAUDE.md §4 load
# order) into wwwroot/dist/product-ui.min.css, minified. Regenerated on
# every `dotnet build`/`dotnet clean` + rebuild by ProductUI.csproj's
# BuildStyles target (CLAUDE.md §7's verification workflow is the single
# source of truth for this file) — never edit dist/product-ui.min.css by
# hand, edit the source layer files under wwwroot/styles instead.
#
# Minification is regex-based (strip comments, collapse whitespace), not a
# full CSS parser — safe for this file set because none of the 5 layers
# contain data: URIs or quoted content strings with embedded punctuation
# (confirmed by grep before writing this script); revisit if either is
# ever added to wwwroot/styles.

$ErrorActionPreference = 'Stop'

$root = $PSScriptRoot
$srcDir = Join-Path $root 'wwwroot/styles'
$distDir = Join-Path $root 'wwwroot/dist'
$outFile = Join-Path $distDir 'product-ui.min.css'

# CLAUDE.md §4 order: tokens -> mudblazor-overrides -> components -> utilities -> responsive-rtl
$files = @(
    'tokens.css',
    'mudblazor-overrides.css',
    'components.css',
    'utilities.css',
    'responsive-rtl.css'
)

New-Item -ItemType Directory -Force -Path $distDir | Out-Null

$combined = ($files | ForEach-Object {
    $path = Join-Path $srcDir $_
    if (-not (Test-Path $path)) {
        throw "build-styles.ps1: expected style layer not found: $path"
    }
    Get-Content -Raw -Encoding utf8 $path
}) -join "`n"

$minified = $combined `
    -replace '/\*[\s\S]*?\*/', '' `
    -replace '\r?\n', ' ' `
    -replace '\s+', ' ' `
    -replace '\s*([{}:;,])\s*', '$1' `
    -replace ';}', '}'

$minified = $minified.Trim()

$header = "/* GENERATED FILE - do not edit. Source: wwwroot/styles/*.css, built by build-styles.ps1. */`n"
Set-Content -Path $outFile -Value ($header + $minified) -NoNewline -Encoding utf8

$sizeKb = [math]::Round((Get-Item $outFile).Length / 1kb, 1)
Write-Host "build-styles.ps1: wrote $outFile ($sizeKb KB)"
