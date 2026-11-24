Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Text.Json
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.TSK
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.BioSystem.Core
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.genomics.Analysis.FuzzyGene
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.FuzzyGene.Fitting
Imports SMRUCC.genomics.Analysis.FuzzyGene.Hybrid
Imports SMRUCC.genomics.Analysis.FuzzyGene.IO
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation.Analysis
Imports SMRUCC.genomics.Analysis.FuzzyGene.Solver
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
        If args.Length > 0 AndAlso String.Equals(args(0), "--bench-steady", StringComparison.OrdinalIgnoreCase) Then
            Return RunSteadyStateBenchmark(args)
        End If
        If args.Length > 0 AndAlso String.Equals(args(0), "--trace-model", StringComparison.OrdinalIgnoreCase) Then
            Return RunTraceModel(args)
        End If
        If args.Length > 0 AndAlso String.Equals(args(0), "--analysis", StringComparison.OrdinalIgnoreCase) Then
            Return RunAnalysis(args)
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
        TestScatter(staticData, prior)
        TestSolverAndHybrid(staticData, prior)

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

#Region "稳态迭代基准：--bench-steady [blockGenes] [tiles] [rows] [steps]"

    ''' <summary>
    ''' 稳态迭代基准：验证「并行」与「串行」输出逐位一致，并给出加速比与分配量。
    ''' </summary>
    ''' <remarks>
    ''' 用法：<c>dotnet run -c Debug -p:Platform=x64 -- --bench-steady 64 48 400 50</c>
    ''' （依次为种子基因数、平铺倍数、训练行数、迭代步数；均可省略）
    ''' <para>
    ''' 只训练一个小规模「种子」网络，然后平铺成上万基因的块对角网络：
    ''' 稳态迭代的耗时只与基因数、每基因的规则数有关，平铺可以在秒级完成
    ''' 却保留与真实网络同量级的单基因推理成本。
    ''' </para>
    ''' </remarks>
    Private Function RunSteadyStateBenchmark(args As String()) As Integer
        Dim blockGenes As Integer = If(args.Length > 1, CInt(args(1)), 64)
        Dim tiles As Integer = If(args.Length > 2, CInt(args(2)), 48)
        Dim rows As Integer = If(args.Length > 3, CInt(args(3)), 400)
        Dim steps As Integer = If(args.Length > 4, CInt(args(4)), 50)
        Dim degree As Integer = If(args.Length > 5, CInt(args(5)), 0)

        Call DebugAudit.Reset()

        Console.WriteLine("=== FuzzyGene steady-state benchmark ===")
        Console.WriteLine($"seed block: {blockGenes} genes x {rows} samples, tiled x{tiles}, steps = {steps}")
        Console.WriteLine($"logical cores: {Environment.ProcessorCount}, parallel degree: {If(degree > 0, degree, Environment.ProcessorCount)}")

        ' 先用一段纯 CPU 循环标定本机实际能拿到的并行加速比：
        ' 若这里也接近 1x，说明是运行环境的 CPU 配额/亲和性限制，不是稳态迭代代码的问题
        For Each probe As Integer In New Integer() {1, 2, 4, Environment.ProcessorCount}.Distinct()
            Console.WriteLine($"raw CPU scaling @ {probe,2} worker(s): {MeasureParallelScaling(probe):F2}x")
        Next

        Console.WriteLine()

        Call DebugAudit.Mark("bench.degree", If(degree > 0, degree, Environment.ProcessorCount))

        ' ==================== 1. 训练种子网络 ====================
        Dim data As GeneExpressionData = BuildBenchmarkMatrix(blockGenes, rows)
        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.CorrelationOnly,
            .MaxRegulators = 3,
            .MaxCandidateGenes = 0,
            .MfCount = 2,
            .MaxEpoch = 20,
            .Standardize = True,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = steps,
            .DegreeOfParallelism = 1
        }

        Dim seedModel As New FuzzyGeneModel(data, Nothing, config)
        Dim report As FuzzyGeneTrainingResult = seedModel.Train()

        Console.WriteLine($"seed: {report.NTrained}/{blockGenes} targets fitted, mean R2 = {report.MeanR2:F3}")
        Console.WriteLine()

        ' ==================== 2. 平铺成大规模网络 ====================
        Dim net As FuzzyGeneNetwork = TileNetwork(seedModel.Network, tiles)

        Call DebugAudit.Mark("bench.genes", net.NGene)
        Call DebugAudit.Mark("bench.modelled", net.NModelled)
        Call DebugAudit.Mark("bench.edges", net.Edges.Length)
        Call DebugAudit.Mark("bench.steps", steps)
        Call DebugAudit.Mark("bench.cores", Environment.ProcessorCount)

        Console.WriteLine($"benchmark network: {net.NGene} genes, {net.NModelled} modelled, " &
                          $"avg rules = {If(net.NModelled > 0, net.Models.Where(Function(m) m IsNot Nothing).Average(Function(m) m.RuleCount), 0):F1}")
        Console.WriteLine()

        ' 扰动一个基因：同时覆盖「首步全量重算」与后续脏集传播
        Dim target As String = net.GeneNames(0)
        Dim clamps As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
            {target, net.BaselineRaw(0) * 0.2}
        }

        ' ==================== 3. 预热 ====================
        ' JIT、线程池、线程局部缓冲与规则缓存的首次开销都不应计入任何一侧的计时窗口
        net.Config.DegreeOfParallelism = 1
        Call net.SteadyState(net.BaselineRaw, clamps, 1, 0.5, 0.000001)
        net.Config.DegreeOfParallelism = 0
        Call net.SteadyState(net.BaselineRaw, clamps, 1, 0.5, 0.000001)

        ' ==================== 4. 串行基准（取 3 次最优）====================
        Dim serialRun = MeasureSteady(net, clamps, steps, 1, 3)
        Dim parallelRun = MeasureSteady(net, clamps, steps, degree, 3)

        Dim serialResult As Double() = serialRun.result
        Dim serialMs As Double = serialRun.ms
        Dim serialAlloc As Long = serialRun.alloc
        Dim serialGen2 As Integer = serialRun.gen2
        Dim parallelResult As Double() = parallelRun.result
        Dim parallelMs As Double = parallelRun.ms
        Dim parallelAlloc As Long = parallelRun.alloc
        Dim parallelGen2 As Integer = parallelRun.gen2

        ' ==================== 5. 等价性：必须逐位相同 ====================
        Dim mismatch As Integer = 0
        Dim maxDiff As Double = 0.0

        For i As Integer = 0 To net.NGene - 1
            If serialResult(i) <> parallelResult(i) Then
                mismatch += 1
                maxDiff = std.Max(maxDiff, std.Abs(serialResult(i) - parallelResult(i)))
            End If
        Next

        Call DebugAudit.Test(mismatch = 0,
                             $"parallel steady state must be bit-identical to the serial one ({mismatch} value(s) differ, max diff {maxDiff:G3})")
        Call DebugAudit.Test(IsFinite(serialResult(0)), "the steady state stays finite under clamping")

        ' ==================== 6. 混合循环占比（决定是否值得 SIMD 化）====================
        Dim blendMs As Double = MeasureBlendLoop(net.NGene, steps, 0.5)
        Dim blendShare As Double = If(serialMs > 0.0, blendMs / serialMs * 100.0, 0.0)
        Dim speedup As Double = If(parallelMs > 0.0, serialMs / parallelMs, 0.0)

        ' 诊断：只看「逐基因推理」这一层能不能并行（不经稳态编排，用来定位串行化来源）
        Dim geneLoop = MeasureGeneForwardScaling(net, degree)
        Dim sharedLoop = MeasureGeneForwardScaling(TileNetwork(seedModel.Network, std.Max(1, tiles), shareNetworkPerSeed:=True), degree)

        Call DebugAudit.Mark("forward.serial.ms", geneLoop.serialMs)
        Call DebugAudit.Mark("forward.parallel.ms", geneLoop.parallelMs)
        Call DebugAudit.Mark("forward.speedup", geneLoop.speedup)
        Call DebugAudit.Mark("forward.shared.speedup", sharedLoop.speedup)

        Call DebugAudit.Mark("steady.serial.ms", serialMs)
        Call DebugAudit.Mark("steady.parallel.ms", parallelMs)
        Call DebugAudit.Mark("steady.speedup", speedup)
        Call DebugAudit.Mark("steady.blend.ms", blendMs)
        Call DebugAudit.Mark("steady.blend.share.pct", blendShare)

        Console.WriteLine("mode      | wall(ms) | alloc(MB) | Gen2 | speedup")
        Console.WriteLine("----------+----------+-----------+------+--------")
        Console.WriteLine($"serial    | {serialMs,8:F1} | {serialAlloc / 1024.0 / 1024,9:F2} | {serialGen2,4} |    1.00x")
        Console.WriteLine($"parallel  | {parallelMs,8:F1} | {parallelAlloc / 1024.0 / 1024,9:F2} | {parallelGen2,4} | {speedup,7:F2}x")
        Console.WriteLine()
        Console.WriteLine($"blend loop alone: {blendMs:F1} ms ({blendShare:F2}% of the serial run)")
        Console.WriteLine($"gene forward only: serial {geneLoop.serialMs:F1} ms vs parallel {geneLoop.parallelMs:F1} ms " &
                          $"({geneLoop.speedup:F2}x); shared-network variant: {sharedLoop.speedup:F2}x")
        Console.WriteLine($"bit-identical: {mismatch = 0} ({mismatch} mismatch(es))")
        Console.WriteLine()

        If blendShare > 5.0 Then
            Console.WriteLine("=> the blend loop is a non-trivial share; SIMD-izing BlendRange is worth trying.")
        Else
            Console.WriteLine("=> the blend loop is negligible; gene-level parallelism is the right lever (skip SIMD).")
        End If

        Console.WriteLine()
        Console.WriteLine(DebugAudit.Dump())

        Dim reportPath As String = Path.Combine(
            Path.GetTempPath(),
            $"fuzzygene-steady-bench-{DateTime.Now:yyyyMMdd-HHmmss}.json")

        Call DebugAudit.SaveReport(reportPath)

        Console.WriteLine($"report: {reportPath}")

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

    ''' <summary>
    ''' 把小规模「种子」网络平铺成大规模块对角网络（每块内部拓扑与种子一致）。
    ''' </summary>
    ''' <remarks>
    ''' 训练上万个靶基因要几十分钟，而稳态迭代的耗时只与基因数、每基因的规则数有关。
    ''' 平铺既保留了「逐靶基因一个模型 + 一个 TSK 网络」的生产形态（每个基因都是独立克隆，
    ''' 因此并行阶段不存在共享实例上的伪竞争），又能让基准在秒级完成。
    ''' </remarks>
    Private Function TileNetwork(seed As FuzzyGeneNetwork,
                                 tiles As Integer,
                                 Optional shareNetworkPerSeed As Boolean = False) As FuzzyGeneNetwork
        Dim count As Integer = std.Max(1, tiles)
        Dim block As Integer = seed.NGene
        Dim n As Integer = block * count
        Dim names(n - 1) As String
        Dim models(n - 1) As GeneRegulationModel
        Dim means(n - 1) As Double
        Dim sds(n - 1) As Double
        Dim baseline(n - 1) As Double
        Dim supportMin(n - 1) As Double
        Dim supportMax(n - 1) As Double
        ' 诊断用：每个种子基因只保留一份 TskNetwork，块与块之间共享
        Dim sharedNetworks(block - 1) As TskNetwork

        For t As Integer = 0 To count - 1
            For local As Integer = 0 To block - 1
                Dim g As Integer = t * block + local
                Dim source As GeneRegulationModel = seed.Models(local)

                names(g) = $"{seed.GeneNames(local)}_{t}"
                means(g) = seed.Means(local)
                sds(g) = seed.SDs(local)
                baseline(g) = seed.BaselineRaw(local)
                supportMin(g) = seed.SupportMin(local)
                supportMax(g) = seed.SupportMax(local)

                If source Is Nothing OrElse source.Network Is Nothing OrElse source.Regulators Is Nothing Then
                    Continue For
                End If

                Dim regulators(source.Regulators.Length - 1) As String

                For j As Integer = 0 To source.Regulators.Length - 1
                    ' 种子里的调控因子名（如 "G12"）重映射到本块内的同位置基因（"G12_3"）
                    Dim sourceIndex As Integer = seed.IndexOfGene(source.Regulators(j))

                    regulators(j) = If(sourceIndex < 0,
                                       source.Regulators(j),
                                       $"{seed.GeneNames(sourceIndex)}_{t}")
                Next

                Dim network As TskNetwork

                If shareNetworkPerSeed Then
                    If sharedNetworks(local) Is Nothing Then
                        sharedNetworks(local) = source.Network
                    End If

                    network = sharedNetworks(local)
                Else
                    network = source.Network.Clone()
                End If

                models(g) = New GeneRegulationModel With {
                    .TargetGene = names(g),
                    .Regulators = regulators,
                    .Network = network,
                    .R2 = source.R2,
                    .RMSE = source.RMSE,
                    .Iterations = source.Iterations,
                    .Converged = source.Converged,
                    .Mean = source.Mean,
                    .Sd = source.Sd,
                    .SupportMin = source.SupportMin,
                    .SupportMax = source.SupportMax,
                    .IsLagged = source.IsLagged,
                    .SampleCount = source.SampleCount
                }
            Next
        Next

        Dim tiledConfig As New FuzzyGeneConfig With {
            .PropagationSteps = seed.Config.PropagationSteps,
            .Relaxation = seed.Config.Relaxation,
            .ConvergenceTolerance = seed.Config.ConvergenceTolerance,
            .ClampToSupport = seed.Config.ClampToSupport
        }

        Return New FuzzyGeneNetwork(names, models, New RegulationEdgeModel() {}, tiledConfig,
                                    means, sds, baseline, supportMin, supportMax,
                                    seed.Mode, seed.Standardized)
    End Function

    ''' <summary>
    ''' 单独计时「与 BlendRange 同构」的混合扫描，用于估算它在端到端耗时里的占比。
    ''' </summary>
    ''' <remarks>
    ''' 稳态循环的提前退出会让「只跑混合」的对照组在第一步就收敛，
    ''' 因此这里复制同一段逐元素运算单独计时，作为「SIMD 化是否值得」的判据。
    ''' </remarks>
    Private Function MeasureBlendLoop(n As Integer, steps As Integer, omega As Double) As Double
        Dim z(n - 1) As Double
        Dim nextZ(n - 1) As Double
        Dim dirty(n - 1) As Boolean

        For i As Integer = 0 To n - 1
            z(i) = std.Sin(i * 0.001)
            nextZ(i) = std.Cos(i * 0.001)
        Next

        Dim clock As Stopwatch = Stopwatch.StartNew()

        For stepIndex As Integer = 0 To std.Max(0, steps - 1)
            Dim maxDelta As Double = 0.0

            For i As Integer = 0 To n - 1
                Dim blended As Double = z(i) + omega * (nextZ(i) - z(i))
                Dim delta As Double = std.Abs(blended - z(i))

                z(i) = blended
                dirty(i) = delta > 0.000000000001

                If delta > maxDelta Then
                    maxDelta = delta
                End If
            Next
        Next

        clock.Stop()

        Return clock.Elapsed.TotalMilliseconds
    End Function

    ''' <summary>
    ''' 只对「逐基因推理」这一层做串行/并行对照，用于定位稳态迭代里的串行化来源。
    ''' </summary>
    ''' <remarks>
    ''' 行向量按调控因子顺序预先拼好，循环体内只调用 <see cref="GeneRegulationModel.Predict"/>，
    ''' 因此这个数字反映的是「每基因一次 TSK 前向」能否并行，
    ''' 与稳态编排（双缓冲、钳制、收敛判定）无关。
    ''' </remarks>
    Private Function MeasureGeneForwardScaling(net As FuzzyGeneNetwork,
                                               degree As Integer) As (serialMs As Double, parallelMs As Double, speedup As Double)
        Dim workers As Integer = If(degree > 0, degree, Environment.ProcessorCount)
        Dim n As Integer = net.NGene
        Dim rows As Double()() = New Double(n - 1)() {}
        Dim sink(n - 1) As Double

        For g As Integer = 0 To n - 1
            Dim model As GeneRegulationModel = net.Models(g)

            If model Is Nothing OrElse model.Regulators Is Nothing Then
                rows(g) = New Double() {}
                Continue For
            End If

            Dim row(model.Regulators.Length - 1) As Double

            For j As Integer = 0 To row.Length - 1
                Dim idx As Integer = net.IndexOfGene(model.Regulators(j))

                row(j) = If(idx < 0, 0.0, net.BaselineRaw(idx))
            Next

            rows(g) = row
        Next

        ' 预热：让每个线程的标量缓冲先创建好
        Call RunGeneForward(net, rows, sink, 1, 1)

        Dim serialMs As Double = RunGeneForward(net, rows, sink, 1, 2)
        Dim parallelMs As Double = RunGeneForward(net, rows, sink, workers, 2)

        Return (serialMs, parallelMs, If(parallelMs > 0.0, serialMs / parallelMs, 0.0))
    End Function

    ''' <summary>按给定并行度跑一遍全基因推理，返回最快一次的毫秒数。</summary>
    Private Function RunGeneForward(net As FuzzyGeneNetwork, rows As Double()(), sink As Double(),
                                    workers As Integer, repeats As Integer) As Double
        Dim n As Integer = net.NGene
        Dim best As Double = Double.MaxValue

        For repeat As Integer = 1 To std.Max(1, repeats)
            Dim clock As Stopwatch = Stopwatch.StartNew()

            If workers <= 1 Then
                For g As Integer = 0 To n - 1
                    Dim model As GeneRegulationModel = net.Models(g)

                    If model IsNot Nothing Then
                        sink(g) = model.Predict(rows(g))
                    End If
                Next
            Else
                System.Threading.Tasks.Parallel.For(0, n,
                    New System.Threading.Tasks.ParallelOptions With {.MaxDegreeOfParallelism = workers},
                    Sub(g)
                        Dim model As GeneRegulationModel = net.Models(g)

                        If model IsNot Nothing Then
                            sink(g) = model.Predict(rows(g))
                        End If
                    End Sub)
            End If

            clock.Stop()

            If clock.Elapsed.TotalMilliseconds < best Then
                best = clock.Elapsed.TotalMilliseconds
            End If
        Next

        Return best
    End Function

    ''' <summary>
    ''' 用一段纯 CPU 循环标定本机实际的并行加速比（环境是否真的能跑满多核）。
    ''' </summary>
    ''' <remarks>
    ''' 容器 / 沙箱环境经常出现「报告 24 个逻辑核，但 CPU 配额只有 1 核」的情况，
    ''' 这时任何并行改造都不可能拿到加速。先量出这个基线，才能判断
    ''' 稳态迭代的加速比偏低是环境问题还是代码问题。
    ''' </remarks>
    Private Function MeasureParallelScaling(workers As Integer) As Double
        workers = std.Max(1, workers)

        Dim n As Integer = 4000000
        Dim v(n - 1) As Double

        For i As Integer = 0 To n - 1
            v(i) = i * 0.000001
        Next

        Dim sink As Double = 0.0
        Dim clock As Stopwatch = Stopwatch.StartNew()

        For i As Integer = 0 To n - 1
            sink += std.Sqrt(v(i))
        Next

        clock.Stop()

        Dim serialMs As Double = clock.Elapsed.TotalMilliseconds

        If workers = 1 Then
            Return 1.0
        End If

        Dim partials(workers - 1) As Double
        Dim chunk As Integer = (n + workers - 1) \ workers

        clock.Restart()

        System.Threading.Tasks.Parallel.For(0, workers,
            New System.Threading.Tasks.ParallelOptions With {.MaxDegreeOfParallelism = workers},
            Sub(c)
                Dim start As Integer = c * chunk
                Dim ends As Integer = std.Min(start + chunk, n)
                Dim acc As Double = 0.0

                For i As Integer = start To ends - 1
                    acc += std.Sqrt(v(i))
                Next

                partials(c) = acc
            End Sub)

        clock.Stop()

        Dim parallelMs As Double = clock.Elapsed.TotalMilliseconds
        Dim total As Double = 0.0

        For i As Integer = 0 To workers - 1
            total += partials(i)
        Next

        If total < sink Then
            sink = total
        End If

        Return If(parallelMs > 0.0, serialMs / parallelMs, 0.0)
    End Function

    ''' <summary>把 GC 基线拉平，避免把上一阶段的数据生成垃圾算进计时窗口。</summary>
    Private Sub CollectGarbage()
        Call GC.Collect()
        Call GC.WaitForPendingFinalizers()
        Call GC.Collect()
    End Sub

    ''' <summary>
    ''' 以给定并行度重复运行稳态迭代，返回最快一次的耗时、分配量与运行结果。
    ''' </summary>
    ''' <remarks>
    ''' 取最优而不是平均值：稳态迭代本身没有共享可变状态，重复运行之间的差异
    ''' 来自 JIT 分层编译与线程池调度，取最优更接近「稳定态」的真实成本。
    ''' </remarks>
    Private Function MeasureSteady(net As FuzzyGeneNetwork,
                                   clamps As Dictionary(Of String, Double),
                                   steps As Integer,
                                   degree As Integer,
                                   repeats As Integer) As (ms As Double, alloc As Long, gen2 As Integer, result As Double())
        net.Config.DegreeOfParallelism = degree

        Dim bestMs As Double = Double.MaxValue
        Dim bestAlloc As Long = 0
        Dim bestGen2 As Integer = 0
        Dim result As Double() = Nothing

        For repeat As Integer = 1 To std.Max(1, repeats)
            Call CollectGarbage()

            Dim allocBefore As Long = GC.GetTotalAllocatedBytes(precise:=True)
            Dim gen2Before As Integer = GC.CollectionCount(2)
            Dim clock As Stopwatch = Stopwatch.StartNew()

            result = net.SteadyState(net.BaselineRaw, clamps, steps, 0.5, 0.000001)

            clock.Stop()

            If clock.Elapsed.TotalMilliseconds < bestMs Then
                bestMs = clock.Elapsed.TotalMilliseconds
                bestAlloc = GC.GetTotalAllocatedBytes(precise:=True) - allocBefore
                bestGen2 = GC.CollectionCount(2) - gen2Before
            End If
        Next

        Return (bestMs, bestAlloc, bestGen2, result)
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

            Dim coverage As ContextAlignmentDiagnostics = model.LoadContext(contextFile)

            Console.WriteLine(coverage.ToString())

            Check(coverage.Matched = net.NGene, $"every model gene is covered (actual {coverage.Matched})")
            Check(std.Abs(coverage.Coverage - 1.0) < 1.0E-09, "coverage reaches 100%")
            Check(coverage.Notes.Any(Function(n) n.Contains("gene_name")), "the gene column is detected")
            Check(coverage.InvalidValues = 1, $"the NA row is skipped instead of becoming zero (actual {coverage.InvalidValues})")
            Check(model.IsContextSet, "the model reports the context as applied")
            Check(coverage.Aligned, "the context is aligned through the public component")
            Check(std.Abs(model.Context(indexA) - contextA) > 1.0E-09,
                  "quantile alignment maps the external value onto the model training scale")

            ' ---- 部分覆盖：未给出的基因继续沿用训练基线 ----
            Dim partialInput As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {{"A", contextA}}

            Call model.SetContext(partialInput)

            Dim partialContext As ContextAlignmentDiagnostics = model.ContextDiagnostics

            Check(partialContext.Matched = 1 AndAlso partialContext.MissingGenes.Length = net.NGene - 1,
                  $"a partial context keeps the remaining genes on the training baseline ({partialContext.Matched}/{net.NGene})")
            Check(std.Abs(partialContext.Coverage - 1.0 / net.NGene) < 1.0E-09, "partial coverage is reported correctly")
            Check(std.Abs(model.Context(indexA) - contextA) < 1.0E-09,
                  "a single-gene context is injected verbatim (cross-gene quantile is undefined)")

            ' ---- 覆盖率门槛 ----
            Dim rejected As Boolean = False

            Try
                model.MinContextCoverage = 0.9
                Call model.SetContext(partialInput)
            Catch ex As ArgumentException
                rejected = True
            Finally
                model.MinContextCoverage = 0
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

            ' ---- 迭代扰动：上游突变体稳态 → 下一轮的对照基线 ----
            Dim round1 As InterventionResult = model.KnockoutGene("A")

            Check(round1 IsNot Nothing AndAlso Not round1.Undefined, "the first perturbation round produces a defined result")

            Dim chained As ContextAlignmentDiagnostics = model.LoadContext(round1)

            Check(model.IsContextSet, "the upstream result is injected as the context")
            Check(chained.Matched = net.NGene, $"the upstream context covers every model gene (actual {chained.Matched})")

            Dim exact As Boolean = True

            For i As Integer = 0 To net.NGene - 1
                If std.Abs(model.Context(i) - round1.MutantMeans(i)) > 1.0E-12 Then
                    exact = False
                End If
            Next

            Check(exact, "the injected context equals the upstream mutant state")

            Dim round2 As InterventionResult = model.KnockoutGene("B")

            Check(round2 IsNot Nothing AndAlso round2.FoldChanges.Length = net.NGene,
                  "the chained round still returns a full result")

            ' 第 2 轮的对照态应当更接近上游突变体，而不是原始基线
            Dim toUpstream As Double = 0.0
            Dim toBaseline As Double = 0.0

            For i As Integer = 0 To net.NGene - 1
                toUpstream = std.Max(toUpstream, std.Abs(round2.WildtypeMeans(i) - round1.MutantMeans(i)))
                toBaseline = std.Max(toBaseline, std.Abs(round2.WildtypeMeans(i) - baselineRun.WildtypeMeans(i)))
            Next

            Check(toUpstream <= toBaseline,
                  $"the chained control state moves towards the upstream mutant (dUp={toUpstream:G3} vs dBase={toBaseline:G3})")

            ' ---- 未定义的上游结果必须被拒绝，而不是被静默应用 ----
            Dim rejectedUpstream As Boolean = False

            Try
                Call model.LoadContext(New InterventionResult With {.Undefined = True})
            Catch ex As InvalidOperationException
                rejectedUpstream = True
            End Try

            Check(rejectedUpstream, "an undefined upstream result is rejected")

            Call model.ResetContext()
        Finally
            If File.Exists(contextFile) Then
                Call File.Delete(contextFile)
            End If
        End Try
    End Sub

