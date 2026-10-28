Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.SGrid.Perturbation
Imports std = System.Math
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention

Namespace GRNDemo

    ''' <summary>
    ''' The end to end demo of the S-system gene regulatory network inference.
    ''' (S-system 基因表达调控网络拟合与虚拟扰动实验的端到端演示)
    ''' </summary>
    ''' <remarks>
    ''' 流程：
    ''' 1. 读取先验知识网络，按先验连接度为基因打分并挑出一个连通子网
    ''' 2. 从数 GB 的表达矩阵之中流式提取出该子网的表达子集
    ''' 3. 用 <see cref="Matrix.LoadData"/> 载入子集矩阵，按 pseudotime 分箱得到时序数据
    ''' 4. 带先验软约束的 S-system 拟合并稀疏化为调控网络
    ''' 5. 把模型持久化为压缩包并逐位校验往返一致性
    ''' 6. 在重载之后的模型之上做枢纽基因的敲除与过表达虚拟扰动实验
    ''' </remarks>
    Public Module DemoRunner

        ''' <summary>
        ''' 执行演示流程并返回进程退出码
        ''' </summary>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Function Run(options As DemoOptions) As Integer
            Dim failures As New List(Of String)
            Dim report As New StringBuilder()
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Call report.AppendLine("=== SGrid GRN demo ===")
            Call report.AppendLine($"command  : {options.Command}")
            Call report.AppendLine($"matrix   : {options.MatrixFile}")
            Call report.AppendLine($"mapping  : {options.MappingFile}")
            Call report.AppendLine($"prior    : {options.PriorFile}")
            Call report.AppendLine($"output   : {options.OutDir}")
            Call report.AppendLine()

            Try
                Call Directory.CreateDirectory(options.OutDir)
            Catch ex As Exception
                Call Console.WriteLine($"[ERROR] unable to create the output directory '{options.OutDir}': {ex.Message}")

                Return 2
            End Try

            Try
                Select Case options.Command
                    Case "extract"
                        Call RunExtract(options, report, failures)
                    Case "fit"
                        Call RunFit(options, report, failures, runPerturbation:=False)
                    Case "all"
                        Call RunFit(options, report, failures, runPerturbation:=True)
                    Case "export"
                        Call RunExport(options, report, failures)
                    Case "context"
                        Call RunContext(options, report, failures)
                    Case "persist"
                        Call RunPersist(options, report, failures)
                    Case "perturb"
                        Call RunPerturbOnly(options, report, failures)
                    Case Else
                        Call Console.WriteLine($"[ERROR] unknown command '{options.Command}', type 'help' for the usage.")

                        Return 2
                End Select
            Catch ex As Exception
                Call Console.WriteLine($"[ERROR] the demo was aborted: {ex.Message}")
                Call Console.WriteLine(ex.ToString)
                Call report.AppendLine($"[ERROR] {ex.Message}")

                Return 1
            End Try

            Call report.AppendLine()
            Call report.AppendLine($"elapsed  : {watch.Elapsed.TotalSeconds.ToString("F1")}s")

            If failures.Count = 0 Then
                Call report.AppendLine("PASS: all of the assertions were satisfied.")
            Else
                Call report.AppendLine($"FAIL: {failures.Count} assertion(s) were violated:")

                For Each failure As String In failures
                    Call report.AppendLine($"  - {failure}")
                Next
            End If

            Call writeText(Path.Combine(options.OutDir, "demo_report.txt"), report.ToString())
            Call Console.WriteLine()
            Call Console.WriteLine(report.ToString())

            Return If(failures.Count = 0, 0, 1)
        End Function

