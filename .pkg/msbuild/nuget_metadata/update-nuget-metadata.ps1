<#
.SYNOPSIS
    Idempotently apply NuGet package metadata to all SMRUCC.genomics *.vbproj files.

.DESCRIPTION
    Reads msbuild/nuget_metadata/nuget-metadata.json (project -> metadata mapping) and writes
    the unified/standard NuGet package properties into the first unconditional <PropertyGroup>
    of every target vbproj. The icon <None Include> is repointed to assets/logo.png using the
    per-project relative path, and any <PackageLicenseFile> element is removed.

    The script performs minimal text-level edits (NOT full XML re-serialization) so that the
    surrounding project structure, original line endings (CRLF/LF) and UTF-8 BOM are preserved.

.PARAMETER Root
    Repository root directory (defaults to G:\GCModeller).

.PARAMETER WhatIf
    When set, no files are written; a summary of changes is printed instead.

.PARAMETER Limit
    Process at most N projects (useful for dry-run sampling). Default: all.

.PARAMETER Only
    Comma-separated list of relative project paths to restrict processing (for targeted re-runs).
#>
[CmdletBinding()]
param(
    [string]$Root = 'G:\GCModeller',
    [switch]$WhatIf,
    [int]$Limit = 0,
    [string]$Only = ''
)

$ErrorActionPreference = 'Stop'

$root = Resolve-Path $Root
$metaPath = Join-Path $root 'msbuild\nuget_metadata\nuget-metadata.json'
if (-not (Test-Path $metaPath)) {
    throw "Metadata mapping not found: $metaPath"
}

$meta = Get-Content $metaPath -Raw | ConvertFrom-Json
$projects = $meta.projects

