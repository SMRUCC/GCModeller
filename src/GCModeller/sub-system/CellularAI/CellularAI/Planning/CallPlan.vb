Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Statistics
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports std = System.Math

Namespace Planning

    ''' <summary>
    ''' 一个扰动的调用计划：被调用的基因下标（按优先级）、方向、优先级，
    ''' 以及非计划基因的"微调方向与强度"。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="NudgeSign"/> 与 <see cref="NudgeStrength"/> 覆盖<b>全部基因</b>：
    ''' 未进入计划集的基因不需要被"调用"，但合成阶段仍要给它们一个与证据一致的方向与强度，
    ''' 否则整个转录组会显得比真实数据"过于安静"。
    ''' </remarks>
    Public Class CallEntry

        ''' <summary>响应基因下标（按优先级从高到低）。</summary>
        Public Property Index As Integer()

        ''' <summary>每个被调用基因的方向（+1 / -1）。</summary>
        Public Property Sign As Double()

        ''' <summary>每个被调用基因的优先级（1 = 最高）。</summary>
        Public Property Priority As Double()

        ''' <summary>每个基因的微调方向（<c>0</c> 表示方向未知）。</summary>
        Public Property NudgeSign As SByte()

        ''' <summary>每个基因的微调强度（<c>0</c> 表示不微调）。</summary>
        Public Property NudgeStrength As Double()

        ''' <summary>调用数量。</summary>
        Public Property NCalls As Integer

    End Class

    ''' <summary>
    ''' 从"合并效应 + 显著性"选出每个扰动的响应基因集。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 排序分数是 <c>z/σ_z + w_pow·功效/σ_功效</c>：既看统计显著性，也看"这个基因在该深度下
    ''' 是否有可能被测到"。只用 z 会偏向高表达基因；只用功效会偏向效应大的噪声。
    ''' </para>
    ''' <para>
    ''' 每个扰动的自身靶基因<b>永远被排除</b>：它的效应是"敲低本身"，
    ''' 由合成阶段按计划直接表达，不该混进响应集。
    ''' </para>
    ''' </remarks>
    Public Module CallPlan

        ''' <summary>
        ''' 选出每个扰动的响应基因。
        ''' </summary>
        ''' <param name="sizes">每个扰动的调用数量；<c>Nothing</c> 时按强度公式现算</param>
        ''' <param name="medianCalls">强度公式里的中位调用数（当 <paramref name="sizes"/> 为 <c>Nothing</c> 时使用）</param>
        Public Function SelectCalls(l2 As Double()(), z As Double()(), cpm As Double(),
                                    genes As String(), perts As String(), depth As Double,
                                    model As CellularAiModel,
                                    Optional medianCalls As Double = 0.0,
                                    Optional sizes As Integer() = Nothing) As Dictionary(Of String, CallEntry)

            Dim nGenes As Integer = l2.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, l2(0).Length)
            Dim zValues As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    Dim value As Double = z(g)(p)

                    zValues(g)(p) = If(Double.IsNaN(value) OrElse Double.IsInfinity(value), 0.0, value)
                Next
            Next

            Dim eligible As Boolean() = New Boolean(If(nGenes = 0, -1, nGenes - 1)) {}

            For g As Integer = 0 To nGenes - 1
                eligible(g) = cpm(g) > model.Calls.EligibleCpm
            Next

            Dim geneIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For g As Integer = 0 To nGenes - 1
                geneIndex(genes(g)) = g
            Next

            Dim tidx(nPerts - 1) As Integer

            For p As Integer = 0 To nPerts - 1
                Dim at As Integer = -1

                Call geneIndex.TryGetValue(perts(p), at)
                tidx(p) = at
            Next

            Dim K(nPerts - 1) As Integer

            If sizes Is Nothing Then
                Dim strength As Double() = PertStrength(zValues, l2, eligible, tidx, cpm, depth, model, nGenes, nPerts)
                Dim eligibleCount As Integer = eligible.Count(Function(b) b)

                For p As Integer = 0 To nPerts - 1
                    Dim n As Double = medianCalls * std.Exp(model.Calls.Gamma * strength(p))
                    Dim upper As Integer = std.Min(model.Calls.NMax, eligibleCount - 1)

                    K(p) = CInt(std.Round(std.Max(std.Min(n, upper), model.Calls.NMin), MidpointRounding.AwayFromZero))
                Next
            Else
                For p As Integer = 0 To std.Min(nPerts, sizes.Length) - 1
                    K(p) = sizes(p)
                Next
            End If

            Call Guards.Fired("no_eligible_genes", K.Count(Function(v) v < 0 OrElse v > nGenes), K.Length)

            ' 排序分数
            Dim mu(nGenes - 1) As Double
            Dim power(nGenes - 1)() As Double

            For g As Integer = 0 To nGenes - 1
                mu(g) = cpm(g) * depth / 1000000.0
            Next

            Dim powerZ(nGenes - 1)() As Double

            For g As Integer = 0 To nGenes - 1
                powerZ(g) = New Double(nPerts - 1) {}

                For p As Integer = 0 To nPerts - 1
                    powerZ(g)(p) = std.Abs(l2(g)(p)) / std.Sqrt(1.0 / std.Max(mu(g), 0.001) + model.Calls.Phi)
                Next
            Next

            Dim sdZ As Double = StdOfPositive(zValues)
            Dim sdPower As Double = StdOfPositive(powerZ)
            Dim score(nGenes - 1)() As Double

            For g As Integer = 0 To nGenes - 1
                score(g) = New Double(nPerts - 1) {}

                For p As Integer = 0 To nPerts - 1
                    score(g)(p) = zValues(g)(p) / sdZ + model.Calls.WPow * powerZ(g)(p) / sdPower
                Next
            Next

            Dim result As New Dictionary(Of String, CallEntry)(StringComparer.Ordinal)

            For p As Integer = 0 To nPerts - 1
                Dim ranking As New List(Of Integer)

                For g As Integer = 0 To nGenes - 1
                    If Not eligible(g) OrElse g = tidx(p) Then
                        Continue For
                    End If

                    Call ranking.Add(g)
                Next

                Dim ordered As Integer() = ranking.OrderByDescending(Function(g) score(g)(p)).ThenBy(Function(g) g).ToArray
                Dim count As Integer = std.Min(std.Max(K(p), 0), ordered.Length)
                Dim picked(count - 1) As Integer
                Dim sign(count - 1) As Double
                Dim priority(count - 1) As Double

                For i As Integer = 0 To count - 1
                    Dim g As Integer = ordered(i)

                    picked(i) = g
                    sign(i) = If(std.Sign(l2(g)(p)) = 0, 1.0, std.Sign(l2(g)(p)))
                    priority(i) = 1.0 - i / std.Max(count, 1)
                Next

                ' 非计划基因的微调强度：排在后面的基因强度递减
                Dim nudgeSign(nGenes - 1) As SByte
                Dim nudgeStrength(nGenes - 1) As Double
                Dim rest As Integer() = ordered.Skip(count).ToArray

                For g As Integer = 0 To nGenes - 1
                    nudgeSign(g) = CSByte(std.Sign(l2(g)(p)))
                Next

                For i As Integer = 0 To rest.Length - 1
                    nudgeStrength(rest(i)) = 1.0 - (i + 0.5) / rest.Length
                Next

                result(perts(p)) = New CallEntry With {
                    .Index = picked,
                    .Sign = sign,
                    .Priority = priority,
                    .NudgeSign = nudgeSign,
                    .NudgeStrength = nudgeStrength,
                    .NCalls = count
                }
            Next

            Return result
        End Function

        ''' <summary>
        ''' 按给定尺寸生成计划，并保证每个可用基因都有微调方向
        ''' （证据不足时用训练数据的共同方向 <paramref name="common"/> 兜底）。
        ''' </summary>
        Public Function SizedPlan(K As Integer(), l2 As Double()(), zplan As Double()(),
                                  meanCpm As Double(), genes As String(), perts As String(),
                                  depth As Double, common As Double,
                                  model As CellularAiModel) As Dictionary(Of String, CallEntry)

            Dim nGenes As Integer = l2.Length
            Dim clean As Double()() = RectangularArray.Matrix(Of Double)(nGenes, If(nGenes = 0, 0, l2(0).Length))

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To l2(g).Length - 1
                    clean(g)(p) = If(Double.IsNaN(l2(g)(p)), 0.0, l2(g)(p))
                Next
            Next

            Dim cleanZ As Double()() = RectangularArray.Matrix(Of Double)(nGenes, If(nGenes = 0, 0, zplan(0).Length))

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To zplan(g).Length - 1
                    Dim value As Double = zplan(g)(p)

                    cleanZ(g)(p) = If(Double.IsNaN(value) OrElse Double.IsInfinity(value), 0.0, value)
                Next
            Next

            Dim calls As Dictionary(Of String, CallEntry) =
                SelectCalls(clean, cleanZ, meanCpm, genes, perts, depth, model, sizes:=K)
            Dim result As New Dictionary(Of String, CallEntry)(StringComparer.Ordinal)

            For Each pair As KeyValuePair(Of String, CallEntry) In calls
                Dim entry As CallEntry = pair.Value
                Dim sign As Double() = New Double(nGenes - 1) {}

                For g As Integer = 0 To nGenes - 1
                    Dim value As Double = entry.NudgeSign(g)

                    If meanCpm(g) > model.Calls.EligibleCpm AndAlso value = 0.0 Then
                        value = If(common < 0.0, -1.0, 1.0)
                    End If

                    sign(g) = If(meanCpm(g) > model.Calls.EligibleCpm, value, 0.0)
                Next

                result(pair.Key) = New CallEntry With {
                    .Index = entry.Index,
                    .Sign = entry.Sign,
                    .Priority = entry.Priority,
                    .NudgeSign = Array.ConvertAll(sign, Function(v) CSByte(v)),
                    .NudgeStrength = entry.NudgeStrength,
                    .NCalls = entry.NCalls
                }
            Next

            Return result
        End Function

