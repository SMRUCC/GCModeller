Imports System
Imports System.Collections.Generic
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports std = System.Math

Namespace Hybrid

    ''' <summary>
    ''' 概率布尔网络采样：把「模型结构的不确定性」传播到扰动结果上。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 流程：按 CoD 概率为每个基因抽一条布尔规则 → 得到一张确定性布尔网络实例 →
    ''' 从扰动后的初态跑到吸引子（有限状态空间必然停下）→ 得到该样本的定性结论。
    ''' 连续层再把这个定性结论作为**热启动**（<see cref="HybridOptions.ModeBiasWeight"/>），
    ''' 或者只用来统计符号一致率（权重为 0 时，数值结论保持确定性单点解）。
    ''' </para>
    ''' <para>
    ''' 「按规则采样」而不是「给数值抖噪声」的差别很重要：
    ''' 前者衡量的是「换一条同样可信的规则，结论会不会翻转」，
    ''' 后者只是给同一个模型加噪声，得到的置信区间没有统计意义。
    ''' </para>
    ''' <para>
    ''' 成本：每个样本的布尔推进是 <c>O(步数 · 基因数)</c>；
    ''' 连续求解只在 <see cref="HybridOptions.ModeBiasWeight"/> &gt; 0 时逐样本执行，
    ''' 否则复用确定性参考解，采样开销只落在布尔层。
    ''' </para>
    ''' </remarks>
    Public Class PbnEnsemble

        ''' <summary>布尔推进的最大步数（吸引子通常在 10 步内出现）。</summary>
        Private Const MaxBooleanSteps As Integer = 64

        ''' <summary>循环检测的回看窗口。</summary>
        Private Const CycleWindow As Integer = 8

        Private ReadOnly _network As FuzzyGeneNetwork
        Private ReadOnly _prior As PbnRegulatorPrior
        Private ReadOnly _config As FuzzyGeneConfig

        ''' <summary>绑定网络与 PBN 层。</summary>
        Public Sub New(network As FuzzyGeneNetwork, prior As PbnRegulatorPrior, config As FuzzyGeneConfig)
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If
            If prior Is Nothing Then
                Throw New ArgumentNullException(NameOf(prior))
            End If

            _network = network
            _prior = prior
            _config = If(config, network.Config)
        End Sub

        ''' <summary>
        ''' 执行集成预测。
        ''' </summary>
        ''' <param name="baseState">基线（原始表达量），用于确定布尔初态</param>
        ''' <param name="clamps">钳制条件（原始表达量）</param>
        ''' <param name="reference">确定性单点解；<c>Nothing</c> 时退化为纯定性统计</param>
        ''' <param name="nSamples">采样次数；&lt;= 1 时直接返回单点结果</param>
        ''' <param name="referenceConverged">确定性参考解是否收敛</param>
        ''' <param name="seed">随机种子（固定种子 = 结果可复现）</param>
        Public Function Predict(baseState As Double(),
                                clamps As Dictionary(Of String, Double),
                                reference As Double(),
                                nSamples As Integer,
                                Optional referenceConverged As Boolean = True,
                                Optional seed As Integer = 20261119) As EnsemblePerturbationResult

            Dim genes As String() = _prior.GeneNames
            Dim result As New EnsemblePerturbationResult With {
                .GeneNames = CType(_network.GeneNames.Clone(), String()),
                .Reference = reference
            }

            If genes Is Nothing OrElse genes.Length = 0 Then
                result.Samples = 0
                result.Notes = "the PBN layer is empty"
                result.BooleanHighFraction = New Double(-1) {}

                Return result
            End If

            If nSamples <= 1 Then
                result.Samples = If(reference Is Nothing, 0, 1)
                result.Mean = reference
                result.LowerBound = reference
                result.UpperBound = reference
                result.StandardDeviation = New Double(_network.NGene - 1) {}
                result.SignAgreement = NaNFilled(_network.NGene)
                result.BooleanHighFraction = NaNFilled(genes.Length)
                result.ConvergedFraction = If(referenceConverged, 1.0, 0.0)
                result.Notes = "nSamples <= 1: the ensemble degenerates to the deterministic solution"

                Return result
            End If

            Dim random As New Random(seed)
            Dim n As Integer = genes.Length
            Dim initial As Boolean() = Binarize(baseState, genes)
            Dim clampBits As Dictionary(Of Integer, Boolean) = ResolveClampBits(clamps, genes)
            Dim weight As Double = std.Max(0.0, std.Min(1.0, _config.Hybrid.ModeBiasWeight))
            Dim samples As New List(Of Double())(nSamples)
            Dim highCount(n - 1) As Double
            Dim periods As New Dictionary(Of Integer, Integer)()
            Dim converged As Integer = 0

            For s As Integer = 1 To nSamples
                Dim net As Dictionary(Of String, PbnRule) = _prior.SampleNetwork(random)
                Dim period As Integer = 0
                Dim final As Boolean() = RunAttractor(initial, clampBits, net, genes, period)

                If periods.ContainsKey(period) Then
                    periods(period) += 1
                Else
                    periods(period) = 1
                End If

                For c As Integer = 0 To n - 1
                    If final(c) Then
                        highCount(c) += 1.0
                    End If
                Next

                Dim state As Double()

                If weight > 0.0 Then
                    state = SolveWithMode(baseState, clamps, final, genes, weight)

                    If state IsNot Nothing Then
                        converged += 1
                    Else
                        state = reference
                    End If
                Else
                    state = reference
                End If

                If state IsNot Nothing Then
                    Call samples.Add(state)
                End If
            Next

            result.Samples = samples.Count
            result.AttractorPeriods = periods
            result.ConvergedFraction = If(weight > 0.0,
                converged / CDbl(nSamples),
                If(referenceConverged AndAlso reference IsNot Nothing, 1.0, 0.0))

            Dim highFraction(n - 1) As Double

            For c As Integer = 0 To n - 1
                highFraction(c) = highCount(c) / nSamples
            Next

            result.BooleanHighFraction = highFraction

            If samples.Count = 0 Then
                result.Notes = "no continuous sample was produced"
                result.Mean = reference
                result.LowerBound = reference
                result.UpperBound = reference
                result.StandardDeviation = New Double(_network.NGene - 1) {}
                result.SignAgreement = NaNFilled(_network.NGene)

                Return result
            End If

            Call Aggregate(result, samples, reference)

            If weight <= 0.0 Then
                result.Notes = "ModeBiasWeight = 0: the numeric result is the deterministic solution; " &
                               "the ensemble only reports the boolean attractor distribution and the sign agreement"
            End If

            Return result
        End Function

        ''' <summary>汇总均值/标准差/分位/符号一致率。</summary>
        Private Shared Sub Aggregate(result As EnsemblePerturbationResult,
                                     samples As List(Of Double()),
                                     reference As Double())
            Dim genes As Integer = samples(0).Length
            Dim mean(genes - 1) As Double
            Dim sd(genes - 1) As Double
            Dim lower(genes - 1) As Double
            Dim upper(genes - 1) As Double
            Dim sign(genes - 1) As Double
            Dim values(samples.Count - 1) As Double

            For g As Integer = 0 To genes - 1
                Dim sum As Double = 0.0

                For i As Integer = 0 To samples.Count - 1
                    values(i) = samples(i)(g)
                    sum += values(i)
                Next

                mean(g) = sum / samples.Count

                Dim variance As Double = 0.0

                For i As Integer = 0 To samples.Count - 1
                    Dim d As Double = values(i) - mean(g)

                    variance += d * d
                Next

                sd(g) = std.Sqrt(variance / samples.Count)

                Call System.Array.Sort(values)

                lower(g) = values(CInt(std.Floor(0.05 * (values.Length - 1))))
                upper(g) = values(CInt(std.Ceiling(0.95 * (values.Length - 1))))

                ' 符号一致率：与该基因「平均变化方向」一致的样本比例
                If reference IsNot Nothing Then
                    Dim expected As Double = mean(g) - reference(g)
                    Dim direction As Double = std.Sign(expected)

                    If direction <> 0.0 Then
                        Dim same As Integer = 0

                        For i As Integer = 0 To samples.Count - 1
                            If std.Sign(samples(i)(g) - reference(g)) = direction Then
                                same += 1
                            End If
                        Next

                        sign(g) = same / CDbl(samples.Count)
                    Else
                        sign(g) = Double.NaN
                    End If
                Else
                    sign(g) = Double.NaN
                End If
            Next

            result.Mean = mean
            result.StandardDeviation = sd
            result.LowerBound = lower
            result.UpperBound = upper
            result.SignAgreement = sign
        End Sub

        ''' <summary>把基线二值化（以训练均值为界）。</summary>
        Private Function Binarize(baseState As Double(), genes As String()) As Boolean()
            Dim bits(genes.Length - 1) As Boolean

            For c As Integer = 0 To genes.Length - 1
                Dim g As Integer = _network.IndexOfGene(genes(c))

                If g >= 0 AndAlso baseState IsNot Nothing Then
                    bits(c) = baseState(g) > _network.Means(g)
                End If
            Next

            Return bits
        End Function

        ''' <summary>把钳制条件换算成布尔位（以训练均值为界）。</summary>
        Private Function ResolveClampBits(clamps As Dictionary(Of String, Double), genes As String()) As Dictionary(Of Integer, Boolean)
            Dim bits As New Dictionary(Of Integer, Boolean)()

            If clamps Is Nothing Then
                Return bits
            End If

            For Each item As KeyValuePair(Of String, Double) In clamps
                Dim c As Integer = _prior.GeneIndexOf(item.Key)

                If c < 0 OrElse c >= genes.Length Then
                    Continue For
                End If

                Dim g As Integer = _network.IndexOfGene(genes(c))

                If g < 0 Then
                    Continue For
                End If

                bits(c) = item.Value > _network.Means(g)
            Next

            Return bits
        End Function

        ''' <summary>把状态里的钳制位写成实验施加值（钳制基因不参与规则更新）。</summary>
        Private Shared Sub ApplyClamps(state As Boolean(), clampBits As Dictionary(Of Integer, Boolean))
            For Each item As KeyValuePair(Of Integer, Boolean) In clampBits
                If item.Key >= 0 AndAlso item.Key < state.Length Then
                    state(item.Key) = item.Value
                End If
            Next
        End Sub

        ''' <summary>同步推进布尔网络直到检出吸引子。</summary>
        Private Shared Function RunAttractor(initial As Boolean(),
                                             clampBits As Dictionary(Of Integer, Boolean),
                                             net As Dictionary(Of String, PbnRule),
                                             genes As String(),
                                             ByRef period As Integer) As Boolean()
            Dim n As Integer = genes.Length
            Dim state As Boolean() = CType(initial.Clone(), Boolean())

            Call ApplyClamps(state, clampBits)

            Dim history As New List(Of Boolean())()
            Call history.Add(CType(state.Clone(), Boolean()))

            ' 注意：局部变量不能叫 step —— 那是 VB 的关键字（For ... Step）
            For iteration As Integer = 1 To MaxBooleanSteps
                Dim nextState(n - 1) As Boolean

                For c As Integer = 0 To n - 1
                    Dim rule As PbnRule = Nothing

                    If net IsNot Nothing AndAlso net.TryGetValue(genes(c), rule) AndAlso rule IsNot Nothing Then
                        nextState(c) = rule.Evaluate(state)
                    Else
                        ' 没有规则的基因保持不变：布尔层的不确定性不应该被当成「表达归零」
                        nextState(c) = state(c)
                    End If
                Next

                Call ApplyClamps(nextState, clampBits)

                Dim lower As Integer = std.Max(0, history.Count - CycleWindow)

                For i As Integer = history.Count - 1 To lower Step -1
                    If SameState(history(i), nextState) Then
                        period = history.Count - i

                        Return nextState
                    End If
                Next

                Call history.Add(CType(nextState.Clone(), Boolean()))
                state = nextState
            Next

            period = 0

            Return state
        End Function

        ''' <summary>把布尔模式作为热启动，注入连续求解。</summary>
        ''' <remarks>
        ''' 这是**初值**，不是约束：牛顿法在同一个不动点上收敛，因此模式只会影响
        ''' 「落到哪个吸引子」，不会改变方程的根。
        ''' </remarks>
        Private Function SolveWithMode(baseState As Double(),
                                       clamps As Dictionary(Of String, Double),
                                       bits As Boolean(),
                                       genes As String(),
                                       weight As Double) As Double()
            Dim start As Double() = CType(baseState.Clone(), Double())

            For c As Integer = 0 To genes.Length - 1
                Dim g As Integer = _network.IndexOfGene(genes(c))

                If g < 0 Then
                    Continue For
                End If

                Dim target As Double = _network.Means(g) + If(bits(c), 1.0, -1.0) * _network.SDs(g)

                start(g) = (1.0 - weight) * start(g) + weight * target
            Next

            Try
                Dim report As Solver.SteadyStateReport = _network.SteadyStateWithReport(start, clamps)

                Return report.FinalState
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Shared Function SameState(a As Boolean(), b As Boolean()) As Boolean
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then
                Return False
            End If

            For i As Integer = 0 To a.Length - 1
                If a(i) <> b(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Shared Function NaNFilled(count As Integer) As Double()
            Dim values(std.Max(0, count - 1)) As Double

            For i As Integer = 0 To values.Length - 1
                values(i) = Double.NaN
            Next

            Return values
        End Function

    End Class

End Namespace
