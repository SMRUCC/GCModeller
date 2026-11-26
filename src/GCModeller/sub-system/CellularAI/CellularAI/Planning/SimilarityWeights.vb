Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Evidence
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports std = System.Math

Namespace Planning

    ''' <summary>相似度权重及其原始相关系数。</summary>
    Public Class SimilarityResult

        ''' <summary>归一化权重（和为 1）。</summary>
        Public Property Weights As Double()

        ''' <summary>每个训练行与新上下文的相关系数。</summary>
        Public Property Correlations As Double()

    End Class

    ''' <summary>
    ''' 训练细胞系与新上下文的相似度权重、以及基于这些权重的证据加权合并。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 相似度用"对照表达谱"衡量：把每个训练行的对照计数换成 CPM 并取 <c>log1p</c>，
    ''' 在新上下文与训练行之间<b>方差最大</b>的若干基因上算 Pearson 相关
    ''' （高变基因让相关性能反映谱形差异，而不是被大量零表达基因稀释）。
    ''' </para>
    ''' <para>
    ''' 相关系数先做 z 标准化（标准差有下限，避免"几乎全同"时把微小差异放大成极端权重），
    ''' 再取 <c>exp((z − z_max)/τ)</c> 并归一化。随后用"上限水位法"把单项权重压到
    ''' <c>sim_cap</c> 以下：把超出的部分按比例分给未触顶的行，
    ''' 直到没有行超过上限（或所有行都被压在同一个水平上）。
    ''' </para>
    ''' </remarks>
    Public Module SimilarityWeights

        ''' <summary>计算相似度权重。</summary>
        ''' <param name="genes">任务基因轴</param>
        ''' <param name="targetCpm">新上下文对照细胞的平均 CPM</param>
        ''' <param name="referenceCtrl">训练行的对照剖面（键为细胞系名）</param>
        ''' <param name="lines">参与加权的训练行（顺序决定权重顺序）</param>
        ''' <param name="tau">softmax 温度</param>
        ''' <param name="ntop">参与相关的高变基因数</param>
        ''' <param name="sdMin">相关系数标准差的下限</param>
        Public Function Similarity(genes As String(),
                                   targetCpm As Double(),
                                   referenceCtrl As Dictionary(Of String, ControlProfile),
                                   lines As String(),
                                   tau As Double,
                                   ntop As Integer,
                                   sdMin As Double) As SimilarityResult

            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim nLines As Integer = If(lines Is Nothing, 0, lines.Length)
            Dim weights(nLines - 1) As Double
            Dim correlations(nLines - 1) As Double

            For t As Integer = 0 To nLines - 1
                weights(t) = 1.0 / std.Max(nLines, 1)
            Next

            If nLines = 0 OrElse nGenes = 0 OrElse referenceCtrl Is Nothing Then
                Return New SimilarityResult With {.Weights = weights, .Correlations = correlations}
            End If

            ' 训练行 × 基因的对照计数（缺失为 NaN）
            Dim counts As Double()() = RectangularArray.Matrix(Of Double)(nLines, nGenes)

            For t As Integer = 0 To nLines - 1
                Dim profile As ControlProfile = Nothing

                If Not referenceCtrl.TryGetValue(lines(t), profile) Then
                    For j As Integer = 0 To nGenes - 1
                        counts(t)(j) = Double.NaN
                    Next

                    Continue For
                End If

                Dim lookup As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For i As Integer = 0 To profile.Count - 1
                    lookup(profile.Genes(i)) = i
                Next

                For j As Integer = 0 To nGenes - 1
                    Dim at As Integer = -1

                    If lookup.TryGetValue(genes(j), at) Then
                        counts(t)(j) = profile.Counts(at)
                    Else
                        counts(t)(j) = Double.NaN
                    End If
                Next
            Next

            ' 只在"所有行都有测量"的基因上比较
            Dim usable As New List(Of Integer)

            For j As Integer = 0 To nGenes - 1
                Dim complete As Boolean = True

                For t As Integer = 0 To nLines - 1
                    If Double.IsNaN(counts(t)(j)) Then
                        complete = False
                        Exit For
                    End If
                Next

                If complete Then
                    Call usable.Add(j)
                End If
            Next

            If usable.Count = 0 Then
                Return New SimilarityResult With {.Weights = weights, .Correlations = correlations}
            End If

            ' log1p(CPM)
            Dim logCpm As Double()() = RectangularArray.Matrix(Of Double)(nLines, usable.Count)

            For t As Integer = 0 To nLines - 1
                Dim total As Double = 0.0

                For i As Integer = 0 To usable.Count - 1
                    total += counts(t)(usable(i))
                Next

                If total <= 0.0 Then
                    total = 1.0
                End If

                For i As Integer = 0 To usable.Count - 1
                    logCpm(t)(i) = std.Log(1.0 + 1000000.0 * counts(t)(usable(i)) / total)
                Next
            Next

            Dim target As Double() = New Double(usable.Count - 1) {}

            For i As Integer = 0 To usable.Count - 1
                target(i) = std.Log(1.0 + targetCpm(usable(i)))
            Next

            ' 跨行方差最大的 ntop 个基因
            Dim variance As Double() = New Double(usable.Count - 1) {}

            For i As Integer = 0 To usable.Count - 1
                Dim column(nLines - 1) As Double

                For t As Integer = 0 To nLines - 1
                    column(t) = logCpm(t)(i)
                Next

                variance(i) = PoolingFactors.NanVariance(column)
            Next

            Dim order As Integer() = Enumerable.Range(0, usable.Count).
                OrderByDescending(Function(i) If(Double.IsNaN(variance(i)), 0.0, variance(i))).
                ToArray

            Dim take As Integer = std.Min(std.Max(ntop, 1), usable.Count)
            Dim selected As Integer() = order.Take(take).ToArray

            Dim sample As Double() = New Double(take - 1) {}
            Dim reference As Double() = New Double(take - 1) {}

            For i As Integer = 0 To take - 1
                reference(i) = target(selected(i))
            Next

            For t As Integer = 0 To nLines - 1
                For i As Integer = 0 To take - 1
                    sample(i) = logCpm(t)(selected(i))
                Next

                correlations(t) = Pearson(sample, reference)
            Next

            ' z 标准化 + softmax
            Dim mean As Double = correlations.Average
            Dim acc As Double = 0.0

            For t As Integer = 0 To nLines - 1
                Dim d As Double = correlations(t) - mean

                acc += d * d
            Next

            Dim floor As Double = std.Max(std.Sqrt(acc / std.Max(nLines, 1)), sdMin)
            Dim zz(nLines - 1) As Double
            Dim top As Double = Double.NegativeInfinity

            For t As Integer = 0 To nLines - 1
                zz(t) = (correlations(t) - mean) / floor
                top = std.Max(top, zz(t))
            Next

            Dim sum As Double = 0.0

            For t As Integer = 0 To nLines - 1
                weights(t) = std.Exp((zz(t) - top) / std.Max(tau, 1.0E-12))
                sum += weights(t)
            Next

            For t As Integer = 0 To nLines - 1
                weights(t) /= std.Max(sum, 1.0E-300)
            Next

            Return New SimilarityResult With {.Weights = weights, .Correlations = correlations}
        End Function

        ''' <summary>
        ''' 上限水位法：把超过 <paramref name="cap"/> 的权重削到上限，超出部分按比例分给未触顶的行。
        ''' </summary>
        ''' <param name="w">权重（会被归一化）</param>
        ''' <param name="cap">单项上限</param>
        Public Function CapWeights(w As Double(), cap As Double) As Double()
            Dim values As Double() = DirectCast(w.Clone(), Double())
            Dim total As Double = values.Sum

            If total <= 0.0 Then
                Return values
            End If

            For i As Integer = 0 To values.Length - 1
                values(i) /= total
            Next

            Dim capped As Boolean = False

            For iteration As Integer = 1 To 100
                Dim over As Boolean() = values.Select(Function(v) v > cap + 1.0E-12).ToArray

                If Not over.Any(Function(b) b) Then
                    Exit For
                End If

                Dim excess As Double = 0.0

                For i As Integer = 0 To values.Length - 1
                    If over(i) Then
                        excess += values(i) - cap
                        values(i) = cap
                    End If
                Next

                Dim free As New List(Of Integer)

                For i As Integer = 0 To values.Length - 1
                    If Not over(i) AndAlso values(i) < cap Then
                        Call free.Add(i)
                    End If
                Next

                If free.Count = 0 Then
                    capped = True
                    Exit For
                End If

                Dim freeTotal As Double = 0.0

                For Each i As Integer In free
                    freeTotal += values(i)
                Next

                If freeTotal <= 0.0 Then
                    capped = True
                    Exit For
                End If

                For Each i As Integer In free
                    values(i) += excess * values(i) / freeTotal
                Next
            Next

            Call Guards.Fired("cap_weights_all_capped", capped)

            If capped Then
                Dim normalizer As Double = values.Sum

                If normalizer > 0.0 Then
                    For i As Integer = 0 To values.Length - 1
                        values(i) /= normalizer
                    Next
                End If
            End If

            Return values
        End Function

        ''' <summary>
        ''' 计划显著性：把"均匀权重"与"相似度权重"混合后做 Stouffer 合并，取绝对值。
        ''' </summary>
        ''' <param name="mixZ">逐行带符号正态分数（行×基因×扰动）</param>
        ''' <param name="mixLines">逐行细胞系名</param>
        ''' <param name="referenceCtrl">训练行的对照剖面</param>
        ''' <param name="genes">任务基因轴</param>
        ''' <param name="meanCpm">新上下文对照细胞的平均 CPM</param>
        Public Function PlanZ(mixZ As Double()()(),
                              mixLines As String(),
                              referenceCtrl As Dictionary(Of String, ControlProfile),
                              genes As String(),
                              meanCpm As Double(),
                              plan As PlanOptions,
                              guards As GuardOptions) As Double()()

            Dim nRows As Integer = mixZ.Length
            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim nPerts As Integer = If(nRows = 0 OrElse mixZ(0).Length = 0, 0, mixZ(0)(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            If nRows = 0 Then
                Return result
            End If

            Dim fitted As SimilarityResult = Similarity(genes, meanCpm, referenceCtrl, mixLines,
                                                        plan.SimTau, plan.SimNtop, guards.OrderSdMin)
            Dim capped As Double() = CapWeights(fitted.Weights, plan.SimCap)
            Dim W(nRows - 1) As Double
            Dim squareSum As Double = 0.0

            For t As Integer = 0 To nRows - 1
                W(t) = plan.PlanZUniform / nRows + (1.0 - plan.PlanZUniform) * capped(t)
                squareSum += W(t) * W(t)
            Next

            Call CellularAI.Guards.Share("plan", W, guards)

            Dim denominator As Double = std.Sqrt(std.Max(squareSum, 1.0E-300))
            Dim num(nPerts - 1) As Double

            For g As Integer = 0 To nGenes - 1
                Call Array.Clear(num, 0, nPerts)

                For t As Integer = 0 To nRows - 1
                    For p As Integer = 0 To nPerts - 1
                        Dim value As Double = mixZ(t)(g)(p)

                        If Not Double.IsNaN(value) Then
                            num(p) += W(t) * value
                        End If
                    Next
                Next

                For p As Integer = 0 To nPerts - 1
                    result(g)(p) = std.Abs(num(p)) / denominator
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 相似度加权的 Stouffer：只用"有测量"的行参与，分母是 <c>√Σ(w²·measured)</c>。
        ''' </summary>
        Public Function SimilarityZ(simZ As Double()()(),
                                    simMeasured As Boolean()()(),
                                    simLines As String(),
                                    referenceCtrl As Dictionary(Of String, ControlProfile),
                                    genes As String(),
                                    meanCpm As Double(),
                                    plan As PlanOptions,
                                    guards As GuardOptions) As Double()()

            Dim nRows As Integer = simZ.Length
            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim nPerts As Integer = If(nRows = 0 OrElse simZ(0).Length = 0, 0, simZ(0)(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            Dim numerator(nPerts - 1) As Double
            Dim denominator(nPerts - 1) As Double

            If nRows = 0 Then
                Return result
            End If

            Dim fitted As SimilarityResult = Similarity(genes, meanCpm, referenceCtrl, simLines,
                                                        plan.SimTau, plan.SimNtop, guards.OrderSdMin)
            Dim capped As Double() = CapWeights(fitted.Weights, plan.SimCap)

            Call CellularAI.Guards.Share("sim", capped, guards)

            For g As Integer = 0 To nGenes - 1
                Call Array.Clear(numerator, 0, nPerts)
                Call Array.Clear(denominator, 0, nPerts)

                For t As Integer = 0 To nRows - 1
                    Dim w As Double = capped(t)

                    If w <= 0.0 Then
                        Continue For
                    End If

                    For p As Integer = 0 To nPerts - 1
                        If Not simMeasured(t)(g)(p) Then
                            Continue For
                        End If

                        numerator(p) += w * simZ(t)(g)(p)
                        denominator(p) += w * w
                    Next
                Next

                For p As Integer = 0 To nPerts - 1
                    result(g)(p) = If(denominator(p) > 0.0,
                                      numerator(p) / std.Sqrt(std.Max(denominator(p), 1.0E-300)),
                                      0.0)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 训练数据广度（每个扰动一个值）：用 <c>xsim_tau</c> 温度下的相似度权重对逐行广度加权平均。
        ''' </summary>
        Public Function XSim(rel As Double()(),
                             lines As String(),
                             referenceCtrl As Dictionary(Of String, ControlProfile),
                             genes As String(),
                             meanCpm As Double(),
                             plan As PlanOptions,
                             guards As GuardOptions) As Double()

            Dim nRows As Integer = rel.Length
            Dim nPerts As Integer = If(nRows = 0, 0, rel(0).Length)

            If nRows = 0 Then
                Return New Double(-1) {}
            End If

            ' 只保留"有对照剖面"的行
            Dim keep As New List(Of Integer)

            For t As Integer = 0 To nRows - 1
                If referenceCtrl.ContainsKey(lines(t)) Then
                    Call keep.Add(t)
                End If
            Next

            Dim result(nPerts - 1) As Double

            If keep.Count = 0 Then
                For p As Integer = 0 To nPerts - 1
                    Dim acc As Double = 0.0

                    For t As Integer = 0 To nRows - 1
                        acc += rel(t)(p)
                    Next

                    result(p) = acc / nRows
                Next

                Return result
            End If

            Dim keepLines As String() = keep.Select(Function(i) lines(i)).ToArray
            Dim fitted As SimilarityResult = Similarity(genes, meanCpm, referenceCtrl, keepLines,
                                                        plan.XsimTau, plan.SimNtop, guards.OrderSdMin)
            Dim capped As Double() = CapWeights(fitted.Weights, plan.SimCap)

            Call CellularAI.Guards.Share("xsim", capped, guards)

            For p As Integer = 0 To nPerts - 1
                Dim weighted As Double = 0.0
                Dim weightSum As Double = 0.0

                For i As Integer = 0 To keep.Count - 1
                    Dim value As Double = rel(keep(i))(p)

                    If Double.IsNaN(value) Then
                        Continue For
                    End If

                    weighted += value * capped(i)
                    weightSum += capped(i)
                Next

                If weightSum > 0.0 Then
                    result(p) = weighted / weightSum
                Else
                    Dim acc As Double = 0.0

                    For t As Integer = 0 To nRows - 1
                        acc += If(Double.IsNaN(rel(t)(p)), 0.0, rel(t)(p))
                    Next

                    result(p) = acc / nRows
                End If
            Next

            Return result
        End Function

        ''' <summary>Pearson 相关（两个等长数组，无缺失）。</summary>
        Private Function Pearson(x As Double(), y As Double()) As Double
            Dim n As Integer = x.Length

            If n < 2 Then
                Return 0.0
            End If

            Dim meanX As Double = x.Average
            Dim meanY As Double = y.Average
            Dim sxy As Double = 0.0
            Dim sxx As Double = 0.0
            Dim syy As Double = 0.0

            For i As Integer = 0 To n - 1
                Dim dx As Double = x(i) - meanX
                Dim dy As Double = y(i) - meanY

                sxy += dx * dy
                sxx += dx * dx
                syy += dy * dy
            Next

            If sxx <= 0.0 OrElse syy <= 0.0 Then
                Return 0.0
            End If

            Return sxy / std.Sqrt(sxx * syy)
        End Function

    End Module
End Namespace
