#!/usr/bin/env pwsh
# 兼容 Windows PowerShell 5.1 与 PowerShell 7+（不要加 #Requires -Version 7.0）

<#
.SYNOPSIS
    启动 R# 解释器运行指定脚本，并持续监控目标进程的内存占用；
    当内存占用达到阈值（默认 80GB）时立即强制终止进程，防止系统内存被耗尽。

.DESCRIPTION
    脚本会：
      1. 以指定工作目录启动 R#.exe（可选 -MonitorOnly 仅监控已存在的 R# 进程）
      2. 按间隔采样 PrivateMemorySize64 / WorkingSet64，写入 CSV
      3. 把子进程 stdout / stderr 重定向到日志文件
      4. 超过 -ThresholdGB 或 -TimeoutMinutes 时强制杀掉进程树
      5. 结束时输出峰值内存、运行时长、退出码，并把 stdout 末尾若干行写入 summary

.EXAMPLE
    # 默认：在 R# App 目录下运行 K:\hsa_grn\bnlearn.R，80GB 阈值守护
    .\watch-memory.ps1

.EXAMPLE
    # 自定义阈值与超时，日志打上 baseline 标签
    .\watch-memory.ps1 -ThresholdGB 80 -TimeoutMinutes 240 -Tag baseline

.EXAMPLE
    # 不启动新进程，仅守护当前已存在的 R# 进程
    .\watch-memory.ps1 -MonitorOnly
#>

[CmdletBinding()]
param(
    [string]$RSharpExe = "G:\GCModeller\src\R-sharp\App\net10.0\R#.exe",
    [string]$WorkDir = "G:\GCModeller\src\R-sharp\App\net10.0",
    [string]$Script = "K:\hsa_grn\bnlearn.R",
    [string]$Attach = "G:\Erica",
    [double]$ThresholdGB = 80,
    [int]$IntervalSec = 5,
    [int]$TimeoutMinutes = 0,
    [string]$LogDir = "",
    [string]$Tag = "run",
    [int]$TailLines = 80,
    [switch]$MonitorOnly
)

$ErrorActionPreference = 'Stop'

# $PSScriptRoot 在 Windows PowerShell 5.1 的 param 默认值中可能尚未初始化，
# 这里在脚本体内再解析脚本所在目录。
if ([string]::IsNullOrWhiteSpace($LogDir)) {
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    if ([string]::IsNullOrWhiteSpace($scriptDir)) { $scriptDir = $PSScriptRoot }
    $LogDir = Join-Path $scriptDir "logs"
}

# ---------------------------------------------------------------- 参数校验
if (-not (Test-Path $RSharpExe)) { throw "R# 解释器不存在: $RSharpExe" }
if (-not $MonitorOnly -and -not (Test-Path $Script)) { throw "R 脚本不存在: $Script" }
if (-not (Test-Path $WorkDir)) { throw "工作目录不存在: $WorkDir" }

$thresholdBytes = [int64][math]::Round($ThresholdGB * 1GB)
$timeoutSeconds = if ($TimeoutMinutes -gt 0) { $TimeoutMinutes * 60 } else { 0 }

if (-not (Test-Path $LogDir)) {
    New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
}

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$csvPath = Join-Path $LogDir "memory-$Tag-$stamp.csv"
$outLog = Join-Path $LogDir "stdout-$Tag-$stamp.log"
$errLog = Join-Path $LogDir "stderr-$Tag-$stamp.log"
$sumPath = Join-Path $LogDir "summary-$Tag-$stamp.txt"

# ---------------------------------------------------------------- 启动进程
$proc = $null

