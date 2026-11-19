Imports System.Diagnostics
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

Namespace Perturbation.Analysis

    ''' <summary>
    ''' 求解器稳定性自检：ω 不变性、步数不变性、lag 排序稳健性。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这一组分析的共同前提是「同一个扰动、多次求解、只换求解参数」。
    ''' 因此每个方法都直接调用 <see cref="FuzzyGeneModel.TracePerturbation"/>，
    ''' 用它的 <c>relaxation</c> 参数按次覆盖阻尼系数，而不去改全局配置——
    ''' 后者会让「扫描途中参数被上一次调用改掉」这种 bug 变得不可复现。
    ''' </para>
    ''' <para>
    ''' 每次调用都会同时得到突变型与野生型两条轨迹（<see cref="PerturbationTrace"/> 自带），
    ''' 所以一轮 ω 扫描的求解次数是 <c>ω 个数 × 2</c>。
    ''' </para>
    ''' </remarks>
    Public Module SolverStability

        ''' <summary>ω 扫描的默认网格。</summary>
        Public ReadOnly Property DefaultOmegas As Double()
            Get
                Return New Double() {0.2, 0.5, 0.8, 1.0}
            End Get
        End Property

        ''' <summary>步数扫描的默认网格。</summary>
        Public ReadOnly Property DefaultSteps As Integer()
            Get
                Return New Integer() {50, 200, 500}
            End Get
        End Property

        ''' <summary>
        ''' 在若干 ω 上各求解一次，比较终态是否一致。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="gene">被扰动的靶基因</param>
        ''' <param name="mode">干预模式</param>
        ''' <param name="omegas">被扫描的 ω 列表；<c>Nothing</c> 时取 <see cref="DefaultOmegas"/></param>
        ''' <param name="steps">每次求解的最大迭代步数</param>
        ''' <param name="referenceOmega">作为比较基准的 ω；<c>NaN</c> 时取列表里最接近 0.5 的那个</param>
        ''' <param name="tolerance">判定「不变」的相对偏差阈值</param>
        ''' <returns>扫描结果</returns>
        ''' <exception cref="InvalidOperationException">目标基因不在模型里</exception>
        Public Function SweepOmega(model As FuzzyGeneModel,
                                   gene As String,
                                   Optional mode As InterventionMode = InterventionMode.Knockout,
                                   Optional omegas As Double() = Nothing,
                                   Optional steps As Integer = 50,
                                   Optional referenceOmega As Double = Double.NaN,
                                   Optional tolerance As Double = 0.001) As OmegaSweepResult

            Call CheckArguments(model, gene, steps, tolerance)

            Dim values As Double() = NormalizeOmegas(omegas)
            Dim reference As Double = ResolveReferenceOmega(values, referenceOmega)
            Dim referenceIndex As Integer = IndexOfOmega(values, reference)
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim n As Integer = model.Network.NGene
            Dim runs As Integer = values.Length

            Dim converged(runs - 1) As Boolean
            Dim iterations(runs - 1) As Integer
            Dim lastDelta(runs - 1) As Double
            Dim states As New List(Of Double())(runs)

            For k As Integer = 0 To runs - 1
                Dim trace As PerturbationTrace = model.TracePerturbation(gene, mode, steps, values(k))

                If trace Is Nothing OrElse trace.Mutant Is Nothing OrElse trace.Mutant.Final Is Nothing Then
                    Throw New InvalidOperationException($"the target gene '{gene}' is not modelled by this network")
                End If

                converged(k) = trace.Mutant.Converged
                iterations(k) = trace.Mutant.Iterations
                lastDelta(k) = LastDeltaOf(trace.Mutant)
                Call states.Add(trace.Mutant.Final)
            Next

            ' 参考值本身没收敛时，拿它做基准是没有意义的（偏差会全部算到别人头上）。
            ' 优先改用「第一个真正收敛的 ω」，并在结果里如实报告换了哪个。
            If Not converged(referenceIndex) Then
                For k As Integer = 0 To runs - 1
                    If converged(k) Then
                        referenceIndex = k
                        reference = values(k)

                        Exit For
                    End If
                Next
            End If

            Dim referenceState As Double() = states(referenceIndex)
            Dim scale As Double = 1.0

            For g As Integer = 0 To n - 1
                Dim magnitude As Double = std.Abs(referenceState(g))

                If magnitude > scale Then
                    scale = magnitude
                End If
            Next

            Dim absDeviation(runs - 1) As Double
            Dim relDeviation(runs - 1) As Double
            Dim invariant As Boolean = True

            For k As Integer = 0 To runs - 1
                Dim deviation As Double = 0.0

                For g As Integer = 0 To n - 1
                    Dim delta As Double = std.Abs(states(k)(g) - referenceState(g))

                    If delta > deviation Then
                        deviation = delta
                    End If
                Next

                absDeviation(k) = deviation
                relDeviation(k) = deviation / scale

                If relDeviation(k) > tolerance Then
                    invariant = False
                End If
            Next

            watch.Stop()

            Dim result As New OmegaSweepResult With {
                .Gene = gene,
                .Mode = mode,
                .Steps = steps,
                .ReferenceOmega = reference,
                .InvariantTolerance = tolerance,
                .Omegas = values,
                .Converged = converged,
                .Iterations = iterations,
                .LastMaxDelta = lastDelta,
                .MaxAbsDeviation = absDeviation,
                .MaxRelativeDeviation = relDeviation,
                .Invariant = invariant,
                .GeneNames = CType(model.Network.GeneNames.Clone(), String()),
                .FinalStates = states,
                .Runs = runs * 2,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }

            Call result.Validate()

            Return result
        End Function

        ''' <summary>
        ''' 在若干步数上各求解一次，以最长步数为参考比较终态。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="gene">被扰动的靶基因</param>
        ''' <param name="mode">干预模式</param>
        ''' <param name="stepsList">被扫描的步数列表；<c>Nothing</c> 时取 <see cref="DefaultSteps"/></param>
        ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
        ''' <param name="tolerance">判定「不变」的相对偏差阈值</param>
        ''' <returns>扫描结果</returns>
        Public Function SweepSteps(model As FuzzyGeneModel,
                                   gene As String,
                                   Optional mode As InterventionMode = InterventionMode.Knockout,
                                   Optional stepsList As Integer() = Nothing,
                                   Optional omega As Double = Double.NaN,
                                   Optional tolerance As Double = 0.001) As StepSweepResult

            Dim values As Integer() = NormalizeSteps(stepsList)

            Call CheckArguments(model, gene, values(values.Length - 1), tolerance)

            Dim referenceSteps As Integer = values(values.Length - 1)
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim n As Integer = model.Network.NGene
            Dim runs As Integer = values.Length

            Dim actual(runs - 1) As Integer
            Dim converged(runs - 1) As Boolean
            Dim lastDelta(runs - 1) As Double
            Dim states As New List(Of Double())(runs)

            For k As Integer = 0 To runs - 1
                Dim trace As PerturbationTrace = model.TracePerturbation(gene, mode, values(k), omega)

                If trace Is Nothing OrElse trace.Mutant Is Nothing OrElse trace.Mutant.Final Is Nothing Then
                    Throw New InvalidOperationException($"the target gene '{gene}' is not modelled by this network")
                End If

                actual(k) = trace.Mutant.Iterations
                converged(k) = trace.Mutant.Converged
                lastDelta(k) = LastDeltaOf(trace.Mutant)
                Call states.Add(trace.Mutant.Final)
            Next

            Dim referenceState As Double() = states(runs - 1)
            Dim scale As Double = 1.0

            For g As Integer = 0 To n - 1
                Dim magnitude As Double = std.Abs(referenceState(g))

                If magnitude > scale Then
                    scale = magnitude
                End If
            Next

            Dim absDeviation(runs - 1) As Double
            Dim relDeviation(runs - 1) As Double
            Dim invariant As Boolean = True

            For k As Integer = 0 To runs - 1
                Dim deviation As Double = 0.0

                For g As Integer = 0 To n - 1
                    Dim delta As Double = std.Abs(states(k)(g) - referenceState(g))

                    If delta > deviation Then
                        deviation = delta
                    End If
                Next

                absDeviation(k) = deviation
                relDeviation(k) = deviation / scale

                If relDeviation(k) > tolerance Then
                    invariant = False
                End If
            Next

            watch.Stop()

            Dim result As New StepSweepResult With {
                .Gene = gene,
                .Mode = mode,
                .Omega = omega,
                .ReferenceSteps = referenceSteps,
                .InvariantTolerance = tolerance,
                .StepsList = values,
                .ActualIterations = actual,
                .Converged = converged,
                .LastMaxDelta = lastDelta,
                .MaxAbsDeviation = absDeviation,
                .MaxRelativeDeviation = relDeviation,
                .Invariant = invariant,
                .GeneNames = CType(model.Network.GeneNames.Clone(), String()),
                .FinalStates = states,
                .Runs = runs * 2,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }

            Call result.Validate()

            Return result
        End Function

        ''' <summary>
        ''' 跨 ω 比较 lag 排序的稳健性（Spearman 秩相关）。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="gene">被扰动的靶基因（自身不参与 lag 统计）</param>
        ''' <param name="mode">干预模式</param>
        ''' <param name="omegas">被扫描的 ω 列表；<c>Nothing</c> 时取 <see cref="DefaultOmegas"/></param>
        ''' <param name="steps">每次求解的最大迭代步数</param>
        ''' <param name="thresholdRatio">触发阈值占该基因终态变化量的比例</param>
        ''' <returns>lag 矩阵与 Spearman 矩阵</returns>
        ''' <remarks>
        ''' lag 的定义是「第一个使 <c>|mutant(t) − control(t)|</c> 达到
        ''' <c>thresholdRatio × |mutant_final − control_final|</c> 的帧号 t」（t 从 1 起）。
        ''' 因为阈值相对该基因自身的幅度，强响应与弱响应的基因才可比。
        ''' </remarks>
        Public Function LagRobustness(model As FuzzyGeneModel,
                                      gene As String,
                                      Optional mode As InterventionMode = InterventionMode.Knockout,
                                      Optional omegas As Double() = Nothing,
                                      Optional steps As Integer = 50,
                                      Optional thresholdRatio As Double = 0.1) As LagRobustnessResult

            Call CheckArguments(model, gene, steps, thresholdRatio)

            Dim values As Double() = NormalizeOmegas(omegas)
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim n As Integer = model.Network.NGene
            Dim targetIndex As Integer = model.Network.IndexOfGene(gene)
            Dim runs As Integer = values.Length

            Dim lagPerRun As New List(Of Double())(runs)
            Dim thresholdPerRun As New List(Of Double())(runs)

            For k As Integer = 0 To runs - 1
                Dim trace As PerturbationTrace = model.TracePerturbation(gene, mode, steps, values(k))

                If trace Is Nothing OrElse trace.Mutant Is Nothing OrElse trace.Control Is Nothing Then
                    Throw New InvalidOperationException($"the target gene '{gene}' is not modelled by this network")
                End If

                Dim lag(n - 1) As Double
                Dim threshold(n - 1) As Double

                For g As Integer = 0 To n - 1
                    lag(g) = Double.NaN

                    Dim finalDelta As Double = trace.Mutant.Final(g) - trace.Control.Final(g)
                    Dim magnitude As Double = std.Abs(finalDelta)

                    If magnitude <= 0.000000000001 Then
                        Continue For
                    End If

                    threshold(g) = thresholdRatio * magnitude

                    ' 突变型与对照的帧数**不一定相等**：求根/阻尼收敛的步数取决于各自的起点，
                    ' 直接按 Mutant.Iterations 索引 Control.States 会越界
                    Dim frames As Integer = std.Min(trace.Mutant.States.Count, trace.Control.States.Count)

                    For t As Integer = 1 To frames - 1
                        Dim delta As Double = std.Abs(trace.Mutant.States(t)(g) - trace.Control.States(t)(g))

                        If delta >= threshold(g) Then
                            lag(g) = t
                            Exit For
                        End If
                    Next
                Next

                Call lagPerRun.Add(lag)
                Call thresholdPerRun.Add(threshold)
            Next

            ' 只保留「在所有 ω 下都响应」的基因：否则不同 ω 之间的 rho 分母不同，不可比
            Dim common As New List(Of Integer)()

            For g As Integer = 0 To n - 1
                If g = targetIndex Then
                    Continue For
                End If

                Dim responds As Boolean = True

                For k As Integer = 0 To runs - 1
                    If Double.IsNaN(lagPerRun(k)(g)) Then
                        responds = False
                        Exit For
                    End If
                Next

                If responds Then
                    Call common.Add(g)
                End If
            Next

            Dim m As Integer = common.Count
            ' m = 0（没有公共响应基因）时第二维长度为 0，VB 允许 Dim a(n, -1)
            Dim lagMatrix(runs - 1, m - 1) As Double
            Dim rho(runs - 1, runs - 1) As Double
            Dim counts(runs - 1, runs - 1) As Integer
            Dim thresholds(m - 1) As Double
            Dim geneNames(m - 1) As String
            Dim geneIndices(m - 1) As Integer

            For j As Integer = 0 To m - 1
                geneIndices(j) = common(j)
                geneNames(j) = model.Network.GeneNames(common(j))
                thresholds(j) = thresholdPerRun(0)(common(j))
            Next

            For k As Integer = 0 To runs - 1
                For j As Integer = 0 To m - 1
                    lagMatrix(k, j) = lagPerRun(k)(common(j))
                Next
            Next

            Dim rhoSum As Double = 0.0
            Dim rhoCount As Integer = 0

            For k As Integer = 0 To runs - 1
                For j As Integer = 0 To runs - 1
                    If m = 0 Then
                        rho(k, j) = Double.NaN
                        counts(k, j) = 0
                        Continue For
                    End If

                    counts(k, j) = m

                    If k = j Then
                        rho(k, j) = 1.0
                    Else
                        Dim a(m - 1) As Double
                        Dim b(m - 1) As Double

                        For q As Integer = 0 To m - 1
                            a(q) = lagMatrix(k, q)
                            b(q) = lagMatrix(j, q)
                        Next

                        rho(k, j) = Spearman(a, b)

                        If k < j AndAlso Not Double.IsNaN(rho(k, j)) Then
                            rhoSum += rho(k, j)
                            rhoCount += 1
                        End If
                    End If
                Next
            Next

            watch.Stop()

            Dim result As New LagRobustnessResult With {
                .Gene = gene,
                .Mode = mode,
                .Steps = steps,
                .ThresholdRatio = thresholdRatio,
                .Omegas = values,
                .GeneNames = geneNames,
                .GeneIndices = geneIndices,
                .Thresholds = thresholds,
                .Lag = lagMatrix,
                .Spearman = rho,
                .SpearmanN = counts,
                .MeanRho = If(rhoCount = 0, Double.NaN, rhoSum / rhoCount),
                .Runs = runs * 2,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }

            Call result.Validate()

            Return result
        End Function

        ''' <summary>
        ''' Spearman 秩相关（并列取平均秩后再算 Pearson，与其等价定义一致）。
        ''' </summary>
        ''' <param name="a">第一个序列</param>
        ''' <param name="b">第二个序列</param>
        ''' <returns>秩相关系数；任一侧方差为 0 时返回 <c>NaN</c></returns>
        Public Function Spearman(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length < 2 Then
                Return Double.NaN
            End If

            Return Pearson(RankOf(a), RankOf(b))
        End Function

        ''' <summary>平均秩（1 起）：并列值取它们所占名次的平均值。</summary>
        Private Function RankOf(values As Double()) As Double()
            Dim n As Integer = values.Length
            ' lambda 参数不能叫 i —— 下面还有一个同名的循环变量，会触发 BC36641
            Dim order As Integer() = Enumerable.Range(0, n).OrderBy(Function(idx) values(idx)).ToArray()
            Dim ranks(n - 1) As Double
            Dim i As Integer = 0

            While i < n
                Dim j As Integer = i

                ' lag 是整数，用精确相等判并列是安全的
                While j + 1 < n AndAlso values(order(j + 1)) = values(order(i))
                    j += 1
                End While

                Dim average As Double = (i + j) / 2.0 + 1.0

                For k As Integer = i To j
                    ranks(order(k)) = average
                Next

                i = j + 1
            End While

            Return ranks
        End Function

        Private Function Pearson(a As Double(), b As Double()) As Double
            Dim n As Integer = a.Length
            Dim meanA As Double = a.Average()
            Dim meanB As Double = b.Average()
            Dim covariance As Double = 0.0
            Dim varianceA As Double = 0.0
            Dim varianceB As Double = 0.0

            For i As Integer = 0 To n - 1
                Dim x As Double = a(i) - meanA
                Dim y As Double = b(i) - meanB

                covariance += x * y
                varianceA += x * x
                varianceB += y * y
            Next

            If varianceA <= 0.0 OrElse varianceB <= 0.0 Then
                Return Double.NaN
            End If

            Return covariance / std.Sqrt(varianceA * varianceB)
        End Function

        ''' <summary>该轨迹最后一步的最大变化量；没有记录任何步骤时返回 0。</summary>
        Private Function LastDeltaOf(trace As Data.SteadyStateTrace) As Double
            If trace Is Nothing OrElse trace.MaxDeltas Is Nothing OrElse trace.MaxDeltas.Count = 0 Then
                Return 0.0
            End If

            Return trace.MaxDeltas(trace.MaxDeltas.Count - 1)
        End Function

        Private Sub CheckArguments(model As FuzzyGeneModel, gene As String, steps As Integer, tolerance As Double)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If model.Network Is Nothing Then
                Throw New InvalidOperationException("the model has not been trained yet")
            End If
            If String.IsNullOrEmpty(gene) Then
                Throw New ArgumentException("a target gene is required", NameOf(gene))
            End If
            If steps < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(steps), steps, "the step count must be positive")
            End If
            If tolerance <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(tolerance), tolerance, "the tolerance must be positive")
            End If
        End Sub

        ''' <summary>去重、按升序排列，并校验每个 ω 都落在 (0, 1]。</summary>
        Private Function NormalizeOmegas(omegas As Double()) As Double()
            Dim values As Double() = If(omegas Is Nothing OrElse omegas.Length = 0, DefaultOmegas, omegas)

            values = values _
                .Where(Function(v) Not Double.IsNaN(v) AndAlso v > 0.0 AndAlso v <= 1.0) _
                .Distinct() _
                .OrderBy(Function(v) v) _
                .ToArray()

            If values.Length = 0 Then
                Throw New ArgumentException("at least one relaxation value in (0, 1] is required", NameOf(omegas))
            End If

            Return values
        End Function

        ''' <summary>去重、按升序排列，并校验每个步数都为正。</summary>
        Private Function NormalizeSteps(stepsList As Integer()) As Integer()
            Dim values As Integer() = If(stepsList Is Nothing OrElse stepsList.Length = 0, DefaultSteps, stepsList)

            values = values _
                .Where(Function(v) v > 0) _
                .Distinct() _
                .OrderBy(Function(v) v) _
                .ToArray()

            If values.Length = 0 Then
                Throw New ArgumentException("at least one positive step count is required", NameOf(stepsList))
            End If

            Return values
        End Function

        ''' <summary>未指定参考 ω 时取最接近 0.5 的那个；指定了就必须在列表里。</summary>
        Private Function ResolveReferenceOmega(omegas As Double(), requested As Double) As Double
            If Double.IsNaN(requested) Then
                Return omegas.OrderBy(Function(v) std.Abs(v - 0.5)).First()
            End If

            Dim index As Integer = IndexOfOmega(omegas, requested)

            If index < 0 Then
                Throw New ArgumentException($"the reference relaxation {requested} is not in the sweep list")
            End If

            Return omegas(index)
        End Function

        Private Function IndexOfOmega(omegas As Double(), value As Double) As Integer
            For i As Integer = 0 To omegas.Length - 1
                If std.Abs(omegas(i) - value) <= 0.000000001 Then
                    Return i
                End If
            Next

            Return -1
        End Function

    End Module

End Namespace
