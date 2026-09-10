<#
.SYNOPSIS
  Keep every "SMRUCC.genomics" SDK-style class library vbproj registered in
  GCModeller.slnx and wired up for the "nuget_release|x64" packaging config.

.DESCRIPTION
  Scans the repository for *.vbproj files and processes the ones that are

    * SDK style       -- <Project Sdk="Microsoft.NET.Sdk">
    * package worthy  -- <RootNamespace> starts with "SMRUCC.genomics"
    * class library   -- <OutputType> is absent or "Library"
                         (Exe / WinExe projects are ignored)

  For every matching project the script performs these idempotent checks:

    1. Solution membership
       If the project is not referenced by GCModeller.slnx it is appended to the
       solution folder given by -SolutionFolder (created on demand), together
       with the "*|x64 -> x64" platform mapping used by the existing entries.
       Membership is decided on *resolved absolute paths*, so the different
       spellings already present in the file never cause duplicates, and the
       literal '#' / '%' characters in directory names (for example
       "runtime/sciBASIC#/mime/application%json") are handled verbatim.
       Immediately before a project is registered its <RootNamespace> and
       <OutputType> are read again from disk: the namespace must still start
       with "SMRUCC.genomics" and the project must still be a class library,
       otherwise it is reported as skipped and never added.

    2. Configuration declaration
       <Configurations> gains "nuget_release" and <Platforms> gains "x64" when
       missing, otherwise MSBuild/Visual Studio would not offer the config.

    3. Packaging properties
       The four properties that make MSBuild actually emit a .nupkg on build are
       upserted into the first unconditional PropertyGroup of every matching
       project (a project missing them silently produces no package):

           GeneratePackageOnBuild          = True
           PackageRequireLicenseAcceptance = True
           IncludeSymbols                  = True
           SymbolPackageFormat             = snupkg

       Properties that are already present with the target value are left alone;
       a present but different value is rewritten. Use -SkipPackagingProps to
       turn this step off.

    4. Conditional property group
       <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='nuget_release|x64'">
       is created when absent. The condition is compared with all whitespace
       removed, and the three part form that also carries $(TargetFramework) is
       never treated as a match.

    5. Output path
       <OutputPath> inside that group is forced to the relative path pointing at
       the repository level ".nuget" directory, e.g. "../../../../.nuget" for
       GCModeller/core/Bio.Assembly/biocore-netcore5.vbproj. Missing values are
       inserted, differing ones rewritten. NuGet defaults PackageOutputPath to
       OutputPath, so the .nupkg / .snupkg land in that same directory.

  The script is safe to run repeatedly: a file is only rewritten when at least
  one real change was produced, and encoding (UTF-8 BOM or not), line endings
  (CRLF/LF) and the surrounding indentation are preserved. No backup copies are
  created -- use git to review or revert the result.

  Requires Windows PowerShell 5.1 or newer; no external modules.

.PARAMETER Root
  Repository root to scan. Defaults to the parent of the folder holding this
  script, i.e. the GCModeller repository root.

.PARAMETER Solution
  The .slnx solution file to synchronise. Defaults to "<Root>\src\GCModeller.slnx".

.PARAMETER SolutionFolder
  Solution folder that receives newly registered projects. Must start and end
  with '/'. Defaults to "/nuget_packages/".

.PARAMETER NugetDir
  Package output directory that <OutputPath> should point at.
  Defaults to "<Root>\.nuget".

.PARAMETER ProjectFilter
  Wildcard applied to the repository relative project path (forward slashes),
  e.g. '*Bio.Assembly*'. Defaults to '*' (everything).

.PARAMETER ExcludePattern
  Regular expression matched against the relative project path. Any project
  whose path contains one of these directory segments is ignored.

.PARAMETER ReportFile
  Optional CSV path receiving one row per matched project with the individual
  operations that were applied.

.PARAMETER SkipPackagingProps
  Do not touch GeneratePackageOnBuild / PackageRequireLicenseAcceptance /
  IncludeSymbols / SymbolPackageFormat.

.PARAMETER DryRun
  Run every check and print the resulting operations without writing anything.

.EXAMPLE
  .\sync_nuget_projects.ps1 -DryRun

  Preview mode: reports what would be changed, touches no file.

.EXAMPLE
  .\sync_nuget_projects.ps1

  Apply the changes to GCModeller.slnx and to the project files.

.EXAMPLE
  .\sync_nuget_projects.ps1 -ProjectFilter '*GCModeller/data/*' -ReportFile .\logs\nuget_sync.csv -Verbose

  Restrict the run to the database projects, dump a CSV report and trace every
  decision.
