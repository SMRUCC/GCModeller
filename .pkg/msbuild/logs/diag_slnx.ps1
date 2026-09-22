$ErrorActionPreference = 'Stop'
$slnxPath = 'G:\GCModeller\src\GCModeller.slnx'
$dir      = 'G:\GCModeller\src'

$d = New-Object System.Xml.XmlDocument
$d.PreserveWhitespace = $true
$d.Load($slnxPath)
$root = $d.DocumentElement

Write-Host '=== direct children of <Solution> ==='
foreach ($c in $root.ChildNodes) {
    if ($c.NodeType -ne 'Element') { continue }
    $name = $c.GetAttribute('Name')
    Write-Host ("  {0,-16} {1}" -f $c.LocalName, $name)
}

Write-Host ''
Write-Host '=== project path sanity ==='
$nodes = $d.SelectNodes('//Project[@Path]')
Write-Host ("  total project nodes: {0}" -f $nodes.Count)

$missing = 0
$special = 0
foreach ($n in $nodes) {
    $p = $n.GetAttribute('Path')
    if ($p -match '[#%]') { $special++; Write-Host ("  SPECIAL CHAR: " + $p) }
    $abs = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($dir, ($p -replace '/', '\')))
    if (-not (Test-Path -LiteralPath $abs)) { $missing++; Write-Host ("  MISSING FILE: " + $p) }
}
Write-Host ("  missing files : {0}" -f $missing)
Write-Host ("  special chars : {0}" -f $special)

Write-Host ''
Write-Host '=== attribute names on Project / Platform nodes ==='
$attrNames = @{}
foreach ($n in $d.SelectNodes('//Project|//Platform|//BuildType|//Folder')) {
    foreach ($a in $n.Attributes) {
        $k = $n.LocalName + '@' + $a.LocalName
        if (-not $attrNames.ContainsKey($k)) { $attrNames[$k] = $a.LocalName }
    }
}
foreach ($k in ($attrNames.Keys | Sort-Object)) { Write-Host ("  " + $k) }
