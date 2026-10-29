Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.FuzzyGene
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.FuzzyGene.Fitting
Imports SMRUCC.genomics.Analysis.FuzzyGene.IO
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' FuzzyGene 端到端自测：合成一个已知拓扑的非线性调控网络，
''' 依次验证拟合精度、边方向、三类虚拟扰动、持久化往返与时序自动切换。
''' </summary>
''' <remarks>
''' 运行方式：<c>dotnet run --project test.vbproj</c> 或直接执行编好的 <c>test.dll</c>。
''' 全部断言通过时退出码为 0，否则为 1。
''' </remarks>
Module Program

    Private passes As Integer = 0
    Private failures As Integer = 0

    Private geneNames As String() = {"A", "B", "C", "D", "E", "F"}

    Function Main(args As String()) As Integer
        If args.Length > 0 AndAlso String.Equals(args(0), "--bench", StringComparison.OrdinalIgnoreCase) Then
            Return RunBenchmark(args)
        End If

        Console.WriteLine("=== FuzzyGene self test ===")
        Console.WriteLine()

        Dim staticData As GeneExpressionData = BuildStaticExpression()
        Dim prior As PriorNetwork = BuildPrior()

        Console.WriteLine($"synthetic matrix: {staticData.NGene} genes x {staticData.NSample} samples")

        TestPriorOnlyModel(staticData, prior)
        TestHybridModel(staticData, prior)
        TestRawScaleModel(staticData, prior)
        TestLaggedModel()
        TestContext(staticData, prior)
        TestNovelty(staticData, prior)

        Console.WriteLine()
        Console.WriteLine($"PASS: {passes} assertion(s), {failures} failure(s)")

        Return If(failures = 0, 0, 1)
    End Function