#>
[CmdletBinding()]
param(
    [string]$Root           = '',
    [string]$Solution       = '',
    [string]$SolutionFolder = '/nuget_packages/',
    [string]$NugetDir       = '',
    [string]$ProjectFilter  = '*',
    [string]$ExcludePattern = '(^|[\\/])(obj|bin|\.git|\.vs|packages|package-install-cache|package-install-SetupFiles)([\\/]|$)',
    [string]$ReportFile     = '',
    [switch]$SkipPackagingProps,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

# Windows PowerShell does not populate $PSScriptRoot while binding the default
# values of an advanced (CmdletBinding) script, so the location aware defaults
# are resolved here instead of in the param block.
$scriptDir = $PSScriptRoot
if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
if (-not $Root)      { $Root      = Split-Path -Parent $scriptDir }

# The MSBuild condition we are looking for / creating. Built by concatenation so
# that PowerShell never tries to expand the $(...) MSBuild placeholders.
$TargetConfiguration = 'nuget_release'
$TargetPlatform      = 'x64'
$TargetCondition     = "'" + '$(Configuration)|$(Platform)' + "'=='" + `
                       $TargetConfiguration + '|' + $TargetPlatform + "'"

# Defaults used when a project declares no <Configurations> / <Platforms> at all.
$DefaultConfigurations = 'Debug;Release'
$DefaultPlatforms      = 'AnyCPU'

# Only projects whose <RootNamespace> starts with this prefix are picked up, and
# only such projects are ever registered in the solution.
$NamespacePrefix = 'SMRUCC.genomics'

# Executable projects are out of scope: only class libraries are packed.
$ExecutableOutputTypes = @('Exe', 'WinExe')

# Without these four properties MSBuild never emits a .nupkg for a project, so
# they are upserted into the first unconditional PropertyGroup of every project
# that is picked up. Values match the convention already used by the packed
# projects in this repository (e.g. GCModeller/core/Bio.Assembly).
$PackagingProps = [ordered]@{
    GeneratePackageOnBuild          = 'True'
    PackageRequireLicenseAcceptance = 'True'
    IncludeSymbols                  = 'True'
    SymbolPackageFormat             = 'snupkg'
}

# ---------------------------------------------------------------------------
# Generic XML helpers
# ---------------------------------------------------------------------------

function Test-WhitespaceNode($node) {
    return ($null -ne $node) -and `
           ($node.NodeType -eq [System.Xml.XmlNodeType]::Whitespace -or `
            $node.NodeType -eq [System.Xml.XmlNodeType]::SignificantWhitespace)
}

function Get-Indents($container) {
    $groupIndent = ''
    if (Test-WhitespaceNode $container.PreviousSibling) {
        $groupIndent = ($container.PreviousSibling.Value -split "`n")[-1]
    }
    $elemIndent = ''
    $first = $container.FirstChild
    if (Test-WhitespaceNode $first) {
        $elemIndent = ($first.Value -split "`n")[-1]
    }
    if (-not $elemIndent) { $elemIndent = $groupIndent + '  ' }
    return @{ Group = $groupIndent; Elem = $elemIndent }
}

function Append-Element($doc, $container, $elem, $indents) {
    # Strip trailing whitespace so the closing tag can be re-indented cleanly.
    $last = $container.LastChild
    while (Test-WhitespaceNode $last) {
        $prev = $last.PreviousSibling
        [void]$container.RemoveChild($last)
        $last = $prev
    }
    [void]$container.AppendChild($doc.CreateWhitespace("`n" + $indents.Elem))
    [void]$container.AppendChild($elem)
    [void]$container.AppendChild($doc.CreateWhitespace("`n" + $indents.Group))
}

function Find-ChildElement($parent, [string]$name) {
    foreach ($c in $parent.ChildNodes) {
        if ($c.NodeType -eq 'Element' -and $c.LocalName -eq $name) { return $c }
    }
    return $null
}

function Get-NodeText($parent, [string]$name) {
    $elem = Find-ChildElement $parent $name
    if ($null -eq $elem) { return $null }
    return $elem.InnerText.Trim()
}

function Set-Property($doc, $group, [string]$name, [string]$value, $indents) {
    $existing = Find-ChildElement $group $name
    if ($existing) {
        if ($existing.InnerText -ne $value) {
            $existing.InnerText = $value
            return 'updated'
        }
        return 'unchanged'
    }
    $elem = $doc.CreateElement($name, $doc.DocumentElement.NamespaceURI)
    $elem.InnerText = $value
    Append-Element $doc $group $elem $indents
    return 'added'
}

