' ============================================================================
' Program.vb — MiniBlast 命令行入口
' ----------------------------------------------------------------------------
' 用法：
'   MiniBlast blastn --query q.fa --db db.fa [--out r.json] [选项]
'   MiniBlast blastp --query q.fa --db db.fa [--out r.json] [选项]
'
' 内置自检位于独立的 test 工程（SelfTest.vb），运行：cd test && dotnet run
'
' 任务预设（[README §2.1 / §3.1]）：
'   blastn 任务：megablast | dc-megablast | blastn | blastn-short
'   blastp 任务：blastp | blastp-short
' 命令行显式给出的参数覆盖预设值。
'
' 输出：JSON（System.Text.Json，BCL）。默认写 stdout，--out 指定文件。
' ============================================================================

Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports MiniBlast.Core
Imports MiniBlast.Model
Imports MiniBlast.Options
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Program

    Private ReadOnly Property VersionString As String
        Get
            Return Core.BlastSearch.VersionString
        End Get
    End Property

    Public Function Main(args As String()) As Integer
        If args.Length = 0 OrElse args(0) = "--help" OrElse args(0) = "-h" Then
            PrintUsage()
            Return 0
        End If

        Dim cmd = args(0).ToLowerInvariant()

        If cmd <> "blastn" AndAlso cmd <> "blastp" Then
            Console.Error.WriteLine($"未知子命令: {cmd}")
            PrintUsage()
            Return 2
        End If

        Try
            Dim opts = ParseArgs(args, cmd)
            Dim report = RunSearch(opts)
            Dim json = SerializeReport(report, opts)
            If opts.OutputPath IsNot Nothing Then
                File.WriteAllText(opts.OutputPath, json)
                Console.Error.WriteLine($"结果已写入 {opts.OutputPath}")
            Else
                Console.Out.WriteLine(json)
            End If
            Return 0
        Catch ex As Exception
            Console.Error.WriteLine($"错误: {ex.Message}")
            Return 1
        End Try
    End Function

    Private Class RunOptions
        Inherits BlastOptions

        Public QueryPath As String
        Public DbPath As String
        Public OutputPath As String
        Public Pretty As Boolean

    End Class

    Private Function ParseArgs(args As String(), program As String) As RunOptions
        Dim o As New RunOptions With {.Program = program, .Task = program}
        Dim userWordSize = False, userReward = False, userPenalty = False
        Dim userGap = False, userMatrix = False

        Dim flags As New Dictionary(Of String, String)()
        Dim i As Integer = 1
        While i < args.Length
            Dim a = args(i)
            If a.StartsWith("--") Then
                Dim key = a.Substring(2)
                Dim val = "true"
                If i + 1 < args.Length AndAlso Not args(i + 1).StartsWith("--") Then
                    i += 1
                    val = args(i)
                End If
                flags(key.ToLowerInvariant()) = val
            Else
                ' 按顺序解释位置参数（query / db）
                If o.QueryPath Is Nothing Then
                    o.QueryPath = a
                ElseIf o.DbPath Is Nothing Then
                    o.DbPath = a
                End If
            End If
            i += 1
        End While

        ' ---- 任务预设 [README §2.1/§3.1]（与自检共用 TaskPresets，避免两套参数漂移）----
        If flags.ContainsKey("task") Then o.Task = flags("task").ToLowerInvariant()
        TaskPresets.Apply(o)

        ' ---- 显式参数覆盖预设 ----
        If flags.ContainsKey("query") Then o.QueryPath = flags("query")
        If flags.ContainsKey("db") Then o.DbPath = flags("db")
        If flags.ContainsKey("out") Then o.OutputPath = flags("out")
        If flags.ContainsKey("word-size") Then
            o.WordSize = Integer.Parse(flags("word-size"))
            userWordSize = True
        End If
        If flags.ContainsKey("reward") Then
            o.Reward = Double.Parse(flags("reward"))
            userReward = True
        End If
        If flags.ContainsKey("penalty") Then
            o.Penalty = Double.Parse(flags("penalty"))
            userPenalty = True
        End If
        If flags.ContainsKey("gap-open") Then
            o.GapOpen = Double.Parse(flags("gap-open"))
            userGap = True
        End If
        If flags.ContainsKey("gap-extend") Then
            o.GapExtend = Double.Parse(flags("gap-extend"))
            userGap = True
        End If
        If flags.ContainsKey("matrix") Then
            o.Matrix = flags("matrix").ToUpperInvariant()
            userMatrix = True
        End If
        If flags.ContainsKey("threshold") Then o.Threshold = Integer.Parse(flags("threshold"))
        If flags.ContainsKey("evalue") Then o.EvalueCutoff = Double.Parse(flags("evalue"))
        If flags.ContainsKey("window") Then o.WindowTwoHit = Integer.Parse(flags("window"))
        If flags.ContainsKey("two-hit") Then o.UseTwoHit = flags("two-hit") = "yes"
        If flags.ContainsKey("dust") Then o.Dust = flags("dust") = "yes"
        If flags.ContainsKey("seg") Then o.Seg = flags("seg") = "yes"
        If flags.ContainsKey("comp-based-stats") Then o.CompBasedStats = Integer.Parse(flags("comp-based-stats"))
        If flags.ContainsKey("max-target-seqs") Then o.MaxTargetSeqs = Integer.Parse(flags("max-target-seqs"))
        If flags.ContainsKey("max-hsps") Then o.MaxHsps = Integer.Parse(flags("max-hsps"))
        If flags.ContainsKey("xdrop-ungap") Then o.XdropUngap = Double.Parse(flags("xdrop-ungap"))
        If flags.ContainsKey("xdrop-gap") Then o.XdropGap = Double.Parse(flags("xdrop-gap"))
        If flags.ContainsKey("xdrop-gap-final") Then o.XdropGapFinal = Double.Parse(flags("xdrop-gap-final"))
        If flags.ContainsKey("pretty") Then o.Pretty = flags("pretty") = "true"

        ' megablast：reward/penalty 被显式修改且 gap 未显式给出 → 重算动态 gap
        If program = "blastn" AndAlso o.Task = "megablast" AndAlso
           (userReward OrElse userPenalty) AndAlso Not userGap Then
            o.GapExtend = TaskPresets.MegablastGapExtend(o.Reward, o.Penalty)
        End If

        If o.QueryPath Is Nothing OrElse o.DbPath Is Nothing Then
            Throw New ArgumentException("必须提供 --query 与 --db")
        End If
        Return o
    End Function

    Private Function RunSearch(o As RunOptions) As BlastReport
        ' 与自检共用 BlastSearch，保证 CLI 与自检走完全相同的搜索路径
        Dim queries = FastaFile.Read(o.QueryPath)
        Dim dbSeqs = FastaFile.Read(o.DbPath)
        If queries.Count = 0 Then Throw New ArgumentException("查询文件为空")
        If dbSeqs.Count = 0 Then Throw New ArgumentException("数据库文件为空")

        Console.Error.WriteLine($"MiniBlast {VersionString} ({o.Task})")
        Console.Error.WriteLine($"查询: {queries.Count} 条  数据库: {dbSeqs.Count} 条")
        Console.Error.WriteLine($"数据库总残基: {dbSeqs.Sum(Function(s) CLng(s.SequenceData.Length))}")

        Return Core.BlastSearch.RunQueries(queries, dbSeqs, o)
    End Function

    Private Function SerializeReport(report As BlastReport, o As RunOptions) As String
        Return Model.BlastReportJson.ToJson(report, o.Pretty)
    End Function

    Private Sub PrintUsage()
        Console.WriteLine("MiniBlast — 从头实现的 BLASTN/BLASTP（纯 BCL，JSON 输出）")
        Console.WriteLine()
        Console.WriteLine("用法:")
        Console.WriteLine("  MiniBlast blastn --query q.fa --db db.fa [--out r.json] [--pretty] [选项]")
        Console.WriteLine("  MiniBlast blastp --query q.fa --db db.fa [--out r.json] [--pretty] [选项]")
        Console.WriteLine()
        Console.WriteLine("内置自检（独立 test 工程）：")
        Console.WriteLine("  cd test && dotnet run")
        Console.WriteLine()
        Console.WriteLine("blastn 选项:")
        Console.WriteLine("  --task megablast|dc-megablast|blastn|blastn-short   任务预设（默认 blastn）")
        Console.WriteLine("  --word-size N        word 长度 W")
        Console.WriteLine("  --reward N --penalty N   匹配/错配得分")
        Console.WriteLine("  --gap-open N --gap-extend N   gap 代价")
        Console.WriteLine("  --dust yes|no        DUST 低复杂度过滤（默认随任务）")
        Console.WriteLine("  --window N           两-hit 窗 A（默认 40）")
        Console.WriteLine("  --two-hit yes|no     两-hit 法开关")
        Console.WriteLine("blastp 选项:")
        Console.WriteLine("  --matrix BLOSUM62|BLOSUM45|BLOSUM80|PAM250")
        Console.WriteLine("  --threshold N        邻域词阈值 T（默认 11）")
        Console.WriteLine("  --seg yes|no         SEG 过滤（默认 yes）")
        Console.WriteLine("  --comp-based-stats 0|1   组成校正（默认 0；2/3 回落为 1）")
        Console.WriteLine("通用选项:")
        Console.WriteLine("  --evalue N           E 值截止（默认 10）")
        Console.WriteLine("  --max-target-seqs N --max-hsps N")
        Console.WriteLine("  --out FILE           输出文件（默认 stdout）")
    End Sub

End Module


