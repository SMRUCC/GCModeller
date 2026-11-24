Imports System.Diagnostics
Imports SMRUCC.genomics.Analysis.FuzzyGene.Perturbation
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

Namespace Perturbation.Analysis

    ''' <summary>
    ''' 剂量学分析：剂量-响应曲线与剂量-时间曲面。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 剂量 <c>d ∈ [0, 1]</c>，钳制值 = 基线 × (1 − d)：<c>d = 0</c> 把靶基因钉在基线，
    ''' <c>d = 1</c> 是完全敲除。走 <see cref="InterventionMode.Custom"/>，
    ''' 因此不需要改 <c>KnockdownResidual</c> 之类的全局配置。
    ''' </para>
    ''' <para>
    ''' <b>重要陷阱</b>：<c>d = 0</c> 并<b>不等价于</b>「没有扰动」。此时靶基因被钉在基线
    ''' 不动，而野生型轨迹会从基线自由漂走，所以它的 fold change 恰好为 0，
    ''' 其他基因仍可能因为系统自身的漂移而变化。只有对照轨迹才能说明这一点。
    ''' </para>
    ''' </remarks>
    Public Module DoseScan

        ''' <summary>剂量响应的默认网格（5 个点，含两端）。</summary>
        Public ReadOnly Property DefaultDoses As Double()
            Get
                Return New Double() {0.0, 0.25, 0.5, 0.75, 1.0}
            End Get
        End Property

        ''' <summary>
        ''' 对若干剂量点各求解一次，输出全基因的终态响应。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="gene">被扰动的靶基因</param>
        ''' <param name="doses">剂量点；<c>Nothing</c> 时取 <see cref="DefaultDoses"/></param>
        ''' <param name="steps">每次求解的最大迭代步数</param>
        ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
        ''' <param name="top">保留的 Top-K 基因数（按最大剂量下的 |Z| 排序）；&lt;= 0 或超过基因数表示全部</param>
        ''' <returns>剂量-响应结果</returns>
        ''' <exception cref="MissingMemberException">目标基因不在模型里</exception>
        Public Function DoseResponse(model As FuzzyGeneModel,
                                     gene As String,
                                     Optional doses As Double() = Nothing,
                                     Optional steps As Integer = 60,
                                     Optional omega As Double = Double.NaN,
                                     Optional top As Integer = 50) As DoseResponseResult

            Call CheckArguments(model, gene, steps)

            Dim values As Double() = NormalizeDoses(doses)
            Dim geneIndex As Integer = model.Network.IndexOfGene(gene)

            If geneIndex < 0 Then
                Throw New MissingMemberException($"the target gene '{gene}' is not modelled by this network")
            End If

            Dim baseline As Double = model.Network.BaselineRaw(geneIndex)
            Dim clamps(values.Length - 1) As Double

            For k As Integer = 0 To values.Length - 1
                clamps(k) = baseline * (1.0 - values(k))
            Next

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim n As Integer = model.Network.NGene
            Dim points As Integer = values.Length
            Dim fold(points - 1, n - 1) As Double
            Dim percent(points - 1, n - 1) As Double
            Dim zscores(points - 1, n - 1) As Double
            Dim significant(points - 1, n - 1) As Boolean

            For k As Integer = 0 To points - 1
                Dim trace As PerturbationTrace = model.TracePerturbation(
                    gene, InterventionMode.Custom, steps, omega, clamps(k))

                If trace Is Nothing OrElse trace.Result Is Nothing Then
                    Throw New InvalidOperationException($"the dose {values(k)} could not be evaluated")
                End If

                Dim result As InterventionResult = trace.Result

                For g As Integer = 0 To n - 1
                    fold(k, g) = result.FoldChanges(g)
                    percent(k, g) = result.PercentChanges(g)
                    zscores(k, g) = result.ZScores(g)
                    significant(k, g) = result.IsSignificant(g)
                Next
            Next

            Dim topGenes As Integer() = SelectTopGenes(
                Enumerable.Range(0, n).ToArray(),
                Function(g) std.Abs(zscores(points - 1, g)),
                ResolveTop(top, n))

            watch.Stop()

            Dim response As New DoseResponseResult With {
                .Gene = gene,
                .GeneIndex = geneIndex,
                .Steps = steps,
                .Omega = omega,
                .Doses = values,
                .ClampValues = clamps,
                .GeneNames = CType(model.Network.GeneNames.Clone(), String()),
                .FoldChanges = fold,
                .PercentChanges = percent,
                .ZScores = zscores,
                .Significant = significant,
                .TopGenes = topGenes,
                .Runs = points * 2,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }

            Call response.Validate()

            Return response
        End Function

        ''' <summary>
        ''' 剂量 × 迭代步的二维响应曲面（只保留 Top-K 基因）。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="gene">被扰动的靶基因</param>
        ''' <param name="doses">剂量点；<c>Nothing</c> 时取 <see cref="DefaultDoses"/></param>
        ''' <param name="steps">每次求解的最大迭代步数</param>
        ''' <param name="omega">本次使用的 ω；<c>NaN</c> 取配置值</param>
        ''' <param name="top">保留的基因数（按最大剂量下的 |Z| 排序）</param>
        ''' <returns>曲面结果，元素是 <c>mutant(t) − control(t)</c></returns>
        ''' <remarks>
        ''' 与 <see cref="DoseResponse"/> 各自独立求解；两者都会跑一遍剂量扫描。
        ''' 需要两张表时优先用 <c>AnalysisExporter</c> 的一次性编排，避免重复计算。
        ''' </remarks>
        Public Function DoseTimeSurface(model As FuzzyGeneModel,
                                        gene As String,
                                        Optional doses As Double() = Nothing,
                                        Optional steps As Integer = 60,
                                        Optional omega As Double = Double.NaN,
                                        Optional top As Integer = 20) As DoseTimeSurfaceResult

            Call CheckArguments(model, gene, steps)

            Dim values As Double() = NormalizeDoses(doses)
            Dim geneIndex As Integer = model.Network.IndexOfGene(gene)

            If geneIndex < 0 Then
                Throw New MissingMemberException($"the target gene '{gene}' is not modelled by this network")
            End If

            Dim baseline As Double = model.Network.BaselineRaw(geneIndex)
            Dim clamps(values.Length - 1) As Double

            For k As Integer = 0 To values.Length - 1
                clamps(k) = baseline * (1.0 - values(k))
            Next

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim n As Integer = model.Network.NGene
            Dim points As Integer = values.Length
            Dim traces As New List(Of PerturbationTrace)(points)
            Dim ranking(n - 1) As Double

            For k As Integer = 0 To points - 1
                Dim trace As PerturbationTrace = model.TracePerturbation(
                    gene, InterventionMode.Custom, steps, omega, clamps(k))

                If trace Is Nothing OrElse trace.Mutant Is Nothing OrElse trace.Control Is Nothing Then
                    Throw New InvalidOperationException($"the dose {values(k)} could not be evaluated")
                End If

                Call traces.Add(trace)

                ' 排序只用「最大剂量」那一档的 |Z|：dose 越大响应越明确
                If k = points - 1 Then
                    For g As Integer = 0 To n - 1
                        ranking(g) = std.Abs(trace.Result.ZScores(g))
                    Next
                End If
            Next

            Dim geneIndices As Integer() = SelectTopGenes(
                Enumerable.Range(0, n).ToArray(),
                Function(g) ranking(g),
                ResolveTop(top, n))
            Dim k_ As Integer = geneIndices.Length
            Dim frames As Integer = traces(0).Mutant.NFrames

            For k As Integer = 1 To points - 1
                If traces(k).Mutant.NFrames < frames Then
                    frames = traces(k).Mutant.NFrames
                End If
            Next

            Dim deltas(points - 1, frames - 1, k_ - 1) As Double
            Dim geneNames(k_ - 1) As String

            For j As Integer = 0 To k_ - 1
                geneNames(j) = model.Network.GeneNames(geneIndices(j))
            Next

            For d As Integer = 0 To points - 1
                Dim trace As PerturbationTrace = traces(d)

                For f As Integer = 0 To frames - 1
                    For j As Integer = 0 To k_ - 1
                        Dim g As Integer = geneIndices(j)

                        deltas(d, f, j) = trace.Mutant.States(f)(g) - trace.Control.States(f)(g)
                    Next
                Next
            Next

            watch.Stop()

            Dim surface As New DoseTimeSurfaceResult With {
                .Gene = gene,
                .Steps = steps,
                .Omega = omega,
                .Doses = values,
                .ClampValues = clamps,
                .GeneNames = geneNames,
                .GeneIndices = geneIndices,
                .Deltas = deltas,
                .NFrames = frames,
                .Runs = points * 2,
                .ElapsedMs = watch.Elapsed.TotalMilliseconds
            }

            Call surface.Validate()

            Return surface
        End Function

        ''' <summary>按评分取前 N 个下标（评分相同则保持原始顺序）。</summary>
        Private Function SelectTopGenes(candidates As Integer(), scoreOf As Func(Of Integer, Double), count As Integer) As Integer()
            Return candidates _
                .OrderByDescending(Function(g) scoreOf(g)) _
                .Take(count) _
                .ToArray()
        End Function

        Private Function ResolveTop(top As Integer, n As Integer) As Integer
            If top <= 0 OrElse top > n Then
                Return n
            End If

            Return top
        End Function

        ''' <summary>去重、升序，并校验每个剂量都落在 [0, 1]。</summary>
        Private Function NormalizeDoses(doses As Double()) As Double()
            Dim values As Double() = If(doses Is Nothing OrElse doses.Length = 0, DefaultDoses, doses)

            values = values _
                .Where(Function(v) Not Double.IsNaN(v) AndAlso v >= 0.0 AndAlso v <= 1.0) _
                .Distinct() _
                .OrderBy(Function(v) v) _
                .ToArray()

            If values.Length = 0 Then
                Throw New ArgumentException("at least one dose value in [0, 1] is required", NameOf(doses))
            End If

            Return values
        End Function

        Private Sub CheckArguments(model As FuzzyGeneModel, gene As String, steps As Integer)
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
        End Sub

    End Module

End Namespace
