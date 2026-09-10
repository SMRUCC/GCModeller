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
