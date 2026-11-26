Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Design
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Evidence
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Placement
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Planning
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Randomizer
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Residual
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Scoring
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Synthesis
Imports std = System.Math

''' <summary>
''' 单上下文的端到端装配：控制细胞 + 摘要栈 + 任务配置 → 预测计数矩阵 + 诊断报告。
''' </summary>
''' <remarks>
''' <para>
''' 这是本库唯一的公开入口。它把七个阶段串起来，并在最后执行门控：
''' </para>
''' <code>
''' 池化 → 计划 → 残差（可选）→ 总量设计（固定点）→ 合成 → 评分 → 门控
''' </code>
''' <para>
''' <b>不做文件 IO</b>：输入是上游装配好的内存对象，输出是 CSR 计数矩阵与报告对象。
''' </para>
''' <para>
''' <b>可复现</b>：所有随机性来自 "seed" 与扰动名派生的种子，
''' 因此同一组参数重跑两次得到的计数矩阵逐元素相等。
''' </para>
''' </remarks>
Public Module ContextPipeline

    ''' <summary>一次上下文预测的结果。</summary>
    Public Class PredictionResult

        ''' <summary>预测的细胞矩阵（每个扰动一个块，块内 细胞 × 基因）。</summary>
        Public Property Blocks As CsrMatrix()

        ''' <summary>拼接后的矩阵（所有扰动按顺序叠放）。</summary>
        Public Property Matrix As CsrMatrix

        ''' <summary>诊断报告。</summary>
        Public Property Report As PredictionReport

    End Class

    ''' <summary>
    ''' 预测一个上下文。
    ''' </summary>
    ''' <param name="controls">该上下文的对照细胞矩阵（细胞 × 基因）</param>
    ''' <param name="genes">基因轴</param>
    ''' <param name="perturbations">扰动轴</param>
    ''' <param name="bundle">训练摘要（内存版）</param>
    ''' <param name="model">模型参数</param>
    ''' <param name="medianDegs">中位调用基因数（<c>median_degs</c>）</param>
    ''' <param name="callDepth">调用的总深度（<c>calls_depth</c>）</param>
    ''' <param name="nCells">每个扰动的细胞数</param>
    ''' <param name="seed">任务种子</param>
    ''' <param name="reference">可选的参考侧统计（提供时会计算六项指标）</param>
    Public Function Predict(controls As CsrMatrix,
                            genes As String(),
                            perturbations As String(),
                            bundle As SummaryBundle,
                            model As CellularAiModel,
                            medianDegs As Double,
                            callDepth As Double,
                            Optional context As String = "context",
                            Optional nCells As Integer = 400,
                            Optional seed As Integer = 0,
                            Optional reference As SideStats = Nothing,
                            Optional withResidual As Boolean = False) As PredictionResult
        If controls Is Nothing Then
            Throw New ArgumentNullException(NameOf(controls))
        End If
        If bundle Is Nothing Then
            Throw New ArgumentNullException(NameOf(bundle))
        End If
        If model Is Nothing Then
            Throw New ArgumentNullException(NameOf(model))
        End If

        Dim engine As SynthesisEngine = SynthesisEngine.Create(controls, model)
        Dim meanCpm As Double() = engine.MeanCpm

        ' ---- 1) 池化 ----
        Dim evidence As ContextEvidence = MetaPooling.Pool(bundle, genes, perturbations, meanCpm, model)
        Dim effects As New DesignEffects With {
            .L2 = evidence.L2,
            .L2Tot = evidence.L2Tot,
            .Z = evidence.Z,
            .ZTot = evidence.ZTot,
            .Profile = evidence.L2
        }

        If withResidual Then
            Try
                effects.Profile = ResidualProfile(bundle, controls, genes, perturbations, evidence, model)
            Catch ex As Exception
                Call Guards.Fired("residual_failed", True)
            End Try
        End If

        ' ---- 2) 计划 ----
        Dim sizes As Integer() = PlanSizes(engine, effects, genes, perturbations, medianDegs, callDepth, model)

        ' ---- 3) 总量设计 ----
        Dim tidx(perturbations.Length - 1) As Integer

        For p As Integer = 0 To perturbations.Length - 1
            tidx(p) = Array.IndexOf(genes, perturbations(p))
        Next

        Dim calls(perturbations.Length - 1) As CallEntry
        Dim plan As Dictionary(Of String, CallEntry) = CallPlan.SelectCalls(
            effects.L2, evidence.Z, meanCpm, genes, perturbations, callDepth, model, sizes:=sizes)

        For p As Integer = 0 To perturbations.Length - 1
            calls(p) = plan(perturbations(p))
        Next

        Dim draws As Double() = MarginSolver.ControlMoments(controls, model.Totals, nCells)
        Dim credit As Double = perturbations.Length * If(draws.Length > 0, draws.Average, 0.0)

        Dim design As New TotalsDesign(controls, engine, effects, calls, tidx, credit, model, nCells)

        ' ---- 4) 合成（固定点循环内部按扰动并行）----
        Dim jobs As New List(Of SynthesisJob)

        Dim synthesize As Func(Of Double()(), CsrMatrix()) =
            Function(totals As Double()())
                Call jobs.Clear()

                For p As Integer = 0 To perturbations.Length - 1
                    Dim entry As CallEntry = calls(p)

                    Call jobs.Add(New SynthesisJob With {
                        .Name = perturbations(p),
                        .totals = Slice(totals, p),
                        .Libraries = Enumerable.Repeat(CDbl(engine.Depth), nCells).ToArray,
                        .Targets = TargetsFromTotals(totals, p, nCells, perturbations(p)),
                        .PlanIndex = entry.Index,
                        .PlanSign = entry.Sign,
                        .PlanPriority = entry.Priority,
                        .NudgeSign = entry.NudgeSign,
                        .NudgeStrength = entry.NudgeStrength,
                        .seed = PerturbationSeed.Combine(seed, perturbations(p))
                    })
                Next

                Return CellSynthesizer.SynthesizeAll(engine, jobs.ToArray, model, 1)
            End Function

        Dim solved = design.Solve(credit, draws, synthesize)
        Dim blocks As CsrMatrix() = solved.Blocks

        ' ---- 5) 评分（可选）----
        Dim scores As New Dictionary(Of String, Double)(StringComparer.Ordinal)

        If reference IsNot Nothing AndAlso blocks IsNot Nothing Then
            Dim predicted As SideStats = Metrics.BuildSide(blocks, perturbations, genes,
                                                           Function(g As Integer) engine.SortedCpm(g),
                                                           Function(g As Integer) engine.TieSum(g),
                                                           withControl:=False)

            scores = Metrics.Score(predicted, reference, perturbations)
        End If

        ' ---- 6) 门控 ----
        Dim report As PredictionReport = Evaluate(context, blocks, perturbations, genes, nCells,
                                                  engine, solved.Report, design, effects, evidence,
                                                  scores, model)

        Return New PredictionResult With {
            .Blocks = blocks,
            .Matrix = If(blocks Is Nothing OrElse blocks.Length = 0, Nothing, Stack(blocks)),
            .Report = report
        }
    End Function

#Region "plan & residual"

    ''' <summary>计划尺寸：调用选择器按曲线尺寸 + 功效重排 + 相近度抬升给出每扰动的调用数。</summary>
    Private Function PlanSizes(engine As SynthesisEngine, effects As DesignEffects,
                               genes As String(), perturbations As String(),
                               medianDegs As Double, depth As Double,
                               model As CellularAiModel) As Integer()

        Dim breadth(perturbations.Length - 1) As Double

        For p As Integer = 0 To perturbations.Length - 1
            Dim count As Double = 0.0

            For g As Integer = 0 To genes.Length - 1
                If engine.Eligible(g) AndAlso Array.IndexOf(perturbations, genes(g)) <> p AndAlso
                    Not Double.IsNaN(effects.L2(g)(p)) AndAlso std.Abs(effects.L2(g)(p)) > model.Calls.MinLfc Then

                    count += 1.0
                End If
            Next

            breadth(p) = count
        Next

        Dim capacity As Integer = 0

        For g As Integer = 0 To genes.Length - 1
            If engine.Eligible(g) Then
                capacity += 1
            End If
        Next

        Dim curve As Integer() = CallSizing.CurveSizes(breadth, capacity, model, medianDegs)

        Return curve
    End Function

    ''' <summary>把残差剖面融合回池化效应（需要训练记录的效应栈与控制计数）。</summary>
    Private Function ResidualProfile(bundle As SummaryBundle, controls As CsrMatrix,
                                   genes As String(), perturbations As String(),
                                   evidence As ContextEvidence, model As CellularAiModel) As Double()()
        Dim stacked = bundle.StackEffects(perturbations)
        Dim nRows As Integer = stacked.Lines.Length
        Dim stack(nRows - 1)()() As Double
        Dim counts(nRows - 1)() As Double

        For t As Integer = 0 To nRows - 1
            Dim row As SummaryRow = bundle.Catalog.PrimaryRow(stacked.Lines(t))

            stack(t) = SummaryBundle.AlignPanel(row.EffectGenes, row.EffectPerts, row.Effect, genes, perturbations)
            counts(t) = ControlCounts(row, genes)
        Next

        Return ControlResidual.ProfileEffects(evidence.L2, evidence.L2Tot, stack, counts,
                                              genes, perturbations,
                                              controls.ColumnSums(), genes, perturbations, model.Totals)
    End Function

    Private Function ControlCounts(row As SummaryRow, genes As String()) As Double()
        Dim result(genes.Length - 1) As Double

        If row.Control Is Nothing Then
            For i As Integer = 0 To result.Length - 1
                result(i) = Double.NaN
            Next

            Return result
        End If

        Dim lookup As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        For i As Integer = 0 To row.Control.Count - 1
            lookup(row.Control.Genes(i)) = i
        Next

        For g As Integer = 0 To genes.Length - 1
            Dim at As Integer = -1

            result(g) = If(lookup.TryGetValue(genes(g), at), row.Control.Counts(at), Double.NaN)
        Next

        Return result
    End Function

#End Region

#Region "gates"

    ''' <summary>逐项门控并生成报告。</summary>
    Private Function Evaluate(context As String, blocks As CsrMatrix(), perturbations As String(),
                              genes As String(), nCells As Integer, engine As SynthesisEngine,
                              design As DesignReport, plan As TotalsDesign, effects As DesignEffects,
                              evidence As ContextEvidence, scores As Dictionary(Of String, Double),
                              model As CellularAiModel) As PredictionReport
        Dim gates As New List(Of GateResult)

        ' 1) 形状
        Dim shapeOk As Boolean = blocks IsNot Nothing AndAlso blocks.Length = perturbations.Length

        If shapeOk Then
            For i As Integer = 0 To blocks.Length - 1
                If blocks(i).RowDimension <> nCells OrElse blocks(i).ColumnDimension <> genes.Length Then
                    shapeOk = False
                End If
            Next
        End If

        Call gates.Add(Gate("axes", shapeOk, $"{perturbations.Length} block(s) of {nCells} x {genes.Length}"))

        ' 2) 整数性 / 非负 / 2^24
        Dim integral As Boolean = True
        Dim bounded As Boolean = True

        If blocks IsNot Nothing Then
            For i As Integer = 0 To blocks.Length - 1
                For k As Integer = 0 To blocks(i).Values.Length - 1
                    Dim value As Double = blocks(i).Values(k)

                    If value < 0.0 OrElse value <> std.Floor(value) Then
                        integral = False
                    End If
                    If value >= 16777216.0 Then
                        bounded = False
                    End If
                Next
            Next
        End If

        Call gates.Add(Gate("integral", integral, "all counts are non-negative integers"))
        Call gates.Add(Gate("count_limit", bounded, "all counts stay below 2^24"))

        ' 3) 信用区间与收敛
        Dim share As Double = design.CreditShare
        Dim band As Boolean = share >= model.Guards.CreditBandLo AndAlso share <= model.Guards.CreditKink

        Call gates.Add(Gate("credit_band", band, $"realized/target = {share:F4} in [{model.Guards.CreditBandLo}, {model.Guards.CreditKink}]"))
        Call gates.Add(Gate("converged", design.Converged, $"converged = {design.Converged}, {design.Targets.Length} pass(es)"))
        Call gates.Add(Gate("margin", design.MarginReached, $"margin = {design.Margin:G6}, reached = {design.MarginReached}"))

        ' 4) 未测靶与护栏
        Call gates.Add(Gate("unmeasured", evidence.Unmeasured.Length <= model.Guards.MaxUnmeasured,
                            $"{evidence.Unmeasured.Length} unmeasured target(s)"))

        Dim guardOk As Boolean = True
        Dim worst As String = ""

        For Each pair As KeyValuePair(Of String, Double()) In evidence.Guards
            Dim limit As Double = 0.0

            If Not model.Guards.MaxHitRate.TryGetValue(pair.Key, limit) Then
                Continue For
            End If

            If pair.Value(2) > limit Then
                guardOk = False
                worst = $"{pair.Key} = {pair.Value(2):P2} > {limit:P2}"
            End If
        Next

        Call gates.Add(Gate("guards", guardOk, If(worst = "", "all guard hit rates within tolerance", worst)))

        ' 5) 评分（提供参考侧时）
        Dim total As Double = 0.0

        If scores.Count > 0 Then
            total = Metrics.AnchorScore(scores)

            Call gates.Add(Gate("fid", If(scores.ContainsKey("fid"), scores("fid") > 0.0, True),
                                If(scores.ContainsKey("fid"), $"fid = {scores("fid"):F4}", "not evaluated")))
        End If

        Dim accepted As Boolean = gates.All(Function(g) g.Passed)

        Return New PredictionReport With {
            .Context = context,
            .Accepted = accepted,
            .Gates = gates.ToArray,
            .Perturbations = perturbations,
            .NCells = nCells,
            .Depth = engine.Depth,
            .GeneCount = genes.Length,
            .Credit = design.CreditShare * If(plan.Weights.Length > 0, 1.0, 1.0),
            .Margin = design.Margin,
            .MarginReached = design.MarginReached,
            .Converged = design.Converged,
            .CreditShare = share,
            .Amplitude = plan.Amplitude,
            .Scores = scores,
            .Total = total,
            .Unmeasured = evidence.Unmeasured,
            .Guards = evidence.Guards
        }
    End Function

    Private Function Gate(name As String, passed As Boolean, detail As String) As GateResult
        Return New GateResult With {.Name = name, .Passed = passed, .Detail = detail}
    End Function

#End Region

#Region "helpers"

    Private Function Slice(totals As Double()(), p As Integer) As Double()
        Dim column(totals(0).Length - 1) As Double

        For g As Integer = 0 To column.Length - 1
            column(g) = totals(p)(g)
        Next

        Return column
    End Function

    ''' <summary>
    ''' 目标平均 CPM：把"设计总量 ÷ (nCells × 10⁴)"近似成该基因的目标表达水平，
    ''' 用于放置层的平均表达量约束。
    ''' </summary>
    Private Function TargetsFromTotals(totals As Double()(), p As Integer, nCells As Integer,
                                       pert As String) As Double()
        Dim result(totals(0).Length - 1) As Double
        Dim scale As Double = 1000000.0 / (std.Max(nCells, 1) * 10000.0)

        For g As Integer = 0 To result.Length - 1
            result(g) = totals(p)(g) * scale
        Next

        Return result
    End Function

    ''' <summary>把多个块按行拼接成一个 CSR。</summary>
    Private Function Stack(blocks As CsrMatrix()) As CsrMatrix
        Return CsrMatrix.Stack(blocks)
    End Function

#End Region

End Module
