Imports System.Diagnostics
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

Namespace Perturbation.Analysis

    ''' <summary>
    ''' 双基因组合扰动的非加性分析（终态 + 逐帧两个口径）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 加性期望的定义是「把两个单扰动的效应直接相加」：
    ''' <c>additive(g) = control(g) + (A(g) − control(g)) + (B(g) − control(g))</c>；
    ''' 非加性 = <c>joint(g) − additive(g)</c>：正值表示协同，负值表示拮抗。
    ''' </para>
    ''' <para>
    ''' 逐帧口径用同一公式在每一帧上计算，能看出联合扰动的偏差是「同步出现」还是
    ''' 「延迟出现」——这是只看终态发现不了的信息。
    ''' </para>
    ''' <para>
    ''' 三次扰动各自都会跑一次对照轨迹（<see cref="PerturbationTrace.Control"/>），
    ''' 因此一轮分析共 6 次求解；三条对照轨迹在数值上完全等价，
    ''' 这里统一用联合那一次的对照，避免把不同对照混进同一个减法里。
    ''' </para>
    ''' </remarks>
    Public Module CombinationTrace

        ''' <summary>
        ''' 分析两个基因联合扰动时的非加性效应。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="geneA">第一个靶基因</param>
        ''' <param name="geneB">第二个靶基因</param>
        ''' <param name="mode">干预模式（两个基因使用同一模式）</param>
        ''' <param name="steps">每次求解的最大迭代步数</param>
        ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
        ''' <param name="top">逐帧口径保留的基因数（按 |非加性| 排序）</param>
        ''' <returns>终态与逐帧的非加性结果</returns>
        ''' <exception cref="ArgumentException">两个基因相同或为空</exception>
        ''' <exception cref="MissingMemberException">任一基因不在模型里</exception>
        Public Function Analyze(model As FuzzyGeneModel,
                                geneA As String,
                                geneB As String,
                                Optional mode As InterventionMode = InterventionMode.Knockout,
                                Optional steps As Integer = 60,
                                Optional omega As Double = Double.NaN,
                                Optional top As Integer = 50) As CombinationTraceResult

            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If model.Network Is Nothing Then
                Throw New InvalidOperationException("the model has not been trained yet")
            End If
            If String.IsNullOrEmpty(geneA) OrElse String.IsNullOrEmpty(geneB) Then
                Throw New ArgumentException("two target genes are required")
            End If
            If String.Equals(geneA, geneB, StringComparison.OrdinalIgnoreCase) Then
                Throw New ArgumentException("the two target genes must be different", NameOf(geneB))
            End If
            If steps < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(steps), steps, "the step count must be positive")
            End If

            Dim indexA As Integer = model.Network.IndexOfGene(geneA)
            Dim indexB As Integer = model.Network.IndexOfGene(geneB)

            If indexA < 0 Then
                Throw New MissingMemberException($"the target gene '{geneA}' is not modelled by this network")
            End If
            If indexB < 0 Then
                Throw New MissingMemberException($"the target gene '{geneB}' is not modelled by this network")
            End If

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim specA As New InterventionSpec With {.GeneName = geneA, .GeneIndex = -1, .Mode = mode}
            Dim specB As New InterventionSpec With {.GeneName = geneB, .GeneIndex = -1, .Mode = mode}

            Dim traceA As PerturbationTrace = model.TracePerturbation(geneA, mode, steps, omega)
            Dim traceB As PerturbationTrace = model.TracePerturbation(geneB, mode, steps, omega)
            Dim traceAB As PerturbationTrace =
                model.TracePerturbation(New InterventionSpec() {specA, specB}, steps, omega)

            If traceA Is Nothing OrElse traceA.Mutant Is Nothing OrElse
               traceB Is Nothing OrElse traceB.Mutant Is Nothing OrElse
               traceAB Is Nothing OrElse traceAB.Mutant Is Nothing OrElse traceAB.Control Is Nothing Then
                Throw New InvalidOperationException("the combination could not be evaluated")
            End If

            Dim n As Integer = model.Network.NGene
            Dim control As Double() = traceAB.Control.Final
            Dim singleA As Double() = traceA.Mutant.Final
            Dim singleB As Double() = traceB.Mutant.Final
            Dim joint As Double() = traceAB.Mutant.Final

            Dim additive(n - 1) As Double
            Dim nonAdditive(n - 1) As Double
            Dim maxAbs As Double = 0.0

            For g As Integer = 0 To n - 1
                additive(g) = control(g) + (singleA(g) - control(g)) + (singleB(g) - control(g))
                nonAdditive(g) = joint(g) - additive(g)

                If std.Abs(nonAdditive(g)) > maxAbs Then
                    maxAbs = std.Abs(nonAdditive(g))
                End If
            Next

            Dim k As Integer = If(top <= 0 OrElse top > n, n, top)
            Dim topGenes As Integer() = Enumerable.Range(0, n) _
                .OrderByDescending(Function(g) std.Abs(nonAdditive(g))) _
                .Take(k) _
                .ToArray()

            Dim trajectory As CombinationTrajectory = BuildTrajectory(traceA, traceB, traceAB, topGenes)

            watch.Stop()

            Dim result As New CombinationTraceResult With {
                .GeneA = geneA,
                .GeneB = geneB,
                .Mode = mode,
                .Steps = steps,
                .Omega = omega,
                .GeneNames = CType(model.Network.GeneNames.Clone(), String()),
                .SingleAMeans = singleA,
                .SingleBMeans = singleB,
                .JointMeans = joint,
                .AdditiveMeans = additive,
                .NonAdditive = nonAdditive,
                .TopGenes = topGenes,
                .MaxAbsNonAdditive = maxAbs,
                .Trajectory = trajectory,
                .Runs = 6,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }

            Call result.Validate()

            Return result
        End Function

        ''' <summary>
        ''' 构造逐帧口径：<c>nonAdditive(t) = joint(t) − [control(t) + (A(t)−control(t)) + (B(t)−control(t))]</c>。
        ''' </summary>
        Private Function BuildTrajectory(traceA As PerturbationTrace,
                                         traceB As PerturbationTrace,
                                         traceAB As PerturbationTrace,
                                         genes As Integer()) As CombinationTrajectory

            ' 对照轨迹也要参与取最小帧数：收敛步数由各自的起点决定，
            ' 突变型帧数多于对照时直接索引 Control.States(t) 会越界
            Dim frames As Integer = std.Min(traceA.Mutant.NFrames,
                                            std.Min(traceB.Mutant.NFrames,
                                                     std.Min(traceAB.Mutant.NFrames, traceAB.Control.NFrames)))
            Dim k As Integer = genes.Length
            Dim additive(frames - 1, k - 1) As Double
            Dim joint(frames - 1, k - 1) As Double
            Dim nonAdditive(frames - 1, k - 1) As Double
            Dim names(k - 1) As String

            For j As Integer = 0 To k - 1
                names(j) = traceAB.Control.GeneNames(genes(j))
            Next

            For t As Integer = 0 To frames - 1
                For j As Integer = 0 To k - 1
                    Dim g As Integer = genes(j)
                    Dim baseline As Double = traceAB.Control.States(t)(g)
                    Dim a As Double = traceA.Mutant.States(t)(g)
                    Dim b As Double = traceB.Mutant.States(t)(g)
                    Dim ab As Double = traceAB.Mutant.States(t)(g)

                    additive(t, j) = baseline + (a - baseline) + (b - baseline)
                    joint(t, j) = ab
                    nonAdditive(t, j) = ab - additive(t, j)
                Next
            Next

            Return New CombinationTrajectory With {
                .GeneNames = names,
                .NFrames = frames,
                .Additive = additive,
                .Joint = joint,
                .NonAdditive = nonAdditive
            }
        End Function

    End Module

End Namespace
