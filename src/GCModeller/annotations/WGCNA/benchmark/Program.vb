' ---------------------------------------------------------------------------
'  WGCNA benchmark / 对照测试控制台程序
'
'  用法：
'    WGCNA_benchmark --file <表达矩阵.csv> [--mode blockwise|full|validate]
'                    [--block 5000] [--gpu] [--topN 0] [--minVar 0]
'                    [--seed 12345] [--deepSplit 2] [--power <beta>]
'                    [--cut dynamic|static] [--adjacency 0.6] [--mergeCut 0.15]
'                    [--blocks 1] [--dump] [--out <输出目录>]
'
'  输出（写入 --out 目录）：
'    gene_module.csv   基因 -> 模块 分配表
'    modules.csv       模块名 -> 基因数
'    timing.csv        各阶段耗时（毫秒）
'    summary.txt       汇总信息（基因数、样本数、后端、模块数、峰值内存）
'    cor.csv/adj.csv/tom.csv   （仅 --dump）用于与 GNU R 做逐元素数值对照
' ---------------------------------------------------------------------------

Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.WGCNA
Imports stdNum = System.Math

Module Program

    ''' <summary>
    ''' 命令行参数解析结果
    ''' </summary>
    Class Options
        Public file As String = ""
        Public mode As String = "blockwise"
        Public block As Integer = 5000
        Public gpu As Boolean = False
        Public topN As Integer = 0
        Public minVar As Double = 0
        Public seed As Integer = 12345
        Public deepSplit As Integer = 2
        Public power As Double = Double.NaN
        Public cut As String = "dynamic"
        Public cutHeight As Double = Double.NaN
        Public adjacency As Double = 0.6
        Public mergeCut As Double = 0.15
        Public blocks As Integer = 1
        Public dump As Boolean = False
        Public out As String = ""
        Public verbose As Boolean = True
    End Class

    Function Main(args As String()) As Integer
        Dim opt As Options = ParseArgs(args)

        If opt.file = "" Then
            Call Console.WriteLine(Usage())
            Return 1
        End If

        If opt.out = "" Then
            opt.out = Path.Combine(AppContext.BaseDirectory, "result")
        End If

        Call Directory.CreateDirectory(opt.out)

        Dim proc As Process = Process.GetCurrentProcess()
        Dim swAll As New Stopwatch()

        Call swAll.Start()

        ' ------------------------------------------------------------ 加载
        Dim sw As New Stopwatch()

        sw.Start()

        Dim samples As Matrix = Matrix.LoadData(opt.file, tqdm_wrap:=False)

        sw.Stop()

        Dim loadMs As Double = sw.ElapsedMilliseconds

        Call Console.WriteLine($"loaded: {samples.size} genes x {samples.sample_count} samples in {loadMs / 1000:F1}s")

        ' ------------------------------------------------------------ 配置
        Dim config As New WGCNAConfig With {
            .maxBlockSize = opt.block,
            .adjacency = opt.adjacency,
            .power = opt.power,
            .treeCut = If(opt.cut = "static", TreeCutMethod.StaticCut, TreeCutMethod.Dynamic),
            .deepSplit = opt.deepSplit,
            .mergeCutHeight = opt.mergeCut,
            .useGpu = opt.gpu,
            .maxConcurrentBlocks = stdNum.Max(1, opt.blocks),
            .filterTopN = opt.topN,
            .minVariance = opt.minVar,
            .randomSeed = opt.seed,
            .verbose = opt.verbose,
            .buildGraph = False
        }

        Dim result As Result = Nothing
        Dim dumpCor As Double()() = Nothing
        Dim dumpAdj As Double()() = Nothing
        Dim dumpTom As Double()() = Nothing
        Dim dumpKeys As String() = Nothing

        ' ------------------------------------------------------------ 运行
        Select Case opt.mode.ToLower()
            Case "blockwise"
                result = Analysis.RunBlockwise(samples, config)

            Case "full", "validate"
                ' 单块路径：基因数较大时必须先限制规模，否则 n^2 矩阵会直接 OOM
                If opt.topN > 0 Then
                    samples = GeneFilter.ByVariance(samples, opt.topN, opt.minVar)
                End If

                If samples.size > opt.block Then
                    samples = GeneFilter.ByVariance(samples, opt.block, 0)
                End If

                config.pcaLayout = False
                config.buildGraph = False
                config.cutHeight = opt.cutHeight

                result = Analysis.Run(samples, config)

                If opt.dump Then
                    Call DumpMatrices(samples, result, dumpKeys, dumpCor, dumpAdj, dumpTom)
                End If

            Case Else
                Call Console.WriteLine($"unknown mode: {opt.mode}")
                Return 2
        End Select

        Call swAll.Stop()

        ' ------------------------------------------------------------ 输出
        Call WriteModules(result, opt.out)
        Call WriteTiming(result, opt.out, loadMs, swAll.ElapsedMilliseconds)

        If dumpCor IsNot Nothing Then
            Call WriteMatrix(Path.Combine(opt.out, "cor.csv"), dumpKeys, dumpCor)
            Call WriteMatrix(Path.Combine(opt.out, "adj.csv"), dumpKeys, dumpAdj)
            Call WriteMatrix(Path.Combine(opt.out, "tom.csv"), dumpKeys, dumpTom)
        End If

        Dim peakMB As Double = proc.PeakWorkingSet64 / 1024 / 1024
        Dim gcMB As Double = GC.GetTotalMemory(False) / 1024 / 1024

        Call WriteSummary(opt, samples, result, opt.out, loadMs, swAll.ElapsedMilliseconds, peakMB, gcMB)

        Call Console.WriteLine($"done in {swAll.ElapsedMilliseconds / 1000:F1}s, peak RSS = {peakMB:F0} MB, " &
                               $"{If(result.modules Is Nothing, 0, result.modules.Count)} modules")
        Call Console.WriteLine($"output -> {opt.out}")

        Return 0
    End Function

    ''' <summary>
    ''' 重新计算并导出 cor / adjacency / TOM 三个矩阵，用于与 GNU R 做逐元素对照
    ''' </summary>
    Private Sub DumpMatrices(samples As Matrix, result As Result,
                             ByRef geneKeys As String(), ByRef corMat As Double()(),
                             ByRef adjMat As Double()(), ByRef tomMat As Double()())

        geneKeys = samples.expression.Select(Function(gene) gene.geneID).ToArray()

        Dim n As Integer = geneKeys.Length
        Dim beta As Double = result.beta.Power
        Dim tcor As TensorCorrelation = TensorCorrelation.Create(samples)
        Dim absCor As Double() = WeightedNetwork.AbsCorrelation(tcor)
        Dim adjacency As Double() = WeightedNetwork.BuildAdjacency(absCor, beta, 0.6)
        Dim k As Double() = WeightedNetwork.ConnectivityOf(adjacency, n)
        Dim tomBuf As Double() = TOM.Matrix(adjacency, k, n)

        corMat = TensorOps.ToJagged(tcor.Buffer, n, n)
        adjMat = TensorOps.ToJagged(adjacency, n, n)
        tomMat = TensorOps.ToJagged(tomBuf, n, n)

        Call Console.WriteLine($"dump: beta={beta}, n={n}")
    End Sub

    ''' <summary>
    ''' 写出 gene -> module 分配表与模块统计
    ''' </summary>
    Private Sub WriteModules(result As Result, outDir As String)
        Dim modules As Dictionary(Of String, String()) = result.modules

        If modules Is Nothing Then
            modules = New Dictionary(Of String, String())()
        End If

        Using writer As New StreamWriter(Path.Combine(outDir, "gene_module.csv"), False, New UTF8Encoding(False))
            Call writer.WriteLine("gene,module")

            For Each kv In modules.OrderBy(Function(x) x.Key)
                For Each gene As String In kv.Value
                    Call writer.WriteLine($"{gene},{kv.Key}")
                Next
            Next
        End Using

        Using writer As New StreamWriter(Path.Combine(outDir, "modules.csv"), False, New UTF8Encoding(False))
            Call writer.WriteLine("module,size")

            For Each kv In modules.OrderByDescending(Function(x) x.Value.Length)
                Call writer.WriteLine($"{kv.Key},{kv.Value.Length}")
            Next
        End Using

        If result.blocks IsNot Nothing Then
            Using writer As New StreamWriter(Path.Combine(outDir, "blocks.csv"), False, New UTF8Encoding(False))
                Call writer.WriteLine("block,gene")

                For i As Integer = 0 To result.blocks.Length - 1
                    For Each gene As String In result.blocks(i)
                        Call writer.WriteLine($"{i + 1},{gene}")
                    Next
                Next
            End Using
        End If
    End Sub

    ''' <summary>
    ''' 写出分段耗时
    ''' </summary>
    Private Sub WriteTiming(result As Result, outDir As String, loadMs As Double, totalMs As Double)
        Using writer As New StreamWriter(Path.Combine(outDir, "timing.csv"), False, New UTF8Encoding(False))
            Call writer.WriteLine("stage,ms")
            Call writer.WriteLine($"load,{loadMs.ToString(CultureInfo.InvariantCulture)}")

            If result.timing IsNot Nothing Then
                For Each kv In result.timing
                    Call writer.WriteLine($"{kv.Key},{kv.Value.ToString(CultureInfo.InvariantCulture)}")
                Next
            End If

            If result.blockResults IsNot Nothing Then
                For Each br As BlockResult In result.blockResults
                    If br Is Nothing OrElse br.timing Is Nothing Then Continue For

                    For Each kv In br.timing
                        Call writer.WriteLine($"block{br.index}.{kv.Key},{kv.Value.ToString(CultureInfo.InvariantCulture)}")
                    Next
                Next
            End If

            Call writer.WriteLine($"wallclock,{totalMs.ToString(CultureInfo.InvariantCulture)}")
        End Using
    End Sub

    ''' <summary>
    ''' 写出汇总报告
    ''' </summary>
    Private Sub WriteSummary(opt As Options, samples As Matrix, result As Result, outDir As String,
                            loadMs As Double, totalMs As Double, peakMB As Double, gcMB As Double)

        Dim sb As New StringBuilder()

        Call sb.AppendLine("WGCNA benchmark summary")
        Call sb.AppendLine("=======================")
        Call sb.AppendLine($"file      : {opt.file}")
        Call sb.AppendLine($"mode      : {opt.mode}")
        Call sb.AppendLine($"genes     : {samples.size}")
        Call sb.AppendLine($"samples   : {samples.sample_count}")
        Call sb.AppendLine($"backend   : {Analysis.Backend}")
        Call sb.AppendLine($"treeCut   : {opt.cut}")
        Call sb.AppendLine($"maxBlock  : {opt.block}")
        Call sb.AppendLine($"topN      : {opt.topN}")
        Call sb.AppendLine($"parallel  : {opt.blocks}")
        Call sb.AppendLine($"seed      : {opt.seed}")
        Call sb.AppendLine()

        If result.beta IsNot Nothing Then
            Call sb.AppendLine($"soft power: {result.beta.Power}")
        End If

        If result.modules IsNot Nothing Then
            Call sb.AppendLine($"modules   : {result.modules.Count}")
            Call sb.AppendLine($"assigned  : {result.modules.Sum(Function(kv) kv.Value.Length)}")

            For Each kv In result.modules.OrderByDescending(Function(x) x.Value.Length).Take(20)
                Call sb.AppendLine($"  {kv.Key}: {kv.Value.Length}")
            Next
        End If

        Call sb.AppendLine()
        Call sb.AppendLine($"load       : {loadMs / 1000:F1} s")
        Call sb.AppendLine($"wall clock : {totalMs / 1000:F1} s")
        Call sb.AppendLine($"peak RSS   : {peakMB:F0} MB")
        Call sb.AppendLine($"managed heap: {gcMB:F0} MB")

        If result.timing IsNot Nothing Then
            Call sb.AppendLine()
            Call sb.AppendLine("stage timings (ms):")

            For Each kv In result.timing
                Call sb.AppendLine($"  {kv.Key}: {kv.Value}")
            Next
        End If

        Call File.WriteAllText(Path.Combine(outDir, "summary.txt"), sb.ToString(), New UTF8Encoding(False))
    End Sub

    ''' <summary>
    ''' 以 CSV 写出带行列名的稠密矩阵
    ''' </summary>
    Private Sub WriteMatrix(path As String, keys As String(), mat As Double()())
        Using writer As New StreamWriter(path, False, New UTF8Encoding(False))
            Call writer.WriteLine("," & keys.JoinBy(","))

            For i As Integer = 0 To mat.Length - 1
                Dim row As Double() = mat(i)
                Dim sb As New StringBuilder(keys(i))

                For j As Integer = 0 To row.Length - 1
                    Call sb.Append(","c)
                    Call sb.Append(row(j).ToString("G17", CultureInfo.InvariantCulture))
                Next

                Call writer.WriteLine(sb.ToString())
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 解析命令行参数
    ''' </summary>
    Private Function ParseArgs(args As String()) As Options
        Dim opt As New Options()

        For i As Integer = 0 To args.Length - 1
            Dim key As String = args(i)

            If Not key.StartsWith("--") Then Continue For

            Dim name As String = key.Substring(2).ToLower()
            Dim argValue As String = ""

            If i + 1 < args.Length AndAlso Not args(i + 1).StartsWith("--") Then
                argValue = args(i + 1)
                i += 1
            End If

            Select Case name
                Case "file" : opt.file = argValue
                Case "mode" : opt.mode = argValue
                Case "block" : opt.block = CInt(Val(argValue))
                Case "gpu" : opt.gpu = True
                Case "topn" : opt.topN = CInt(Val(argValue))
                Case "minvar" : opt.minVar = Val(argValue)
                Case "seed" : opt.seed = CInt(Val(argValue))
                Case "deepsplit" : opt.deepSplit = CInt(Val(argValue))
                Case "power" : opt.power = Val(argValue)
                Case "cut" : opt.cut = argValue
                Case "cutheight" : opt.cutHeight = Val(argValue)
                Case "adjacency" : opt.adjacency = Val(argValue)
                Case "mergecut" : opt.mergeCut = Val(argValue)
                Case "blocks" : opt.blocks = CInt(Val(argValue))
                Case "dump" : opt.dump = True
                Case "out" : opt.out = argValue
                Case "quiet" : opt.verbose = False
            End Select
        Next

        Return opt
    End Function

    ''' <summary>
    ''' 使用说明
    ''' </summary>
    Private Function Usage() As String
        Dim sb As New StringBuilder()

        Call sb.AppendLine("WGCNA_benchmark --file <expr.csv> [--mode blockwise|full|validate]")
        Call sb.AppendLine("                [--block 5000] [--gpu] [--topN 0] [--minVar 0]")
        Call sb.AppendLine("                [--seed 12345] [--deepSplit 2] [--power <beta>]")
        Call sb.AppendLine("                [--cut dynamic|static] [--adjacency 0.6] [--mergeCut 0.15]")
        Call sb.AppendLine("                [--blocks 1] [--dump] [--out <dir>]")

        Return sb.ToString()
    End Function
End Module