# Optional targeted restriction
if ($Only -ne '') {
    $onlySet = @{}
    foreach ($p in ($Only -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' })) {
        $onlySet[$p] = $true
    }
    $projects = @($projects | Where-Object { $onlySet.ContainsKey($_.path) })
}

# Optional sampling limit
if ($Limit -gt 0) {
    $projects = @($projects | Select-Object -First $Limit)
}

# XML-escape a string for safe inclusion as element/attribute text.
function XmlEscape([string]$s) {
    if ($null -eq $s) { return '' }
    return [System.Security.SecurityElement]::Escape($s)
}

# Standard (fully-determined) field values shared by every project.
$standard = [ordered]@{
    PackageProjectUrl       = 'https://gcmodeller.org/'
    RepositoryUrl           = 'https://github.com/SMRUCC/GCModeller.git'
    PackageLicenseExpression = 'GPL-3.0-or-later'
    Copyright               = 'Copyright © SMRUCC genomics, GuiLin China, 2026'
    Authors                 = 'xieguigang<xie.guigang@gcmodeller.org>'
    Company                 = 'SMRUCC genomics institute'
    Product                 = 'GCModeller'
    PackageIcon             = 'logo.png'
}

# Per-project descriptive fields: vbproj element name -> JSON property name.
$descriptiveMap = [ordered]@{
    Title              = 'title'
    Description        = 'description'
    PackageTags        = 'tags'
    PackageReleaseNotes = 'releaseNotes'
}

# Apply/insert a single property inside the inner text of a PropertyGroup.
function Set-Prop([string]$inner, [string]$name, [string]$value, [string]$nl) {
    $escValue = XmlEscape $value
    $tagPat = [regex]("<$name>.*?</$name>")
    if ($inner -match $tagPat) {
        # Replace existing element (use evaluator to avoid $ / backslash interpretation).
        $inner = [regex]::Replace($inner, "(?s)<$name>.*?</$name>", { param($m) "<$name>$escValue</$name>" }.GetNewClosure())
    } else {
        # Append before the closing </PropertyGroup> (inner ends just before it;
        # the trailing newline + close indentation is captured separately in $close).
        $inner = $inner + $nl + "    <$name>$escValue</$name>"
    }
    return $inner
}

# Remove any <PackageLicenseFile> element (with its trailing newline) from the inner text.
function Remove-LicenseFile([string]$inner) {
    return [regex]::Replace($inner, "(?s)\r?\n\s*<PackageLicenseFile>.*?</PackageLicenseFile>", '')
}

$processed = 0
$skipped = 0
$failed = 0

foreach ($proj in $projects) {
    $relPath = $proj.path
    $absPath = Join-Path $root ($relPath -replace '/', '\')
    if (-not (Test-Path $absPath)) {
        Write-Warning "MISSING: $relPath"
        $skipped++
        continue
    }

    # --- Read raw bytes to preserve BOM + line endings ---
    $bytes = [System.IO.File]::ReadAllBytes($absPath)
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    $enc = New-Object System.Text.UTF8Encoding($false)
    $text = $enc.GetString($bytes, $(if ($hasBom) { 3 } else { 0 }), $bytes.Length - $(if ($hasBom) { 3 } else { 0 }))
    $nl = if ($text.Contains("`r`n")) { "`r`n" } else { "`n" }

    $relLogo = $proj.logoRelative -replace '/', '\'

    # --- Locate the first UNCONDITIONAL PropertyGroup (capture its close tag incl. indentation) ---
    $pgPat = [regex]('(?s)(<PropertyGroup>)(.*?)(\r?\n\s*</PropertyGroup>)')
    $m = $pgPat.Match($text)
    if (-not $m.Success) {
        Write-Warning "NO unconditional PropertyGroup: $relPath"
        $skipped++
        continue
    }

    $open = $m.Groups[1].Value
    $inner = $m.Groups[2].Value
    $close = $m.Groups[3].Value

    # Build the ordered property set for this project.
    $props = [ordered]@{}
    foreach ($elem in $descriptiveMap.Keys) {
        $jsonKey = $descriptiveMap[$elem]
        $val = $proj.$jsonKey
        if ($null -ne $val -and $val -ne '') {
            $props[$elem] = $val
        }
    }
    foreach ($k in $standard.Keys) {
        $props[$k] = $standard[$k]
    }

    foreach ($k in $props.Keys) {
        $inner = Set-Prop $inner $k $props[$k] $nl
    }
    $inner = Remove-LicenseFile $inner

    $newBlock = $open + $inner + $close
    $text = $text.Remove($m.Index, $m.Length).Insert($m.Index, $newBlock)

    # --- Repoint logo <None Include> to assets/logo.png ---
    $logoPat = [regex]('(?s)(<None Include=")[^"]*logo\.png("[^>]*>)')
    if ($text -match $logoPat) {
        $text = [regex]::Replace($text, '(?s)(<None Include=")[^"]*logo\.png("[^>]*>)', {
            param($mm)
            $mm.Groups[1].Value + $relLogo + $mm.Groups[2].Value
        }.GetNewClosure())
    } else {
        # No logo <None Include> at all: add a fresh ItemGroup before </Project>.
        $addBlock = $nl +
            '  <ItemGroup>' + $nl +
            '    <None Include="' + $relLogo + '">' + $nl +
            '      <Pack>True</Pack>' + $nl +
            '      <PackagePath>\</PackagePath>' + $nl +
            '    </None>' + $nl +
            '  </ItemGroup>'
        $text = [regex]::Replace($text, '(?s)</Project>', {
            param($mm)
            $addBlock + $nl + '</Project>'
        }.GetNewClosure())
    }

    if ($WhatIf) {
        Write-Host "[WhatIf] would update: $relPath (logo=$relLogo)"
        $processed++
        continue
    }

    # --- Write back with original encoding / BOM / line endings ---
    $outEnc = New-Object System.Text.UTF8Encoding($hasBom)
    [System.IO.File]::WriteAllText($absPath, $text, $outEnc)
    $processed++
    Write-Host "updated: $relPath"
}

Write-Host "`nDone. processed=$processed skipped=$skipped failed=$failed"