#End Region

#Region "场景 7：散射划分（scatter partitioning）"

    Private Sub TestScatter(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 7: scatter partitioning ---")

        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.CorrelationOnly,
            .MaxRegulators = 5,
            .MfCount = 2,
            .MaxEpoch = 15,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .RulePartition = RulePartitionKind.Scatter,
            .ScatterRuleCount = 8,
            .ScatterInputs = 4,
            .ScatterMinInputs = 2,
            .DegreeOfParallelism = 1
        }

        Dim model As New FuzzyGeneModel(data, Nothing, config)
        Dim report As FuzzyGeneTrainingResult = model.Train()

        Console.WriteLine(report.ToString())

        Dim fitted As GeneFitMetric() = report.GeneMetrics _
            .Where(Function(m) m IsNot Nothing AndAlso String.Equals(m.Status, "ok", StringComparison.OrdinalIgnoreCase)) _
            .ToArray()

        Check(fitted.Length > 0, $"the scatter route fits at least one gene (actual {fitted.Length})")
        Check(fitted.All(Function(m) String.Equals(m.Partition, "scatter", StringComparison.OrdinalIgnoreCase)),
              "every fitted gene reports the scatter partition")

        Dim probe As GeneRegulationModel = model.Network.Models _
            .First(Function(m) m IsNot Nothing AndAlso m.Network IsNot Nothing)
        Dim net As TskNetwork = probe.Network

        Check(net.Rules.IsScatter, "the rule base is a scatter rule base")
        Check(net.Rules.RuleCount <= config.ScatterRuleCount,
              $"the rule count never exceeds the cluster count ({net.Rules.RuleCount} <= {config.ScatterRuleCount})")
        Check(net.PremiseParameterCount = net.Rules.RuleCount * net.InputCount * 2,
              $"the premise vector holds (center, sigma) per rule and input ({net.PremiseParameterCount})")

        Dim activeInputs As Integer() = net.Rules.ActiveInputs(0)

        Check(activeInputs.Length >= config.ScatterMinInputs AndAlso activeInputs.Length <= config.ScatterInputs,
              $"the antecedent count is adaptive within [{config.ScatterMinInputs}, {config.ScatterInputs}] (actual {activeInputs.Length})")

        ' ---- 前向：标量与批量共用同一个散射分支 ----
        Dim row(net.InputCount - 1) As Double

        For j As Integer = 0 To row.Length - 1
            row(j) = 0.1 * (j + 1)
        Next

        Dim scalar As Double = net.PredictScalar(row)
        Dim batch As Double()() = net.Predict(New Double()() {row})

        Check(Not Double.IsNaN(scalar) AndAlso Not Double.IsInfinity(scalar), "the scatter forward produces a finite output")
        Check(std.Abs(scalar - batch(0)(0)) < 1.0E-12,
              $"scalar and batch inference agree on the scatter model ({scalar:G6})")

        ' ---- 规则文本：散射前件必须渲染成 N(center, sigma)，不能出现越界的 mf 标签 ----
        Dim text As String() = probe.RulesText()

        Check(text.Length = net.Rules.RuleCount, $"every rule is rendered ({text.Length})")
        Check(text(0).Contains("N("), $"scatter rules describe gaussian antecedents ({text(0)})")

        ' ---- 前提参数往返：写入再读出必须逐位一致（含 don't care 输入） ----
        Dim premise As Double() = net.GetPremiseParameters()
        Dim probeValues As Double() = CType(premise.Clone(), Double())

        For k As Integer = 0 To probeValues.Length - 1
            probeValues(k) += 0.01 * (k Mod 3 - 1)
        Next

        Call net.SetPremiseParameters(probeValues)

        Dim roundTrip As Double() = net.GetPremiseParameters()
        Dim identical As Boolean = True

        For k As Integer = 0 To premise.Length - 1
            If std.Abs(roundTrip(k) - probeValues(k)) > 1.0E-12 Then
                identical = False
            End If
        Next

        Check(identical, "the premise vector round-trips through the scatter layout")

        Call net.SetPremiseParameters(premise)

        ' ---- DTO 往返 ----
        Dim restored As TskNetwork = TskNetwork.FromModelDto(net.ToJsonDto())

        Check(restored.Rules.IsScatter, "the restored model keeps the scatter partition")
        Check(std.Abs(restored.PredictScalar(row) - scalar) < 1.0E-12, "the restored scatter model predicts identically")

        ' ---- 包往返 ----
        Dim package As String = Path.Combine(Path.GetTempPath(), "fuzzygene-scatter-" & Guid.NewGuid().ToString("N") & FuzzyGeneFile.PackageExtension)

        Try
            Call model.Network.Save(package)

            Dim loaded As FuzzyGeneNetwork = FuzzyGeneFile.Load(package)
            Dim geneIndex As Integer = Array.IndexOf(model.Network.Models, probe)

            Check(loaded.NGene = model.Network.NGene, "the scatter package reloads the full gene list")
            Check(std.Abs(loaded.Models(geneIndex).Network.PredictScalar(row) - scalar) < 1.0E-12,
                  "the reloaded scatter package predicts identically")
        Finally
            If File.Exists(package) Then
                Call File.Delete(package)
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

