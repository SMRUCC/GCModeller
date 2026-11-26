Imports CellularAI.Model
Imports CellularAI.Summary
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace Evidence

    ''' <summary>两个证据流上的因子表达式（解析后的名字序列）。</summary>
    Public Class PoolingOps

        Public Property L2Row As String() = {}
        Public Property L2Col As String() = {}
        Public Property L2Mat As String() = {}

        Public Property ZRow As String() = {}
        Public Property ZCol As String() = {}
        Public Property ZMat As String() = {}

        ''' <summary>从模型配置解析出六个表达式。</summary>
        Public Shared Function FromModel(model As CellularAiModel) As PoolingOps
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            Dim pooling As PoolingOptions = model.Pooling

            Return New PoolingOps With {
                .L2Row = FactorExpression.Parse(pooling.L2Row, FactorAxis.Row),
                .L2Col = FactorExpression.Parse(pooling.L2Col, FactorAxis.Col),
                .L2Mat = FactorExpression.Parse(pooling.L2Mat, FactorAxis.Mat),
                .ZRow = FactorExpression.Parse(pooling.ZRow, FactorAxis.Row),
                .ZCol = FactorExpression.Parse(pooling.ZCol, FactorAxis.Col),
                .ZMat = FactorExpression.Parse(pooling.ZMat, FactorAxis.Mat)
            }
        End Function

    End Class

    ''' <summary>一次池化所需的全部输入。</summary>
    Public Class PoolingRequest

        ''' <summary>任务基因轴对齐后的效应栈（行数×基因数×扰动数，<c>NaN</c> 表示未测）。</summary>
        Public Property Stack As Double()()()

        ''' <summary>与 <see cref="Stack"/> 同形状的双侧概率栈。</summary>
        Public Property Probability As Double()()()

        ''' <summary>逐行的中心化表（顺序与 <see cref="Stack"/> 的第一维一致）。</summary>
        Public Property Tables As OffsetTable()

        ''' <summary>任务基因轴。</summary>
        Public Property Genes As String()

        ''' <summary>任务扰动轴。</summary>
        Public Property Perts As String()

        ''' <summary>逐行的整行倍数（辅助行为 <c>auxiliary_weight</c>，其余为 1）。</summary>
        Public Property Multipliers As Double()

        ''' <summary>因子表达式。</summary>
        Public Property Ops As PoolingOps

        ''' <summary>相对方差地板系数。</summary>
        Public Property VFloorRel As Double = 0.0024

        ''' <summary>细胞系因子。</summary>
        Public Property Factors As PoolingFactors

        ''' <summary>合并后 log2FC 的绝对值上限。</summary>
        Public Property L2AbsMax As Double = 6.25

        ''' <summary>合并后 z 的绝对值上限。</summary>
        Public Property ZAbsMax As Double = 187.5

    End Class

    ''' <summary>
    ''' 三维加权池化：逐基因的 DerSimonian–Laird 随机效应合并 + 加权均值 + Stouffer 合并。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么不能直接用 <c>RandomEffects.PoolMatrix</c></b>：那里的权重是"每个研究一个倍数"
    ''' （<c>WeightMultiplier As Func(Of Integer, Double)</c>），而这里的权重是
    ''' <c>row[t,g] × col[t,p] × mat[t]</c>（逐基因 × 逐扰动），必须自己实现。
    ''' </para>
    ''' <para>
    ''' <b>两遍结构</b>：先用逆响应方差估计逐基因的研究间方差 τ²，再用
    ''' <c>m_g / max(variance + τ², v_floor_rel·m_g)</c> 作为局部权重参与合并。
    ''' τ² 只用逆方差权重，不掺入相似度/敲低等因子。
    ''' </para>
    ''' <para>
    ''' <b>缺失语义</b>：log2 通道里"未测"按"值为 0、权重照算"计入分母；
    ''' z 通道里"未测"完全剔除。两者不同，不能混用。
    ''' </para>
    ''' <para>
    ''' <b>内存</b>：按基因流式处理，只需 <c>O(行数×扰动数)</c> 的中间数组；
    ''' 逐行中心与方差在进入循环前一次对齐好。
    ''' </para>
    ''' </remarks>
    Public Module MetaPooling

        Private Const VarianceFloor As Double = 0.00000001
        Private Const L2Floor As Double = -39.86313713864835    ' log2(1e-12)
        Private Const ProbabilityFloor As Double = 1.0E-300

        ''' <summary>从内存版摘要直接池化一个上下文。</summary>
        ''' <param name="bundle">训练摘要</param>
        ''' <param name="genes">任务基因轴</param>
        ''' <param name="perturbations">任务扰动轴</param>
        ''' <param name="targetCpm">新上下文对照细胞的平均 CPM（按任务基因轴）</param>
        ''' <param name="model">模型参数</param>
        ''' <param name="exclude">需要排除的细胞系</param>
        ''' <remarks>进入时重置护栏计数器，返回的证据携带本次池化的完整快照。</remarks>
        Public Function Pool(bundle As SummaryBundle,
                             genes As String(),
                             perturbations As String(),
                             targetCpm As Double(),
                             model As CellularAiModel,
                             Optional exclude As IEnumerable(Of String) = Nothing) As PooledEvidence

            If bundle Is Nothing Then
                Throw New ArgumentNullException(NameOf(bundle))
            End If
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            Call Guards.Reset()

            Dim rows As SummaryRow() = bundle.TrainingRows(model.Pooling.Views, exclude)
            Dim rowLines As String() = rows.Select(Function(r) r.CellLine).ToArray
            Dim factors As PoolingFactors = PoolingFactors.Compute(bundle, genes, perturbations, targetCpm, model, rowLines, exclude)
            Dim nRows As Integer = rows.Length
            Dim stack(nRows - 1)()() As Double
            Dim probability(nRows - 1)()() As Double
            Dim tables(nRows - 1) As OffsetTable
            Dim multipliers(nRows - 1) As Double

            For t As Integer = 0 To nRows - 1
                Dim row As SummaryRow = rows(t)

                stack(t) = If(row.Effect Is Nothing, Nothing,
                              SummaryBundle.AlignPanel(row.EffectGenes, row.EffectPerts, row.Effect, genes, perturbations))
                probability(t) = If(row.Probability Is Nothing, Nothing,
                                    SummaryBundle.AlignPanel(row.EffectGenes, row.EffectPerts, row.Probability, genes, perturbations))
                tables(t) = row.Offset
                multipliers(t) = If(row.IsAuxiliary, model.Pooling.AuxiliaryWeight, 1.0)

                If stack(t) Is Nothing Then
                    stack(t) = EmptyPanel(genes.Length, perturbations.Length)
                End If
                If probability(t) Is Nothing Then
                    probability(t) = EmptyPanel(genes.Length, perturbations.Length)
                End If
            Next

            Return Combine(New PoolingRequest With {
                .Stack = stack,
                .Probability = probability,
                .Tables = tables,
                .Genes = genes,
                .Perts = perturbations,
                .Multipliers = multipliers,
                .Ops = PoolingOps.FromModel(model),
                .VFloorRel = model.Guards.VFloorRel,
                .Factors = factors,
                .L2AbsMax = model.Guards.L2AbsMax,
                .ZAbsMax = model.Guards.ZAbsMax
            })
        End Function

        ''' <summary>执行池化。</summary>
        Public Function Combine(request As PoolingRequest) As PooledEvidence
            If request Is Nothing Then
                Throw New ArgumentNullException(NameOf(request))
            End If
            If request.Stack Is Nothing Then
                Throw New ArgumentNullException(NameOf(request.Stack))
            End If
            If request.Factors Is Nothing Then
                Throw New ArgumentNullException(NameOf(request.Factors))
            End If

            Dim nRows As Integer = request.Stack.Length
            Dim nGenes As Integer = If(request.Genes Is Nothing, 0, request.Genes.Length)
            Dim nPerts As Integer = If(request.Perts Is Nothing, 0, request.Perts.Length)

            Dim l2 As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            Dim z As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    l2(i)(j) = Double.NaN
                    z(i)(j) = 0.0
                Next
            Next

            ' ---- 0) 逐行中心与方差一次对齐 ----
            Dim offsetL2 As Double()() = AlignOffsets(request.Tables, request.Genes, "l2")
            Dim offsetZ As Double()() = AlignOffsets(request.Tables, request.Genes, "z")
            Dim variance As Double()() = AlignVariance(request.Tables, request.Genes)
            Dim geneMedian As Double() = GeneMedians(variance, nRows, nGenes)
            Dim fallback As Double = PoolingFactors.NanMedian(geneMedian)

            If Double.IsNaN(fallback) OrElse fallback <= 0.0 Then
                fallback = 1.0
            End If

            For g As Integer = 0 To geneMedian.Length - 1
                If Double.IsNaN(geneMedian(g)) Then
                    geneMedian(g) = fallback
                End If
            Next

            Dim useLocalL2 As Boolean = FactorExpression.RequiresLocal(request.Ops.L2Row)
            Dim useLocalZ As Boolean = FactorExpression.RequiresLocal(request.Ops.ZRow)
            Dim useAuxL2 As Boolean = HasAuxiliary(request.Ops.L2Mat)
            Dim useAuxZ As Boolean = HasAuxiliary(request.Ops.ZMat)

            ' ---- 1) 逐基因的 τ² 与随机效应权重 ----
            Dim localWeight As Double()() = RectangularArray.Matrix(Of Double)(nRows, nGenes)
            Dim ratios As New List(Of Double)

            For g As Integer = 0 To nGenes - 1
                Dim V(nRows - 1) As Double
                Dim mG As Double = geneMedian(g)

                For t As Integer = 0 To nRows - 1
                    Dim raw As Double = variance(t)(g)

                    V(t) = If(Double.IsNaN(raw) OrElse raw <= 0.0, mG, raw)
                Next

                Dim tau2 As Double = DlTau2(request.Stack, offsetL2, V, g, nRows, nPerts)
                Dim floor As Double = request.VFloorRel * mG

                For t As Integer = 0 To nRows - 1
                    localWeight(t)(g) = mG / std.Max(V(t) + tau2, floor)
                    Call ratios.Add(localWeight(t)(g) / mG)
                Next
            Next

            Call Guards.Under(ratios.ToArray, request.VFloorRel, "v_floor_rel")

            ' ---- 2) 合并 ----
            Dim numL(nPerts - 1) As Double
            Dim denL(nPerts - 1) As Double
            Dim numZ(nPerts - 1) As Double
            Dim sqwZ(nPerts - 1) As Double

            For g As Integer = 0 To nGenes - 1
                Call Array.Clear(numL, 0, nPerts)
                Call Array.Clear(denL, 0, nPerts)
                Call Array.Clear(numZ, 0, nPerts)
                Call Array.Clear(sqwZ, 0, nPerts)

                For t As Integer = 0 To nRows - 1
                    Dim auxL2 As Double = If(useAuxL2 AndAlso request.Multipliers IsNot Nothing, request.Multipliers(t), 1.0)
                    Dim auxZ As Double = If(useAuxZ AndAlso request.Multipliers IsNot Nothing, request.Multipliers(t), 1.0)
                    Dim rowL2 As Double = request.Factors.RowWeight(t, request.Ops.L2Row, g)
                    Dim matL2 As Double = request.Factors.MatWeight(t, request.Ops.L2Mat)
                    Dim rowZ As Double = request.Factors.RowWeight(t, request.Ops.ZRow, g)
                    Dim matZ As Double = request.Factors.MatWeight(t, request.Ops.ZMat)
                    Dim baseL2 As Double = rowL2 * matL2 * auxL2 * If(useLocalL2, localWeight(t)(g), 1.0)
                    Dim baseZ As Double = rowZ * matZ * auxZ * If(useLocalZ, localWeight(t)(g), 1.0)

                    For p As Integer = 0 To nPerts - 1
                        Dim raw As Double = request.Stack(t)(g)(p)
                        Dim w As Double = baseL2 * request.Factors.ColWeight(t, p, request.Ops.L2Col)

                        ' log2 通道：未测按"值 0、权重照算"计入分母
                        denL(p) += w

                        If Not Double.IsNaN(raw) Then
                            numL(p) += w * (raw - offsetL2(t)(g))
                        End If

                        ' z 通道：只用实测项
                        Dim magnitude As Double = ScoreMagnitude(request.Probability, t, g, p)

                        If Not Double.IsNaN(magnitude) AndAlso Not Double.IsNaN(raw) Then
                            Dim wz As Double = baseZ * request.Factors.ColWeight(t, p, request.Ops.ZCol)
                            Dim signed As Double = std.Sign(raw) * magnitude - offsetZ(t)(g)

                            numZ(p) += wz * signed
                            sqwZ(p) += wz * wz
                        End If
                    Next
                Next

                For p As Integer = 0 To nPerts - 1
                    If denL(p) > 0.0 Then
                        l2(g)(p) = std.Max(numL(p) / denL(p), L2Floor)
                    End If

                    If sqwZ(p) > 0.0 Then
                        z(g)(p) = std.Abs(numZ(p)) / std.Sqrt(sqwZ(p))
                    Else
                        z(g)(p) = 0.0
                    End If
                Next
            Next

            ' ---- 3) 护栏与裁剪 ----
            Call ClipMatrix(l2, request.L2AbsMax, "l2_abs_max")
            Call ClipMatrix(z, request.ZAbsMax, "z_abs_max")

            Return New PooledEvidence With {
                .L2 = l2,
                .Z = z,
                .L2Tot = CloneMatrix(l2),
                .ZTot = CloneMatrix(z),
                .Unmeasured = FindUnmeasured(request.Stack, request.Perts),
                .Guards = Guards.Snapshot()
            }
        End Function