#Region "commands"

        Private Sub RunExtract(options As DemoOptions, report As StringBuilder, failures As List(Of String))
            Dim prior As PriorNet = loadPrior(options, report)

            If prior Is Nothing Then
                Call failures.Add("the prior knowledge network can not be loaded")

                Return
            End If

            Dim binned As BinnedExpression = prepare(options, prior, report, failures)

            If binned Is Nothing Then
                Return
            End If

            Call report.AppendLine()
            Call report.AppendLine($"binned   : {binned.ToString}")
            Call report.AppendLine($"dropped  : {binned.DroppedCells} cell(s) without pseudotime")
            Call report.AppendLine($"empty bin: {binned.EmptyBins}")
        End Sub

        Private Sub RunFit(options As DemoOptions, report As StringBuilder, failures As List(Of String), runPerturbation As Boolean)
            Dim prior As PriorNet = loadPrior(options, report)

            If prior Is Nothing Then
                Call failures.Add("the prior knowledge network can not be loaded")

                Return
            End If

            Dim binned As BinnedExpression = prepare(options, prior, report, failures)

            If binned Is Nothing Then
                Return
            End If

            Dim fitOptions As FittingOptions = createFittingOptions(options, prior, binned)

            Call Console.WriteLine("[INFO] fitting the S-system model...")

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim network As SSystemNetwork = SSystemNetwork.Fit(binned.Matrix, fitOptions)

            Call Console.WriteLine($"[INFO] fitting finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {network.ToString}")

            Call report.AppendLine()
            Call report.AppendLine($"fitting  : {watch.Elapsed.TotalSeconds.ToString("F1")}s, mean slope RMSE={network.Parameters.MeanRMSE.ToString("G6")}")
            Call report.AppendLine($"edges    : {network.Edges.Length}")
            Call report.AppendLine($"prior    : {If(network.Prior Is Nothing, "none", network.Prior.EdgeCount() & " edge(s)")}")

            If Not network.PriorStatistics Is Nothing Then
                Call report.AppendLine()
                Call report.Append(network.PriorStatistics.Summary())
            End If

            ' 拟合产物（表格 + 模型脚本）
            Call network.SaveAll(options.FitDir)
            Call exportForR(network, options)
            Call writeText(Path.Combine(options.OutDir, "fit_report.txt"), network.Report())
            Call writeText(Path.Combine(options.OutDir, "prior_metrics.txt"), If(network.PriorStatistics Is Nothing, "prior is not enabled", network.PriorStatistics.Summary()))

            ' 持久化 + 往返校验
            Dim reloaded As SSystemNetwork = saveAndVerify(network, options, report, failures)

            If runPerturbation Then
                Dim model As SSystemNetwork = If(reloaded Is Nothing, network, reloaded)

                Call perturb(model, options, report, failures)
            End If
        End Sub

        ''' <summary>
        ''' 以 pseudobulk context 作为 ODE 积分的初值，对目标基因清单做虚拟敲降分析
        ''' </summary>
        ''' <remarks>
        ''' 每一个 panel 都会：
        ''' 1. 把 context 的原始 counts 通过 <see cref="SSystemNetwork.MapContext"/> 映射到模型的状态空间
        ''' 2. 用同一组初值积分得到该 panel 自己的 control 轨迹
        ''' 3. 用同一组初值 + 敲降实验积分得到 perturbed 轨迹，并与 control 逐步相减
        ''' 
        ''' 这样三个 panel 得到的是三套互不相同的基线，而不是同一套 pseudotime 起点。
        ''' </remarks>
        Private Sub RunContext(options As DemoOptions, report As StringBuilder, failures As List(Of String))
            Dim model As SSystemNetwork = SSystemNetwork.LoadZip(options.ModelZipFile)
            Dim targetFile As String = If(String.IsNullOrEmpty(options.PertFile), options.MustIncludeFile, options.PertFile)
            Dim targets As String() = readGeneList(targetFile)
            Dim panels As List(Of Tuple(Of String, String)) = contextPanels(options.ContextFile)

            Call report.AppendLine()
            Call report.AppendLine($"context  : {options.ContextFile}")
            Call report.AppendLine($"panels   : {panels.Count}")
            Call report.AppendLine($"knockdown: {options.Knockdown.ToString("P0")} of the activity, start at {options.ContextStart.ToString("P0")} of the simulation")
            Call report.AppendLine($"targets  : {targets.Length} gene(s) from {targetFile}")

            If panels.Count = 0 Then
                Call failures.Add($"no pseudobulk context csv file was found: '{options.ContextFile}'")

                Return
            End If
            If targets.Length = 0 Then
                Call failures.Add("the knockdown target list is empty")

                Return
            End If

            ' 原始基因符号 → 模型内部（规整化）编号
            Dim idMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To model.Parameters.nGenes - 1
                Dim original As String = model.Parameters.OriginalNames(i)
                Dim sanitized As String = model.Parameters.GeneNames(i)

                If Not String.IsNullOrEmpty(original) AndAlso Not idMap.ContainsKey(original) Then
                    Call idMap.Add(original, sanitized)
                End If
                If Not idMap.ContainsKey(sanitized) Then
                    Call idMap.Add(sanitized, sanitized)
                End If
            Next

            Dim modeled As New List(Of String)
            Dim missing As New List(Of String)

            For Each gene As String In targets
                Dim id As String = Nothing

                If idMap.TryGetValue(gene, id) Then
                    Call modeled.Add(id)
                Else
                    Call missing.Add(gene)
                End If
            Next

            Call Console.WriteLine($"[INFO] {modeled.Count}/{targets.Length} knockdown target(s) is inside the fitted network.")

            If options.ContextTargets > 0 AndAlso modeled.Count > options.ContextTargets Then
                Call Console.WriteLine($"[INFO] only the first {options.ContextTargets} target(s) are simulated (--context-targets).")

                Call modeled.RemoveRange(options.ContextTargets, modeled.Count - options.ContextTargets)
            End If

            If options.ContextPrecise > 0 Then
                model.Options.Precise = options.ContextPrecise
            End If

            Dim tEnd As Double = If(options.ContextTime > 0, options.ContextTime, model.Series.TimeEnd)
            Dim root As String = Path.Combine(options.OutDir, "context")

            Call Console.WriteLine($"[INFO] context simulation horizon: 0 ~ {tEnd.ToString("G4")} (pseudotime), knockdown starts at {((tEnd * options.ContextStart)).ToString("G4")}")

            Call writeText(Path.Combine(root, "unmodeled_targets.txt"), String.Join(vbLf, missing))
            Call report.AppendLine($"modeled  : {modeled.Count} target(s), {missing.Count} target(s) is not in the model")

            If modeled.Count = 0 Then
                Call failures.Add("none of the knockdown targets is inside the fitted network")

                Return
            End If

            Dim summary As New StringBuilder()

            Call summary.AppendLine("panel,targets,records,usable,diverged,responders,control_drift")

            For Each panel As Tuple(Of String, String) In panels
                Dim counts As Dictionary(Of String, Double) = readContextPanel(panel.Item2)
                Dim initial As Dictionary(Of String, Double) = model.MapContext(counts)
                Dim designs As New List(Of PerturbationDesign)

                For Each gene As String In modeled
                    Call designs.Add(New PerturbationDesign With {
                        .TargetGene = gene,
                        .Type = InterventionMode.Knockdown,
                        .Magnitude = options.Knockdown,
                        .Start = tEnd * options.ContextStart,
                        .Interval = 1,
                        .Kicks = 1
                    })
                Next

                Dim dir As String = Path.Combine(root, panel.Item1)
                Dim watch As Stopwatch = Stopwatch.StartNew()

                Call Console.WriteLine($"[INFO] panel {panel.Item1}: {counts.Count} gene(s) in the context, {designs.Count} knockdown design(s)...")

                ' 计算与 IO 分离：Run 只做积分，轨迹通过回调交给 SaveTrajectory 落盘
                Dim screening As ScreeningResult = VirtualScreening.Run(
                    model,
                    designs,
                    finalTime:=tEnd,
                    context:=initial,
                    onSimulated:=Sub(design, output)
                                     Call VirtualScreening.SaveTrajectory(
                                         output,
                                         Path.Combine(dir, VirtualScreening.TrajectoryFileOf(design)))
                                 End Sub)

                Call VirtualScreening.SaveScreening(screening, dir)

                ' 换到差异很大的样本环境之后，部分基因的正反馈会让积分发散；
                ' 这些记录的状态量已经溢出，直接丢弃，只统计有效范围内的响应
                Dim cap As Double = options.ContextMaxLevel
                Dim usable As ScreeningRecord() = screening _
                    .Records _
                    .SafeQuery() _
                    .Where(Function(r) isUsable(r, cap)) _
                    .ToArray()
                Dim diverged As Integer = screening.Records.SafeQuery().Count() - usable.Length
                Dim responders As Integer = usable _
                    .Where(Function(r) Not r.IsTarget AndAlso r.MaxDeviation > 0.000000001) _
                    .Count()
                Dim drift As String = controlDrift(Path.Combine(dir, "control.csv"), options.ContextMaxLevel)

                Call summary.AppendLine($"{panel.Item1},{designs.Count},{screening.Records.SafeQuery().Count()},{usable.Length},{diverged},{responders},{drift}")
                Call Console.WriteLine($"[INFO] panel {panel.Item1} finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {usable.Length} usable record(s) ({diverged} diverged), {responders} propagated response(s), diverged control: {drift}")

                ' 每个 panel 只报告最强的前若干个下游响应，避免刷屏
                For Each record As ScreeningRecord In usable _
                    .Where(Function(r) Not r.IsTarget) _
                    .OrderByDescending(Function(r) std.Abs(r.MaxDeviation)) _
                    .Take(10)

                    Call summary.AppendLine($"  {panel.Item1}: {record.Target} -> {record.Gene}, maxdev={record.MaxDeviation.ToString("G4")}, fold={record.FoldChange.ToString("G4")}, response={record.ResponseTime.ToString("G4")}")
                Next
            Next

            Call writeText(Path.Combine(root, "context_summary.txt"), summary.ToString())
            Call report.Append(summary.ToString())
        End Sub

        ''' <summary>
        ''' control 轨迹的漂移倍数：积分窗口之内状态的最大绝对值与初值最大绝对值之比。
        ''' 该值接近 1 说明这个窗口之内系统还处于 context 附近，
        ''' 远大于 1 则说明微分方程已经把状态带离了训练轨迹，此时的“响应”主要是漂移而不是扰动
        ''' </summary>
        ''' <param name="controlFile"></param>
        ''' <returns></returns>
        Private Function controlDrift(controlFile As String, cap As Double) As String
            If Not File.Exists(controlFile) Then
                Return "n/a"
            End If

            Dim peaks As New List(Of Double)
            Dim header As Boolean = True
            Dim firstData As Boolean = True

            Using reader As New StreamReader(controlFile, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Dim line As String

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If
                    If String.IsNullOrWhiteSpace(line) Then
                        Continue Do
                    End If

                    Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                    If header Then
                        header = False
                        Continue Do
                    End If
                    If fields.Length < 2 Then
                        Continue Do
                    End If

                    For i As Integer = 1 To fields.Length - 1
                        If firstData Then
                            Call peaks.Add(0)
                        End If

                        Dim value As Double

                        If Not Double.TryParse(fields(i), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                            Continue For
                        End If
                        If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                            Continue For
                        End If
                        If i - 1 < peaks.Count Then
                            peaks(i - 1) = std.Max(peaks(i - 1), std.Abs(value))
                        End If
                    Next

                    firstData = False
                Loop
            End Using

            ' 按基因统计发散比例：单个基因的溢出不应该主导整个 panel 的诊断
            Dim exploded As Integer = peaks.Where(Function(x) x > cap).Count()
            Dim ratio As Double = If(peaks.Count = 0, 0, exploded / peaks.Count)

            Return $"{ratio.ToString("P1")} gene(s) exceeded {cap.ToString("G3")}"
        End Function

        ''' <summary>
        ''' 判断一条扰动响应是否落在数值有效范围之内（发散或者 NaN 的记录不可用）
        ''' </summary>
        ''' <param name="record"></param>
        ''' <param name="cap"></param>
        ''' <returns></returns>
        Private Function isUsable(record As ScreeningRecord, cap As Double) As Boolean
            If record Is Nothing Then
                Return False
            End If
            If Double.IsNaN(record.ControlLevel) OrElse Double.IsInfinity(record.ControlLevel) Then
                Return False
            End If
            If Double.IsNaN(record.PerturbedLevel) OrElse Double.IsInfinity(record.PerturbedLevel) Then
                Return False
            End If
            If std.Abs(record.ControlLevel) > cap OrElse std.Abs(record.PerturbedLevel) > cap Then
                Return False
            End If
            If Double.IsNaN(record.MaxDeviation) OrElse Double.IsInfinity(record.MaxDeviation) Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' 读取一个 pseudobulk context 表格（列：gene_name,gene_id,mean_counts）
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <returns></returns>
        Private Function readContextPanel(filePath As String) As Dictionary(Of String, Double)
            Dim counts As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            Dim iGene As Integer = 0
            Dim iValue As Integer = 2
            Dim header As Boolean = True

            Using reader As New StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Dim line As String

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If
                    If String.IsNullOrWhiteSpace(line) Then
                        Continue Do
                    End If

                    Dim fields As String() = PriorKnowledge.SplitCsvLine(line)

                    If header Then
                        header = False

                        For i As Integer = 0 To fields.Length - 1
                            Select Case fields(i).Trim().ToLowerInvariant()
                                Case "gene_name", "gene", "symbol"
                                    iGene = i
                                Case "mean_counts", "counts", "mean", "expression"
                                    iValue = i
                            End Select
                        Next

                        Continue Do
                    End If

                    If fields.Length <= std.Max(iGene, iValue) Then
                        Continue Do
                    End If

                    Dim gene As String = fields(iGene).Trim().Trim(""""c)
                    Dim value As Double

                    If gene.Length = 0 Then
                        Continue Do
                    End If
                    If Not Double.TryParse(fields(iValue), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                        Continue Do
                    End If

                    counts(gene) = value
                Loop
            End Using

            Return counts
        End Function

        ''' <summary>
        ''' 取得所有的 panel：单个文件视为一个 panel，文件夹则枚举其中的 csv
        ''' </summary>
        ''' <param name="contextPath"></param>
        ''' <returns></returns>
        Private Function contextPanels(contextPath As String) As List(Of Tuple(Of String, String))
            Dim panels As New List(Of Tuple(Of String, String))

            If String.IsNullOrEmpty(contextPath) Then
                Return panels
            End If
            If File.Exists(contextPath) Then
                Call panels.Add(New Tuple(Of String, String)(Path.GetFileNameWithoutExtension(contextPath), contextPath))

                Return panels
            End If
            If Directory.Exists(contextPath) Then
                For Each file As String In Directory _
                    .GetFiles(contextPath, "*.csv") _
                    .OrderBy(Function(f) f, StringComparer.OrdinalIgnoreCase)

                    Call panels.Add(New Tuple(Of String, String)(Path.GetFileNameWithoutExtension(file), file))
                Next
            End If

            Return panels
        End Function

        ''' <summary>
        ''' 读取基因清单（每行一个）或者 csv 表格的第一列
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <returns></returns>
        Private Function readGeneList(filePath As String) As String()
            Dim genes As New List(Of String)

            If String.IsNullOrEmpty(filePath) OrElse Not File.Exists(filePath) Then
                Return genes.ToArray()
            End If

            For Each line As String In File.ReadAllLines(filePath)
                Dim text As String = If(line, "").Trim()

                If text.Length = 0 Then
                    Continue For
                End If

                Dim fields As String() = PriorKnowledge.SplitCsvLine(text)
                Dim gene As String = fields(0).Trim().Trim(""""c)

                If String.Equals(gene, "target_gene", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Call genes.Add(gene)
            Next

            Return genes.ToArray()
        End Function

        ''' <summary>
        ''' 判断基因清单之中是否包含指定的基因（大小写不敏感）
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="gene"></param>
        ''' <returns></returns>
        Private Function containsGene(genes As String(), gene As String) As Boolean
            For Each item As String In genes.SafeQuery()
                If String.Equals(item, gene, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 从已经保存的模型压缩包重新导出系数矩阵（不再重新拟合）
        ''' </summary>
        Private Sub RunExport(options As DemoOptions, report As StringBuilder, failures As List(Of String))
            Dim model As SSystemNetwork = SSystemNetwork.LoadZip(options.ModelZipFile)

            Call exportForR(model, options)

            Call report.AppendLine()
            Call report.AppendLine($"exported : {model.ToString}")
            Call report.AppendLine($"r2 median: {medianR2(model).ToString("G4")}")
        End Sub

        ''' <summary>
        ''' 模型中各基因斜率拟合 R² 的中位数
        ''' </summary>
        ''' <param name="network"></param>
        ''' <returns></returns>
        Private Function medianR2(network As SSystemNetwork) As Double
            Dim values As Double() = Enumerable _
                .Range(0, network.Parameters.nGenes) _
                .Select(Function(i) network.Parameters.RSquared(i)) _
                .OrderBy(Function(x) x) _
                .ToArray()

            If values.Length = 0 Then
                Return 0
            End If

            Return values(values.Length \ 2)
        End Function

        Private Sub RunPersist(options As DemoOptions, report As StringBuilder, failures As List(Of String))
            Dim model As SSystemNetwork = SSystemNetwork.LoadZip(options.ModelZipFile)

            Call report.AppendLine()
            Call report.AppendLine($"loaded   : {model.ToString}")
            Call report.AppendLine($"series   : {model.Series.ToString}")
            Call report.AppendLine($"eps'     : mean RMSE={model.Parameters.MeanRMSE.ToString("G6")}, refined={model.Parameters.TrajectoryRefined}")

            If Not model.PriorStatistics Is Nothing Then
                Call report.AppendLine()
                Call report.Append(model.PriorStatistics.Summary())
            End If

            ' 重新保存一次然后再加载，确认持久化链路自身是幂等的
            Dim temp As String = Path.Combine(options.OutDir, "sgrid_network.roundtrip.zip")

            Call model.SaveZip(temp)

            Dim again As SSystemNetwork = SSystemNetwork.LoadZip(temp)
            Dim delta As Double = maxDelta(model, again)

            Call report.AppendLine()
            Call report.AppendLine($"round trip max |delta| = {delta.ToString("G6", CultureInfo.InvariantCulture)}")
            Call writeText(Path.Combine(options.OutDir, "reload_check.txt"), $"round trip max |delta| = {delta.ToString("G17", CultureInfo.InvariantCulture)}")

            If delta <> 0 Then
                Call failures.Add($"the model round trip is not lossless: max |delta| = {delta.ToString("G6")}")
            End If

            Call File.Delete(temp)
        End Sub

        Private Sub RunPerturbOnly(options As DemoOptions, report As StringBuilder, failures As List(Of String))
            Dim model As SSystemNetwork = SSystemNetwork.LoadZip(options.ModelZipFile)

            Call report.AppendLine()
            Call report.AppendLine($"loaded   : {model.ToString}")

            Call perturb(model, options, report, failures)
        End Sub

#End Region

#Region "data preparation"

        Private Function loadPrior(options As DemoOptions, report As StringBuilder) As PriorNet
            If Not File.Exists(options.PriorFile) Then
                Call Console.WriteLine($"[ERROR] the prior knowledge file '{options.PriorFile}' does not exist!")

                Return Nothing
            End If

            Dim prior As PriorNet = PriorKnowledge.LoadPriorNetwork(options.PriorFile)

            Call Console.WriteLine($"[INFO] prior knowledge: {prior.EdgeCount()} edge(s), {prior.GeneSymbols().Length} gene symbol(s) ({prior.TypeStatistic()})")
            Call report.AppendLine($"prior    : {prior.EdgeCount()} edge(s), {prior.GeneSymbols().Length} gene symbol(s), {prior.TypeStatistic()}")

            Return prior
        End Function

        Private Function prepare(options As DemoOptions,
                                 prior As PriorNet,
                                 report As StringBuilder,
                                 failures As List(Of String)) As BinnedExpression

            If Not File.Exists(options.MatrixFile) Then
                Call Console.WriteLine($"[ERROR] the expression matrix '{options.MatrixFile}' does not exist!")
                Call failures.Add("the expression matrix file is missing")

                Return Nothing
            End If
            If Not File.Exists(options.MappingFile) Then
                Call Console.WriteLine($"[ERROR] the pseudotime mapping '{options.MappingFile}' does not exist!")
                Call failures.Add("the pseudotime mapping file is missing")

                Return Nothing
            End If

            ' 1. 表达矩阵的基因清单（结果会被缓存，避免重复扫描数 GB 的文件）
            Dim matrixGenes As String() = GeneSelector.ReadMatrixGenes(options.MatrixFile, options.MatrixGeneFile, options.Force, options.Quiet)

            ' 2. 先验驱动的基因挑选（必选基因优先进入核心子网）
            Dim mandatory As String() = readGeneList(options.MustIncludeFile)

            Call Console.WriteLine($"[INFO] {mandatory.Length} mandatory gene(s) is declared by --must-include.")

            Dim selected As String() = GeneSelector.SelectGenes(prior, matrixGenes, options.FitGenes, options.ExtractGenes, options.Quiet, mandatory)
            Dim cached As String() = GeneSelector.ReadGeneList(options.SubsetGeneFile)
            Dim cacheHit As Boolean = Not options.Force AndAlso File.Exists(options.SubsetFile) AndAlso sameList(cached, selected)

            If cacheHit Then
                Call Console.WriteLine($"[INFO] subset matrix cache hit: {options.SubsetFile}")
            Else
                Call Console.WriteLine($"[INFO] extracting {selected.Length} gene(s) from the expression matrix, this may take a while...")

                Call GeneSelector.WriteGeneList(options.SubsetGeneFile, selected)

                Dim written As Integer = GeneSelector.ExtractSubset(options.MatrixFile, options.SubsetFile, selected, options.Quiet)

                If written <> selected.Length Then
                    Call Console.WriteLine($"[WARN] only {written} of the {selected.Length} selected gene(s) was found in the expression matrix.")
                    Call failures.Add($"only {written} of the {selected.Length} selected gene(s) was found in the expression matrix")
                End If
            End If

            ' 3. 用矩阵对象载入子集表达矩阵
            Call Console.WriteLine($"[INFO] loading the subset expression matrix: {options.SubsetFile}")

            Dim subsetMatrix As Matrix = Matrix.LoadData(options.SubsetFile)

            Call Console.WriteLine($"[INFO] subset matrix: {subsetMatrix.size} gene(s) x {subsetMatrix.sample_count} cell(s)")
            Call report.AppendLine($"subset   : {subsetMatrix.size} gene(s) x {subsetMatrix.sample_count} cell(s)")

            If subsetMatrix.size < 2 Then
                Call failures.Add("the subset expression matrix does not contain enough genes")

                Return Nothing
            End If

            ' 4. 沿着 pseudotime 轴分箱
            Dim mapping As Dictionary(Of String, Double) = PseudotimeMatrix.LoadMapping(options.MappingFile)

            Call Console.WriteLine($"[INFO] pseudotime mapping: {mapping.Count} cell(s)")

            Dim binned As BinnedExpression = PseudotimeMatrix.Bin(subsetMatrix, mapping, options.Bins)

            Call Console.WriteLine($"[INFO] binned time series: {binned.ToString}, dropped {binned.DroppedCells} cell(s), {binned.EmptyBins} empty bin(s)")
            Call report.AppendLine($"binned   : {binned.ToString}")
            Call report.AppendLine($"dropped  : {binned.DroppedCells} cell(s), empty bin: {binned.EmptyBins}")

            Call PseudotimeMatrix.SaveCsv(binned, options.BinnedFile)

            If binned.nBins < 8 Then
                Call failures.Add($"only {binned.nBins} pseudotime bin(s) can be built, at least 8 bins are required")
            End If

            ' 5. 挑选动态范围最大的基因参与拟合（必选基因无论如何都保留）
            Dim fitData As BinnedExpression = PseudotimeMatrix.SelectDynamic(binned, options.FitGenes, mandatory)

            If mandatory.Length > 0 Then
                Dim kept As Integer = fitData.Matrix.expression.Count(Function(row) containsGene(mandatory, row.geneID))

                Call Console.WriteLine($"[INFO] {kept}/{mandatory.Length} mandatory gene(s) is kept in the fitted time series.")
                Call report.AppendLine($"mandatory: {kept}/{mandatory.Length} gene(s) kept in the fitting matrix")

                If kept < mandatory.Length Then
                    Call failures.Add($"only {kept} of the {mandatory.Length} mandatory gene(s) is present in the binned matrix")
                End If
            End If

            Call PseudotimeMatrix.SaveCsv(fitData, Path.Combine(options.OutDir, "fit_expression_matrix.csv"))

            Call Console.WriteLine($"[INFO] genes used by the fitting: {fitData.Size}")

            If fitData.Size < 2 Then
                Call failures.Add("the binned expression matrix does not contain enough genes for the fitting")
            End If

            Return fitData
        End Function

        Private Function createFittingOptions(options As DemoOptions, prior As PriorNet, binned As BinnedExpression) As FittingOptions
            Dim fitOptions As New FittingOptions With {
                .Normalize = parseNormalize(options.Normalize),
                .PseudoCount = 1.0R,
                .TimePoints = binned.TimePoints,
                .Denoise = True,
                .SmoothWindow = options.Smooth,
                .Sparsity = options.Sparsity,
                .MaxEdgesPerGene = options.MaxEdgesPerGene,
                .L1Penalty = options.L1Penalty,
                .StabilityPenalty = options.StabilityPenalty,
                .MaxRegulators = options.MaxRegulators,
                .KeepSelfLoop = True,
                .Prior = prior,
                .UsePrior = True,
                .PriorPenalty = options.PriorPenalty,
                .PriorSignPenalty = options.PriorSignPenalty,
                .UsePriorSign = True,
                .PriorRankBoost = options.PriorRankBoost,
                .Profile = options.Profile,
                .PriorCandidateRestriction = False,
                .DEPopulationSize = options.DEPopulation,
                .DEMaxIterations = options.DEIterations,
                .DEThreshold = 0.0001R,
                .LBFGSIterations = 400,
                .RefineTrajectory = False,
                .Seed = options.Seed,
                .Parallel = options.Parallel,
                .Verbose = Not options.Quiet,
                .FinalTime = binned.TimePoints(binned.TimePoints.Length - 1),
                .Precise = options.Precise,
                .Steps = 0
            }

            Return fitOptions
        End Function

#End Region

#Region "persistence"

        Private Function saveAndVerify(network As SSystemNetwork,
                                       options As DemoOptions,
                                       report As StringBuilder,
                                       failures As List(Of String)) As SSystemNetwork
            Call Console.WriteLine($"[INFO] saving the model archive: {options.ModelZipFile}")

            Call network.SaveZip(options.ModelZipFile)

            Dim reloaded As SSystemNetwork = SSystemNetwork.LoadZip(options.ModelZipFile)
            Dim delta As Double = maxDelta(network, reloaded)
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"archive      : {options.ModelZipFile}")
            Call sb.AppendLine($"size         : {New FileInfo(options.ModelZipFile).Length} bytes")
            Call sb.AppendLine($"genes        : {reloaded.Parameters.nGenes}")
            Call sb.AppendLine($"time points  : {reloaded.Series.nTimePoints}")
            Call sb.AppendLine($"edges        : {reloaded.Edges.Length}")
            Call sb.AppendLine($"prior edges  : {reloaded.Prior.EdgeCount()}")
            Call sb.AppendLine($"max |delta|  : {delta.ToString("G17", CultureInfo.InvariantCulture)}")

            Call Console.WriteLine($"[INFO] model archive reloaded, max |delta| = {delta.ToString("G6", CultureInfo.InvariantCulture)}")
            Call report.AppendLine()
            Call report.Append(sb.ToString())

            Call writeText(Path.Combine(options.OutDir, "reload_check.txt"), sb.ToString())

            If delta <> 0 Then
                Call failures.Add($"the model zip archive round trip is not lossless: max |delta| = {delta.ToString("G6")}")
            End If

            Return reloaded
        End Function

        ''' <summary>
        ''' 比较两个网络模型对象之间的最大数值差异
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function maxDelta(a As SSystemNetwork, b As SSystemNetwork) As Double
            Dim delta As Double = 0

            delta = std.Max(delta, std.Abs(a.Parameters.nGenes - b.Parameters.nGenes))
            delta = std.Max(delta, std.Abs(a.Series.nTimePoints - b.Series.nTimePoints))
            delta = std.Max(delta, std.Abs(a.Edges.SafeQuery().Count() - b.Edges.SafeQuery().Count()))

            For i As Integer = 0 To a.Parameters.nGenes - 1
                delta = std.Max(delta, std.Abs(a.Parameters.alpha(i) - b.Parameters.alpha(i)))
                delta = std.Max(delta, std.Abs(a.Parameters.beta(i) - b.Parameters.beta(i)))
                delta = std.Max(delta, std.Abs(a.Parameters.SlopeSSE(i) - b.Parameters.SlopeSSE(i)))
                delta = std.Max(delta, std.Abs(a.Parameters.SlopeRMSE(i) - b.Parameters.SlopeRMSE(i)))

                For j As Integer = 0 To a.Parameters.nGenes - 1
                    delta = std.Max(delta, std.Abs(a.Parameters.G(i)(j) - b.Parameters.G(i)(j)))
                    delta = std.Max(delta, std.Abs(a.Parameters.H(i)(j) - b.Parameters.H(i)(j)))
                Next

                For k As Integer = 0 To a.Series.nTimePoints - 1
                    delta = std.Max(delta, std.Abs(a.Series.X(i)(k) - b.Series.X(i)(k)))
                Next
            Next

            For k As Integer = 0 To a.Series.nTimePoints - 1
                delta = std.Max(delta, std.Abs(a.Series.TimePoints(k) - b.Series.TimePoints(k)))
            Next

            For i As Integer = 0 To a.Edges.SafeQuery().Count() - 1
                delta = std.Max(delta, std.Abs(a.Edges(i).weight - b.Edges(i).weight))
                delta = std.Max(delta, std.Abs(a.Edges(i).g - b.Edges(i).g))
                delta = std.Max(delta, std.Abs(a.Edges(i).h - b.Edges(i).h))
            Next

            Return delta
        End Function

#End Region

#Region "virtual perturbation"

        Private Sub perturb(network As SSystemNetwork, options As DemoOptions, report As StringBuilder, failures As List(Of String))
            Dim hubs As String() = hubGenes(network, options.Hubs)

            If hubs.IsNullOrEmpty Then
                Call Console.WriteLine("[WARN] the fitted network does not contain any regulatory edge, the virtual perturbation is skipped.")
                Call report.AppendLine()
                Call report.AppendLine("virtual perturbation: skipped (no regulatory edge was recovered)")
                Call failures.Add("no regulatory edge was recovered, the virtual perturbation can not be demonstrated")

                Return
            End If

            Dim finalTime As Double = network.Series.TimeEnd
            Dim start As Double = finalTime * 0.1R
            Dim designs As New List(Of PerturbationDesign)

            ' 单次扰动很容易被 S-system 的自我衰减项在很短的时间之内拉回稳态，
            ' 故这里使用周期性的刺激（模拟持续的敲除/过表达处理）来放大下游的响应
            Dim kicks As Integer = 6
            Dim interval As Double = std.Max((finalTime - start) / kicks, 0.000001R)

            For Each hub As String In hubs
                Call designs.Add(New PerturbationDesign With {
                    .TargetGene = hub,
                    .Type = InterventionMode.Knockout,
                    .Start = start,
                    .Interval = interval,
                    .Kicks = kicks
                })
                Call designs.Add(New PerturbationDesign With {
                    .TargetGene = hub,
                    .Type = InterventionMode.Overexpression,
                    .Magnitude = 1.0R,
                    .Start = start,
                    .Interval = interval,
                    .Kicks = kicks
                })
            Next

            Call Console.WriteLine($"[INFO] virtual perturbation on {hubs.Length} hub gene(s): {hubs.JoinBy(", ")}")

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim screening As ScreeningResult = Nothing

            Try
                Dim perturbDir As String = options.PerturbDir

                screening = VirtualScreening.Run(
                    network,
                    designs,
                    finalTime:=finalTime,
                    onSimulated:=Sub(design, output)
                                     Call VirtualScreening.SaveTrajectory(
                                         output,
                                         Path.Combine(perturbDir, VirtualScreening.TrajectoryFileOf(design)))
                                 End Sub)

                Call VirtualScreening.SaveScreening(screening, perturbDir)
            Catch ex As Exception
                Call Console.WriteLine($"[ERROR] the virtual perturbation screening was failed: {ex.Message}")
                Call failures.Add($"the virtual perturbation screening was failed: {ex.Message}")

                Return
            End Try

            Call Console.WriteLine($"[INFO] virtual perturbation finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {screening.Records.SafeQuery().Count()} record(s)")

            Call writeText(Path.Combine(options.OutDir, "perturb_summary.txt"), screening.Summary())

            Call report.AppendLine()
            Call report.AppendLine($"perturb  : {designs.Count} design(s), {screening.Records.SafeQuery().Count()} record(s) in {watch.Elapsed.TotalSeconds.ToString("F1")}s")
            Call report.AppendLine("top downstream responses:")

            Dim top As ScreeningRecord() = screening _
                .Downstream(10) _
                .Where(Function(r) Not Double.IsNaN(r.MaxDeviation) AndAlso Not Double.IsInfinity(r.MaxDeviation)) _
                .ToArray

            For Each record As ScreeningRecord In top
                Call report.AppendLine($"  {record.Target} ({record.Perturbation}) -> {record.Gene}: fold={record.FoldChange.ToString("G4")}, maxdev={record.MaxDeviation.ToString("G4")}, response={record.ResponseTime.ToString("G4")}")
            Next

            If screening.Records.IsNullOrEmpty Then
                Call failures.Add("the virtual perturbation experiment did not produce any response record")
            Else
                Dim invalid As Integer = screening _
                    .Records _
                    .Where(Function(r) Double.IsNaN(r.MaxDeviation) OrElse Double.IsInfinity(r.MaxDeviation) OrElse Double.IsNaN(r.FoldChange) OrElse Double.IsInfinity(r.FoldChange)) _
                    .Count()

                If invalid > 0 Then
                    Call failures.Add($"{invalid} of the perturbation response record(s) is not a finite number")
                End If

                Dim responded As Integer = screening _
                    .Records _
                    .Where(Function(r) Not r.IsTarget AndAlso r.MaxDeviation > 0.000000001) _
                    .Count()

                Call report.AppendLine($"propagated responses: {responded}")

                If responded = 0 Then
                    Call failures.Add("the perturbation signal does not propagate to any downstream gene")
                End If
            End If
        End Sub

        ''' <summary>
        ''' 导出面向 R 分析脚本的系数矩阵（使用**原始**基因符号，
        ''' 否则带连字符的基因名（如 NKX3-1）在规整化之后无法匹配 GO/KEGG 与先验表）
        ''' </summary>
        ''' <param name="network"></param>
        ''' <param name="options"></param>
        Public Sub exportForR(network As SSystemNetwork, options As DemoOptions)
            Dim dir As String = Path.Combine(options.OutDir, "r_model")

            Call network.ExportFormulaTable(Path.Combine(dir, "elastic_formulas.csv"), useOriginal:=True)
            Call network.ExportCoefficientMatrix(Path.Combine(dir, "coefficient_matrix.csv"), useOriginal:=True)

            Call Console.WriteLine($"[INFO] coefficient matrix for the R analysis: {dir}")
        End Sub

        ''' <summary>
        ''' 依据出度挑选枢纽基因
        ''' </summary>
        ''' <param name="network"></param>
        ''' <param name="top"></param>
        ''' <returns></returns>
        Public Function hubGenes(network As SSystemNetwork, Optional top As Integer = 5) As String()
            Dim degree As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            Dim strength As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For Each edge As RegulatoryEdge In network.Edges.SafeQuery()
                Dim count As Integer
                Dim weight As Double

                degree.TryGetValue(edge.source, count)
                degree(edge.source) = count + 1

                strength.TryGetValue(edge.source, weight)
                strength(edge.source) = weight + std.Abs(edge.weight)
            Next

            If degree.Count = 0 Then
                Return New String() {}
            End If

            Return degree _
                .OrderByDescending(Function(a) a.Value) _
                .ThenByDescending(Function(a) strength(a.Key)) _
                .Take(std.Max(1, top)) _
                .Select(Function(a) a.Key) _
                .ToArray()
        End Function

#End Region

#Region "helpers"

        Private Function sameList(a As String(), b As String()) As Boolean
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If
            If a.Length <> b.Length Then
                Return False
            End If

            For i As Integer = 0 To a.Length - 1
                If Not String.Equals(a(i), b(i), StringComparison.OrdinalIgnoreCase) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Function parseNormalize(text As String) As NormalizationMethods
            If String.IsNullOrEmpty(text) Then
                Return NormalizationMethods.Log2
            End If

            Select Case text.Trim().ToLowerInvariant()
                Case "none"
                    Return NormalizationMethods.None
                Case "log2", "log"
                    Return NormalizationMethods.Log2
                Case "zscore", "z-score", "rowzscore"
                    Return NormalizationMethods.RowZScore
                Case "quantile"
                    Return NormalizationMethods.Quantile
                Case Else
                    Call Console.WriteLine($"[WARN] unknown normalization method '{text}', using log2 instead.")

                    Return NormalizationMethods.Log2
            End Select
        End Function

        Private Sub writeText(filePath As String, text As String)
            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(filePath))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Call File.WriteAllText(filePath, If(text, ""), New UTF8Encoding(False))
        End Sub

#End Region
    End Module
End Namespace