#Region "求解器诊断与三层融合"

    ''' <summary>
    ''' 收敛判据、增益校准与 PBN / FCM 融合层的回归测试。
    ''' </summary>
    ''' <remarks>
    ''' 这一组针对的是本轮修复的核心：
    ''' ① 钳制基因的约束违背量不再污染收敛判据；
    ''' ② 野生型参考是真正的不动点；
    ''' ③ 增益校准只作用于调控环、且能真的把谱半径压到 1-ε 以下；
    ''' ④ PBN 正则 / 采样与 FCM 热启动都不破坏原有的扰动路径。
    ''' </remarks>
    Private Sub TestSolverAndHybrid(data As GeneExpressionData, prior As PriorNetwork)
        Console.WriteLine()
        Console.WriteLine("--- scenario 8: solver diagnostics, calibration and hybrid layers ---")

        Dim config As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorOnly,
            .MaxRegulators = 3,
            .MfCount = 3,
            .MaxEpoch = 40,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 40,
            .Relaxation = 0.5,
            .PruneRuleThreshold = 0.0,
            .DegreeOfParallelism = 1
        }

        Dim model As New FuzzyGeneModel(data, prior, config)

        Call model.Train()

        ' ---------- 8.1 收敛判据：钳制基因不参与残差 ----------
        Dim trace As PerturbationTrace = model.TracePerturbation("A", InterventionMode.Knockout, config.PropagationSteps)
        Dim mutant As SteadyStateTrace = trace.Mutant

        Call AuditTest(mutant IsNot Nothing, "solver: the knockout trace is produced")

        If mutant Is Nothing Then
            Return
        End If

        Call AuditTest(mutant.ExitReason = ExitReason.Converged,
                       $"solver: a knockout converges with the Newton solver (actual {mutant.ExitReason})")
        Call AuditTest(mutant.FreeResidual <= config.ConvergenceTolerance,
                       $"solver: the free residual is below the tolerance (actual {mutant.FreeResidual:G3})")
        ' 目标基因 A 没有拟合模型（PriorOnly 下它是根），因此它的 F 是恒等映射、违背量为 0；
        ' 换一个有入边的基因才能看出「约束违背量」这一诊断确实在统计
        Dim hubTrace As PerturbationTrace = model.TracePerturbation("D", InterventionMode.Knockout, config.PropagationSteps)

        Call AuditTest(hubTrace.Mutant IsNot Nothing AndAlso hubTrace.Mutant.ClampResidual > 0.0,
                       $"solver: the constraint violation is reported separately (actual {If(hubTrace.Mutant Is Nothing, 0, hubTrace.Mutant.ClampResidual):G3})")
        Call AuditTest(hubTrace.Mutant IsNot Nothing AndAlso hubTrace.Mutant.ExitReason = ExitReason.Converged,
                       "solver: a knockout on a modelled gene also converges")
        Call AuditTest(mutant.Iterations < config.PropagationSteps,
                       $"solver: convergence is not masked by the step budget ({mutant.Iterations}/{config.PropagationSteps})")

        ' 旧实现里这一条必然失败：maxDelta 恒被 ω|F(clamp)-clamp| 顶在高位
        Call AuditTest(mutant.Converged, "solver: Converged no longer depends on the clamped gene")

        ' ---------- 8.2 Predict 与 Trace 逐位一致 ----------
        Dim plain As InterventionResult = model.KnockoutGene("A")

        Call AuditTest(SameArray(plain.FoldChanges, trace.Result.FoldChanges),
                       "solver: Predict() and TracePerturbation() are bit-identical")

        ' ---------- 8.3 野生型参考是不动点 ----------
        Call AuditTest(trace.Control IsNot Nothing AndAlso trace.Control.ExitReason = ExitReason.Converged,
                       "solver: the control trajectory also converges")

        Dim controlDrift As Double = 0.0

        If trace.Control IsNot Nothing Then
            For g As Integer = 0 To model.Network.NGene - 1
                controlDrift = std.Max(controlDrift, std.Abs(trace.Control.Final(g) - plain.WildtypeMeans(g)))
            Next
        End If

        Call AuditTest(controlDrift <= config.ConvergenceTolerance * 10.0,
                       $"solver: the reported wild type is the control fixed point (max diff {controlDrift:G3})")

        ' 基线本身（训练均值）与不动点的差就是「模型自漂移」，它现在是可见的
        Dim baselineDrift As Double = 0.0

        If trace.Control IsNot Nothing Then
            For g As Integer = 0 To model.Network.NGene - 1
                baselineDrift = std.Max(baselineDrift, std.Abs(trace.Control.Final(g) - model.Network.BaselineRaw(g)))
            Next
        End If

        Call DebugAudit.Mark("solver.baseline.drift", baselineDrift)

        ' ---------- 8.4 增益校准 ----------
        Dim cyclicConfig As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorFirst,
            .MaxRegulators = 3,
            .MfCount = 3,
            .MaxEpoch = 20,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 40,
            .Relaxation = 0.5,
            .PruneRuleThreshold = 0.0,
            .DegreeOfParallelism = 1
        }

        ' 无环网络（PriorOnly）的 ρ 必须为 0，校准不得改动它——否则会白白牺牲 R²
        Call AuditTest(model.LastTrainingResult.SpectralRadius <= 1.0E-9,
                       $"calibration: an acyclic network has rho = 0 (actual {model.LastTrainingResult.SpectralRadius:G3})")

        cyclicConfig.Hybrid.Calibration = StabilityCalibrationMode.SpectralRadius
        cyclicConfig.Hybrid.CalibrationEpsilon = 0.05

        Dim calibrated As New FuzzyGeneModel(data, prior, cyclicConfig)
        Dim calibratedReport As FuzzyGeneTrainingResult = calibrated.Train()
        Dim target As Double = 1.0 - cyclicConfig.Hybrid.CalibrationEpsilon

        Call DebugAudit.Mark("calibration.rho", calibratedReport.SpectralRadius)
        Call DebugAudit.Mark("calibration.gamma", calibratedReport.CalibrationGamma)
        Call DebugAudit.Mark("calibration.r2.pre", calibratedReport.PreCalibrationR2)
        Call DebugAudit.Mark("calibration.r2.post", calibratedReport.PostCalibrationR2)

        Call AuditTest(calibrated.Network.CalibrationMode = StabilityCalibrationMode.SpectralRadius,
                       "calibration: the mode is recorded on the network")
        Call AuditTest(calibratedReport.PostCalibrationR2 <> 0.0,
                       "calibration: the post-calibration R2 is reported")
        Call AuditTest(calibrated.Network.SpectralRadius <= target + 1.0E-6,
                       $"calibration: rho is pushed below 1-eps (actual {calibrated.Network.SpectralRadius:G3})")
        Call AuditTest(calibratedReport.CalibratedGenes > 0,
                       $"calibration: only the cyclic genes are rescaled (actual {calibratedReport.CalibratedGenes})")

        ' ---------- 8.5 PBN 定性正则 ----------
        Dim pbnConfig As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorFirst,
            .MaxRegulators = 4,
            .MfCount = 3,
            .MaxEpoch = 40,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 40,
            .Relaxation = 0.5,
            .PruneRuleThreshold = 0.0,
            .DegreeOfParallelism = 1
        }

        pbnConfig.Hybrid.PbnRegularization = PbnRegularizationMode.Weighted
        pbnConfig.Hybrid.PbnWeight = 1.0
        pbnConfig.Hybrid.EnsembleSamples = 8
        pbnConfig.Hybrid.ModeBiasWeight = 0.3

        Dim pbnModel As New FuzzyGeneModel(data, prior, pbnConfig)
        Dim pbnReport As FuzzyGeneTrainingResult = pbnModel.Train()

        Call AuditTest(pbnModel.PbnPrior IsNot Nothing, "pbn: the qualitative layer is built")
        Call AuditTest(pbnModel.PbnPrior IsNot Nothing AndAlso pbnModel.PbnPrior.TargetsAnalysed > 0,
                       $"pbn: at least one target gene was analysed (actual {If(pbnModel.PbnPrior Is Nothing, 0, pbnModel.PbnPrior.TargetsAnalysed)})")
        Call AuditTest(pbnModel.Network.Edges.Any(Function(e) e.IsPrior),
                       "pbn: the prior edges survive the regularisation")
        Call AuditTest(pbnReport.NTrained > 0, $"pbn: fitting still succeeds ({pbnReport.NTrained} target gene(s))")

        ' ---------- 8.6 PBN 采样集成 ----------
        Dim knockouts As InterventionSpec() = {
            New InterventionSpec With {.GeneName = "A", .GeneIndex = -1, .Mode = InterventionMode.Knockout}
        }
        Dim ensemble As EnsemblePerturbationResult = pbnModel.PredictEnsemble(knockouts, 8)

        Call AuditTest(ensemble.Samples = 8, $"ensemble: the requested samples are produced (actual {ensemble.Samples})")
        Call AuditTest(ensemble.Mean IsNot Nothing AndAlso ensemble.Mean.Length = pbnModel.Network.NGene,
                       "ensemble: the mean vector matches the gene count")
        Call AuditTest(ensemble.StandardDeviation IsNot Nothing AndAlso ensemble.StandardDeviation.Length = pbnModel.Network.NGene,
                       "ensemble: the standard deviation has one entry per gene")
        Call AuditTest(ensemble.AttractorPeriods.Count > 0,
                       $"ensemble: boolean attractors are reported ({ensemble.AttractorPeriods.Count} distinct period(s))")
        Call AuditTest(Not ensemble.Degenerate, "ensemble: the result is not a degenerate single point")

        ' ---------- 8.7 FCM 热启动 ----------
        Dim fcmConfig As New FuzzyGeneConfig With {
            .Selection = RegulatorSelection.PriorFirst,
            .MaxRegulators = 4,
            .MfCount = 3,
            .MaxEpoch = 40,
            .TargetMode = FuzzyTargetMode.StaticRoute,
            .PropagationSteps = 40,
            .Relaxation = 0.5,
            .PruneRuleThreshold = 0.0,
            .DegreeOfParallelism = 1
        }

        fcmConfig.Hybrid.FcmWarmStart = True
        fcmConfig.Hybrid.FcmModuleCount = 2
        ' 这张网络是环状的（PriorFirst 会补上反函数边），谱半径 > 1；
        ' 不开校准的话牛顿法会在 20 轮内耗尽预算——这正是校准存在的理由
        fcmConfig.Hybrid.Calibration = StabilityCalibrationMode.SpectralRadius

        Dim fcmModel As New FuzzyGeneModel(data, prior, fcmConfig)

        Call fcmModel.Train()

        Call AuditTest(fcmModel.Network.Modules IsNot Nothing, "fcm: the module assignment is stored on the network")
        Call AuditTest(fcmModel.Network.Modules IsNot Nothing AndAlso fcmModel.Network.Modules.Length = fcmModel.Network.NGene,
                       "fcm: every gene carries a module id")

        Dim layer As New FcmModuleLayer(fcmModel.Network)

        Call AuditTest(layer.Available, "fcm: the module layer is available")

        Dim warm As FcmModuleResult = layer.BuildInitial(fcmModel.Network.BaselineRaw)

        Call AuditTest(warm IsNot Nothing AndAlso warm.GeneInitial IsNot Nothing AndAlso warm.GeneInitial.Length = fcmModel.Network.NGene,
                       "fcm: the warm start fills every gene")

        Dim fcmWatch As Stopwatch = Stopwatch.StartNew()
        Dim fcmTrace As PerturbationTrace = fcmModel.TracePerturbation("A", InterventionMode.Knockout, fcmConfig.PropagationSteps)

        fcmWatch.Stop()
        Call DebugAudit.Mark("fcm.trace.seconds", fcmWatch.Elapsed.TotalSeconds)

        Console.WriteLine($"  fcm trace: {fcmWatch.Elapsed.TotalSeconds:F2}s, " &
                          $"exit={If(fcmTrace.Mutant Is Nothing, "none", fcmTrace.Mutant.ExitReason.ToString())}, " &
                          $"free={If(fcmTrace.Mutant Is Nothing, 0, fcmTrace.Mutant.FreeResidual):G3}")

        Call AuditTest(fcmTrace.Mutant IsNot Nothing AndAlso fcmTrace.Mutant.ExitReason = ExitReason.Converged,
                       "fcm: the warm start does not break the perturbation solver")

        ' ---------- 8.8 新元数据的持久化往返 ----------
        Dim package As String = IO.Path.Combine(IO.Path.GetTempPath(), "fuzzygene-hybrid-" & Guid.NewGuid().ToString("N") & ".fuzzygene.zip")

        Try
            Call fcmModel.Save(package)

            Dim restored As FuzzyGeneModel = FuzzyGeneModel.Load(package)

            Call AuditTest(restored.Network.Modules IsNot Nothing AndAlso restored.Network.Modules.Length = fcmModel.Network.NGene,
                           "package: the module assignment survives the round trip")
            Call AuditTest(restored.Network.CalibrationMode = fcmModel.Network.CalibrationMode,
                           "package: the calibration metadata survives the round trip")
            Call AuditTest(restored.TracePerturbation("A", InterventionMode.Knockout, 40).Mutant IsNot Nothing,
                           "package: the restored model still runs perturbations")
        Finally
            If IO.File.Exists(package) Then
                Call IO.File.Delete(package)
            End If
        End Try
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

