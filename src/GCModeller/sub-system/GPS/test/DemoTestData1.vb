Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports GPS.Test.Demo
Imports EnrichBackground = SMRUCC.genomics.Analysis.HTS.GSEA.Background

''' <summary>
''' 基于 <c>demo/TestData1</c> 真实数据的端到端 DEMO：
''' 数据加载 → GPS 模型训练 → 虚拟敲除 → Limma 差异分析 → Fisher 通路富集。
''' </summary>
''' <remarks>
''' 通过 <c>--demo</c> 触发，例如：
''' <code>
''' dotnet run --project GPS.Test.vbproj -- --demo
''' dotnet run --project GPS.Test.vbproj -- --demo --epochs 12 --tf spo0A --samples 8
''' </code>
''' 不带 <c>--demo</c> 时仍然执行原有的 87 项自测。
''' </remarks>
Public Module DemoTestData1

    ''' <summary>
    ''' DEMO 命令行选项。
    ''' </summary>
    Private Class DemoOptions

        Public Property DataDirectory As String
        Public Property OutputDirectory As String
        Public Property TargetTF As String
        Public Property FinetuneEpochs As Integer = 8
        Public Property PretrainEpochs As Integer = 3
        Public Property SamplesPerGroup As Integer = 6
        Public Property NoiseRatio As Double = 1.0
        Public Property TopN As Integer = 20
        Public Property LearningRate As Double = 0.003
        Public Property Seed As Integer = 2024
        Public Property LogFC As Double = 0.5
        Public Property Fdr As Double = 0.05

    End Class

    ''' <summary>
    ''' 执行 DEMO。
    ''' </summary>
    ''' <param name="args">命令行参数（需包含 <c>--demo</c>）</param>
    ''' <returns>0 表示成功；非 0 表示断言失败或数据缺失</returns>
    Public Function Run(args As String()) As Integer
        Dim options As DemoOptions

        Try
            options = ParseOptions(args)
        Catch ex As Exception
            Console.WriteLine($"[DEMO] 参数错误：{ex.Message}")
            PrintUsage()

            Return 2
        End Try

        Dim timer As Stopwatch = Stopwatch.StartNew()
        Dim report As New StringBuilder()

        Try
            Console.WriteLine("================================================================================")
            Console.WriteLine(" GPS DEMO：TestData1 时序表达 → GNN+Transformer 训练 → 虚拟敲除 → Limma → 富集")
            Console.WriteLine("================================================================================")

            ' ==================== 1. 数据加载 ====================
            Dim dataDirectory As String = If(options.DataDirectory, DemoDataLoader.ResolveDataDirectory())

            Ensure(DemoDataLoader.IsDataDirectory(dataDirectory),
                   $"数据目录不完整：{dataDirectory}")

            Section(report, "数据")
            Console.WriteLine($"[数据] 目录 = {dataDirectory}")

            Dim expression As GeneExpressionData = DemoDataLoader.LoadExpressionMatrix(
                Path.Combine(dataDirectory, DemoDataLoader.ExpressionFile))
            Dim prior As PriorNetwork = DemoDataLoader.LoadPriorNetwork(
                Path.Combine(dataDirectory, DemoDataLoader.PriorFile))

            Ensure(expression.NGene > 10, $"表达矩阵基因数过少：{expression.NGene}")
            Ensure(expression.NSample > 10, $"表达矩阵样本数过少：{expression.NSample}")
            Ensure(prior.Edges.Count > 0, "先验调控网络为空")

            ' ==================== 2. 模型构建 ====================
            Dim config As New GPSConfig With {
                .MaxGenes = 0,                ' 367 个基因全部参与
                .Window = 4,
                .DModel = 32,
                .NLayers = 2,
                .NHeads = 4,
                .FfnDim = 64,
                .GruHidden = 24,
                .PeDim = 8,
                .SpdMaxDist = 4,
                .Dropout = 0.05,
                .TrainRatio = 0.6,
                .ValidationRatio = 0.2,
                .PretrainEpochs = options.PretrainEpochs,
                .FinetuneEpochs = options.FinetuneEpochs,
                .LearningRate = options.LearningRate,
                .EarlyStopPatience = 4,
                .PrintEvery = Math.Max(1, options.FinetuneEpochs \ 4),
                .Seed = options.Seed,
                .MaskRatio = 0.15,
                .UseSpdBias = True,
                .UseGlobalAttention = True,
                .UseLocalMpnn = True
            }

            Dim gps As New GPSModel(expression, prior, config)
            Dim pathways As EnrichBackground = DemoDataLoader.LoadPathways(
                Path.Combine(dataDirectory, DemoDataLoader.PathwayFile),
                gps.GeneNames.Length)

            ' 从实测重复估计测量噪声（log2 单位），作为 in-silico 重复的噪声尺度
            Dim replicateNoise As Double() = DemoDataLoader.EstimateReplicateNoise(expression)
            Dim noiseMedian As Double = replicateNoise.OrderBy(Function(v) v).
                Skip(replicateNoise.Length \ 2).
                First()

            Dim summary As DemoDataSummary = DemoDataLoader.Summarize(expression, prior, pathways)

            Write(report, Console.Out, $"[数据] {summary}")
            Write(report, Console.Out,
                  $"[数据] 测量噪声（同时间点重复间 SD 中位数）= {noiseMedian:G3} log2 单位")
            Write(report, Console.Out, $"[数据] 建模基因 {gps.GeneNames.Length}；" &
                                       $"滑窗 训练/验证/测试 = {gps.Dataset.Train().Count}/{gps.Dataset.Validation().Count}/{gps.Dataset.Test().Count}" &
                                       $"（窗口长度 {gps.Dataset.WindowSize}）")

            Dim plannedSteps As Integer = gps.Dataset.Train().Count * options.FinetuneEpochs

            Write(report, Console.Out,
                  $"[训练] 结构 d_model={config.DModel} layers={config.NLayers} heads={config.NHeads} " &
                  $"ffn={config.FfnDim} gru={config.GruHidden}；预计 {plannedSteps} 个训练步")

            ' ==================== 3. 训练与评估 ====================
            Section(report, "训练")
            Dim sw As Stopwatch = Stopwatch.StartNew()
            Dim training As GPSTrainingResult = gps.Train(config.PrintEvery)

            sw.Stop()

            Ensure(gps.IsTrained, "模型未完成训练")
            Ensure(training.TrainLoss.Count > 0, "训练损失曲线为空")

            Write(report, Console.Out,
                  $"[训练] 完成，用时 {sw.Elapsed.TotalSeconds:F1}s；" &
                  $"预训练 {training.PretrainLoss.Count} 轮，微调 {training.FinetuneEpochs} 轮")

            If training.PretrainLoss.Count > 0 Then
                Write(report, Console.Out,
                      $"  预训练损失 {training.PretrainLoss.First():G6} → {training.PretrainLoss.Last():G6}")
            End If

            Write(report, Console.Out,
                  $"  微调损失 {training.TrainLoss.First():G6} → {training.TrainLoss.Last():G6}" &
                  If(training.ValidationLoss.Count > 0,
                     $"；验证 {training.ValidationLoss.First():G6} → {training.ValidationLoss.Last():G6}" &
                     $"（最优 epoch {training.BestEpoch}，{training.BestValidationLoss:G6}）",
                     ""))

            Ensure(training.TrainLoss.All(Function(v) Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v)),
                   "训练损失出现 NaN/Inf")

            Section(report, "评估")
            Dim metrics As EvaluationMetrics = gps.Evaluate()
            Dim baseline As EvaluationMetrics = gps.EvaluateBaseline()

            Write(report, Console.Out, $"[评估] 模型 {metrics}")
            Write(report, Console.Out, $"[评估] 基线 {baseline}")
            Write(report, Console.Out,
                  $"[评估] 图外基因：模型 MSE={metrics.IsolatedMSE:G4} R²={metrics.IsolatedR2:F4}；" &
                  $"基线 MSE={baseline.IsolatedMSE:G4} R²={baseline.IsolatedR2:F4}")

            Ensure(Not Double.IsNaN(metrics.MSE), "评估未返回有效 MSE")

            If metrics.R2 < baseline.R2 Then
                Write(report, Console.Out,
                      $"  说明：本次仅训练 {training.FinetuneEpochs} 轮（{plannedSteps} 步），" &
                      "模型尚未超过历史持续性基线；增大 --epochs 可继续改善。")
            End If

            ' ==================== 4. 虚拟敲除 ====================
            Section(report, "扰动")
            Dim targetTF As String = If(options.TargetTF, SelectTargetTF(prior, gps.GeneNames))

            Ensure(Not String.IsNullOrEmpty(targetTF), "先验网络中没有可用的 TF")

            Write(report, Console.Out, $"[扰动] 目标 TF = {targetTF}" &
                                       If(options.TargetTF Is Nothing, "（自动选取出度最大的 TF）", "（命令行指定）"))

            Dim knockout As InterventionResult = gps.KnockoutGene(targetTF)

            Ensure(Not knockout.Undefined, $"敲除 {targetTF} 返回未定义结果")
            Ensure(knockout.FoldChanges.Length = gps.GeneNames.Length, "扰动结果长度与基因数不一致")

            Dim topUp As List(Of (GeneName As String, FoldChange As Double, PercentChange As Double)) =
                knockout.GetTopChangedGenes(200).
                Where(Function(g) g.FoldChange > 0).
                Take(options.TopN).
                ToList()
            Dim topDown As List(Of (GeneName As String, FoldChange As Double, PercentChange As Double)) =
                knockout.GetTopChangedGenes(200).
                Where(Function(g) g.FoldChange < 0).
                Take(options.TopN).
                ToList()

            Write(report, Console.Out, $"  显著变化基因 {knockout.NAffected}/{knockout.GeneNames.Length}")
            Write(report, Console.Out, "  上调 Top：" & FormatGenes(topUp, 6))
            Write(report, Console.Out, "  下调 Top：" & FormatGenes(topDown, 6))

            ' ==================== 5. Limma 差异分析 ====================
            Section(report, "Limma")
            Dim limma As DemoLimmaResult = DemoLimma.Run(
                gps.GeneNames,
                replicateNoise,
                knockout.WildtypeMeans,
                knockout.MutantMeans,
                nSamples:=options.SamplesPerGroup,
                noiseRatio:=options.NoiseRatio,
                seed:=options.Seed,
                logFCCutoff:=options.LogFC,
                fdrCutoff:=options.Fdr,
                topN:=Math.Max(10, options.TopN))

            Ensure(limma.Table.Length = gps.GeneNames.Length,
                   $"Limma 结果行数 {limma.Table.Length} 与基因数 {gps.GeneNames.Length} 不一致")

            Write(report, Console.Out,
                  $"[Limma] {limma.NGenes} 基因 × {limma.NSamplesPerGroup * 2} 样本" &
                  $"（每组 {limma.NSamplesPerGroup} 个 in-silico 技术重复，" &
                  $"噪声 σ = {options.NoiseRatio:G2}×实测重复SD，中位 {noiseMedian:G3}）")
            Write(report, Console.Out, $"[Limma] {limma.ThresholdNote}")
            Write(report, Console.Out, $"[Limma] 上调 {limma.Up.Count} / 下调 {limma.Down.Count}")

            For Each row In limma.Table.Take(8)
                Write(report, Console.Out,
                      $"  {row.id,-12} logFC={row.logFC,7:F3}  P={row.P_Value:G3}  adj.P={row.adj_P_Val:G3}")
            Next

            Ensure(limma.Up.Count + limma.Down.Count > 0, "Limma 未筛出任何差异基因")

            ' ==================== 6. 通路富集 ====================
            Section(report, "富集")
            Dim deAll As String() = limma.Up.Select(Function(g) g.id).
                Concat(limma.Down.Select(Function(g) g.id)).
                ToArray()

            Dim enrichAll As DemoEnrichmentResult = DemoEnrichment.Run(pathways, deAll)
            Dim enrichUp As DemoEnrichmentResult = DemoEnrichment.Run(pathways, limma.Up.Select(Function(g) g.id))
            Dim enrichDown As DemoEnrichmentResult = DemoEnrichment.Run(pathways, limma.Down.Select(Function(g) g.id))

            Write(report, Console.Out, "[富集] 差异基因全集（上调 + 下调）")
            Write(report, Console.Out, DemoEnrichment.Format(enrichAll))
            Write(report, Console.Out, "[富集] 仅上调基因")
            Write(report, Console.Out, DemoEnrichment.Format(enrichUp, 5))
            Write(report, Console.Out, "[富集] 仅下调基因")
            Write(report, Console.Out, DemoEnrichment.Format(enrichDown, 5))

            Ensure(enrichAll.All.Length > 0, "富集分析没有返回任何通路结果")

            ' ==================== 7. 结果落盘 ====================
            Section(report, "输出")
            Dim outputDirectory As String = If(options.OutputDirectory,
                                               Path.Combine(Environment.CurrentDirectory, "demo_output"))

            If Not Directory.Exists(outputDirectory) Then
                Call Directory.CreateDirectory(outputDirectory)
            End If

            Dim limmaAllFile As String = Path.Combine(outputDirectory, "limma_all.tsv")
            Dim limmaUpFile As String = Path.Combine(outputDirectory, "limma_up.tsv")
            Dim limmaDownFile As String = Path.Combine(outputDirectory, "limma_down.tsv")
            Dim enrichAllFile As String = Path.Combine(outputDirectory, "enrichment_all.tsv")
            Dim enrichUpFile As String = Path.Combine(outputDirectory, "enrichment_up.tsv")
            Dim enrichDownFile As String = Path.Combine(outputDirectory, "enrichment_down.tsv")
            Dim enrichSigFile As String = Path.Combine(outputDirectory, "enrichment_significant.tsv")

            Call DemoLimma.SaveTable(limmaAllFile, limma.Table)
            Call DemoLimma.SaveTable(limmaUpFile, limma.Up)
            Call DemoLimma.SaveTable(limmaDownFile, limma.Down)
            Call DemoEnrichment.SaveTable(enrichAllFile, enrichAll.All)
            Call DemoEnrichment.SaveTable(enrichUpFile, enrichUp.All)
            Call DemoEnrichment.SaveTable(enrichDownFile, enrichDown.All)
            Call DemoEnrichment.SaveTable(enrichSigFile, enrichAll.Significant)

            For Each outputFile As String In New String() {
                limmaAllFile, limmaUpFile, limmaDownFile,
                enrichAllFile, enrichUpFile, enrichDownFile, enrichSigFile}

                Ensure(System.IO.File.Exists(outputFile) AndAlso New FileInfo(outputFile).Length > 0,
                       $"输出文件为空：{outputFile}")

                Write(report, Console.Out, $"  {outputFile}")
            Next

            ' ==================== 8. 汇总报告 ====================
            Dim reportFile As String = Path.Combine(outputDirectory, "report.txt")

            Call File.WriteAllText(reportFile, report.ToString())
            Write(report, Console.Out, $"  {reportFile}")

            Console.WriteLine()
            Console.WriteLine($"DEMO 完成，总用时 {timer.Elapsed.TotalSeconds:F1}s")
            Console.WriteLine("提示：扰动样本的重复是为统计检验构造的 in-silico 技术重复（高斯噪声），")
            Console.WriteLine("      不是实测生物学重复；logFC 处于模型处理后的 log2 表达尺度。")

            Return 0

        Catch ex As Exception
            Console.WriteLine()
            Console.WriteLine($"[DEMO] 失败：{ex.Message}")
            Console.WriteLine(ex.ToString())

            Return 1
        End Try
    End Function

    ''' <summary>
    ''' 自动选取先验网络中出度（调控靶基因数）最大的 TF，且必须存在于建模基因集合中。
    ''' </summary>
    ''' <param name="prior">先验网络</param>
    ''' <param name="geneNames">建模基因</param>
    ''' <returns>TF 名称；没有可用 TF 时返回空字符串</returns>
    Private Function SelectTargetTF(prior As PriorNetwork, geneNames As String()) As String
        Dim available As New HashSet(Of String)(geneNames, StringComparer.OrdinalIgnoreCase)
        Dim ranked As List(Of KeyValuePair(Of String, Integer)) = prior.Edges.
            GroupBy(Function(e) e.TF, StringComparer.OrdinalIgnoreCase).
            Select(Function(g) New KeyValuePair(Of String, Integer)(g.Key, g.Count())).
            Where(Function(kv) available.Contains(kv.Key)).
            OrderByDescending(Function(kv) kv.Value).
            ThenBy(Function(kv) kv.Key, StringComparer.OrdinalIgnoreCase).
            ToList()

        If ranked.Count = 0 Then Return ""

        Return ranked(0).Key
    End Function

    ''' <summary>把基因-效应列表格式化为一行。</summary>
    ''' <param name="genes">基因效应</param>
    ''' <param name="count">展示数量</param>
    ''' <returns>格式化文本</returns>
    Private Function FormatGenes(genes As List(Of (GeneName As String, FoldChange As Double, PercentChange As Double)),
                                 count As Integer) As String
        If genes Is Nothing OrElse genes.Count = 0 Then Return "（无）"

        Return String.Join(", ",
            genes.Take(count).Select(Function(g) $"{g.GeneName}({g.FoldChange:+0.###;-0.###})"))
    End Function

    ''' <summary>在报告与控制台同时输出一行。</summary>
    Private Sub Write(report As StringBuilder, console As TextWriter, text As String)
        Call report.AppendLine(text)
        Call console.WriteLine(text)
    End Sub

    ''' <summary>报告分节标题。</summary>
    Private Sub Section(report As StringBuilder, name As String)
        Call report.AppendLine()
        Call report.AppendLine($"---- {name} ----")
        Console.WriteLine()
    End Sub

    ''' <summary>断言：不满足时抛出（由 <see cref="Run"/> 捕获并返回非零退出码）。</summary>
    Private Sub Ensure(condition As Boolean, message As String)
        If Not condition Then
            Throw New InvalidOperationException(message)
        End If
    End Sub

    ''' <summary>
    ''' 解析命令行选项。
    ''' </summary>
    ''' <param name="args">命令行参数</param>
    ''' <returns>选项对象</returns>
    Private Function ParseOptions(args As String()) As DemoOptions
        Dim options As New DemoOptions()
        Dim index As Integer = 0

        While index < args.Length
            Dim name As String = args(index).ToLowerInvariant()
            Dim value As String = If(index + 1 < args.Length, args(index + 1), Nothing)

            Select Case name
                Case "--demo"
                    index += 1
                Case "--data"
                    options.DataDirectory = RequireValue(name, value)
                    index += 2
                Case "--out"
                    options.OutputDirectory = RequireValue(name, value)
                    index += 2
                Case "--tf"
                    options.TargetTF = RequireValue(name, value)
                    index += 2
                Case "--epochs"
                    options.FinetuneEpochs = ParseInt(name, value, 1, 500)
                    index += 2
                Case "--pretrain"
                    options.PretrainEpochs = ParseInt(name, value, 0, 500)
                    index += 2
                Case "--samples"
                    options.SamplesPerGroup = ParseInt(name, value, 2, 200)
                    index += 2
                Case "--topn"
                    options.TopN = ParseInt(name, value, 3, 500)
                    index += 2
                Case "--noise"
                    options.NoiseRatio = ParseDouble(name, value, 0.0, 10.0)
                    index += 2
                Case "--lr"
                    options.LearningRate = ParseDouble(name, value, 1.0E-06, 1.0)
                    index += 2
                Case "--logfc"
                    options.LogFC = ParseDouble(name, value, 0.0, 100.0)
                    index += 2
                Case "--fdr"
                    options.Fdr = ParseDouble(name, value, 1.0E-06, 1.0)
                    index += 2
                Case "--seed"
                    options.Seed = ParseInt(name, value, 0, Integer.MaxValue)
                    index += 2
                Case Else
                    Console.WriteLine($"[DEMO] 忽略未知参数：{args(index)}")
                    index += 1
            End Select
        End While

        Return options
    End Function

    Private Function RequireValue(name As String, value As String) As String
        If String.IsNullOrWhiteSpace(value) OrElse value.StartsWith("--", StringComparison.Ordinal) Then
            Throw New ArgumentException($"参数 {name} 需要一个取值")
        End If

        Return value
    End Function

    Private Function ParseInt(name As String, value As String, min As Integer, max As Integer) As Integer
        Dim parsed As Integer

        If Not Integer.TryParse(RequireValue(name, value), NumberStyles.Integer, CultureInfo.InvariantCulture, parsed) Then
            Throw New ArgumentException($"参数 {name} 需要整数，实际 '{value}'")
        End If

        If parsed < min OrElse parsed > max Then
            Throw New ArgumentException($"参数 {name} 应在 [{min}, {max}] 范围内，实际 {parsed}")
        End If

        Return parsed
    End Function

    Private Function ParseDouble(name As String, value As String, min As Double, max As Double) As Double
        Dim parsed As Double

        If Not Double.TryParse(RequireValue(name, value), NumberStyles.Float, CultureInfo.InvariantCulture, parsed) Then
            Throw New ArgumentException($"参数 {name} 需要数值，实际 '{value}'")
        End If

        If parsed < min OrElse parsed > max Then
            Throw New ArgumentException($"参数 {name} 应在 [{min:G3}, {max:G3}] 范围内，实际 {parsed:G3}")
        End If

        Return parsed
    End Function

    ''' <summary>打印 DEMO 用法。</summary>
    Public Sub PrintUsage()
        Console.WriteLine("GPS DEMO 用法：")
        Console.WriteLine("  GPS.Test.exe --demo [选项]")
        Console.WriteLine()
        Console.WriteLine("  --data DIR      数据目录（默认为 TestData1，自动向上探测）")
        Console.WriteLine("  --out DIR       输出目录（默认 ./demo_output）")
        Console.WriteLine("  --tf NAME       指定敲除的 TF（默认自动选取出度最大者）")
        Console.WriteLine("  --epochs N      微调轮数（默认 8）")
        Console.WriteLine("  --pretrain N    预训练轮数（默认 3）")
        Console.WriteLine("  --samples N     每组 in-silico 重复数（默认 6）")
        Console.WriteLine("  --noise R       噪声强度（实测重复 SD 的倍数，默认 1.0）")
        Console.WriteLine("  --topn N        报告展示的 Top 基因/通路数（默认 20）")
        Console.WriteLine("  --lr R          Adam 学习率（默认 0.003）")
        Console.WriteLine("  --logfc R       Limma logFC 阈值（默认 0.5）")
        Console.WriteLine("  --fdr R         Limma/FDR 阈值（默认 0.05）")
        Console.WriteLine("  --seed N        随机种子（默认 2024）")
        Console.WriteLine()
        Console.WriteLine("不带 --demo 时执行原有自测。")
    End Sub

End Module
