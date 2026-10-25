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
            Call writeText(Path.Combine(options.OutDir, "fit_report.txt"), network.Report())
            Call writeText(Path.Combine(options.OutDir, "prior_metrics.txt"), If(network.PriorStatistics Is Nothing, "prior is not enabled", network.PriorStatistics.Summary()))

            ' 持久化 + 往返校验
            Dim reloaded As SSystemNetwork = saveAndVerify(network, options, report, failures)

            If runPerturbation Then
                Dim model As SSystemNetwork = If(reloaded Is Nothing, network, reloaded)

                Call perturb(model, options, report, failures)
            End If
        End Sub

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

            ' 2. 先验驱动的基因挑选
            Dim selected As String() = GeneSelector.SelectGenes(prior, matrixGenes, options.FitGenes, options.ExtractGenes, options.Quiet)
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

            ' 5. 挑选动态范围最大的基因参与拟合
            Dim fitData As BinnedExpression = PseudotimeMatrix.SelectDynamic(binned, options.FitGenes)

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
                .Sparsity = options.Sparsity,
                .MaxEdgesPerGene = options.MaxEdgesPerGene,
                .KeepSelfLoop = True,
                .Prior = prior,
                .UsePrior = True,
                .PriorPenalty = options.PriorPenalty,
                .PriorSignPenalty = options.PriorSignPenalty,
                .UsePriorSign = True,
                .PriorRankBoost = options.PriorRankBoost,
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
            Dim interval As Double = std.Max((finalTime - start) / kicks, 1.0E-6R)

            For Each hub As String In hubs
                Call designs.Add(New PerturbationDesign With {
                    .TargetGene = hub,
                    .Type = PerturbationTypes.KnockOut,
                    .Start = start,
                    .Interval = interval,
                    .Kicks = kicks
                })
                Call designs.Add(New PerturbationDesign With {
                    .TargetGene = hub,
                    .Type = PerturbationTypes.OverExpress,
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
                screening = VirtualScreening.Run(network, designs, options.PerturbDir, finalTime)
            Catch ex As Exception
                Call Console.WriteLine($"[ERROR] the virtual perturbation screening was failed: {ex.Message}")
                Call failures.Add($"the virtual perturbation screening was failed: {ex.Message}")

                Return
            End Try

            Call Console.WriteLine($"[INFO] virtual perturbation finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {screening.Records.SafeQuery().Count()} record(s)")

            Call writeText(Path.Combine(options.OutDir, "perturb_summary.txt"), screening.Summary())
            Call writeScreening(options, screening)

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
                    .Where(Function(r) Not r.IsTarget AndAlso r.MaxDeviation > 1.0E-9) _
                    .Count()

                Call report.AppendLine($"propagated responses: {responded}")

                If responded = 0 Then
                    Call failures.Add("the perturbation signal does not propagate to any downstream gene")
                End If
            End If
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

        Private Sub writeScreening(options As DemoOptions, screening As ScreeningResult)
            Dim sb As New StringBuilder()

            Call sb.AppendLine("target,perturbation,gene,control,perturbed,fold_change,max_deviation,response_time,is_target")

            For Each record As ScreeningRecord In screening.Records.SafeQuery()
                Call sb.Append(record.Target)
                Call sb.Append(","c)
                Call sb.Append(record.Perturbation)
                Call sb.Append(","c)
                Call sb.Append(record.Gene)
                Call sb.Append(","c)
                Call sb.Append(record.ControlLevel.ToString("G6", CultureInfo.InvariantCulture))
                Call sb.Append(","c)
                Call sb.Append(record.PerturbedLevel.ToString("G6", CultureInfo.InvariantCulture))
                Call sb.Append(","c)
                Call sb.Append(record.FoldChange.ToString("G6", CultureInfo.InvariantCulture))
                Call sb.Append(","c)
                Call sb.Append(record.MaxDeviation.ToString("G6", CultureInfo.InvariantCulture))
                Call sb.Append(","c)
                Call sb.Append(record.ResponseTime.ToString("G6", CultureInfo.InvariantCulture))
                Call sb.Append(","c)
                Call sb.AppendLine(record.IsTarget.ToString())
            Next

            Call writeText(Path.Combine(options.OutDir, "perturb_records.csv"), sb.ToString())
        End Sub

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