#Region "真实模型轨迹导出：--trace-model"

    ''' <summary>
    ''' 本次真实模型验证实际执行的断言条数。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="DebugAudit.Test"/> 只在条件不成立时记录事件，所以「0 failure」并不能
    ''' 证明断言跑过；这里自己数一份，才能从报告里确认整条链路都验证到了。
    ''' </remarks>
    Private asserts As Integer = 0

    ''' <summary>执行一条断言：交给 <see cref="DebugAudit"/> 记账，同时在控制台留下可读回执。</summary>
    Private Sub AuditTest(condition As Boolean, message As String)
        asserts += 1

        Call Microsoft.VisualBasic.ApplicationServices.Debugging.DebugAudit.Test(condition, message)

        Console.WriteLine($"  {(If(condition, "[ok]  ", "[FAIL]"))} {message}")
    End Sub

    ''' <summary>
    ''' 用真实模型包跑一次「选靶点 → 记录稳态轨迹 → 导出矩阵 → 回读校验」的端到端验证。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 用法：
    ''' </para>
    ''' <code>
    ''' dotnet run -c Debug -p:Platform=x64 -- --trace-model G:\vcc2026\model\model.zip
    ''' dotnet run -c Debug -p:Platform=x64 -- --trace-model model.zip --gene TP53 --mode Knockdown --steps 30 --out D:\trace
    ''' </code>
    ''' <para>
    ''' 省略 <c>--gene</c> 时，先在 <c>--candidates</c> 个候选基因上做响应强度排序，
    ''' 再对最强的一个记录轨迹。默认自测流程不会走到这里，因为加载真实模型包很慢。
    ''' </para>
    ''' </remarks>
    Private Function RunTraceModel(args As String()) As Integer
        Dim package As String = "G:\vcc2026\model\model.zip"
        Dim gene As String = Nothing
        Dim mode As InterventionMode = InterventionMode.Knockout
        Dim steps As Integer = 0
        Dim candidates As Integer = 24
        Dim outDir As String = Path.Combine(Path.GetTempPath(), $"fuzzygene-trace-{DateTime.Now:yyyyMMdd-HHmmss}")

        Dim i As Integer = 1

        While i < args.Length
            Dim key As String = If(args(i), "").ToLowerInvariant()

            Select Case key
                Case "--gene"
                    gene = RequireValue(args, i, key) : i += 2
                Case "--mode"
                    mode = ParseInterventionMode(RequireValue(args, i, key)) : i += 2
                Case "--steps"
                    steps = CInt(RequireValue(args, i, key)) : i += 2
                Case "--candidates"
                    candidates = CInt(RequireValue(args, i, key)) : i += 2
                Case "--out"
                    outDir = RequireValue(args, i, key) : i += 2
                Case Else
                    package = args(i) : i += 1
            End Select
        End While

        Call DebugAudit.Reset()

        Console.WriteLine("=== FuzzyGene real-model trace export ===")
        Console.WriteLine($"package   : {package}")
        Console.WriteLine($"output    : {outDir}")
        Console.WriteLine($"mode      : {mode}, steps = {steps} (0 = use config)")

        If Not File.Exists(package) Then
            Console.WriteLine($"[ERROR] model package not found: {package}")

            Return 2
        End If

        ' ==================== 1. 加载真实模型 ====================
        Console.WriteLine()
        Console.WriteLine("loading model package (manifest + per-gene fuzzy models) ...")

        Dim loadWatch As Stopwatch = Stopwatch.StartNew()
        Dim model As FuzzyGeneModel = FuzzyGeneModel.Load(package)

        loadWatch.Stop()

        Dim net As FuzzyGeneNetwork = model.Network

        Console.WriteLine($"loaded in {loadWatch.Elapsed.TotalSeconds:F1}s: {net}")

        Call DebugAudit.Mark("load.seconds", loadWatch.Elapsed.TotalSeconds)
        Call DebugAudit.Mark("network.genes", net.NGene)
        Call DebugAudit.Mark("network.modelled", net.NModelled)

        ' ==================== 2. 选定靶基因 ====================
        If String.IsNullOrEmpty(gene) Then
            Console.WriteLine()
            Console.WriteLine($"ranking {candidates} candidate perturbation(s) by max |Z| ...")

            Dim rankWatch As Stopwatch = Stopwatch.StartNew()
            Dim ranked As PerturbationCandidate() = model.RankPerturbations(candidates, mode)

            rankWatch.Stop()

            Call DebugAudit.Mark("rank.seconds", rankWatch.Elapsed.TotalSeconds)
            Call AuditTest(ranked.Length > 0, $"at least one candidate was evaluated (actual {ranked.Length})")

            For k As Integer = 0 To std.Min(5, ranked.Length) - 1
                Console.WriteLine($"  {k + 1}. {ranked(k)}")
            Next

            If ranked.Length = 0 Then
                Console.WriteLine("[ERROR] no candidate could be evaluated")

                Return 3
            End If

            Dim descending As Boolean = True
            Dim modelled As Boolean = True

            For k As Integer = 1 To ranked.Length - 1
                If ranked(k - 1).MaxAbsZScore < ranked(k).MaxAbsZScore Then
                    descending = False
                End If
            Next

            For k As Integer = 0 To ranked.Length - 1
                Dim idx As Integer = net.IndexOfGene(ranked(k).GeneName)

                If idx < 0 OrElse net.Models(idx) Is Nothing Then
                    modelled = False
                End If
            Next

            Call AuditTest(descending, "candidates are sorted by max |Z| in descending order")
            Call AuditTest(modelled, "every candidate is a modelled gene of the network")

            gene = ranked(0).GeneName
        End If

        Console.WriteLine()
        Console.WriteLine($"target    : {mode}:{gene}")

        ' ==================== 3. 记录稳态轨迹 ====================
        Dim traceWatch As Stopwatch = Stopwatch.StartNew()
        Dim trace As PerturbationTrace = model.TracePerturbation(gene, mode, steps)

        traceWatch.Stop()

        Dim mutant As SteadyStateTrace = trace.Mutant
        Dim control As SteadyStateTrace = trace.Control

        Call DebugAudit.Mark("trace.seconds", traceWatch.Elapsed.TotalSeconds)
        Call DebugAudit.Mark("trace.frames", If(mutant Is Nothing, 0, mutant.NFrames))
        Call DebugAudit.Mark("trace.iterations", If(mutant Is Nothing, 0, mutant.Iterations))
        Call DebugAudit.Mark("trace.converged", If(mutant IsNot Nothing AndAlso mutant.Converged, 1, 0))

        If mutant Is Nothing Then
            Call AuditTest(False, "mutant trace is present (target gene missing from the network)")

            Return 4
        End If

        Console.WriteLine($"  mutant : {mutant}")
        Console.WriteLine($"  control: {control}")

        If Not mutant.Converged Then
            Console.WriteLine($"  [note] not converged within {mutant.Iterations} iteration(s) " &
                              "- the flat tail of the curve is a truncated plateau, not a steady state")
        End If

        ' ---- 断言 1：帧数与迭代步数自洽 ----
        Call AuditTest(mutant.States.Count = mutant.Iterations + 1,
                             $"frame count equals iterations + 1 ({mutant.States.Count} vs {mutant.Iterations + 1})")
        Call AuditTest(steps <= 0 OrElse mutant.Iterations <= steps,
                             $"iterations do not exceed the requested steps ({mutant.Iterations} vs {steps})")

        ' ---- 断言 2：记录动作不改变数值（与纯求稳态逐位一致）----
        Dim geneIndex As Integer = net.IndexOfGene(gene)
        Dim clamps As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
            {gene, trace.ClampValue}
        }

        Dim expected As Double() = net.SteadyState(net.BaselineRaw, clamps, steps,
                                                  net.Config.Relaxation, net.Config.ConvergenceTolerance)

        ' 与 AnafisPerturbationEngine.Predict 同口径：被扰动基因的终值写回施加值
        expected(geneIndex) = trace.ClampValue

        Call AuditTest(SameArray(expected, mutant.Final),
                             "trace.Final is bit-identical to the plain steady-state solution")

        ' ---- 断言 3：报告结果与轨迹终点一致 ----
        Call AuditTest(trace.Result IsNot Nothing AndAlso SameArray(trace.Result.MutantMeans, mutant.Final),
                             "Result.MutantMeans equals the last trace frame")

        ' ---- 断言 4：被扰动基因在每一帧都是施加值（水平线）----
        Dim clampedMaxError As Double = 0.0

        For t As Integer = 0 To mutant.NFrames - 1
            Dim error_ As Double = std.Abs(mutant.States(t)(geneIndex) - trace.ClampValue)

            If error_ > clampedMaxError Then
                clampedMaxError = error_
            End If
        Next

        Call DebugAudit.Mark("trace.clamped.max_error", clampedMaxError)
        Call AuditTest(clampedMaxError <= 0.000000001,
                             $"the clamped gene stays at the imposed value in every frame (max error {clampedMaxError:G3})")

        ' ---- 断言 5：对照轨迹从基线迭代到自己的不动点 ----
        Dim expectedControl As Double() = net.SteadyState(net.BaselineRaw, Nothing, steps,
                                                          net.Config.Relaxation, net.Config.ConvergenceTolerance)

        Call AuditTest(control IsNot Nothing AndAlso SameArray(expectedControl, control.Final),
                             "control trace ends at the steady state of the unperturbed baseline")

        Dim baselineDrift As Double = 0.0

        For k As Integer = 0 To net.NGene - 1
            Dim drift As Double = std.Abs(control.Final(k) - net.BaselineRaw(k))

            If drift > baselineDrift Then
                baselineDrift = drift
            End If
        Next

        Call DebugAudit.Mark("trace.baseline.drift", baselineDrift)
        Console.WriteLine($"  baseline drift: max |fixed point - baseline| = {baselineDrift:G4}")

        ' ---- 断言 9：PredictWithTrace 的结果与 Predict 逐位一致 ----
        Dim spec As New InterventionSpec With {
            .GeneName = gene,
            .GeneIndex = -1,
            .Mode = mode
        }
        Dim plain As InterventionResult = model.Predict(New InterventionSpec() {spec})

        Call AuditTest(SameArray(plain.FoldChanges, trace.Result.FoldChanges),
                             "PredictWithTrace().Result.FoldChanges is bit-identical to Predict()")

        ' ==================== 4. 导出矩阵 ====================
        Console.WriteLine()
        Console.WriteLine("exporting trajectory matrices ...")

        Dim exportWatch As Stopwatch = Stopwatch.StartNew()
        Dim files As String() = trace.Export(outDir)

        exportWatch.Stop()

        Call DebugAudit.Mark("export.seconds", exportWatch.Elapsed.TotalSeconds)

        ' 循环变量不能叫 path / file —— VB 大小写不敏感，
        ' 会分别遮蔽 System.IO.Path 与 System.IO.File 两个类型
        For Each artifact As String In files
            Dim exists As Boolean = File.Exists(artifact) AndAlso New FileInfo(artifact).Length > 0

            Call AuditTest(exists, $"artifact written: {Path.GetFileName(artifact)}")
            Console.WriteLine($"  {(If(exists, "[ok]  ", "[FAIL]"))} {artifact} ({If(exists, New FileInfo(artifact).Length, 0)} bytes)")
        Next

        ' ==================== 5. 回读校验 ====================
        Console.WriteLine()
        Console.WriteLine("reloading the exported matrix ...")

        Dim mutantCsv As String = Path.Combine(outDir, "trace.mutant.csv")
        Dim mutantInfo As String = Path.Combine(outDir, "trace.mutant.sampleinfo.json")

        If Not File.Exists(mutantCsv) OrElse Not File.Exists(mutantInfo) Then
            Call AuditTest(False, "the mutant matrix and its sample metadata exist")

            Return 5
        End If

        Dim reloaded As Matrix = Matrix.LoadData(mutantCsv)

        Call AuditTest(reloaded.size = net.NGene,
                             $"reloaded matrix has {reloaded.size} rows (expected {net.NGene})")
        Call AuditTest(reloaded.sample_count = mutant.NFrames,
                             $"reloaded matrix has {reloaded.sample_count} columns (expected {mutant.NFrames})")

        ' ---- 断言 8：sidecar 与矩阵列严格对齐 ----
        Dim sidecar As (ID As String, Time As String, Iteration As String, Perturbed As String)() =
            ReadTraceSidecar(mutantInfo)

        Call AuditTest(sidecar.Length = mutant.NFrames,
                             $"sample metadata describes {sidecar.Length} sample(s) (expected {mutant.NFrames})")

        Dim idAligned As Boolean = True
        Dim timeParsable As Boolean = True
        Dim iterationAligned As Boolean = True

        For t As Integer = 0 To sidecar.Length - 1
            If t < reloaded.sampleID.Length AndAlso Not String.Equals(sidecar(t).ID, reloaded.sampleID(t), StringComparison.Ordinal) Then
                idAligned = False
            End If

            Dim parsed As Double

            If Not Double.TryParse(sidecar(t).Time, NumberStyles.Float, CultureInfo.InvariantCulture, parsed) Then
                timeParsable = False
            End If
            If sidecar(t).Iteration <> t.ToString(CultureInfo.InvariantCulture) Then
                iterationAligned = False
            End If
        Next

        Call AuditTest(idAligned, "sample metadata IDs match the matrix columns in order")
        Call AuditTest(timeParsable, "every sample metadata carries a parsable numeric 'time'")
        Call AuditTest(iterationAligned, "sample metadata 'iteration' equals the column index")

        ' ---- 断言：perturbed_genes 能被下游按 JSON 数组解析 ----
        ' 这是 GEARS 读取扰动定义的方式（metadata_perturbed_genes + LoadJSON(Of String())），
        ' 写入侧的值里带 \u0022 转义，这里正好验证「转义不影响语义」
        Dim perturbedGenes As String() = Nothing

        If sidecar.Length > 0 AndAlso Not String.IsNullOrEmpty(sidecar(0).Perturbed) Then
            Try
                perturbedGenes = JsonSerializer.Deserialize(Of String())(sidecar(0).Perturbed)
            Catch ex As Exception
                perturbedGenes = Nothing
            End Try
        End If

        Call AuditTest(SameStringArray(perturbedGenes, New String() {gene}),
                       $"metadata.perturbed_genes parses back to [{gene}] (downstream GEARS contract)")

        ' ---- 断言 7：数值回读误差 ----
        Dim maxRelativeError As Double = 0.0
        Dim checkedCells As Integer = 0

        For Each g As Integer In SpreadIndices(net.NGene, 3)
            For t As Integer = 0 To mutant.NFrames - 1
                Dim actual As Double = reloaded.expression(g).experiments(t)
                Dim error_ As Double = RelativeError(actual, mutant.States(t)(g))

                If error_ > maxRelativeError Then
                    maxRelativeError = error_
                End If

                checkedCells += 1
            Next
        Next

        Call DebugAudit.Mark("reload.relative.error", maxRelativeError)
        Call AuditTest(maxRelativeError <= 0.000000001,
                             $"reloaded values match the in-memory trace ({checkedCells} cells, max relative error {maxRelativeError:G3})")

        ' ==================== 6. 汇总 ====================
        ' DebugAudit.Test 只在失败时留痕，所以把断言总数也记进报告，
        ' 否则「0 failure」无法区分「全过」与「一条都没跑」
        Call DebugAudit.Mark("assertions", asserts)

        Console.WriteLine()
        Console.WriteLine($"assertions: {asserts}, failures: {DebugAudit.FailureCount}")
        Console.WriteLine(DebugAudit.Dump())

        Dim reportPath As String = Path.Combine(outDir, $"fuzzygene-trace-report-{DateTime.Now:yyyyMMdd-HHmmss}.json")

        Call DebugAudit.SaveReport(reportPath)

        Console.WriteLine($"report   : {reportPath}")
        Console.WriteLine($"artifacts: {outDir}")

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

    ''' <summary>读取轨迹 sidecar 里校验需要的字段。</summary>
    ''' <remarks>
    ''' <c>perturbed_genes</c> 只在突变型样本上存在，读不到时返回 <c>Nothing</c>。
    ''' </remarks>
    Private Function ReadTraceSidecar(path As String) As (ID As String, Time As String, Iteration As String, Perturbed As String)()
        Using doc As JsonDocument = JsonDocument.Parse(File.ReadAllText(path))
            Dim items As New List(Of (ID As String, Time As String, Iteration As String, Perturbed As String))

            For Each element As JsonElement In doc.RootElement.EnumerateArray()
                Dim meta As JsonElement = element.GetProperty("metadata")
                Dim perturbed As JsonElement = Nothing

                Call items.Add((
                    element.GetProperty("ID").GetString(),
                    meta.GetProperty("time").GetString(),
                    meta.GetProperty("iteration").GetString(),
                    If(meta.TryGetProperty("perturbed_genes", perturbed), perturbed.GetString(), Nothing)))
            Next

            Return items.ToArray()
        End Using
    End Function

    ''' <summary>两个字符串数组逐位相等（不区分大小写，与基因名匹配口径一致）。</summary>
    Private Function SameStringArray(a As String(), b As String()) As Boolean
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

    ''' <summary>逐位相等（与测试里其它"必须完全一致"的断言同口径）。</summary>
    Private Function SameArray(a As Double(), b As Double()) As Boolean
        If a Is Nothing OrElse b Is Nothing Then
            Return False
        End If
        If a.Length <> b.Length Then
            Return False
        End If

        For i As Integer = 0 To a.Length - 1
            If a(i) <> b(i) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>相对误差，分母至少为 1 以避免接近 0 时的放大。</summary>
    Private Function RelativeError(actual As Double, expected As Double) As Double
        Dim scale As Double = std.Max(1.0, std.Max(std.Abs(actual), std.Abs(expected)))

        Return std.Abs(actual - expected) / scale
    End Function

    ''' <summary>在 <c>[0, total)</c> 上等距取 <paramref name="count"/> 个下标。</summary>
    Private Function SpreadIndices(total As Integer, count As Integer) As Integer()
        If total <= count Then
            Return Enumerable.Range(0, total).ToArray()
        End If

        Dim stride As Double = total / count
        Dim picked(count - 1) As Integer

        For k As Integer = 0 To count - 1
            picked(k) = CInt(std.Floor(k * stride))
        Next

        Return picked
    End Function

    ''' <summary>命令行取值（缺失时抛 <see cref="ArgumentException"/>）。</summary>
    Private Function RequireValue(args As String(), index As Integer, key As String) As String
        If index + 1 >= args.Length Then
            Throw New ArgumentException($"missing value for {key}")
        End If

        Return args(index + 1)
    End Function

    ''' <summary>解析干预模式名；无法识别时回退到敲除。</summary>
    Private Function ParseInterventionMode(text As String) As InterventionMode
        Select Case If(text, "").Trim().ToLowerInvariant()
            Case "knockout", "ko"
                Return InterventionMode.Knockout
            Case "knockdown", "kd"
                Return InterventionMode.Knockdown
            Case "overexpression", "oe"
                Return InterventionMode.Overexpression
            Case Else
                Return InterventionMode.Knockout
        End Select
    End Function