function Save-XmlPreserving($doc, [string]$path) {
    # Explicit byte level handling: the original BOM, XML declaration and line
    # endings are all restored, so a rewrite never churns the whole file.
    $origText = [System.IO.File]::ReadAllText($path)
    $bytes    = [System.IO.File]::ReadAllBytes($path)
    $hasBom   = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    $nl       = if ($origText.Contains("`r`n")) { "`r`n" } else { "`n" }

    # Preserve the *original* XML declaration verbatim. XmlWriter would emit
    # encoding="utf-16" (the backing StringWriter's encoding), so we suppress
    # the declaration entirely and re-prepend the original one.
    $scan = $origText
    if ($scan.Length -gt 0 -and $scan[0] -eq [char]0xFEFF) { $scan = $scan.Substring(1) }
    $decl = ''
    if ($scan -match '^<\?xml[^>]*\?>') { $decl = $Matches[0] }

    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent             = $false
    $settings.OmitXmlDeclaration = $true
    $settings.NewLineHandling    = [System.Xml.NewLineHandling]::None
    $settings.Encoding           = New-Object System.Text.UTF8Encoding($false)

    $sw = New-Object System.IO.StringWriter
    $xw = [System.Xml.XmlWriter]::Create($sw, $settings)
    try { $doc.Save($xw) } finally { $xw.Close() }

    $text = $sw.ToString()
    $text = $text -replace "`r`n", "`n"
    $text = $text -replace "`n", $nl
    if ($text.Length -gt 0 -and $text[0] -eq [char]0xFEFF) { $text = $text.Substring(1) }

    $final = $decl + $text
    if ($final -eq $origText) { return $false }

    $enc = New-Object System.Text.UTF8Encoding($hasBom)
    [System.IO.File]::WriteAllText($path, $final, $enc)
    return $true
}

# ---------------------------------------------------------------------------
# Path helpers
# ---------------------------------------------------------------------------

