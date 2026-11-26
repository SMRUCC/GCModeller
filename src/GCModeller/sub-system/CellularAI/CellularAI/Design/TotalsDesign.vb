Imports CellularAI.Model
Imports CellularAI.Planning
Imports CellularAI.SingleCell
Imports CellularAI.Synthesis
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports std = System.Math

Namespace Design

    ''' <summary>设计层读取的池化证据：两条证据流 + 残差剖面。</summary>
    Public Class DesignEffects

        Public Property L2 As Double()()
        Public Property L2Tot As Double()()
        Public Property Z As Double()()
        Public Property ZTot As Double()()
        Public Property Profile As Double()()

    End Class

    ''' <summary>一次固定点求解的报告。</summary>
    Public Class DesignReport

        ''' <summary>最终边距。</summary>
        Public Property Margin As Double

        ''' <summary>边距是否命中目标。</summary>
        Public Property MarginReached As Boolean

        ''' <summary>信用目标是否收敛（相邻两轮的相对变化 &lt; <c>fixed_rtol</c>）。</summary>
        Public Property Converged As Boolean

        ''' <summary>各轮的信用目标。</summary>
        Public Property Targets As Double()

        ''' <summary>各轮的边距。</summary>
        Public Property Margins As Double()

        ''' <summary>下一轮将使用的信用目标（供诊断）。</summary>
        Public Property NextTarget As Double

        ''' <summary>合成侧的矩与真实侧均值的比值。</summary>
        Public Property ChatOverProxy As Double

        ''' <summary>最后一轮实测离散度 / 信用目标。</summary>
        Public Property CreditShare As Double

        ''' <summary>幅度搜索是否被截断。</summary>
        Public Property AmplitudeCapped As Boolean

    End Class

    ''' <summary>
    ''' 一个上下文的<b>整数基因总量</b>设计，以及把设计对齐到"信用"的固定点迭代。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 设计产出的是"每个扰动在 <c>n_cells</c> 个细胞、对照深度下的逐基因总计数"，
    ''' 形状由 <see cref="ShapedDesign.Fit"/> 给出，尺度由<b>边距</b>决定：
    ''' <c>margin × credit</c> 是离散度目标，而 <c>credit</c> 是"合成得再准也拿不到更多分"的上界。
    ''' </para>
    ''' <para>
    ''' <b>为什么是固定点而不是一次求解</b>：目标本身依赖合成结果——
    ''' <c>target = Σ_p w_p·E[min(Ĉ_p, C_p)]</c>，其中 <c>Ĉ_p</c> 只有把细胞合成出来才知道。
    ''' 所以流程是"解边距 → 合成 → 重算 <c>Ĉ</c> → 更新目标"，最多 <c>fixed_steps</c> 轮，
    ''' 相邻目标相对变化小于 <c>fixed_rtol</c> 时提前收敛。
    ''' </para>
    ''' <para>
    ''' <b>未收敛必须报告</b>：达到轮数上限仍未收敛时把 <c>Converged = False</c> 写进报告，
    ''' 由门控拒绝该上下文，而不是静默接受一个"还在漂移"的设计。
    ''' </para>
    ''' </remarks>
    Public Class TotalsDesign

        Private ReadOnly m_engine As SynthesisEngine
        Private ReadOnly m_effects As DesignEffects
        Private ReadOnly m_calls As CallEntry()
        Private ReadOnly m_options As TotalsOptions
        Private ReadOnly m_model As CellularAiModel

        ''' <summary>"可用"掩码（基因×扰动）。</summary>
        Public ReadOnly Property Mask As Boolean()()

        ''' <summary>逐扰动的评分权重 <c>1/G_p</c>。</summary>
        Public ReadOnly Property Weights As Double()

        ''' <summary>离散度函数 <c>L ↦ spread</c>。</summary>
        Public ReadOnly Property Spread As Func(Of Double()(), Double)

        ''' <summary>幅度设计 <c>amp × l2</c> 的幅度。</summary>
        Public Property Amplitude As Double

        ''' <summary>幅度搜索是否被截断。</summary>
        Public Property AmplitudeCapped As Boolean

        ''' <summary>每个扰动 <c>n_cells</c> 个细胞的计数总量。</summary>
        Public Property TotalCounts As Integer

        ''' <summary>控制细胞的主成分。</summary>
        Public Property ControlPcs As ControlPcs

        ''' <summary>构造设计。</summary>
        ''' <param name="controls">对照细胞矩阵</param>
        ''' <param name="engine">控制侧引擎</param>
        ''' <param name="effects">池化证据</param>
        ''' <param name="calls">每个扰动的调用计划（顺序与扰动轴一致）</param>
        ''' <param name="tidx">每个扰动的自身靶基因下标（-1 表示靶不在基因轴上）</param>
        ''' <param name="credit">控制侧的信用（<c>P × mean(draws)</c>）</param>
        ''' <param name="nCells">每个扰动的细胞数</param>
        Public Sub New(controls As CsrMatrix,
                       engine As SynthesisEngine,
                       effects As DesignEffects,
                       calls As CallEntry(),
                       tidx As Integer(),
                       credit As Double,
                       model As CellularAiModel,
                       Optional nCells As Integer = 400)

            If engine Is Nothing Then
                Throw New ArgumentNullException(NameOf(engine))
            End If
            If effects Is Nothing Then
                Throw New ArgumentNullException(NameOf(effects))
            End If

            m_engine = engine
            m_effects = effects
            m_calls = If(calls, New CallEntry() {})
            m_options = model.Totals
            m_model = model

            Me.Mask = ProfileSpread.OwnMask(engine.GeneCount, effects.L2(0).Length, tidx)
            Me.Weights = ProfileSpread.Weights(engine.GeneCount, tidx)
            Me.Spread = ProfileSpread.Make(engine.Fractions, Me.Mask, model.Guards.Log2Max)
            Me.TotalCounts = CInt(std.Round(nCells * CDbl(engine.Depth)))
            Me.ControlPcs = Design.ControlPcs.Compute(controls, m_options)

            ' 幅度设计：amp × l2，目标是信用的一部分
            Dim amplitudeTarget As Double = m_options.AmpShare * credit
            Dim solution As AmplitudeSolution = MarginSolver.BisectAmp(
                Me.Spread,
                Function(a As Double) As Double()() Return Scale(effects.L2, a) End Function,
                amplitudeTarget)

            Me.Amplitude = solution.Amplitude
            Me.AmplitudeCapped = solution.Capped

            Call Guards.Fired("amp_search_cap", solution.Capped)
        End Sub

        ''' <summary>
        ''' 设计因子 → 整数总量（扰动×基因）：逐扰动把 <c>fc·2^L</c> 换算成"计划基因达到最小 lfc"后的比例，
        ''' 再用最大余数法分摊到 <see cref="TotalCounts"/>。
        ''' </summary>
        Public Function Totals(l2 As Double()()) As Integer()()
            Dim nPerts As Integer = l2(0).Length
            Dim result(nPerts - 1)() As Integer

            For p As Integer = 0 To nPerts - 1
                Dim column(m_engine.GeneCount - 1) As Double

                For g As Integer = 0 To m_engine.GeneCount - 1
                    column(g) = l2(g)(p)
                Next

                Dim call_ As CallEntry = If(p < m_calls.Length, m_calls(p), Nothing)
                Dim guarded As Double()

                If call_ Is Nothing Then
                    guarded = m_engine.GuardL2(column, New Integer() {}, New Double() {}, m_model.Calls.MinLfc)
                Else
                    guarded = m_engine.GuardL2(column, call_.Index, call_.Sign, m_model.Calls.MinLfc)
                End If

                result(p) = Apportion(m_engine.DesignFraction(guarded), Me.TotalCounts)
            Next

            Return result
        End Function

        ''' <summary>在给定边距下求形状设计。</summary>
        ''' <param name="margin">边距（乘到信用上得到离散度目标）</param>
        ''' <param name="credit">该上下文的总信用</param>
        ''' <param name="record">是否把搜索截断记到护栏表（只有被采纳的设计才记）</param>
        Public Function Shaped(margin As Double, credit As Double, Optional record As Boolean = False) As ShapedDesignResult
            Dim result As ShapedDesignResult = ShapedDesign.Fit(
                m_effects.L2Tot, m_effects.ZTot, m_effects.Z, m_effects.Profile,
                m_engine.Fractions, Me.Mask, Me.ControlPcs, Me.Spread,
                margin * credit, m_options)

            If record Then
                Call Guards.Fired("amp_search_cap", result.AmplitudeCapped)
                Call Guards.Fired("shape_search_cap", result.CpCapped, 2)
                Call Guards.Fired("shape_search_cap", result.PwCapped, 2)
            End If

            Return result
        End Function

        ''' <summary>
        ''' 固定点求解：解边距 → 合成 → 用合成结果重算信用目标，最多 <c>fixed_steps</c> 轮。
        ''' </summary>
        ''' <param name="credit">初始信用（<c>P × mean(control_moments)</c>）</param>
        ''' <param name="draws">控制侧的矩样本</param>
        ''' <param name="synthesize">给定整数总量（扰动×基因）返回每个扰动的细胞矩阵</param>
        Public Function Solve(credit As Double, draws As Double(),
                              synthesize As Func(Of Double()(), CsrMatrix())) As (Blocks As CsrMatrix(), Report As DesignReport)
            Dim nPerts As Integer = If(m_effects.L2.Length = 0, 0, m_effects.L2(0).Length)

            Dim cHat(nPerts - 1) As Double
            Dim sigmaHat(nPerts - 1) As Double

            For p As Integer = 0 To nPerts - 1
                cHat(p) = Double.PositiveInfinity
            Next

            Dim targets As New List(Of Double)
            Dim margins As New List(Of Double)
            Dim target As Double = MarginSolver.ExpectedCredit(cHat, draws, Me.Weights)
            Dim margin As Double = 1.0
            Dim reached As Boolean = False
            Dim converged As Boolean = False
            Dim blocks As CsrMatrix() = Nothing
            Dim share As Double = 0.0

            For pass As Integer = 1 To std.Max(m_options.FixedSteps, 1)
                Dim solution As MarginSolution = MarginSolver.SolveMargin(
                    Function(m As Double) As Double
                        Dim totals As Double()() = ProfileSpread.ToDouble(Me.Totals(Me.Shaped(m, credit).L))

                        Return ProfileSpread.FromTotals(totals, Me.Mask, Me.Weights)
                    End Function,
                    target, If(margin > 0.0, margin, 1.0))

                margin = solution.Margin
                reached = solution.Reached

                Dim shapedDesign As ShapedDesignResult = Me.Shaped(margin, credit, record:=True)
                Dim totalsAll As Integer()() = Me.Totals(shapedDesign.L)
                Dim totalsDouble As Double()() = ProfileSpread.ToDouble(totalsAll)

                blocks = synthesize(totalsDouble)
                share = ProfileSpread.FromTotals(totalsDouble, Me.Mask, Me.Weights) / std.Max(target, 1.0E-300)

                For p As Integer = 0 To nPerts - 1
                    cHat(p) = CountStatistics.JackknifeMoment(blocks(p), 512, ProfileSpread.TS)

                    If Not Double.IsFinite(cHat(p)) Then
                        Throw New InvalidOperationException($"non-finite jackknife moment of the synthesized cells of perturbation #{p}")
                    End If
                Next

                Call targets.Add(target)
                Call margins.Add(margin)

                Dim nextTarget As Double = MarginSolver.ExpectedCredit(cHat, draws, Me.Weights)

                converged = std.Abs(nextTarget / std.Max(target, 1.0E-300) - 1.0) < m_options.FixedRtol

                If converged Then
                    target = nextTarget
                    Exit For
                End If

                target = nextTarget
            Next

            Dim proxy As Double = 0.0

            If draws IsNot Nothing AndAlso draws.Length > 0 Then
                Dim mean As Double = 0.0

                For Each draw As Double In draws
                    mean += draw
                Next

                proxy = cHat.Average / std.Max(mean / draws.Length, 1.0E-300)
            End If

            Dim report As New DesignReport With {
                .Margin = margin,
                .MarginReached = reached,
                .Converged = converged,
                .Targets = targets.ToArray,
                .Margins = margins.ToArray,
                .NextTarget = target,
                .ChatOverProxy = proxy,
                .CreditShare = share,
                .AmplitudeCapped = Me.AmplitudeCapped
            }

            Return (blocks, report)
        End Function