#End Region

#Region "多运行自检与剂量分析：--analysis"

    ''' <summary>
    ''' 用真实模型跑一遍「多运行自检 + 剂量分析 + 一键导出」并断言关键不变量。
    ''' </summary>
    ''' <remarks>
    ''' 用法：<c>dotnet run -c Debug -p:Platform=x64 -- --analysis</c>
    ''' 或 <c>-- --analysis --model model.zip --gene TPPP3 --gene2 HSD17B3 --out D:\analysis</c>。
    ''' 网格刻意取小（ω 2 个、步数 2 个、剂量 3 个），整轮约一分钟。
    ''' </remarks>
    Private Function RunAnalysis(args As String()) As Integer
        Dim package As String = "G:\vcc2026\model\model.zip"
        Dim gene As String = "TPPP3"
        Dim geneB As String = "HSD17B3"
        Dim outDir As String = Path.Combine(Path.GetTempPath(), $"fuzzygene-analysis-{DateTime.Now:yyyyMMdd-HHmmss}")

        Dim i As Integer = 1

        While i < args.Length
            Dim key As String = If(args(i), "").ToLowerInvariant()

            Select Case key
                Case "--model"
                    package = RequireValue(args, i, key) : i += 2
                Case "--gene"
                    gene = RequireValue(args, i, key) : i += 2
                Case "--gene2"
                    geneB = RequireValue(args, i, key) : i += 2
                Case "--out"
                    outDir = RequireValue(args, i, key) : i += 2
                Case Else
                    i += 1
            End Select
        End While

        Call DebugAudit.Reset()
        asserts = 0

        Console.WriteLine("=== FuzzyGene multi-run analysis ===")
        Console.WriteLine($"package : {package}")
        Console.WriteLine($"target  : {gene}, combination: {gene} + {geneB}")
        Console.WriteLine($"output  : {outDir}")

        If Not File.Exists(package) Then
            Console.WriteLine($"[ERROR] model package not found: {package}")

            Return 2
        End If

        Dim loadWatch As Stopwatch = Stopwatch.StartNew()
        Dim model As FuzzyGeneModel = FuzzyGeneModel.Load(package)

        loadWatch.Stop()

        Console.WriteLine($"loaded in {loadWatch.Elapsed.TotalSeconds:F1}s: {model.Network}")

        Call DebugAudit.Mark("load.seconds", loadWatch.Elapsed.TotalSeconds)

        ' ---------------- 小网格 ----------------
        Dim omegas As Double() = {0.5, 1.0}
        Dim budgets As Integer() = {20, 40}
        Dim doses As Double() = {0.0, 0.5, 1.0}
        Const steps As Integer = 20
        Const top As Integer = 5
        Const tolerance As Double = 0.001

        ' ---------------- 1. ω 扫描 ----------------
        Dim omega As OmegaSweepResult = model.SweepOmega(gene, InterventionMode.Knockout, omegas, steps, 0.5, tolerance)

        Call AuditTest(omega.Omegas.Length = omegas.Length,
                       $"omega sweep returns one entry per relaxation value ({omega.Omegas.Length})")

        Dim aligned As Boolean = True

        For k As Integer = 0 To omega.FinalStates.Count - 1
            If omega.FinalStates(k).Length <> model.Network.NGene Then
                aligned = False
            End If
        Next

        Call AuditTest(aligned, "every omega final state matches the gene count")

        Dim referenceIndex As Integer = Array.IndexOf(omega.Omegas, omega.ReferenceOmega)
        Dim referenceDeviation As Double = If(referenceIndex < 0, Double.NaN, omega.MaxRelativeDeviation(referenceIndex))

        Call AuditTest(referenceIndex >= 0 AndAlso referenceDeviation = 0.0,
                       $"the reference omega has zero deviation ({referenceDeviation:G3})")

        Dim anyAbove As Boolean = omega.MaxRelativeDeviation.Any(Function(r) r > tolerance)

        Call AuditTest(omega.Invariant = Not anyAbove,
                       $"the invariant flag agrees with the per run deviations ({omega.Invariant})")

        ' ---------------- 2. 步数扫描 ----------------
        Dim stepSweep As StepSweepResult = model.SweepSteps(gene, InterventionMode.Knockout, budgets, Double.NaN, tolerance)

        Dim referenceStepDeviation As Double = stepSweep.MaxRelativeDeviation(stepSweep.MaxRelativeDeviation.Length - 1)
        Dim withinBudget As Boolean = True

        For k As Integer = 0 To stepSweep.ActualIterations.Length - 1
            If stepSweep.ActualIterations(k) > stepSweep.StepsList(k) Then
                withinBudget = False
            End If
        Next

        Call AuditTest(referenceStepDeviation = 0.0,
                       $"the reference step count has zero deviation ({referenceStepDeviation:G3})")
        Call AuditTest(withinBudget, "actual iterations never exceed the requested step count")

        ' ---------------- 3. lag 稳健性 ----------------
        Dim lag As LagRobustnessResult = model.AnalyzeLag(gene, InterventionMode.Knockout, omegas, steps, 0.1)

        Call AuditTest(Array.IndexOf(lag.GeneNames, gene) < 0,
                       "the target gene itself is excluded from the lag statistics")

        Dim lagFinite As Boolean = True

        For k As Integer = 0 To lag.Omegas.Length - 1
            For j As Integer = 0 To lag.GeneNames.Length - 1
                If Double.IsNaN(lag.Lag(k, j)) Then
                    lagFinite = False
                End If
            Next
        Next

        Call AuditTest(lagFinite, $"every retained gene has a finite lag ({lag.GeneNames.Length} gene(s))")

        Dim symmetric As Boolean = True
        Dim bounded As Boolean = True
        Dim diagonal As Boolean = True

        For k As Integer = 0 To lag.Omegas.Length - 1
            If lag.Spearman(k, k) <> 1.0 Then
                diagonal = False
            End If

            For j As Integer = 0 To lag.Omegas.Length - 1
                If Double.IsNaN(lag.Spearman(k, j)) Then
                    Continue For
                End If
                If lag.Spearman(k, j) <> lag.Spearman(j, k) Then
                    symmetric = False
                End If
                If std.Abs(lag.Spearman(k, j)) > 1.0 + 0.000000001 Then
                    bounded = False
                End If
            Next
        Next

        Call AuditTest(diagonal, "the spearman diagonal is 1")
        Call AuditTest(symmetric AndAlso bounded, "the spearman matrix is symmetric and bounded by 1")

        ' ---------------- 4. 剂量分析 ----------------
        Dim dose As DoseResponseResult = model.AnalyzeDose(gene, doses, steps, Double.NaN, top)
        Dim geneIndex As Integer = model.Network.IndexOfGene(gene)
        Dim baseline As Double = model.Network.BaselineRaw(geneIndex)
        Dim lastDose As Integer = dose.Doses.Length - 1

        Call AuditTest(dose.ClampValues(lastDose) = 0.0 AndAlso std.Abs(dose.ClampValues(0) - baseline) <= 0.000000001,
                       $"dose 1 clamps to zero and dose 0 clamps to the baseline ({dose.ClampValues(lastDose):G3} / {dose.ClampValues(0):G3})")
        Call AuditTest(std.Abs(dose.FoldChanges(0, geneIndex)) <= 0.000000001,
                       $"the pinned gene reports zero fold change at dose 0 ({dose.FoldChanges(0, geneIndex):G3})")

        Dim surface As DoseTimeSurfaceResult = model.AnalyzeDoseTime(gene, doses, steps, Double.NaN, top)

        Call AuditTest(surface.Deltas.GetLength(0) = doses.Length AndAlso
                       surface.Deltas.GetLength(1) = surface.NFrames AndAlso
                       surface.Deltas.GetLength(2) = surface.GeneNames.Length,
                       $"the surface cube is [dose, frame, gene] ({surface.Deltas.GetLength(0)} x {surface.Deltas.GetLength(1)} x {surface.Deltas.GetLength(2)})")

        ' ---------------- 5. 组合分析 ----------------
        Dim combination As CombinationTraceResult = model.AnalyzeCombination(gene, geneB, InterventionMode.Knockout, steps, Double.NaN, top)
        Dim additiveHeld As Boolean = True

        For g As Integer = 0 To combination.NonAdditive.Length - 1
            If combination.NonAdditive(g) <> combination.JointMeans(g) - combination.AdditiveMeans(g) Then
                additiveHeld = False
            End If
        Next

        Call AuditTest(additiveHeld, "non additive equals joint minus additive for every gene")

        ' ---------------- 6. relaxation 参数确实被尊重 ----------------
        Const probeOmega As Double = 0.7
        Dim probe As PerturbationTrace = model.TracePerturbation(gene, InterventionMode.Knockout, steps, probeOmega)
        Dim fallback As PerturbationTrace = model.TracePerturbation(gene, InterventionMode.Knockout, steps)

        Call AuditTest(probe.Mutant.Relaxation = probeOmega,
                       $"an explicit relaxation is honoured ({probe.Mutant.Relaxation:G3})")
        Call AuditTest(fallback.Mutant.Relaxation = model.Config.Relaxation,
                       $"an omitted relaxation falls back to the config value ({fallback.Mutant.Relaxation:G3})")

        ' ---------------- 7. 一键导出 ----------------
        Dim options As New AnalysisOptions With {
            .Mode = InterventionMode.Knockout,
            .Omegas = omegas,
            .StepsList = budgets,
            .SweepSteps = steps,
            .Doses = doses,
            .DoseSteps = steps,
            .CombinationSteps = steps,
            .TopGenes = top,
            .SurfaceTopGenes = top,
            .LagThresholdRatio = 0.1,
            .InvariantTolerance = tolerance
        }

        Dim exportWatch As Stopwatch = Stopwatch.StartNew()
        Dim files As String() = model.ExportAnalysisReport(outDir, gene, gene, geneB, options)

        exportWatch.Stop()

        Call DebugAudit.Mark("export.seconds", exportWatch.Elapsed.TotalSeconds)

        Dim allWritten As Boolean = True

        For Each artifact As String In files
            If Not File.Exists(artifact) OrElse New FileInfo(artifact).Length = 0 Then
                allWritten = False
            End If
        Next

        Call AuditTest(allWritten AndAlso files.Length >= 6,
                       $"every analysis artifact was written ({files.Length} file(s))")

        Dim reportPath As String = Path.Combine(outDir, AnalysisExporter.ReportFile)
        Dim runs As Integer = 0
        Dim parsed As Boolean = False

        If File.Exists(reportPath) Then
            Try
                Using doc As JsonDocument = JsonDocument.Parse(File.ReadAllText(reportPath))
                    Dim runsElement As JsonElement

                    If doc.RootElement.TryGetProperty("runs", runsElement) Then
                        runs = runsElement.GetInt32()
                        parsed = runs > 0
                    End If
                End Using
            Catch ex As Exception
                parsed = False
            End Try
        End If

        Call AuditTest(parsed, $"the report is valid json and records a positive run count ({runs})")

        ' ---------------- 汇总 ----------------
        Call DebugAudit.Mark("assertions", asserts)

        Console.WriteLine()
        Console.WriteLine($"assertions: {asserts}, failures: {DebugAudit.FailureCount}")
        Console.WriteLine(DebugAudit.Dump())

        Dim reportFile As String = Path.Combine(outDir, $"fuzzygene-analysis-report-{DateTime.Now:yyyyMMdd-HHmmss}.json")

        Call DebugAudit.SaveReport(reportFile)

        Console.WriteLine($"report   : {reportFile}")
        Console.WriteLine($"artifacts: {outDir}")

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

#End Region

End Module