if ($MonitorOnly) {
    $procs = @(Get-Process -Name 'R#' -ErrorAction SilentlyContinue)
    if ($procs.Count -eq 0) { throw "没有找到正在运行的 R# 进程" }
    $proc = $procs | Sort-Object -Property PrivateMemorySize64 -Descending | Select-Object -First 1
    Write-Host "[watch] 监控已存在进程: R# pid=$($proc.Id)" -ForegroundColor Cyan
} else {
    $argList = @($Script, '--attach', $Attach)
    Write-Host "[watch] 启动: $RSharpExe $($argList -join ' ')" -ForegroundColor Cyan
    Write-Host "[watch] 工作目录: $WorkDir" -ForegroundColor Cyan

    $proc = Start-Process -FilePath $RSharpExe `
        -ArgumentList $argList `
        -WorkingDirectory $WorkDir `
        -RedirectStandardOutput $outLog `
        -RedirectStandardError $errLog `
        -NoNewWindow -PassThru
}

Write-Host "[watch] pid=$($proc.Id) 阈值=$ThresholdGB GB 采样间隔=${IntervalSec}s" -ForegroundColor Cyan
Write-Host "[watch] CSV : $csvPath" -ForegroundColor DarkGray
Write-Host "[watch] 日志: $outLog" -ForegroundColor DarkGray

# ---------------------------------------------------------------- 采样循环
$sw = [System.Diagnostics.Stopwatch]::StartNew()
$writer = [System.IO.StreamWriter]::new($csvPath, $false)
$writer.AutoFlush = $true
$writer.WriteLine("elapsed_sec,timestamp,private_mb,workingset_mb,paged_mb,virtual_mb,threads,handles")

$peakBytes = [int64]0
$peakAt = 0.0
$exitCode = -1
$stopReason = "进程正常退出"
$samples = 0

function Get-ProcMem([System.Diagnostics.Process]$p) {
    $p.Refresh()
    return [pscustomobject]@{
        Private = $p.PrivateMemorySize64
        Working = $p.WorkingSet64
        Paged   = $p.PagedMemorySize64
        Virtual = $p.VirtualMemorySize64
        Threads = $p.Threads.Count
        Handles = $p.HandleCount
    }
}

<#
    强制终止目标进程及其子进程。
    Process.Kill($true) 只在 .NET Core / PowerShell 7 上可用，这里用 taskkill /T /F
    保证在 Windows PowerShell 5.1 下同样能杀掉进程树。
#>
function Stop-TargetProcess([int]$targetId) {
    try { & taskkill /PID $targetId /T /F 2>$null | Out-Null } catch { }
    try { Stop-Process -Id $targetId -Force -ErrorAction SilentlyContinue } catch { }
}

try {
    while ($true) {
        $exited = $false
        try { $exited = $proc.HasExited } catch { $exited = $true }

        if ($exited) {
            try { $exitCode = $proc.ExitCode } catch { $exitCode = -1 }
            break
        }

        $m = Get-ProcMem $proc
        $samples++
        $elapsed = $sw.Elapsed.TotalSeconds

        if ($m.Private -gt $peakBytes) {
            $peakBytes = $m.Private
            $peakAt = $elapsed
        }

        $writer.WriteLine(("{0},{1},{2},{3},{4},{5},{6},{7}" -f
            [math]::Round($elapsed, 1),
            (Get-Date -Format 'HH:mm:ss'),
            [math]::Round($m.Private / 1MB, 1),
            [math]::Round($m.Working / 1MB, 1),
            [math]::Round($m.Paged / 1MB, 1),
            [math]::Round($m.Virtual / 1MB, 1),
            $m.Threads, $m.Handles))

        # 每 12 次采样（约 1 分钟）在控制台汇报一次
        if ($samples % 12 -eq 0) {
            Write-Host ("[watch] t={0}  private={1} GB  workingset={2} GB" -f
                [math]::Round($elapsed / 60, 1),
                [math]::Round($m.Private / 1GB, 2),
                [math]::Round($m.Working / 1GB, 2))
        }

        if ($m.Private -ge $thresholdBytes) {
            $stopReason = "内存达到阈值: $([math]::Round($m.Private / 1GB, 2)) GB >= $ThresholdGB GB"
            Write-Host "[watch] !! $stopReason -> 强制终止进程树" -ForegroundColor Red
            Stop-TargetProcess $proc.Id
            Start-Sleep -Seconds 2
            try { $exitCode = $proc.ExitCode } catch { $exitCode = -1 }
            break
        }

        if ($timeoutSeconds -gt 0 -and $elapsed -ge $timeoutSeconds) {
            $stopReason = "运行超时: $([math]::Round($elapsed / 60, 1)) 分钟 >= $TimeoutMinutes 分钟"
            Write-Host "[watch] !! $stopReason -> 强制终止进程树" -ForegroundColor Red
            Stop-TargetProcess $proc.Id
            Start-Sleep -Seconds 2
            try { $exitCode = $proc.ExitCode } catch { $exitCode = -1 }
            break
        }

        Start-Sleep -Seconds $IntervalSec
    }
} finally {
    $writer.Dispose()
    $sw.Stop()
}

# ---------------------------------------------------------------- 结果汇总
$totalMin = [math]::Round($sw.Elapsed.TotalMinutes, 2)
$peakGB = [math]::Round($peakBytes / 1GB, 2)

$tail = @()
if (-not $MonitorOnly -and (Test-Path $outLog)) {
    $tail = Get-Content -Path $outLog -Tail $TailLines -ErrorAction SilentlyContinue
}
$errTail = @()
if (-not $MonitorOnly -and (Test-Path $errLog)) {
    $errTail = Get-Content -Path $errLog -Tail 20 -ErrorAction SilentlyContinue
}

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("==== R# 运行监控摘要 ====")
[void]$sb.AppendLine("tag                : $Tag")
[void]$sb.AppendLine("pid                : $($proc.Id)")
[void]$sb.AppendLine("command            : $RSharpExe $Script --attach $Attach")
[void]$sb.AppendLine("workdir            : $WorkDir")
[void]$sb.AppendLine("start              : $((Get-Date).AddSeconds(-$sw.Elapsed.TotalSeconds).ToString('yyyy-MM-dd HH:mm:ss'))")
[void]$sb.AppendLine("duration_min       : $totalMin")
[void]$sb.AppendLine("peak_private_gb    : $peakGB")
[void]$sb.AppendLine("peak_at_min        : $([math]::Round($peakAt / 60, 2))")
[void]$sb.AppendLine("threshold_gb       : $ThresholdGB")
[void]$sb.AppendLine("exit_code          : $exitCode")
[void]$sb.AppendLine("stop_reason        : $stopReason")
[void]$sb.AppendLine("samples            : $samples")
[void]$sb.AppendLine("memory_csv         : $csvPath")
[void]$sb.AppendLine("stdout_log         : $outLog")
[void]$sb.AppendLine("stderr_log         : $errLog")
[void]$sb.AppendLine()
[void]$sb.AppendLine("---- stdout 末尾 $TailLines 行 ----")
foreach ($line in $tail) { [void]$sb.AppendLine($line) }
if ($errTail.Count -gt 0) {
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("---- stderr 末尾 20 行 ----")
    foreach ($line in $errTail) { [void]$sb.AppendLine($line) }
}

$summary = $sb.ToString()
Set-Content -Path $sumPath -Value $summary -Encoding UTF8

Write-Host ""
Write-Host $summary
Write-Host "[watch] 摘要已写入: $sumPath" -ForegroundColor Green

exit $exitCode
