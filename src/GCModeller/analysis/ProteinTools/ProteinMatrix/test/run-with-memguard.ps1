<#
.SYNOPSIS
    在内存看护(memory guard)下运行 Linclust 测试程序。

.DESCRIPTION
    该脚本启动 test.exe 并持续轮询其内存占用。一旦 WorkingSet 超过
    -LimitMB 指定的阈值,立即强制终止该进程,以避免耗尽系统内存导致
    系统无响应或 IDE 崩溃。

    脚本会输出内存采样序列,可据此判断内存是否随比对次数持续单调增长
    (泄漏)还是稳定在某个水位(正常)。

.PARAMETER Exe
    待运行的可执行文件路径。

.PARAMETER Take
    参与聚类的序列条数,透传给 test.exe 作为第一个命令行参数。

.PARAMETER LimitMB
    内存上限(MB)。超过即刻 kill。

.PARAMETER IntervalMs
    采样间隔(毫秒)。

.PARAMETER TimeoutSec
    最长允许运行时间(秒),超时同样终止,避免脚本悬挂。

.EXAMPLE
    .\run-with-memguard.ps1 -Take 50 -LimitMB 2048
#>
param(
    [string] $Exe        = "$PSScriptRoot\bin\Debug\net10.0\test.exe",
    [int]    $Take       = 50,
    [int]    $LimitMB    = 2048,
    [int]    $IntervalMs = 300,
    [int]    $TimeoutSec = 600
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Exe)) {
    Write-Error "找不到可执行文件: $Exe  (请先 dotnet build)"
    exit 2
}

Write-Host "=== Memory Guard ===" -ForegroundColor Cyan
Write-Host "exe      : $Exe"
Write-Host "take     : $Take"
Write-Host "limit    : $LimitMB MB  (超过立即终止)"
Write-Host "interval : $IntervalMs ms"
Write-Host "timeout  : $TimeoutSec s"
Write-Host ""

# 将 stdout/stderr 重定向到文件,避免控制台缓冲区阻塞子进程
$outFile = Join-Path $env:TEMP "linclust_memguard_out.txt"
$errFile = Join-Path $env:TEMP "linclust_memguard_err.txt"

$proc = Start-Process -FilePath $Exe -ArgumentList @("$Take") `
                      -PassThru -NoNewWindow `
                      -RedirectStandardOutput $outFile `
                      -RedirectStandardError  $errFile

$peakMB    = 0.0
$samples   = New-Object System.Collections.Generic.List[double]
$sw        = [System.Diagnostics.Stopwatch]::StartNew()
$killed    = $false
$killCause = ''

try {
    while (-not $proc.HasExited) {

        try   { $proc.Refresh() }
        catch { break }          # 进程已退出

        if ($proc.HasExited) { break }

        $wsMB = [math]::Round($proc.WorkingSet64 / 1MB, 1)
        $samples.Add($wsMB)
        if ($wsMB -gt $peakMB) { $peakMB = $wsMB }

        Write-Host ("[{0,7:F1}s] WorkingSet = {1,9:F1} MB" -f $sw.Elapsed.TotalSeconds, $wsMB)

        # --- 硬性内存上限保护 ---
        if ($wsMB -gt $LimitMB) {
            $killCause = "内存超过上限 ${LimitMB}MB (实测 ${wsMB}MB)"
            $killed = $true
            break
        }

        # --- 超时保护 ---
        if ($sw.Elapsed.TotalSeconds -gt $TimeoutSec) {
            $killCause = "运行超时 (> ${TimeoutSec}s)"
            $killed = $true
            break
        }

        Start-Sleep -Milliseconds $IntervalMs
    }
}
finally {
    if ($killed -and -not $proc.HasExited) {
        Write-Host ""
        Write-Host "!!! 立即终止进程: $killCause" -ForegroundColor Red
        try { $proc.Kill($true) } catch { try { $proc.Kill() } catch {} }
        try { $proc.WaitForExit(5000) | Out-Null } catch {}
    }
}

Write-Host ""
Write-Host "--- 程序输出 ---" -ForegroundColor Yellow
if (Test-Path $outFile) { Get-Content $outFile -Tail 80 }
if ((Test-Path $errFile) -and (Get-Item $errFile).Length -gt 0) {
    Write-Host "--- stderr ---" -ForegroundColor Red
    Get-Content $errFile -Tail 40
}

Write-Host ""
Write-Host "--- 内存统计 ---" -ForegroundColor Yellow
Write-Host ("采样点数 : {0}" -f $samples.Count)
Write-Host ("峰值     : {0:F1} MB" -f $peakMB)

if ($samples.Count -ge 4) {
    # 用首尾各 1/4 段的均值粗略判断内存趋势:
    # 若尾段显著高于首段,说明内存随比对次数持续增长(疑似泄漏)。
    $q     = [int][math]::Floor($samples.Count / 4)
    $head  = ($samples[0..($q-1)]                        | Measure-Object -Average).Average
    $tail  = ($samples[($samples.Count-$q)..($samples.Count-1)] | Measure-Object -Average).Average
    Write-Host ("首段均值 : {0:F1} MB" -f $head)
    Write-Host ("尾段均值 : {0:F1} MB" -f $tail)
    if ($head -gt 0) {
        Write-Host ("增长倍数 : {0:F2}x" -f ($tail / $head))
    }
}

if ($killed) {
    Write-Host ""
    Write-Host "结论: 测试被主动终止 —— $killCause" -ForegroundColor Red
    exit 1
} else {
    Write-Host ""
    Write-Host ("结论: 进程正常退出, ExitCode = {0}" -f $proc.ExitCode) -ForegroundColor Green
    exit $proc.ExitCode
}