#Region "kernels"

        ''' <summary>逐基因的 DerSimonian–Laird τ²（权重为逆响应方差）。</summary>
        Private Function DlTau2(stack As Double()()(), offsetL2 As Double()(),
                                V As Double(), gene As Integer, nRows As Integer, nPerts As Integer) As Double
            Dim s0(nPerts - 1) As Double
            Dim s1(nPerts - 1) As Double
            Dim s2(nPerts - 1) As Double
            Dim sq(nPerts - 1) As Double
            Dim k(nPerts - 1) As Double

            For t As Integer = 0 To nRows - 1
                Dim w As Double = 1.0 / std.Max(V(t), VarianceFloor)
                Dim offset As Double = offsetL2(t)(gene)

                For p As Integer = 0 To nPerts - 1
                    Dim raw As Double = stack(t)(gene)(p)

                    If Double.IsNaN(raw) Then
                        Continue For
                    End If

                    Dim value As Double = raw - offset

                    s0(p) += w
                    s1(p) += w * value
                    s2(p) += w * value * value
                    sq(p) += w * w
                    k(p) += 1.0
                Next
            Next

            Dim Q As Double = 0.0
            Dim C As Double = 0.0
            Dim df As Double = 0.0

            For p As Integer = 0 To nPerts - 1
                If s0(p) <= 0.0 Then
                    Continue For
                End If

                Q += std.Max(0.0, s2(p) - s1(p) * s1(p) / s0(p))
                C += s0(p) - sq(p) / s0(p)
                df += std.Max(k(p) - 1.0, 0.0)
            Next

            If C <= 0.0 Then
                Return 0.0
            End If

            Return std.Max((Q - df) / std.Max(C, 1.0E-300), 0.0)
        End Function

        ''' <summary>把逐行中心化表映射到任务基因轴：返回 行×G，缺失为 0（"不做事"）。</summary>
        Private Function AlignOffsets(tables As OffsetTable(), genes As String(), stream As String) As Double()()
            Dim nRows As Integer = If(tables Is Nothing, 0, tables.Length)
            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nRows, nGenes)

            For t As Integer = 0 To nRows - 1
                Dim table As OffsetTable = If(tables Is Nothing, Nothing, tables(t))
                Dim center As Double() = Nothing

                If table IsNot Nothing Then
                    Select Case (If(stream, "")).ToLowerInvariant()
                        Case "l2"
                            center = table.EffectCenter
                        Case "z"
                            center = table.SignedScoreCenter
                    End Select
                End If

                If center Is Nothing OrElse table.Genes Is Nothing Then
                    Continue For
                End If

                Dim positions As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For i As Integer = 0 To table.Genes.Length - 1
                    If i < center.Length Then
                        positions(table.Genes(i)) = i
                    End If
                Next

                For g As Integer = 0 To nGenes - 1
                    Dim at As Integer = -1

                    If positions.TryGetValue(genes(g), at) Then
                        Dim value As Double = center(at)

                        result(t)(g) = If(Double.IsNaN(value), 0.0, value)
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>把逐行的响应方差映射到任务基因轴：返回 行×G，缺失为 NaN。</summary>
        Private Function AlignVariance(tables As OffsetTable(), genes As String()) As Double()()
            Dim nRows As Integer = If(tables Is Nothing, 0, tables.Length)
            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nRows, nGenes)

            For t As Integer = 0 To nRows - 1
                For g As Integer = 0 To nGenes - 1
                    result(t)(g) = Double.NaN
                Next
            Next

            For t As Integer = 0 To nRows - 1
                Dim table As OffsetTable = If(tables Is Nothing, Nothing, tables(t))

                If table Is Nothing OrElse table.ResponseVariance Is Nothing OrElse table.Genes Is Nothing Then
                    Continue For
                End If

                Dim positions As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For i As Integer = 0 To table.Genes.Length - 1
                    If i < table.ResponseVariance.Length Then
                        positions(table.Genes(i)) = i
                    End If
                Next

                For g As Integer = 0 To nGenes - 1
                    Dim at As Integer = -1

                    If positions.TryGetValue(genes(g), at) Then
                        result(t)(g) = table.ResponseVariance(at)
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>逐基因的响应方差中位数（只用正估计）。</summary>
        Private Function GeneMedians(variance As Double()(), nRows As Integer, nGenes As Integer) As Double()
            Dim result(If(nGenes = 0, -1, nGenes - 1)) As Double
            Dim column(If(nRows = 0, -1, nRows - 1)) As Double

            For g As Integer = 0 To nGenes - 1
                For t As Integer = 0 To nRows - 1
                    Dim v As Double = variance(t)(g)

                    column(t) = If(Double.IsNaN(v) OrElse v <= 0.0, Double.NaN, v)
                Next

                result(g) = PoolingFactors.NanMedian(column)
            Next

            Return result
        End Function

        ''' <summary>双侧概率 → 正态分数量级（<c>Φ⁻¹(1 − p/2)</c>）；缺失返回 NaN。</summary>
        Private Function ScoreMagnitude(probability As Double()()(), t As Integer, g As Integer, p As Integer) As Double
            If probability Is Nothing OrElse t >= probability.Length Then
                Return Double.NaN
            End If

            Dim panel As Double()() = probability(t)

            If panel Is Nothing OrElse g >= panel.Length Then
                Return Double.NaN
            End If

            Dim row As Double() = panel(g)

            If row Is Nothing OrElse p >= row.Length Then
                Return Double.NaN
            End If

            Dim value As Double = row(p)

            If Double.IsNaN(value) Then
                Return Double.NaN
            End If

            Return NormalDistribution.InvCCDF(std.Max(ProbabilityFloor, std.Min(value, 1.0)) / 2.0)
        End Function

        ''' <summary>找出所有训练行都没有测量到的扰动。</summary>
        Private Function FindUnmeasured(stack As Double()()(), perts As String()) As String()
            Dim nPerts As Integer = If(perts Is Nothing, 0, perts.Length)
            Dim result As New List(Of String)
            Dim nRows As Integer = stack.Length
            Dim nGenes As Integer = If(nRows = 0, 0, stack(0).Length)

            For p As Integer = 0 To nPerts - 1
                Dim measured As Boolean = False

                For t As Integer = 0 To nRows - 1
                    For g As Integer = 0 To nGenes - 1
                        If Not Double.IsNaN(stack(t)(g)(p)) Then
                            measured = True
                            Exit For
                        End If
                    Next

                    If measured Then
                        Exit For
                    End If
                Next

                If Not measured Then
                    Call result.Add(perts(p))
                End If
            Next

            Return result.ToArray
        End Function

        Private Function HasAuxiliary(names As String()) As Boolean
            Return names IsNot Nothing AndAlso Array.IndexOf(names, "auxiliary") >= 0
        End Function

        Private Function EmptyPanel(nGenes As Integer, nPerts As Integer) As Double()()
            Dim panel As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    panel(i)(j) = Double.NaN
                Next
            Next

            Return panel
        End Function

        ''' <summary>把矩阵裁到 ±limit，并记录命中比例。</summary>
        Private Sub ClipMatrix(values As Double()(), limit As Double, name As String)
            Dim hit As Integer = 0
            Dim count As Integer = 0

            For i As Integer = 0 To values.Length - 1
                For j As Integer = 0 To values(i).Length - 1
                    Dim v As Double = values(i)(j)

                    count += 1

                    If Double.IsNaN(v) Then
                        Continue For
                    End If

                    If v > limit Then
                        values(i)(j) = limit
                        hit += 1
                    ElseIf v < -limit Then
                        values(i)(j) = -limit
                        hit += 1
                    End If
                Next
            Next

            Call Guards.Fired(name, hit, count)
        End Sub

        Private Function CloneMatrix(values As Double()()) As Double()()
            Dim copy(values.Length - 1)() As Double

            For i As Integer = 0 To values.Length - 1
                copy(i) = DirectCast(values(i).Clone(), Double())
            Next

            Return copy
        End Function

#End Region

    End Module
End Namespace