function Get-RelativePathUnix([string]$baseDir, [string]$targetPath) {
    # [System.IO.Path]::GetRelativePath is .NET Core only, and Uri.MakeRelativeUri
    # mangles the '%' characters present in directory names such as
    # "mime/application%json". Plain string arithmetic on the path segments is
    # both portable to PowerShell 5.1 and '%' safe.
    $base   = [System.IO.Path]::GetFullPath($baseDir.TrimEnd('\', '/') + '\').TrimEnd('\')
    $target = [System.IO.Path]::GetFullPath($targetPath).TrimEnd('\')

    $baseParts   = @($base   -split '\\')
    $targetParts = @($target -split '\\')

    $common = 0
    while ($common -lt $baseParts.Count -and $common -lt $targetParts.Count -and
           [string]::Equals($baseParts[$common], $targetParts[$common], [System.StringComparison]::OrdinalIgnoreCase)) {
        $common++
    }
    if ($common -eq 0) {
        # Different volume: a relative path does not exist, keep it absolute.
        Write-Warning "No relative path from '$baseDir' to '$targetPath' (different volume) -- using the absolute path"
        return ($target -replace '\\', '/')
    }

    $segments = @()
    for ($i = $common; $i -lt $baseParts.Count;   $i++) { $segments += '..' }
    for ($i = $common; $i -lt $targetParts.Count; $i++) { $segments += $targetParts[$i] }
    if ($segments.Count -eq 0) { return '.' }
    return ($segments -join '/')
}

function Get-AbsolutePathKey([string]$baseDir, [string]$relativePath) {
    $combined = [System.IO.Path]::Combine($baseDir, ($relativePath -replace '/', '\'))
    return ([System.IO.Path]::GetFullPath($combined)).TrimEnd('\').ToLowerInvariant()
}

# ---------------------------------------------------------------------------
# vbproj helpers
# ---------------------------------------------------------------------------

function Find-RootNsPropertyGroup($doc) {
    foreach ($pg in $doc.DocumentElement.ChildNodes) {
        if ($pg.NodeType -ne 'Element' -or $pg.LocalName -ne 'PropertyGroup') { continue }
        if ($pg.GetAttribute('Condition')) { continue }
        if (Find-ChildElement $pg 'RootNamespace') { return $pg }
    }
    foreach ($pg in $doc.DocumentElement.ChildNodes) {
        if ($pg.NodeType -eq 'Element' -and $pg.LocalName -eq 'PropertyGroup' -and -not $pg.GetAttribute('Condition')) {
            return $pg
        }
    }
    return $null
}

function Get-RootNamespace($doc) {
    foreach ($pg in $doc.DocumentElement.ChildNodes) {
        if ($pg.NodeType -ne 'Element' -or $pg.LocalName -ne 'PropertyGroup') { continue }
        $v = Get-NodeText $pg 'RootNamespace'
        if ($v) { return $v }
    }
    return $null
}

function Get-OutputType($doc) {
    # <OutputType> may sit in a conditional group as well; any declaration counts
    # because a project that produces an executable anywhere is not a library.
    foreach ($pg in $doc.DocumentElement.ChildNodes) {
        if ($pg.NodeType -ne 'Element' -or $pg.LocalName -ne 'PropertyGroup') { continue }
        $v = Get-NodeText $pg 'OutputType'
        if ($v) { return $v }
    }
    return $null
}

function Test-ClassLibrary($doc) {
    # SDK style projects default to Library when <OutputType> is absent.
    $outputType = Get-OutputType $doc
    if (-not $outputType) { return $true }
    foreach ($exe in $ExecutableOutputTypes) {
        if ([string]::Equals($outputType, $exe, [System.StringComparison]::OrdinalIgnoreCase)) { return $false }
    }
    return $true
}

function Test-RootNamespaceAllowed([string]$rootNs) {
    if (-not $rootNs) { return $false }
    return $rootNs.StartsWith($NamespacePrefix, [System.StringComparison]::OrdinalIgnoreCase)
}

function Test-CandidateFromDisk([string]$path) {
    # Independent re-read straight from disk. Used as a second line of defence
    # right before a project is registered in the solution, so a future change to
    # the filtering logic can never leak a foreign or executable project into the
    # solution. $null is returned for anything unreadable -- the caller declines.
    $probe = New-Object System.Xml.XmlDocument
    $probe.PreserveWhitespace = $true
    try { $probe.Load($path) }
    catch { return $null }
    if ($probe.DocumentElement.GetAttribute('Sdk') -ne 'Microsoft.NET.Sdk') { return $null }
    return @{
        RootNamespace = (Get-RootNamespace $probe)
        IsLibrary     = (Test-ClassLibrary $probe)
    }
}

function Find-PropertyOwnerGroup($doc, [string]$name) {
    # The property may live in any unconditional PropertyGroup, not necessarily
    # the one carrying RootNamespace. Patch it where the author put it.
    foreach ($pg in $doc.DocumentElement.ChildNodes) {
        if ($pg.NodeType -ne 'Element' -or $pg.LocalName -ne 'PropertyGroup') { continue }
        if ($pg.GetAttribute('Condition')) { continue }
        if (Find-ChildElement $pg $name) { return $pg }
    }
    return $null
}

function Remove-DuplicateProperty($group, [string]$name) {
    # MSBuild honours the *last* declaration of a property, so a file carrying
    # two copies of e.g. PackageRequireLicenseAcceptance keeps the trailing one.
    # Drop every earlier copy so the value that is actually in effect is also the
    # only one a reader sees.
    $copies = @()
    foreach ($c in $group.ChildNodes) {
        if ($c.NodeType -eq 'Element' -and $c.LocalName -eq $name) { $copies += $c }
    }
    if ($copies.Count -le 1) { return 0 }

    for ($i = 0; $i -lt $copies.Count - 1; $i++) {
        $node = $copies[$i]
        $prev = $node.PreviousSibling
        [void]$group.RemoveChild($node)
        if (Test-WhitespaceNode $prev) { [void]$group.RemoveChild($prev) }
    }
    return ($copies.Count - 1)
}

function Test-ShouldPatchPackagingProp($doc, [string]$name, [string]$value) {
    # Only an *unconditional* definition counts as "already declared": a value
    # that merely exists inside a conditional PropertyGroup (for instance the
    # nuget_release|x64 group) is not a project wide setting and must still be
    # added, otherwise every other configuration builds without a package.
    $owner = Find-PropertyOwnerGroup $doc $name
    if ($null -eq $owner) { return $true }
    $existing = Find-ChildElement $owner $name
    if ($null -eq $existing) { return $true }
    if ($existing.InnerText.Trim() -eq $value) { return $false }
    # Declared unconditionally, but with another value -- rewrite it in place.
    $mainGroup = Find-RootNsPropertyGroup $doc
    return ($null -ne $mainGroup) -and ($owner -eq $mainGroup)
}

function Test-TargetCondition([string]$condition) {
    # Whitespace insensitive comparison against the two part condition. The three
    # part form carrying $(TargetFramework) differs textually and therefore never
    # matches, which is exactly what we want.
    if (-not $condition) { return $false }
    $a = $condition       -replace '\s', ''
    $b = $TargetCondition -replace '\s', ''
    return [string]::Equals($a, $b, [System.StringComparison]::OrdinalIgnoreCase)
}

function Find-TargetGroups($doc) {
    $found = @()
    foreach ($pg in $doc.DocumentElement.ChildNodes) {
        if ($pg.NodeType -ne 'Element' -or $pg.LocalName -ne 'PropertyGroup') { continue }
        if (Test-TargetCondition $pg.GetAttribute('Condition')) { $found += $pg }
    }
    return $found
}

function New-ConditionalPropertyGroup($doc, [string]$condition) {
    $root = $doc.DocumentElement

    # Anchor after the last top level PropertyGroup so the new group stays with
    # its peers and above the ItemGroup section.
    $anchor = $null
    foreach ($c in $root.ChildNodes) {
        if ($c.NodeType -eq 'Element' -and $c.LocalName -eq 'PropertyGroup') { $anchor = $c }
    }

    $groupIndent = '  '
    if ($null -ne $anchor -and (Test-WhitespaceNode $anchor.PreviousSibling)) {
        $probe = ($anchor.PreviousSibling.Value -split "`n")[-1]
        if ($probe) { $groupIndent = $probe }
    }
    $elemIndent = $groupIndent + '  '

    $group = $doc.CreateElement('PropertyGroup', $root.NamespaceURI)
    $group.SetAttribute('Condition', $condition)
    # Seed the whitespace so Get-Indents can derive the child indentation.
    [void]$group.AppendChild($doc.CreateWhitespace("`n" + $elemIndent))

    if ($null -ne $anchor) {
        [void]$root.InsertAfter($group, $anchor)
        [void]$root.InsertAfter($doc.CreateWhitespace("`n`n" + $groupIndent), $anchor)
    }
    else {
        $last = $root.LastChild
        while (Test-WhitespaceNode $last) {
            $prev = $last.PreviousSibling
            [void]$root.RemoveChild($last)
            $last = $prev
        }
        [void]$root.AppendChild($doc.CreateWhitespace("`n`n" + $groupIndent))
        [void]$root.AppendChild($group)
        [void]$root.AppendChild($doc.CreateWhitespace("`n"))
    }
    return $group
}

function Set-SemicolonToken($doc, $group, [string]$name, [string]$token, [string]$seed, $indents) {
    $existing = Find-ChildElement $group $name
    if ($null -eq $existing) {
        $value = @($seed -split ';' | Where-Object { $_ }) + $token
        $elem  = $doc.CreateElement($name, $doc.DocumentElement.NamespaceURI)
        $elem.InnerText = ($value -join ';')
        Append-Element $doc $group $elem $indents
        return 'added'
    }
    $tokens = @($existing.InnerText -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ })
    foreach ($t in $tokens) {
        if ([string]::Equals($t, $token, [System.StringComparison]::OrdinalIgnoreCase)) { return 'unchanged' }
    }
    $existing.InnerText = (($tokens + $token) -join ';')
    return 'updated'
}

# ---------------------------------------------------------------------------
# slnx helpers
# ---------------------------------------------------------------------------

function Find-SolutionFolder($doc, [string]$name) {
    foreach ($c in $doc.DocumentElement.ChildNodes) {
        if ($c.NodeType -eq 'Element' -and $c.LocalName -eq 'Folder' -and $c.GetAttribute('Name') -eq $name) {
            return $c
        }
    }
    return $null
}

function New-SolutionFolder($doc, [string]$name) {
    $root = $doc.DocumentElement

    # Keep the file's layout: <Configurations>, then every <Folder> in alphabetical
    # order, then the root level <Project> entries. The anchor is the last folder
    # that still sorts before the new name (falling back to <Configurations>).
    $anchor = $null
    foreach ($c in $root.ChildNodes) {
        if ($c.NodeType -ne 'Element' -or $c.LocalName -ne 'Folder') { continue }
        if ([string]::Compare($c.GetAttribute('Name'), $name, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
            $anchor = $c
        }
    }
    if ($null -eq $anchor) {
        foreach ($c in $root.ChildNodes) {
            if ($c.NodeType -eq 'Element' -and $c.LocalName -eq 'Configurations') { $anchor = $c }
        }
    }

    $folderIndent = '  '
    if ($null -ne $anchor -and (Test-WhitespaceNode $anchor.PreviousSibling)) {
        $probe = ($anchor.PreviousSibling.Value -split "`n")[-1]
        if ($probe) { $folderIndent = $probe }
    }

    $folder = $doc.CreateElement('Folder', $root.NamespaceURI)
    $folder.SetAttribute('Name', $name)
    [void]$folder.AppendChild($doc.CreateWhitespace("`n" + $folderIndent + '  '))

    if ($null -ne $anchor) {
        [void]$root.InsertAfter($folder, $anchor)
        [void]$root.InsertAfter($doc.CreateWhitespace("`n" + $folderIndent), $anchor)
    }
    else {
        $last = $root.LastChild
        while (Test-WhitespaceNode $last) {
            $prev = $last.PreviousSibling
            [void]$root.RemoveChild($last)
            $last = $prev
        }
        [void]$root.AppendChild($doc.CreateWhitespace("`n" + $folderIndent))
        [void]$root.AppendChild($folder)
        [void]$root.AppendChild($doc.CreateWhitespace("`n"))
    }
    return $folder
}

function Add-SolutionProject($doc, $folder, [string]$relativePath) {
    $indents = Get-Indents $folder
    $ns      = $doc.DocumentElement.NamespaceURI

    $project = $doc.CreateElement('Project', $ns)
    $project.SetAttribute('Path', $relativePath)

    # Every existing entry maps the solution x64 platform onto the project x64
    # platform; build type mappings are left to Visual Studio, which resolves
    # same named configurations automatically.
    $platform = $doc.CreateElement('Platform', $ns)
    $platform.SetAttribute('Solution', '*|' + $TargetPlatform)
    $platform.SetAttribute('Project', $TargetPlatform)

    [void]$project.AppendChild($doc.CreateWhitespace("`n" + $indents.Elem + '  '))
    [void]$project.AppendChild($platform)
    [void]$project.AppendChild($doc.CreateWhitespace("`n" + $indents.Elem))

    Append-Element $doc $folder $project $indents
}

# ---------------------------------------------------------------------------
# Resolve inputs
# ---------------------------------------------------------------------------

if (-not (Test-Path -LiteralPath $Root)) { throw "Repository root not found: $Root" }
$Root = (Resolve-Path -LiteralPath $Root).Path.TrimEnd('\')

if (-not $Solution) { $Solution = Join-Path $Root 'src\GCModeller.slnx' }
if (-not (Test-Path -LiteralPath $Solution)) { throw "Solution file not found: $Solution" }
$Solution    = (Resolve-Path -LiteralPath $Solution).Path
$solutionDir = Split-Path -Parent $Solution

if ($SolutionFolder -notmatch '^/.*/$') {
    throw "SolutionFolder must start and end with '/', got: $SolutionFolder"
}

if (-not $NugetDir) { $NugetDir = Join-Path $Root '.nuget' }
$NugetDir = [System.IO.Path]::GetFullPath($NugetDir).TrimEnd('\')

Write-Host "Repository root : $Root"           -ForegroundColor Cyan
Write-Host "Solution        : $Solution"       -ForegroundColor Cyan
Write-Host "Target folder   : $SolutionFolder" -ForegroundColor Cyan
Write-Host "Package output  : $NugetDir"       -ForegroundColor Cyan
Write-Host "Namespace       : $NamespacePrefix*" -ForegroundColor Cyan
Write-Host "Configuration   : $TargetConfiguration|$TargetPlatform" -ForegroundColor Cyan
Write-Host "Mode            : $(if ($DryRun) { 'DryRun (no write)' } else { 'APPLY' })" -ForegroundColor Yellow
Write-Host ""

if (-not (Test-Path -LiteralPath $NugetDir)) {
    Write-Warning "Package output directory does not exist yet: $NugetDir (it is created by the build)"
}

# ---------------------------------------------------------------------------
# Load the solution and index its projects by absolute path
# ---------------------------------------------------------------------------

$slnx = New-Object System.Xml.XmlDocument
$slnx.PreserveWhitespace = $true
$slnx.Load($Solution)

$registered = @{}
foreach ($node in $slnx.SelectNodes('//Project[@Path]')) {
    $key = Get-AbsolutePathKey $solutionDir $node.GetAttribute('Path')
    $registered[$key] = $node.GetAttribute('Path')
}
Write-Host ("Solution entries: {0} project reference(s)" -f $registered.Count) -ForegroundColor Cyan

# ---------------------------------------------------------------------------
# Discover candidate project files
# ---------------------------------------------------------------------------

$allProjects = @(Get-ChildItem -LiteralPath $Root -Filter '*.vbproj' -Recurse -File | Where-Object {
        $rel = $_.FullName.Substring($Root.Length + 1)
        ($rel -notmatch $ExcludePattern) -and (($rel -replace '\\', '/') -like $ProjectFilter)
    } | Sort-Object FullName)

Write-Host ("Discovered      : {0} *.vbproj after filtering" -f $allProjects.Count) -ForegroundColor Cyan
Write-Host ""

# ---------------------------------------------------------------------------
# Process
# ---------------------------------------------------------------------------

$stats = @{
    scanned               = 0
    matched               = 0
    skippedLegacy         = 0
    skippedRootNamespace  = 0
    skippedExecutable     = 0
    skippedByGuard        = 0
    failed                = 0
    slnxAdded             = 0
    projectsChanged       = 0
    configurationsPatched = 0
    platformsPatched      = 0
    groupsAdded           = 0
    outputPathAdded       = 0
    outputPathUpdated     = 0
    packagingAdded        = 0
    packagingUpdated      = 0
    packagingDupesRemoved = 0
}
$report        = New-Object System.Collections.Generic.List[object]
$slnxDirty     = $false
$newFolderMade = $false
$targetFolder  = $null

foreach ($file in $allProjects) {
    $stats.scanned++
    $relUnix = ($file.FullName.Substring($Root.Length + 1)) -replace '\\', '/'

    $doc = New-Object System.Xml.XmlDocument
    $doc.PreserveWhitespace = $true
    try { $doc.Load($file.FullName) }
    catch {
        Write-Warning "[WARN] unparsable: $relUnix -> $($_.Exception.Message)"
        $stats.failed++
        continue
    }

    if ($doc.DocumentElement.GetAttribute('Sdk') -ne 'Microsoft.NET.Sdk') {
        Write-Verbose "[SKIP] not SDK style (Sdk='$($doc.DocumentElement.GetAttribute('Sdk'))'): $relUnix"
        $stats.skippedLegacy++
        continue
    }

    $rootNs = Get-RootNamespace $doc
    if (-not (Test-RootNamespaceAllowed $rootNs)) {
        Write-Verbose "[SKIP] RootNamespace='$rootNs': $relUnix"
        $stats.skippedRootNamespace++
        continue
    }

    if (-not (Test-ClassLibrary $doc)) {
        Write-Verbose "[SKIP] OutputType='$(Get-OutputType $doc)' (not a class library): $relUnix"
        $stats.skippedExecutable++
        continue
    }

    $stats.matched++
    $ops = @()

    # ---- 1. solution membership -------------------------------------------
    $slnxOp  = 'present'
    $absKey  = $file.FullName.TrimEnd('\').ToLowerInvariant()
    if (-not $registered.ContainsKey($absKey)) {
        # Second line of defence: re-read the project straight from disk and
        # refuse to register anything that is not a SMRUCC.genomics class
        # library. Anything unreadable is declined as well.
        $guard = Test-CandidateFromDisk $file.FullName
        if (($null -eq $guard) -or (-not (Test-RootNamespaceAllowed $guard.RootNamespace)) -or (-not $guard.IsLibrary)) {
            Write-Host ("  {0}" -f $relUnix) -ForegroundColor Yellow
            Write-Host ("      [SKIP] slnx:NOT added -- on disk re-check failed " +
                        "(RootNamespace='$(if ($guard) { $guard.RootNamespace })', " +
                        "library=$(if ($guard) { $guard.IsLibrary } else { 'unreadable' }))") -ForegroundColor DarkYellow
            $stats.skippedByGuard++
            $slnxOp = 'skipped(guard)'
        }
        else {
            $slnxRel = Get-RelativePathUnix $solutionDir $file.FullName
            if ($null -eq $targetFolder) {
                $targetFolder = Find-SolutionFolder $slnx $SolutionFolder
                if ($null -eq $targetFolder) {
                    $targetFolder  = New-SolutionFolder $slnx $SolutionFolder
                    $newFolderMade = $true
                }
            }
            Add-SolutionProject $slnx $targetFolder $slnxRel
            $registered[$absKey] = $slnxRel
            $slnxDirty = $true
            $stats.slnxAdded++
            $slnxOp = 'added'
            $ops += "[ADD] slnx($slnxRel)"
        }
    }

    # ---- 2. configuration declarations ------------------------------------
    $mainGroup = Find-RootNsPropertyGroup $doc
    if ($null -eq $mainGroup) {
        Write-Warning "[WARN] no unconditional PropertyGroup in $relUnix -- project settings skipped"
        $stats.failed++
        continue
    }

    $cfgGroup = Find-PropertyOwnerGroup $doc 'Configurations'
    if ($null -eq $cfgGroup) { $cfgGroup = $mainGroup }
    $cfgOp = Set-SemicolonToken $doc $cfgGroup 'Configurations' $TargetConfiguration `
                                $DefaultConfigurations (Get-Indents $cfgGroup)
    if ($cfgOp -ne 'unchanged') { $stats.configurationsPatched++; $ops += "[FIX] Configurations:$cfgOp" }

    $platGroup = Find-PropertyOwnerGroup $doc 'Platforms'
    if ($null -eq $platGroup) { $platGroup = $mainGroup }
    $platOp = Set-SemicolonToken $doc $platGroup 'Platforms' $TargetPlatform `
                                 $DefaultPlatforms (Get-Indents $platGroup)
    if ($platOp -ne 'unchanged') { $stats.platformsPatched++; $ops += "[FIX] Platforms:$platOp" }

    # ---- 3. packaging properties ------------------------------------------
    # Without GeneratePackageOnBuild the build silently produces no .nupkg, so
    # these belong to the same "make this project packable" check as step 5.
    $packIndents = Get-Indents $mainGroup
    if (-not $SkipPackagingProps) {
        foreach ($name in $PackagingProps.Keys) {
            $value = $PackagingProps[$name]
            $op    = 'present'
            if (Test-ShouldPatchPackagingProp $doc $name $value) {
                # Collapse repeated declarations first: the surviving (last) copy
                # is then compared and patched, leaving exactly one entry.
                if ((Remove-DuplicateProperty $mainGroup $name) -gt 0) {
                    $stats.packagingDupesRemoved++
                    $ops += "[FIX] $name`:de-duplicated"
                }
                $op = Set-Property $doc $mainGroup $name $value $packIndents
            }
            if ($op -eq 'added')   { $stats.packagingAdded++ }
            if ($op -eq 'updated') { $stats.packagingUpdated++ }
            if ($op -ne 'present' -and $op -ne 'unchanged') { $ops += "[FIX] $name`:$op" }
        }
    }

    # ---- 4. conditional property group ------------------------------------
    $groups  = @(Find-TargetGroups $doc)
    $groupOp = 'present'
    if ($groups.Count -eq 0) {
        $groups  = @(New-ConditionalPropertyGroup $doc $TargetCondition)
        $groupOp = 'added'
        $stats.groupsAdded++
        $ops += "[ADD] PropertyGroup($TargetConfiguration|$TargetPlatform)"
    }

    # ---- 5. OutputPath -----------------------------------------------------
    $outputPath = Get-RelativePathUnix (Split-Path -Parent $file.FullName) $NugetDir

    $pathOps = @()
    foreach ($group in $groups) {
        $indents = Get-Indents $group
        $opOp    = Set-Property $doc $group 'OutputPath' $outputPath $indents
        if ($opOp -eq 'added')   { $stats.outputPathAdded++ }
        if ($opOp -eq 'updated') { $stats.outputPathUpdated++ }
        if ($opOp -ne 'unchanged') { $ops += "[FIX] OutputPath:$opOp($outputPath)" }
        $pathOps += $opOp
    }

    # ---- persist -----------------------------------------------------------
    $projectOps = @($ops | Where-Object { $_ -notlike '*slnx(*' })
    $written    = $false
    if ($projectOps.Count -gt 0) {
        $stats.projectsChanged++
        if (-not $DryRun) { $written = Save-XmlPreserving $doc $file.FullName }
    }

    if ($ops.Count -gt 0) {
        Write-Host ("  {0}" -f $relUnix) -ForegroundColor Green
        Write-Host ("      " + ($ops -join ', ')) -ForegroundColor DarkGray
    }
    else {
        Write-Verbose "ok (nothing to do): $relUnix"
    }

    $report.Add([ordered]@{
        path            = $relUnix
        rootNamespace   = $rootNs
        solution        = $slnxOp
        configurations  = $cfgOp
        platforms       = $platOp
        propertyGroup   = $groupOp
        outputPath      = ($pathOps -join ';')
        outputPathValue = $outputPath
        rewritten       = $written
    })
}

# ---------------------------------------------------------------------------
# Persist the solution once
# ---------------------------------------------------------------------------

if ($slnxDirty) {
    $slnxName   = Split-Path -Leaf $Solution
    $folderNote = ''
    if ($newFolderMade) { $folderNote = " (+ solution folder $SolutionFolder)" }

    Write-Host ""
    if ($DryRun) {
        Write-Host ("  {0} -- would register {1} project(s){2}" -f $slnxName, $stats.slnxAdded, $folderNote) -ForegroundColor Green
    }
    else {
        [void](Save-XmlPreserving $slnx $Solution)
        Write-Host ("  {0} -- registered {1} project(s){2}" -f $slnxName, $stats.slnxAdded, $folderNote) -ForegroundColor Green
    }
}

if ($ReportFile) {
    $rows = @($report | ForEach-Object { [pscustomobject]$_ })
    $rows | Export-Csv -LiteralPath $ReportFile -NoTypeInformation -Encoding UTF8
    Write-Host ""
    Write-Host "Report          : $ReportFile" -ForegroundColor Cyan
}

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "=========== SUMMARY ===========" -ForegroundColor Cyan
Write-Host ("vbproj scanned          : {0}" -f $stats.scanned)
Write-Host ("matched (SMRUCC.genomics): {0}" -f $stats.matched)
Write-Host ("added to solution       : {0}" -f $stats.slnxAdded)          -ForegroundColor Green
Write-Host ("projects modified       : {0}" -f $stats.projectsChanged)    -ForegroundColor Green
Write-Host ("  Configurations patched: {0}" -f $stats.configurationsPatched)
Write-Host ("  Platforms patched     : {0}" -f $stats.platformsPatched)
Write-Host ("  PropertyGroups added  : {0}" -f $stats.groupsAdded)
Write-Host ("  OutputPath added      : {0}" -f $stats.outputPathAdded)
Write-Host ("  OutputPath rewritten  : {0}" -f $stats.outputPathUpdated)
Write-Host ("  Packaging props added : {0}" -f $stats.packagingAdded)
Write-Host ("  Packaging props fixed : {0}" -f $stats.packagingUpdated)
Write-Host ("  Packaging dupes merged: {0}" -f $stats.packagingDupesRemoved)
Write-Host ("skipped (non SDK style) : {0}" -f $stats.skippedLegacy)        -ForegroundColor DarkGray
Write-Host ("skipped (RootNamespace) : {0}" -f $stats.skippedRootNamespace) -ForegroundColor DarkGray
Write-Host ("skipped (Exe/WinExe)    : {0}" -f $stats.skippedExecutable)   -ForegroundColor DarkGray
Write-Host ("blocked by disk guard   : {0}" -f $stats.skippedByGuard)      -ForegroundColor $(if ($stats.skippedByGuard) { 'Yellow' } else { 'DarkGray' })
Write-Host ("failed                  : {0}" -f $stats.failed)             -ForegroundColor $(if ($stats.failed) { 'Red' } else { 'DarkGray' })
if ($DryRun) {
    Write-Host ""
    Write-Host "DryRun mode -- no file was written. Re-run without -DryRun to apply." -ForegroundColor Yellow
}