#Region "helpers"

        ''' <summary>逐元素缩放 <c>a·L</c>。</summary>
        Private Shared Function Scale(values As Double()(), factor As Double) As Double()()
            Dim result(values.Length - 1)() As Double

            For g As Integer = 0 To values.Length - 1
                result(g) = New Double(values(g).Length - 1) {}

                For p As Integer = 0 To values(g).Length - 1
                    Dim v As Double = values(g)(p)

                    result(g)(p) = If(Double.IsNaN(v), 0.0, factor * v)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 最大余数法：按比例分摊到整数总量，余数按小数部分从大到小分配（并列按出现顺序，保证确定性）。
        ''' </summary>
        Public Shared Function Apportion(fractions As Double(), total As Integer) As Integer()
            Dim n As Integer = fractions.Length
            Dim result(n - 1) As Integer
            Dim raw(n - 1) As Double
            Dim remainder(n - 1) As Double
            Dim sum As Double = 0.0

            For i As Integer = 0 To n - 1
                sum += std.Max(fractions(i), 0.0)
            Next

            If sum <= 0.0 OrElse total <= 0 Then
                Return result
            End If

            Dim assigned As Integer = 0

            For i As Integer = 0 To n - 1
                raw(i) = std.Max(fractions(i), 0.0) / sum * total
                result(i) = CInt(std.Floor(raw(i)))
                remainder(i) = raw(i) - result(i)
                assigned += result(i)
            Next

            Dim left As Integer = total - assigned

            If left > 0 Then
                Dim order As Integer() = Enumerable.Range(0, n).
                    OrderByDescending(Function(i) remainder(i)).ThenBy(Function(i) i).ToArray

                For i As Integer = 0 To std.Min(left, n) - 1
                    result(order(i)) += 1
                Next
            End If

            Return result
        End Function

#End Region

    End Class
End Namespace