#Region "性能基准：--bench [genes] [rows] [dop,...]"

    ''' <summary>
    ''' 训练性能基准：在合成矩阵上跑完整拟合，输出 wall time / 分配字节 / Gen2 次数。
    ''' </summary>
    ''' <remarks>
    ''' 用法：<c>dotnet run -c Debug -p:Platform=x64 -- --bench 300 4000 8,24</c>
    ''' （参数依次为基因数、行数、并行度列表；均可省略）
    ''' </remarks>
    Private Function RunBenchmark(args As String()) As Integer
        Dim genes As Integer = If(args.Length > 1, CInt(args(1)), 300)
        Dim rows As Integer = If(args.Length > 2, CInt(args(2)), 4000)
        Dim degrees As Integer() = {8, 24}

        If args.Length > 3 Then
            degrees = args(3).Split(","c).Select(Function(s) CInt(s.Trim())).ToArray()
        End If

        Console.WriteLine("=== FuzzyGene training benchmark ===")
        Console.WriteLine($"synthetic matrix: {genes} genes x {rows} samples, mf=2, maxEpoch=40, K=4, pool=all")

        Dim data As GeneExpressionData = BuildBenchmarkMatrix(genes, rows)

        Console.WriteLine()
        Console.WriteLine("DOP |  wall(s) | trained |  R2   | allocated(GB) | Gen0/1/2      | alloc/row")
        Console.WriteLine("----+----------+---------+-------+---------------+---------------+----------")

        For Each degree As Integer In degrees
            Dim config As New FuzzyGeneConfig With {
                .Selection = RegulatorSelection.CorrelationOnly,
                .MaxRegulators = 4,
                .MaxCandidateGenes = 0,
                .MfCount = 2,
                .MaxEpoch = 40,
                .MaxTrainRows = rows,
                .TargetMode = FuzzyTargetMode.StaticRoute,
                .DegreeOfParallelism = degree
            }

            Dim model As New FuzzyGeneModel(data, Nothing, config)

            ' 先做一次完整 GC，把基线拉平，避免把数据生成阶段的垃圾算进来
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

            Dim g0 As Integer = GC.CollectionCount(0)
            Dim g1 As Integer = GC.CollectionCount(1)
            Dim g2 As Integer = GC.CollectionCount(2)
            Dim before As Long = GC.GetTotalAllocatedBytes(precise:=True)
            Dim clock As Stopwatch = Stopwatch.StartNew()

            Dim report As FuzzyGeneTrainingResult = model.Train()

            clock.Stop()

            Dim allocated As Long = GC.GetTotalAllocatedBytes(precise:=True) - before

            Console.WriteLine($"{degree,3} | {clock.Elapsed.TotalSeconds,8:F1} | {report.NTrained,7} | {report.MeanR2,6:F3} | " &
                              $"{allocated / 1024.0 / 1024 / 1024,13:F2} | " &
                              $"{GC.CollectionCount(0) - g0}/" &
                              $"{GC.CollectionCount(1) - g1}/" &
                              $"{GC.CollectionCount(2) - g2,-4} | " &
                              $"{allocated / CDbl(rows) / 1024,8:F1} KB")
        Next

        Console.WriteLine()
        Console.WriteLine("done")

        Return 0
    End Function

    ''' <summary>
    ''' 合成基准矩阵：每个基因由两条上游基因的双曲正切组合 + 噪声驱动，
    ''' 保证既没有常数列、也有足够强的相关性供候选筛选使用。
    ''' </summary>
    Private Function BuildBenchmarkMatrix(genes As Integer, rows As Integer) As GeneExpressionData
        Dim rng As New Random(20261029)
        Dim matrix(genes - 1, rows - 1) As Double
        Dim names(genes - 1) As String
        Dim samples(rows - 1) As String

        For i As Integer = 0 To rows - 1
            samples(i) = "s" & i
        Next
        For g As Integer = 0 To genes - 1
            names(g) = "G" & g
        Next

        For s As Integer = 0 To rows - 1
            Dim a As Double = Normal(rng)
            Dim b As Double = Normal(rng)
            Dim c As Double = Normal(rng)

            For g As Integer = 0 To genes - 1
                Dim value As Double

                Select Case g
                    Case 0
                        value = 1.0 + 0.8 * a
                    Case 1
                        value = 1.5 + 1.0 * std.Tanh(a) + 0.2 * b
                    Case 2
                        value = 0.8 - 0.9 * std.Tanh(a) + 0.5 * c
                    Case Else
                        value = 1.2 + 1.1 * std.Tanh(matrix(g \ 2, s)) - 0.7 * std.Tanh(matrix(g - 1, s)) + 0.3 * Normal(rng)
                End Select

                matrix(g, s) = value
            Next
        Next

        Return New GeneExpressionData With {
            .GeneNames = names,
            .SampleNames = samples,
            .Matrix = matrix,
            .TimePoints = Nothing
        }
    End Function

#End Region

#Region "场景 1：仅先验（结构已知，验证扰动方向与边符号）"

    Private Sub TestPriorOnlyModel(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 1: prior-only structure ---")

        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorOnly,
            .MaxRegulators = 3,
            .MfCount = 3,
            .MaxEpoch = 60,
            .LearningRate = 0.02,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 60,
            .Relaxation = 0.6,
            .DegreeOfParallelism = 1
        }

        Dim model As New FuzzyGeneModel(data, prior, config)
        Dim report As FuzzyGeneTrainingResult = model.Train()

        Console.WriteLine(report.ToString())

        Check(report.Mode = FuzzyTargetMode.StaticRoute, "static route selected explicitly")
        Check(report.NTrained = 4, $"4 targets have prior regulators (actual {report.NTrained})")
        Check(report.NFailed = 0, $"no fitting failure (actual {report.NFailed})")
        Check(report.MeanR2 > 0.7, $"mean R2 > 0.7 (actual {report.MeanR2:F4})")

        Dim net As FuzzyGeneNetwork = model.Network

        Console.WriteLine($"edges: {net.Edges.Length}")
        For Each edge As RegulationEdgeModel In net.Edges
            Console.WriteLine("  " & edge.ToString())
        Next

        ' ---- 边符号：真实拓扑 A→B(+), A→C(-), F→C(+), B→D(+), C→D(-), D→E(+) ----
        Dim expected As (source As String, target As String, sign As Integer)() = {
            ("A", "B", 1),
            ("A", "C", -1),
            ("F", "C", 1),
            ("B", "D", 1),
            ("C", "D", -1),
            ("D", "E", 1)
        }

        Dim recovered As Integer = 0
        Dim correctSign As Integer = 0

        For Each item In expected
            Dim edge As RegulationEdgeModel = FindEdge(net, item.source, item.target)

            If edge Is Nothing Then
                Console.WriteLine($"  [miss] {item.source} -> {item.target}")
                Continue For
            End If

            recovered += 1

            Dim positive As Boolean = edge.Weight > 0.0

            If (item.sign > 0) = positive Then
                correctSign += 1
            Else
                Console.WriteLine($"  [wrong sign] {edge}")
            End If
        Next

        Check(recovered = 6, $"all 6 true edges recovered (actual {recovered})")
        Check(correctSign = 6, $"all 6 recovered edges have the correct sign (actual {correctSign})")

        ' ---- 虚拟敲除 A：B 下降、C 上升 ----
        Dim koA As InterventionResult = model.KnockoutGene("A")

        Console.WriteLine($"  knockout A: d(B)={Delta(koA, "B"):F4}, d(C)={Delta(koA, "C"):F4}, d(E)={Delta(koA, "E"):F4}")

        Check(Delta(koA, "A") < -0.5, "knockout A removes its expression")
        Check(Delta(koA, "B") < -0.3, $"knockout A down-regulates B (actual {Delta(koA, "B"):F4})")
        Check(Delta(koA, "C") > 0.3, $"knockout A up-regulates C (actual {Delta(koA, "C"):F4})")

        ' ---- 敲低 A：残余比例 + 效应弱于敲除 ----
        Dim kdA As InterventionResult = model.KnockDownGene("A")
        Dim baselineA As Double = koA.WildtypeMeans(IndexOfGene(net, "A"))

        Check(std.Abs(kdA.MutantMeans(IndexOfGene(net, "A")) - 0.2 * baselineA) < 1.0E-06,
              $"knockdown keeps 20% of the baseline (actual {kdA.MutantMeans(IndexOfGene(net, "A")):F4})")
        Check(std.Abs(Delta(kdA, "B")) < std.Abs(Delta(koA, "B")),
              "partial knockdown is weaker than the full knockout")

        ' ---- 过表达 A：B 上升 ----
        Dim oeA As InterventionResult = model.OverexpressGene("A")

        Console.WriteLine($"  overexpress A: d(B)={Delta(oeA, "B"):F4}")

        Check(Delta(oeA, "A") > 0.1, "overexpression raises the target gene")
        Check(Delta(oeA, "B") > 0.3, $"overexpression up-regulates B (actual {Delta(oeA, "B"):F4})")

        ' ---- 敲除 D：E 下降 ----
        Dim koD As InterventionResult = model.KnockoutGene("D")

        Check(Delta(koD, "E") < -0.05, $"knockout D down-regulates E (actual {Delta(koD, "E"):F4})")

        ' ---- 敲除根节点 F：C 下降 ----
        Dim koF As InterventionResult = model.KnockoutGene("F")

        Check(Delta(koF, "C") < -0.2, $"knockout F down-regulates C (actual {Delta(koF, "C"):F4})")

        ' ---- 接口结构与组合扰动 ----
        Check(koA.GeneNames.Length = net.NGene, "intervention result carries all gene names")
        Check(koA.FoldChanges.Length = net.NGene AndAlso koA.ZScores.Length = net.NGene,
              "fold changes and z-scores are aligned with the gene list")
        Check(koA.Spec IsNot Nothing AndAlso koA.Spec.GeneName = "A", "intervention spec is attached")

        Dim combo As CombinationPerturbationResult = model.PredictCombination(New String() {"A", "D"}, InterventionMode.Knockout)

        Check(combo.Joint IsNot Nothing, "combination perturbation returns a joint result")
        Check(combo.NonAdditive.Length = net.NGene, "combination result exposes non-additive effects")

        ' ---- 缺失基因的两种语义 ----
        model.Strict = False
        Dim missing As InterventionResult = model.KnockoutGene("NOT_A_GENE")

        Check(missing.Undefined, "unknown gene degrades to an undefined result when Strict = False")
        Check(Array.TrueForAll(missing.FoldChanges, Function(v) v = 0.0), "undefined result has no fold change")

        model.Strict = True

        Dim thrown As Boolean = False

        Try
            Call model.KnockoutGene("NOT_A_GENE")
        Catch ex As MissingMemberException
            thrown = True
        Catch ex As Exception
            Console.WriteLine("  unexpected exception: " & ex.GetType().Name)
        End Try

        Check(thrown, "unknown gene throws MissingMemberException when Strict = True")
    End Sub

#End Region

#Region "场景 2：先验 + 相关性补齐（默认配置），验证导出与持久化"

    Private Sub TestHybridModel(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 2: hybrid selection + persistence ---")

        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorFirst,
            .MaxRegulators = 3,
            .MfCount = 3,
            .MaxEpoch = 40,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 40,
            .Relaxation = 0.5,
            .PruneRuleThreshold = 0.001,
            .DegreeOfParallelism = 0
        }

        Dim model As New FuzzyGeneModel(data, prior, config)
        Dim report As FuzzyGeneTrainingResult = model.Train()

        Console.WriteLine(report.ToString())

        Check(report.NTrained >= 4, $"at least 4 targets fitted with hybrid selection (actual {report.NTrained})")
        Check(report.EdgeCount > 0, "signed edges were exported")
        Check(report.MeanR2 > 0.5, $"mean R2 > 0.5 (actual {report.MeanR2:F4})")

        Dim net As FuzzyGeneNetwork = model.Network

        ' ---- 规则文本与网络表 ----
        Dim rules As String() = net.RulesText()

        Check(rules.Length > 0, "IF-THEN rules are generated")
        Check(rules.Any(Function(line) line.Contains("IF ") AndAlso line.Contains(" THEN ")),
              "rule text contains the IF ... THEN structure")

        Dim sif As String = net.EdgesToSif()

        Check(sif.Length > 0 AndAlso sif.Contains(ChrW(9) & "pp" & ChrW(9)), "SIF text uses the pp relation")

        Dim csv As String = net.EdgesToCsv()

        Check(csv.StartsWith(RegulationEdgeModel.CsvHeader()), "CSV export starts with the header")
        Check(csv.Split(ChrW(10)).Length > 1, "CSV export contains at least one edge")

        ' ---- 目录导出 ----
        Dim dir As String = Path.Combine(Path.GetTempPath(), "fuzzygene-selftest-" & Guid.NewGuid().ToString("N"))

        Try
            Dim files As String() = model.Export(dir)

            Check(files.Length = 4, $"export writes 4 files (actual {files.Length})")
            Check(files.All(Function(f) File.Exists(f)), "all exported files exist")
        Finally
            If Directory.Exists(dir) Then
                Call Directory.Delete(dir, recursive:=True)
            End If
        End Try

        ' ---- zip 往返 ----
        Dim state As Double() = CType(net.BaselineRaw.Clone(), Double())
        Dim before As Double() = net.Predict(state)
        Dim package As String = Path.Combine(Path.GetTempPath(), "fuzzygene-selftest-" & Guid.NewGuid().ToString("N") & FuzzyGeneFile.PackageExtension)

        Try
            Call model.Save(package)

            Check(File.Exists(package), "model package is written")
            Check(FuzzyGeneFile.IsModelPackage(package), "the written file is recognised as a model package")

            Using archive As ZipArchive = ZipFile.OpenRead(package)
                Check(archive.GetEntry(FuzzyGeneFile.EntryConfig) IsNot Nothing,
                      "the package carries a standalone config.json")
            End Using

            Dim restored As FuzzyGeneModel = FuzzyGeneModel.Load(package)

            Check(restored.IsTrained, "restored model is marked as trained")
            Check(restored.Network.NGene = net.NGene, "restored gene list matches")
            Check(restored.Network.Edges.Length = net.Edges.Length, "restored edge list matches")
            Check(restored.Rules().Length > 0, "restored model still exposes its rules")

            Dim after As Double() = restored.Network.Predict(state)
            Dim maxDiff As Double = 0.0

            For i As Integer = 0 To before.Length - 1
                maxDiff = std.Max(maxDiff, std.Abs(before(i) - after(i)))
            Next

            Check(maxDiff < 1.0E-09, $"restored model predicts identically (max diff {maxDiff:G3})")

            restored.Strict = True
            Dim ko As InterventionResult = restored.KnockoutGene("A")

            Check(ko.FoldChanges.Length = net.NGene, "restored model can run knockout experiments")
        Finally
            If File.Exists(package) Then
                Call File.Delete(package)
            End If
        End Try

        ' ---- 单样本（复用工作区）与批量推理必须给出同一个值 ----
        Dim probe As GeneRegulationModel = Nothing

        For Each item As GeneRegulationModel In net.Models
            If item IsNot Nothing AndAlso item.Network IsNot Nothing Then
                probe = item
                Exit For
            End If
        Next

        If probe IsNot Nothing Then
            Dim nInput As Integer = probe.Network.InputCount
            Dim row(nInput - 1) As Double

            For j As Integer = 0 To nInput - 1
                row(j) = 0.25 * (j - 1)
            Next

            Dim scalarValue As Double = probe.Network.PredictScalar(row)
            Dim batch As Double()() = probe.Network.Predict(New Double()() {row})

            Check(std.Abs(scalarValue - batch(0)(0)) < 1.0E-12,
                  $"scalar and batch inference agree ({scalarValue:G6} vs {batch(0)(0):G6})")

            ' 工作区复用不能留下脏状态：同一行反复推理必须稳定
            Dim repeated As Double = probe.Network.PredictScalar(row)

            Check(scalarValue = repeated, "repeated scalar inference is stable")
        End If

        ' ---- 调控权重矩阵导出 ----
        Dim weights As Matrix = model.RegulationMatrix()
        Dim columnIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For j As Integer = 0 To weights.sampleID.Length - 1
            columnIndex(weights.sampleID(j)) = j
        Next

        Dim rowLookup As New Dictionary(Of String, Double())(StringComparer.OrdinalIgnoreCase)

        For Each line As DataFrameRow In weights.expression
            rowLookup(line.geneID) = line.experiments
        Next

        Check(weights.size = net.NGene AndAlso weights.sample_count = net.NGene,
              $"regulation matrix is N x N ({weights.size} x {weights.sample_count})")

        Dim consistent As Boolean = True
        Dim nonzero As Integer = 0

        For Each edge As RegulationEdgeModel In net.Edges
            Dim line As Double() = Nothing
            Dim col As Integer = 0

            If rowLookup.TryGetValue(edge.Target, line) AndAlso columnIndex.TryGetValue(edge.Source, col) Then
                If std.Abs(line(col) - edge.Weight) > 1.0E-12 Then
                    consistent = False
                End If
                If line(col) <> 0.0 Then
                    nonzero += 1
                End If
            End If
        Next

        Check(consistent, "every edge weight is placed at [target, regulator]")
        Check(nonzero = net.Edges.Length, $"the matrix carries all {net.Edges.Length} edges")

        ' 基因不会把自己当调控因子：对角必须为 0
        Dim diagonalZero As Boolean = True

        For i As Integer = 0 To weights.size - 1
            Dim name As String = weights.expression(i).geneID
            Dim col As Integer

            If columnIndex.TryGetValue(name, col) AndAlso weights.expression(i).experiments(col) <> 0.0 Then
                diagonalZero = False
            End If
        Next

        Check(diagonalZero, "the diagonal of the regulation matrix is zero")

        ' 这个合成场景里 6 个基因全都参与调控，因此 compact 与 square 尺寸相同；
        ' 这里验证的是「compact 不放大、且边一条不少」
        Dim compact As Matrix = model.RegulationMatrix(RegulationMatrixLayout.Compact)
        Dim compactRows As New Dictionary(Of String, Double())(StringComparer.OrdinalIgnoreCase)
        Dim compactColumns As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For j As Integer = 0 To compact.sampleID.Length - 1
            compactColumns(compact.sampleID(j)) = j
        Next
        For Each line As DataFrameRow In compact.expression
            compactRows(line.geneID) = line.experiments
        Next

        Dim compactEdges As Integer = 0

        For Each edge As RegulationEdgeModel In net.Edges
            Dim line As Double() = Nothing
            Dim col As Integer = 0

            If compactRows.TryGetValue(edge.Target, line) AndAlso compactColumns.TryGetValue(edge.Source, col) Then
                If std.Abs(line(col) - edge.Weight) < 1.0E-12 Then
                    compactEdges += 1
                End If
            End If
        Next

        Check(compact.size <= weights.size AndAlso compact.sample_count <= weights.sample_count AndAlso
              compactEdges = net.Edges.Length,
              $"compact layout keeps every edge without growing ({compact.size} x {compact.sample_count}, {compactEdges} edges)")

        ' ---- 置信度矩阵：形状一致、逐元素对应边表的 Confidence ----
        Dim confidence As Matrix = model.ConfidenceMatrix()

        Check(confidence.size = weights.size AndAlso confidence.sample_count = weights.sample_count,
              $"confidence matrix matches the weight matrix ({confidence.size} x {confidence.sample_count})")

        Dim confidenceRows As New Dictionary(Of String, Double())(StringComparer.OrdinalIgnoreCase)

        For Each line As DataFrameRow In confidence.expression
            confidenceRows(line.geneID) = line.experiments
        Next

        Dim confidenceOk As Boolean = True
        Dim nonNegative As Boolean = True

        For Each edge As RegulationEdgeModel In net.Edges
            Dim line As Double() = Nothing
            Dim col As Integer = 0

            If confidenceRows.TryGetValue(edge.Target, line) AndAlso columnIndex.TryGetValue(edge.Source, col) Then
                If std.Abs(line(col) - edge.Confidence) > 1.0E-12 Then
                    confidenceOk = False
                End If
                If line(col) < 0.0 Then
                    nonNegative = False
                End If
            End If
        Next

        Check(confidenceOk, "every confidence is placed at [target, regulator]")
        Check(nonNegative, "confidence values are non negative")
    End Sub

#End Region

#Region "场景 3：关闭标准化时仍然工作在原始表达量尺度"

    Private Sub TestRawScaleModel(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 3: non standardized (raw scale) training ---")

        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorOnly,
            .MaxRegulators = 3,
            .MfCount = 2,
            .MaxEpoch = 30,
            .Standardize = False,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .DegreeOfParallelism = 1
        }

        Dim model As New FuzzyGeneModel(data, prior, config)
        Dim report As FuzzyGeneTrainingResult = model.Train()

        Console.WriteLine(report.ToString())

        Check(report.NTrained >= 3, $"at least 3 targets fitted on the raw scale (actual {report.NTrained})")
        Check(Not model.Network.Standardized, "the network is flagged as non standardized")

        Dim net As FuzzyGeneNetwork = model.Network
        Dim control As Double() = net.SteadyState(net.BaselineRaw)
        Dim bIndex As Integer = net.IndexOfGene("B")

        Check(IsFinite(control(bIndex)), "raw scale steady state is finite")
        Check(control(bIndex) > -1.0 AndAlso control(bIndex) < 6.0,
              $"raw scale steady state stays in the expression range (actual {control(bIndex):F3})")

        Dim ko As InterventionResult = model.KnockoutGene("A")

        Check(std.Abs(ko.MutantMeans(net.IndexOfGene("A"))) < 1.0E-09, "knockout forces the target to exactly zero")
        Check(Delta(ko, "B") < 0.0, $"raw scale knockout still down-regulates B (actual {Delta(ko, "B"):F4})")
    End Sub

#End Region

#Region "场景 4：时序数据自动切换到滞后路线"

    Private Sub TestLaggedModel()
        Console.WriteLine()
        Console.WriteLine("--- scenario 4: lagged route on time series ---")

        Dim data As GeneExpressionData = BuildTimeSeries()
        Dim config As New FuzzyGeneConfig With {
            .MfCount = 2,
            .MaxEpoch = 40,
            .MaxRegulators = 3,
            .DegreeOfParallelism = 1
        }

        Dim model As New FuzzyGeneModel(data, Nothing, config)
        Dim report As FuzzyGeneTrainingResult = model.Train()

        Console.WriteLine(report.ToString())

        Check(report.Mode = FuzzyTargetMode.LaggedRoute, "time series data selects the lagged route automatically")
        Check(report.NSample = data.UniqueTimePoints.Length - 1, "lagged training rows equal the number of time intervals")
        Check(report.NTrained >= 2, $"at least 2 genes modelled in lagged mode (actual {report.NTrained})")

        Dim cascade As InterventionResult = model.SimulateCascade("D", InterventionMode.Knockout, 5)

        Check(cascade.DynamicTrajectory IsNot Nothing, "cascade returns a dynamic trajectory")
        Check(cascade.DynamicTrajectory.GetLength(0) = 5, "trajectory has the requested number of steps")
        Check(cascade.DynamicTrajectory.GetLength(1) = model.Network.NGene, "trajectory is gene aligned")

        Dim finite As Boolean = True

        For t As Integer = 0 To cascade.DynamicTrajectory.GetLength(0) - 1
            For g As Integer = 0 To cascade.DynamicTrajectory.GetLength(1) - 1
                Dim samples As Double() = cascade.DynamicTrajectory(t, g)

                If samples Is Nothing OrElse samples.Length <> 1 OrElse Not IsFinite(samples(0)) Then
                    finite = False
                End If
            Next
        Next

        Check(finite, "every trajectory cell is a finite single-sample value")
    End Sub

#End Region

#Region "场景 5：转录组上下文（pseudobulk 背景）注入"

    Private Sub TestContext(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 5: transcriptome context injection ---")

        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorOnly,
            .MaxRegulators = 3,
            .MfCount = 2,
            .MaxEpoch = 30,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 40,
            .DegreeOfParallelism = 1
        }

        Dim model As New FuzzyGeneModel(data, prior, config)

        Call model.Train()

        Dim net As FuzzyGeneNetwork = model.Network
        Dim indexA As Integer = net.IndexOfGene("A")
        Dim contextA As Double = net.Means(indexA) + 0.6 * net.SDs(indexA)
        Dim baselineRun As InterventionResult = model.KnockoutGene("A")
        Dim contextFile As String = Path.Combine(Path.GetTempPath(), "fuzzygene_context_" & Guid.NewGuid().ToString("N") & ".csv")

        Try
            ' 模拟 pseudobulk 表：gene_name,gene_id,mean_counts
            Dim lines As New List(Of String) From {
                "gene_name,gene_id,mean_counts",
                $"A,ENSG_A,{contextA.ToString("R", CultureInfo.InvariantCulture)}"
            }

            For Each gene As String In New String() {"B", "C", "D", "E", "F"}
                Dim idx As Integer = net.IndexOfGene(gene)

                Call lines.Add($"{gene},ENSG_{gene},{net.Means(idx).ToString("R", CultureInfo.InvariantCulture)}")
            Next

            ' 模型之外的基因、以及一行非法值
            Call lines.Add("NOT_IN_MODEL,ENSG_XXX,1.5")
            Call lines.Add("C,ENSG_C,NA")

            Call File.WriteAllLines(contextFile, lines)

            Dim coverage As ContextCoverage = model.LoadContext(contextFile)

            Console.WriteLine(coverage.ToString())

            Check(coverage.Matched = net.NGene, $"every model gene is covered (actual {coverage.Matched})")
            Check(std.Abs(coverage.Coverage - 1.0) < 1.0E-09, "coverage reaches 100%")
            Check(String.Equals(coverage.GeneColumn, "gene_name"), $"the gene column is detected (actual {coverage.GeneColumn})")
            Check(coverage.InvalidValues = 1, $"the NA row is skipped instead of becoming zero (actual {coverage.InvalidValues})")
            Check(model.IsContextSet, "the model reports the context as applied")
            Check(std.Abs(model.Context(indexA) - contextA) < 1.0E-09,
                  $"the context value reaches the state vector (actual {model.Context(indexA):F6})")

            ' ---- 部分覆盖：未给出的基因继续沿用训练基线 ----
            Dim partialContext As ContextCoverage = model.SetContext(New String() {"A"}, New Double() {contextA})

            Check(partialContext.Matched = 1 AndAlso partialContext.MissingGenes.Length = net.NGene - 1,
                  $"a partial context keeps the remaining genes on the training baseline ({partialContext.Matched}/{net.NGene})")
            Check(std.Abs(partialContext.Coverage - 1.0 / net.NGene) < 1.0E-09, "partial coverage is reported correctly")

            ' ---- 覆盖率门槛 ----
            Dim rejected As Boolean = False

            Try
                Call model.SetContext(New String() {"A"}, New Double() {contextA}, minCoverage:=0.9)
            Catch ex As ArgumentException
                rejected = True
            End Try

            Check(rejected, "a context below minCoverage is rejected")

            ' ---- 初始条件确实影响（尚未收敛的）传播 ----
            Dim fromContext As Double() = net.SteadyState(model.Context, steps:=2)
            Dim fromBaseline As Double() = net.SteadyState(net.BaselineRaw, steps:=2)
            Dim maxDiff As Double = 0.0

            For i As Integer = 0 To net.NGene - 1
                maxDiff = std.Max(maxDiff, std.Abs(fromContext(i) - fromBaseline(i)))
            Next

            Check(maxDiff > 1.0E-06, $"the initial condition drives the transient propagation (max diff {maxDiff:G3})")

            ' ---- 撤销上下文后必须回到与之前完全一致的基线结果 ----
            Call model.ResetContext()

            Check(Not model.IsContextSet, "reset removes the context")

            Dim again As InterventionResult = model.KnockoutGene("A")
            Dim identical As Boolean = True

            For i As Integer = 0 To net.NGene - 1
                If std.Abs(again.FoldChanges(i) - baselineRun.FoldChanges(i)) > 1.0E-12 Then
                    identical = False
                End If
            Next

            Check(identical, "resetting the context reproduces the baseline perturbation exactly")
        Finally
            If File.Exists(contextFile) Then
                Call File.Delete(contextFile)
            End If
        End Try
    End Sub

#End Region

#Region "场景 6：数据驱动的调控连接发现（novelty 配额）"

    Private Sub TestNovelty(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 6: novelty quota (data driven links) ---")

        Dim build As Func(Of Double, FuzzyGeneConfig) =
            Function(quota) New FuzzyGeneConfig With {
                .Selection = RegulatorSelection.PriorFirst,
                .MaxRegulators = 3,
                .NoveltyQuota = quota,
                .MfCount = 2,
                .MaxEpoch = 20,
                .TargetMode = FuzzyTargetMode.StaticRoute,
                .DegreeOfParallelism = 1
            }

        Dim priorOnly As New FuzzyGeneModel(data, prior, build(0.0))

        Call priorOnly.Train()

        Dim novelty As New FuzzyGeneModel(data, prior, build(0.5))

        Call novelty.Train()

        Dim baseline As Integer = priorOnly.Network.Edges.Count(Function(e) Not e.IsPrior)
        Dim discovered As Integer = novelty.Network.Edges.Count(Function(e) Not e.IsPrior)

        Console.WriteLine($"  non-prior edges: quota=0 -> {baseline}, quota=0.5 -> {discovered}")

        Check(novelty.Network.Edges.Length > 0, "the novelty model still produces edges")
        Check(discovered > baseline,
              $"novelty quota introduces data driven regulators ({baseline} -> {discovered} non-prior edges)")
        Check(novelty.Network.Edges.Any(Function(e) e.IsPrior),
              "prior edges are still kept (the quota only reserves part of the slots)")
    End Sub

#End Region

#Region "合成数据"

    ''' <summary>
    ''' 静态路线数据：A 与 F 是两个独立的根调控因子；B 只受 A 激活，
    ''' C 受 A 抑制且受 F 激活，D 受 B 激活且受 C 抑制，E 只受 D 激活。
    ''' </summary>
    ''' <remarks>
    ''' B 与 C 并不完全共线（C 还带一个来自 F 的独立成分），否则
    ''' <c>∂D/∂B</c> 与 <c>∂D/∂C</c> 在数学上不可辨识——这也正是方法文档里
    ''' 「相关不等于因果」那条限制的具体体现。
    ''' </remarks>
    Private Function BuildStaticExpression() As GeneExpressionData
        Dim rng As New Random(20261028)
        Dim samples As Integer = 400
        Dim matrix(geneNames.Length - 1, samples - 1) As Double
        Dim sampleNames(samples - 1) As String

        For s As Integer = 0 To samples - 1
            sampleNames(s) = "s" & s

            Dim a As Double = 1.0 + 0.8 * Normal(rng)
            Dim f As Double = 1.0 + 0.8 * Normal(rng)
            Dim b As Double = 2.0 + 1.8 * std.Tanh((a - 1.0) / 0.8) + 0.12 * Normal(rng)
            Dim c As Double = 2.0 - 1.8 * std.Tanh((a - 1.0) / 0.8) + 1.0 * std.Tanh((f - 1.0) / 0.8) + 0.12 * Normal(rng)
            Dim d As Double = 1.0 + 2.0 * std.Tanh((b - 2.0) / 1.0) - 1.2 * std.Tanh((c - 2.0) / 1.0) + 0.10 * Normal(rng)
            Dim e As Double = 1.5 * std.Tanh((d - 1.0) / 1.2) + 0.10 * Normal(rng)

            matrix(0, s) = a
            matrix(1, s) = b
            matrix(2, s) = c
            matrix(3, s) = d
            matrix(4, s) = e
            matrix(5, s) = f
        Next

        Return New GeneExpressionData With {
            .GeneNames = CType(geneNames.Clone(), String()),
            .SampleNames = sampleNames,
            .Matrix = matrix,
            .TimePoints = Nothing
        }
    End Function

    ''' <summary>
    ''' 时序数据：A 由外部驱动（正弦），其余基因按 <c>x(t+1) = F(x(t))</c> 演化，
    ''' 每个时间点带 4 个生物学重复与观测噪声。
    ''' </summary>
    Private Function BuildTimeSeries() As GeneExpressionData
        Dim rng As New Random(7)
        Dim timePoints As Integer = 16
        Dim replicates As Integer = 4
        Dim samples As Integer = timePoints * replicates
        Dim matrix(geneNames.Length - 1, samples - 1) As Double
        Dim sampleNames(samples - 1) As String
        Dim times(samples - 1) As Double

        Dim a As Double = 3.0
        Dim b As Double = 2.0
        Dim c As Double = 2.0
        Dim d As Double = 1.0
        Dim e As Double = 0.5

        Dim column As Integer = 0

        For t As Integer = 0 To timePoints - 1
            For r As Integer = 0 To replicates - 1
                matrix(0, column) = a + 0.03 * Normal(rng)
                matrix(1, column) = b + 0.03 * Normal(rng)
                matrix(2, column) = c + 0.03 * Normal(rng)
                matrix(3, column) = d + 0.03 * Normal(rng)
                matrix(4, column) = e + 0.03 * Normal(rng)
                matrix(5, column) = 4.0 + 0.2 * Normal(rng)

                sampleNames(column) = $"t{t}_{r}"
                times(column) = t

                column += 1
            Next

            ' 外部驱动的根节点 + 一阶滞后传播
            Dim nextA As Double = 3.0 + 1.5 * std.Sin(0.45 * t)
            Dim nextB As Double = 2.0 + 1.8 * std.Tanh((a - 3.0) / 1.5)
            Dim nextC As Double = 2.0 - 1.8 * std.Tanh((a - 3.0) / 1.5)
            Dim nextD As Double = 1.0 + 2.0 * std.Tanh((b - 2.0) / 1.0) - 1.2 * std.Tanh((c - 2.0) / 1.0)
            Dim nextE As Double = 1.5 * std.Tanh((d - 1.0) / 1.2)

            a = nextA
            b = nextB
            c = nextC
            d = nextD
            e = nextE
        Next

        Return New GeneExpressionData With {
            .GeneNames = CType(geneNames.Clone(), String()),
            .SampleNames = sampleNames,
            .Matrix = matrix,
            .TimePoints = times
        }
    End Function

    ''' <summary>先验网络：只包含真实调控边（A→B+、A→C−、F→C+、B→D+、C→D−、D→E+）。</summary>
    Private Function BuildPrior() As PriorNetwork
        Dim net As New PriorNetwork()

        Call net.AddEdge("A", "B", Effector.Activator, 0.95, "synthetic")
        Call net.AddEdge("A", "C", Effector.Inhibitor, 0.9, "synthetic")
        Call net.AddEdge("F", "C", Effector.Activator, 0.75, "synthetic")
        Call net.AddEdge("B", "D", Effector.Activator, 0.85, "synthetic")
        Call net.AddEdge("C", "D", Effector.Inhibitor, 0.85, "synthetic")
        Call net.AddEdge("D", "E", Effector.Activator, 0.8, "synthetic")

        Return net
    End Function

    Private Function Normal(rng As Random) As Double
        Dim u1 As Double = 1.0 - rng.NextDouble()
        Dim u2 As Double = rng.NextDouble()

        Return std.Sqrt(-2.0 * std.Log(u1)) * std.Cos(2.0 * std.PI * u2)
    End Function

#End Region

#Region "断言助手"

    Private Sub Check(condition As Boolean, message As String)
        If condition Then
            passes += 1
            Console.WriteLine("  [ok]   " & message)
        Else
            failures += 1
            Console.WriteLine("  [FAIL] " & message)
        End If
    End Sub

    Private Function FindEdge(network As FuzzyGeneNetwork, source As String, target As String) As RegulationEdgeModel
        For Each edge As RegulationEdgeModel In network.Edges
            If String.Equals(edge.Source, source, StringComparison.OrdinalIgnoreCase) AndAlso
               String.Equals(edge.Target, target, StringComparison.OrdinalIgnoreCase) Then
                Return edge
            End If
        Next

        Return Nothing
    End Function

    Private Function IndexOfGene(network As FuzzyGeneNetwork, gene As String) As Integer
        Return network.IndexOfGene(gene)
    End Function

    Private Function Delta(result As InterventionResult, gene As String) As Double
        Dim idx As Integer = Array.FindIndex(result.GeneNames, Function(g) String.Equals(g, gene, StringComparison.OrdinalIgnoreCase))

        If idx < 0 Then
            Return Double.NaN
        End If

        Return result.FoldChanges(idx)
    End Function

    Private Function IsFinite(value As Double) As Boolean
        Return Not (Double.IsNaN(value) OrElse Double.IsInfinity(value))
    End Function

#End Region

End Module