#Region "helpers"

        ''' <summary>
        ''' 扰动强度：<c>rankz(log1p(Σ Φ(k·|lfc|/σ − c)·[|z| &gt; z0]))</c>，再标准化一次。
        ''' </summary>
        Private Function PertStrength(z As Double()(), l2 As Double()(), eligible As Boolean(),
                                      tidx As Integer(), cpm As Double(), depth As Double,
                                      model As CellularAiModel, nGenes As Integer, nPerts As Integer) As Double()
            Dim counts(nPerts - 1) As Double

            For g As Integer = 0 To nGenes - 1
                If Not eligible(g) Then
                    Continue For
                End If

                Dim mu As Double = std.Max(cpm(g) * depth / 1000000.0, 0.000000001)
                Dim sigma As Double = std.Sqrt(1.0 / std.Max(mu, 0.001) + model.Calls.StrengthPhi)

                For p As Integer = 0 To nPerts - 1
                    If g = tidx(p) Then
                        Continue For
                    End If
                    If z(g)(p) <= model.Calls.StrengthZ Then
                        Continue For
                    End If

                    counts(p) += NormalDistribution.CDF(model.Calls.StrengthK * std.Abs(l2(g)(p)) / sigma - model.Calls.StrengthC)
                Next
            Next

            Dim logged(nPerts - 1) As Double

            For p As Integer = 0 To nPerts - 1
                logged(p) = std.Log(1.0 + counts(p))
            Next

            Dim first As Double() = CallSizing.RankZ(logged)

            Return CallSizing.RankZ(first)
        End Function

        ''' <summary>正值的标准差（用于把 z 与功效放到同一量纲）。</summary>
        Private Function StdOfPositive(values As Double()()) As Double
            Dim positive As New List(Of Double)

            For i As Integer = 0 To values.Length - 1
                For j As Integer = 0 To values(i).Length - 1
                    If values(i)(j) > 0.0 Then
                        Call positive.Add(values(i)(j))
                    End If
                Next
            Next

            If positive.Count = 0 Then
                Return 1.0
            End If

            Dim mean As Double = positive.Average
            Dim acc As Double = 0.0

            For i As Integer = 0 To positive.Count - 1
                Dim d As Double = positive(i) - mean

                acc += d * d
            Next

            Dim sd As Double = std.Sqrt(acc / positive.Count)

            Return If(sd > 0.0, sd, 1.0)
        End Function

#End Region

    End Module
End Namespace
