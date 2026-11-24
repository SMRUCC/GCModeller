<#
.SYNOPSIS
    Verify that all SMRUCC.genomics vbproj files carry the unified NuGet metadata.

.DESCRIPTION
    Parses every project listed in nuget-metadata.json and asserts:
      - XML is well-formed
      - The unified standard fields have the exact expected values
      - Title/Description/PackageTags/PackageReleaseNotes are present (coverage)
      - Exactly one logo <None Include> pointing at assets/logo.png
      - No residual <PackageLicenseFile>
    Prints a per-check summary and exits non-zero if any check fails.
#>
[CmdletBinding()]
param(
    [string]$Root = 'G:\GCModeller'
)

$ErrorActionPreference = 'Stop'
$root = Resolve-Path $Root
$metaPath = Join-Path $root 'msbuild\nuget_metadata\nuget-metadata.json'
$meta = Get-Content $metaPath -Raw | ConvertFrom-Json

$expected = [ordered]@{
    PackageProjectUrl       = 'https://gcmodeller.org/'
    RepositoryUrl           = 'https://github.com/SMRUCC/GCModeller.git'
    PackageLicenseExpression = 'GPL-3.0-or-later'
    Copyright               = 'Copyright © SMRUCC genomics, GuiLin China, 2026'
    Authors                 = 'xieguigang<xie.guigang@gcmodeller.org>'
    Company                 = 'SMRUCC genomics institute'
    Product                 = 'GCModeller'
    PackageIcon             = 'logo.png'
}
$descriptive = @('Title', 'Description', 'PackageTags', 'PackageReleaseNotes')

$total = @($meta.projects).Count
$fails = 0
$wellFormed = 0
$descriptiveCoverage = 0
$standardOk = 0
$logoOk = 0
$licenseClean = 0

foreach ($proj in $meta.projects) {
    $abs = Join-Path $root ($proj.path -replace '/', '\')
    $errs = @()

    # XML well-formed
    [xml]$xml = $null
    try {
        $xml = [xml]::new()
        $xml.Load($abs)
        $wellFormed++
    } catch {
        Write-Warning "MALFORMED XML: $($proj.path) - $_"
        $fails++
        continue
    }

    $pg = $xml.Project.PropertyGroup | Where-Object { -not $_.Condition } | Select-Object -First 1
    if (-not $pg) { $errs += 'no unconditional PropertyGroup' }

    # Standard fields
    $stdOk = $true
    foreach ($k in $expected.Keys) {
        $v = $pg.$k
        if ($null -eq $v -or $v.ToString() -ne $expected[$k]) {
            $stdOk = $false
            $errs += "field $k = '$v' (expected '$($expected[$k])')"
        }
    }
    if ($stdOk) { $standardOk++ }

    # Descriptive coverage
    $descOk = $true
    foreach ($k in $descriptive) {
        $v = $pg.$k
        if ($null -eq $v -or [string]::IsNullOrWhiteSpace($v.ToString())) {
            $descOk = $false
            $errs += "missing/empty $k"
        }
    }
    if ($descOk) { $descriptiveCoverage++ }

    # License file removed
    if ([string]::IsNullOrWhiteSpace($pg.PackageLicenseFile)) { $licenseClean++ }
    else { $errs += "PackageLicenseFile still present: $($pg.PackageLicenseFile)" }

    # Logo None include
    $logos = @($xml.Project.ItemGroup.None | Where-Object { $_.Include -like '*logo.png' })
    if ($logos.Count -eq 1 -and $logos[0].Include -like '*\assets\logo.png') { $logoOk++ }
    elseif ($logos.Count -ne 1) { $errs += "logo None count = $($logos.Count)" }
    else { $errs += "logo path = '$($logos[0].Include)' (expected *\assets\logo.png)" }

    if ($errs.Count -gt 0) {
        $fails++
        Write-Host "FAIL $($proj.path)"
        foreach ($e in $errs) { Write-Host "    - $e" }
    }
}

Write-Host ''
Write-Host "Total projects         : $total"
Write-Host "XML well-formed         : $wellFormed / $total"
Write-Host "Standard fields correct : $standardOk / $total"
Write-Host "Descriptive coverage    : $descriptiveCoverage / $total"
Write-Host "Logo include correct    : $logoOk / $total"
Write-Host "LicenseFile removed     : $licenseClean / $total"
Write-Host "Failed projects         : $fails"

if ($fails -gt 0) { exit 1 }
Write-Host 'ALL CHECKS PASSED'
