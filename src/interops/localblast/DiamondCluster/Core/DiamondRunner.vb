' ============================================================================
' DiamondRunner.vb — DIAMOND BLASTP 外部进程封装
' ----------------------------------------------------------------------------
' 封装 diamond makedb 和 diamond blastp 命令行调用，
' 处理进程启动、stdout/stderr 捕获、退出码检查。
'
' 关键命令：
'   建库: diamond makedb --in reformatted.fasta -d protein_db.dmnd
'   比对: diamond blastp --query chunk.fasta --db protein_db.dmnd
'          --out chunk.tsv --outfmt 6 qseqid sseqid pident qcovhsp scovhsp
'          --id {min_identity} --query-cover {min_coverage}
'          --threads {threads} --block-size {block_size}
'          --tmpdir {tmpdir} --max-target-seqs 100000000
'
' 内存控制：
'   --block-size 参数控制 DIAMOND 内存使用（默认 0.5 ≈ 2GB）
'   在 16GB 系统上建议 0.5~1.0
' ============================================================================

Imports System
Imports System.Diagnostics
Imports System.IO

Namespace Core

    ''' <summary>DIAMOND 运行结果</summary>
    Public Class DiamondResult

        ''' <summary>是否成功（退出码 = 0）</summary>
        Public Property Success As Boolean

        ''' <summary>标准输出</summary>
        Public Property StdOut As String

        ''' <summary>标准错误</summary>
        Public Property StdErr As String

        ''' <summary>退出码</summary>
        Public Property ExitCode As Integer

        ''' <summary>耗时（秒）</summary>
        Public Property ElapsedSeconds As Double
    End Class

    ''' <summary>
    ''' DIAMOND BLASTP 命令行封装器。
    ''' 管理 diamond 可执行文件路径、线程数、内存块大小等参数。
    ''' </summary>
    Public Class DiamondRunner

        Private ReadOnly _diamondPath As String
        Private ReadOnly _threads As Integer
        Private ReadOnly _blockSize As Double
        Private ReadOnly _tmpDir As String

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="diamondPath">diamond 可执行文件路径</param>
        ''' <param name="threads">线程数</param>
        ''' <param name="blockSize">DIAMOND block-size 参数（控制内存）</param>
        ''' <param name="tmpDir">DIAMOND 临时目录</param>
        Public Sub New(diamondPath As String, threads As Integer, blockSize As Double, tmpDir As String)
            _diamondPath = diamondPath
            _threads = threads
            _blockSize = blockSize
            _tmpDir = tmpDir

            If Not Directory.Exists(tmpDir) Then
                Directory.CreateDirectory(tmpDir)
            End If
        End Sub

        ''' <summary>
        ''' 检查 diamond 是否可用
        ''' </summary>
        Public Function CheckAvailable() As Boolean
            Try
                Dim result = RunProcess(_diamondPath, "--version")
                Return result.ExitCode = 0
            Catch
                Return False
            End Try
        End Function

        ''' <summary>
        ''' 构建 DIAMOND 数据库
        ''' diamond makedb --in {inputFasta} -d {dbPath}
        ''' </summary>
        ''' <param name="inputFasta">输入 FASTA 文件路径</param>
        ''' <param name="dbPath">输出数据库路径（不含 .dmnd 扩展名）</param>
        Public Function MakeDatabase(inputFasta As String, dbPath As String) As DiamondResult
            Dim args = $"makedb --in ""{inputFasta}"" -d ""{dbPath}"""
            Log("DIAMOND makedb", args)
            Return RunProcess(_diamondPath, args, timeoutMinutes:=600)
        End Function

        ''' <summary>
        ''' 运行 DIAMOND blastp 比对
        ''' diamond blastp --query {queryFasta} --db {dbPath} --out {outputTsv}
        '''   --outfmt 6 qseqid sseqid pident qcovhsp scovhsp
        '''   --id {minIdentity} --query-cover {minCoverage}
        '''   --threads {threads} --block-size {blockSize}
        '''   --tmpdir {tmpDir} --max-target-seqs 100000000
        ''' </summary>
        ''' <param name="queryFasta">查询 FASTA 文件</param>
        ''' <param name="dbPath">DIAMOND 数据库路径</param>
        ''' <param name="outputTsv">输出 TSV 文件路径</param>
        ''' <param name="minIdentity">最小序列相似性百分比</param>
        ''' <param name="minCoverage">最小覆盖度百分比</param>
        Public Function BlastP(queryFasta As String, dbPath As String,
                               outputTsv As String,
                               minIdentity As Double, minCoverage As Double) As DiamondResult
            Dim args = $"blastp " &
                       $"--query ""{queryFasta}"" " &
                       $"--db ""{dbPath}"" " &
                       $"--out ""{outputTsv}"" " &
                       $"--outfmt 6 qseqid sseqid pident qcovhsp scovhsp " &
                       $"--id {minIdentity:F1} " &
                       $"--query-cover {minCoverage:F1} " &
                       $"--threads {_threads} " &
                       $"--block-size {_blockSize:F1} " &
                       $"--tmpdir ""{_tmpDir}"" " &
                       $"--max-target-seqs 100000000"
            Log("DIAMOND blastp", args)
            Return RunProcess(_diamondPath, args, timeoutMinutes:=120)
        End Function

        ''' <summary>
        ''' 运行外部进程并捕获输出
        ''' </summary>
        Private Function RunProcess(fileName As String, arguments As String,
                                    Optional timeoutMinutes As Integer = 30) As DiamondResult
            Dim psi As New ProcessStartInfo()
            psi.FileName = fileName
            psi.Arguments = arguments
            psi.UseShellExecute = False
            psi.RedirectStandardOutput = True
            psi.RedirectStandardError = True
            psi.CreateNoWindow = True

            Dim sw As New Stopwatch()
            sw.Start()

            Using p As Process = Process.Start(psi)
                ' 异步读取 stdout/stderr 避免死锁
                Dim stdoutTask = p.StandardOutput.ReadToEndAsync()
                Dim stderrTask = p.StandardError.ReadToEndAsync()

                Dim exited = p.WaitForExit(timeoutMinutes * 60 * 1000)
                If Not exited Then
                    p.Kill(entireProcessTree:=True)
                    sw.Stop()
                    Return New DiamondResult With {
                        .Success = False,
                        .StdOut = "",
                        .StdErr = $"进程超时（{timeoutMinutes} 分钟）",
                        .ExitCode = -1,
                        .ElapsedSeconds = sw.Elapsed.TotalSeconds
                    }
                End If

                Dim stdout = stdoutTask.Result
                Dim stderr = stderrTask.Result
                sw.Stop()

                Return New DiamondResult With {
                    .Success = (p.ExitCode = 0),
                    .StdOut = stdout,
                    .StdErr = stderr,
                    .ExitCode = p.ExitCode,
                    .ElapsedSeconds = sw.Elapsed.TotalSeconds
                }
            End Using
        End Function

        Private Sub Log(tag As String, message As String)
            Dim ts = DateTime.Now.ToString("HH:mm:ss")
            Console.Error.WriteLine($"[{ts}] [{tag}] {message}")
        End Sub

    End Class

End Namespace
