Imports System.IO
Imports std = System.Math

Namespace Intervention

    ''' <summary>
    ''' 虚拟扰动的「表达状态上下文对齐」策略实现。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 上下文（外部 pseudobulk / 疾病态 / 上游扰动结果）与模型训练矩阵往往来自不同的
    ''' 归一化管线，直接注入会让特征落到决策树的极值叶子上（“平台化”，局部灵敏度趋近 0），
    ''' 或者被线性/高斯模型当成若干倍的浓度差异。本模块提供五种可切换的对齐策略，
    ''' 由 <see cref="ContextAlignmentMode"/> 选择：
    ''' </para>
    ''' <list type="table">
    ''' <listheader><term>模式</term><description>定义与适用模型</description></listheader>
    ''' <item><term>None</term><description>原值直灌。适用于与训练完全同管线、已配对的上下文。</description></item>
    ''' <item><term>Affine</term><description><c>(x - μ_g) / σ_g</c>。线性回归、高斯贝叶斯网等对量级敏感的模型。</description></item>
    ''' <item><term>LogAffine</term><description><c>log2(max(x,0) + pseudo) + δ_g</c>。幂律型 ODE（S-system）。</description></item>
    ''' <item><term>Quantile</term><description>跨基因分位 → 该基因训练种群的同分位。树模型（只看顺序）。</description></item>
    ''' <item><term>QuantileDelta</term><description><c>x + (Q_g(p) - Q_source(p))</c>。既要落在支撑域内又要保留相对幅度。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>Quantile 的语义</b>：把 context 的取值在<b>全部基因之间</b>排序得到它自己的经验分布，
    ''' 每个基因得到分位 <c>p_i</c>；再到该基因<b>自身</b>的训练种群上取同样的分位作为对齐值。
    ''' 这样保留了基因之间的相对高低（跨基因排序是保序的），同时保证取值落回该基因
    ''' 在训练数据中观测到的范围之内。
    ''' </para>
    ''' <para>
    ''' <b>QuantileDelta 的量纲前提</b>：修正量 <c>Q_g(p) - Q_source(p)</c> 位于<b>模型空间</b>，
    ''' 因此只有当 context 已经与模型空间可比（例如同为 log 域，只是分布形状不同）时，
    ''' 直接相加才有意义。若两条管线的量纲本身不同，应先用 <see cref="ContextAlignmentMode.LogAffine"/>
    ''' 或 <see cref="ContextAlignmentMode.Affine"/> 换算，再考虑分位偏移。
    ''' </para>
    ''' </remarks>
    Public Module ContextAligner

        ''' <summary>
        ''' 把外部上下文对齐到模型自身的表达尺度。
        ''' </summary>
        ''' <param name="context">基因名 → 表达值（大小写不敏感；可以只覆盖部分基因）</param>
        ''' <param name="reference">模型侧的训练统计量</param>
        ''' <param name="mode">对齐策略</param>
        ''' <param name="options">可调参数；为空时使用默认值</param>
        ''' <returns>对齐结果（只包含命中模型基因的条目）</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="reference"/> 为空</exception>
        ''' <exception cref="InvalidOperationException">所选模式缺少必需的参考统计量</exception>
        ''' <exception cref="InvalidDataException">命中基因的取值不是有限数，且越界策略为 Reject</exception>
        ''' <remarks>
        ''' 故意不提供“缺数据就静默降级为原值”的分支：静默降级正是此前 FuzzyGene / BNLearn
        ''' 出现尺度错位的根源——调用方以为已经对齐，实际并没有。
        ''' </remarks>
        Public Function Align(context As IDictionary(Of String, Double),
                              reference As ContextAlignmentReference,
                              mode As ContextAlignmentMode,
                              Optional options As ContextAlignmentOptions = Nothing) As ContextAlignmentResult

            If reference Is Nothing Then
                Throw New ArgumentNullException(NameOf(reference))
            End If

            If options Is Nothing Then
                options = ContextAlignmentOptions.Default
            End If

            Dim diagnostics As New ContextAlignmentDiagnostics With {
                .Mode = mode,
                .NGene = reference.NGene
            }
            Dim result As New ContextAlignmentResult With {
                .Mode = mode,
                .Diagnostics = diagnostics
            }

            If context Is Nothing OrElse context.Count = 0 Then
                Return result
            End If

            Call reference.Validate(mode, "ContextAligner.Align")

            ' ---- 1. 基因名匹配 + 有效性过滤 ----
            Dim matched As New List(Of MatchedGene)
            Dim position As New Dictionary(Of Integer, Integer)
            Dim unknown As New List(Of String)

            For Each item As KeyValuePair(Of String, Double) In context
                Dim name As String = If(item.Key, "").Trim()

                If name.Length = 0 Then
                    diagnostics.InvalidValues += 1
                    Continue For
                End If

                Dim value As Double = item.Value

                If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                    diagnostics.InvalidValues += 1
                    Continue For
                End If

                Dim index As Integer

                If Not reference.TryGetIndex(name, index) Then
                    Call unknown.Add(name)
                    Continue For
                End If

                Dim entry As New MatchedGene With {
                    .Gene = name,
                    .Index = index,
                    .Value = value
                }
                Dim slot As Integer

                If position.TryGetValue(index, slot) Then
                    ' 同名基因以最后一次出现的为准
                    matched(slot) = entry
                Else
                    position(index) = matched.Count
                    Call matched.Add(entry)
                End If
            Next

            diagnostics.Provided = matched.Count + unknown.Count
            diagnostics.Matched = matched.Count
            diagnostics.UnknownGenes = unknown.ToArray()

            If matched.Count = 0 Then
                diagnostics.AddNote("上下文中没有任何基因能够映射到模型上，请检查基因标识空间")
                Return result
            End If

            ' ---- 2. 按模式映射 ----
            Dim values As Dictionary(Of String, Double)

            Select Case mode
                Case ContextAlignmentMode.None
                    values = mapNone(matched)
                Case ContextAlignmentMode.Affine
                    values = mapAffine(matched, reference, options, diagnostics)
                Case ContextAlignmentMode.LogAffine
                    values = mapLogAffine(matched, reference, options, diagnostics)
                Case ContextAlignmentMode.Quantile
                    values = mapQuantile(matched, reference, options, diagnostics)
                Case ContextAlignmentMode.QuantileDelta
                    values = mapQuantileDelta(matched, reference, options, diagnostics)
                Case Else
                    Throw New InvalidOperationException($"未知的对齐模式：{mode}")
            End Select

            ' ---- 3. 支持域越界检查 ----
            Call checkSupport(matched, values, reference, options, diagnostics)

            For Each entry As MatchedGene In matched
                result.Raw(entry.Gene) = entry.Value

                Dim aligned As Double

                If values.TryGetValue(entry.Gene, aligned) Then
                    result.Values(entry.Gene) = aligned
                End If
            Next

            diagnostics.Aligned = mode <> ContextAlignmentMode.None
            diagnostics.SpearmanBeforeAfter = spearmanOf(matched, result.Values)

            ' ---- 4. 未覆盖基因 ----
            Dim missing As New List(Of String)

            For i As Integer = 0 To reference.NGene - 1
                Dim name As String = If(reference.GeneNames Is Nothing, Nothing, reference.GeneNames(i))

                If String.IsNullOrEmpty(name) Then
                    Continue For
                End If
                If Not result.Values.ContainsKey(name) Then
                    Call missing.Add(name)

                    If options.FillMissingGenes Then
                        result.Values(name) = fillValue(reference, i, options.MissingGeneFill)
                    End If
                End If
            Next

            diagnostics.MissingGenes = missing.ToArray()

            Return result
        End Function

        ' ==================== 各模式的映射实现 ====================

        ''' <summary>原值直灌。</summary>
        Private Function mapNone(matched As List(Of MatchedGene)) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For Each item As MatchedGene In matched
                result(item.Gene) = item.Value
            Next

            Return result
        End Function

        ''' <summary>逐基因 z-score：<c>(x - μ) / σ</c>；常量基因映射到 0（即该基因的均值）。</summary>
        Private Function mapAffine(matched As List(Of MatchedGene),
                                   reference As ContextAlignmentReference,
                                   options As ContextAlignmentOptions,
                                   diagnostics As ContextAlignmentDiagnostics) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            Dim constant As Integer = 0

            For Each item As MatchedGene In matched
                Dim sd As Double = reference.StandardDeviations(item.Index)
                Dim mapped As Double

                If std.Abs(sd) <= options.ConstantTolerance Then
                    mapped = 0.0R
                    constant += 1
                Else
                    mapped = (item.Value - reference.Means(item.Index)) / sd
                End If

                result(item.Gene) = mapped
            Next

            If constant > 0 Then
                diagnostics.AddNote($"{constant} 个常量基因（训练标准差≈0）已映射到各自均值对应的 0")
            End If

            Return result
        End Function

        ''' <summary>对数换算：<c>log2(max(x,0) + pseudo) + δ</c>。</summary>
        Private Function mapLogAffine(matched As List(Of MatchedGene),
                                      reference As ContextAlignmentReference,
                                      options As ContextAlignmentOptions,
                                      diagnostics As ContextAlignmentDiagnostics) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            Dim negative As Integer = 0

            For Each item As MatchedGene In matched
                If item.Value < 0.0R Then
                    negative += 1
                End If

                Dim raw As Double = If(item.Value < 0.0R, 0.0R, item.Value)
                Dim offset As Double = 0.0R

                If reference.Offsets IsNot Nothing AndAlso item.Index < reference.Offsets.Length Then
                    offset = reference.Offsets(item.Index)
                End If

                result(item.Gene) = std.Log(raw + reference.PseudoCount, 2.0) + offset
            Next

            If negative > 0 Then
                diagnostics.AddNote($"{negative} 个基因的上下文取值为负，已在对数变换前裁剪为 0")
            End If

            Return result
        End Function

        ''' <summary>
        ''' 分位映射：跨基因分位 <c>p_i</c> → 该基因训练种群的同分位。
        ''' </summary>
        ''' <remarks>
        ''' 算法与旧 GeneTree 的 <c>ExpressionContextAlignment.Align</c> 逐位一致：
        ''' 源分布在 <c>log1p</c> 域排序（对秩无影响，等价于纯秩映射），
        ''' 重复值用中位秩，目标分位用线性插值。
        ''' </remarks>
        Private Function mapQuantile(matched As List(Of MatchedGene),
                                     reference As ContextAlignmentReference,
                                     options As ContextAlignmentOptions,
                                     diagnostics As ContextAlignmentDiagnostics) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            If matched.Count < 2 Then
                diagnostics.AddNote(
                    $"上下文只包含 {matched.Count} 个基因，跨基因分位无定义，已按给定值直接注入")

                For Each item As MatchedGene In matched
                    result(item.Gene) = item.Value
                Next

                Return result
            End If

            Dim source As Double() = contextDistribution(matched)
            Dim constant As Integer = 0

            For Each item As MatchedGene In matched
                Dim population As Double() = reference.SortedPopulation(item.Index)
                Dim mapped As Double

                If population Is Nothing OrElse population.Length = 0 Then
                    mapped = std.Exp(item.LogValue) - 1.0R
                ElseIf population.Length = 1 OrElse
                       population(population.Length - 1) - population(0) <= options.ConstantTolerance Then

                    mapped = population(0)
                    constant += 1
                Else
                    Dim p As Double = PercentileOf(source, item.LogValue)

                    mapped = QuantileOf(population, population.Length, p)
                End If

                If options.ClipNegative AndAlso mapped < 0.0R Then
                    mapped = 0.0R
                End If

                result(item.Gene) = mapped
            Next

            If constant > 0 Then
                diagnostics.AddNote($"{constant} 个常量基因（训练种群只有单一取值）已映射为该常数")
            End If

            Return result
        End Function

        ''' <summary>
        ''' 分位偏移：<c>x + (Q_g(p) - Q_source(p))</c>，在分位映射的基础上保留相对幅度。
        ''' </summary>
        ''' <remarks>
        ''' <c>Q_source</c> 取训练均值向量的跨基因经验分布：它刻画了「在模型看来，
        ''' 位于第 p 分位的基因通常有多高」，把这一部分扣掉之后，剩下的偏移量
        ''' 才是 context 相对训练均值的真实变化。
        ''' </remarks>
        Private Function mapQuantileDelta(matched As List(Of MatchedGene),
                                          reference As ContextAlignmentReference,
                                          options As ContextAlignmentOptions,
                                          diagnostics As ContextAlignmentDiagnostics) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            If matched.Count < 2 Then
                diagnostics.AddNote(
                    $"上下文只包含 {matched.Count} 个基因，跨基因分位无定义，已按给定值直接注入")

                For Each item As MatchedGene In matched
                    result(item.Gene) = item.Value
                Next

                Return result
            End If

            Dim source As Double() = contextDistribution(matched)
            Dim referenceDistribution As Double() = sortedMeans(reference)

            If referenceDistribution.Length < 2 Then
                diagnostics.AddNote("训练均值向量的有效取值不足 2 个，QuantileDelta 退化为分位映射")

                Return mapQuantile(matched, reference, options, diagnostics)
            End If

            For Each item As MatchedGene In matched
                Dim population As Double() = reference.SortedPopulation(item.Index)
                Dim p As Double = PercentileOf(source, item.LogValue)
                Dim target As Double
                Dim baseline As Double

                If population Is Nothing OrElse population.Length = 0 Then
                    target = std.Exp(item.LogValue) - 1.0R
                ElseIf population.Length = 1 OrElse
                       population(population.Length - 1) - population(0) <= options.ConstantTolerance Then

                    target = population(0)
                Else
                    target = QuantileOf(population, population.Length, p)
                End If

                baseline = QuantileOf(referenceDistribution, referenceDistribution.Length, p)

                Dim mapped As Double = item.Value + (target - baseline)

                If options.ClipNegative AndAlso mapped < 0.0R Then
                    mapped = 0.0R
                End If

                result(item.Gene) = mapped
            Next

            Return result
        End Function

        ' ==================== 支持域检查 ====================

        Private Sub checkSupport(matched As List(Of MatchedGene),
                                 values As Dictionary(Of String, Double),
                                 reference As ContextAlignmentReference,
                                 options As ContextAlignmentOptions,
                                 diagnostics As ContextAlignmentDiagnostics)
            Dim outside As New List(Of String)
            Dim clamped As Integer = 0

            For Each item As MatchedGene In matched
                Dim lower As Double
                Dim upper As Double

                If Not tryGetSupport(reference, item.Index, lower, upper) Then
                    Continue For
                End If

                Dim value As Double

                If Not values.TryGetValue(item.Gene, value) Then
                    Continue For
                End If

                Dim below As Boolean = value < lower - options.SupportTolerance
                Dim above As Boolean = value > upper + options.SupportTolerance

                If Not below AndAlso Not above Then
                    Continue For
                End If

                Call outside.Add(item.Gene)

                Select Case options.OutOfSupport
                    Case ContextOutOfSupportPolicy.Clamp
                        values(item.Gene) = If(below, lower, upper)
                        clamped += 1
                    Case ContextOutOfSupportPolicy.Reject
                        ' 收集完之后统一抛出，便于一次性看到全部越界基因
                    Case Else
                        ' Report：只统计
                End Select
            Next

            diagnostics.OutsideSupport = outside.Count
            diagnostics.ClampedGenes = clamped

            If outside.Count = 0 Then
                Return
            End If

            If options.OutOfSupport = ContextOutOfSupportPolicy.Reject Then
                Throw New InvalidDataException(
                    $"{outside.Count} 个基因的对齐结果落在训练支持域之外，已按 Reject 策略拒绝：" &
                    String.Join(", ", outside.Take(10)) & If(outside.Count > 10, ", ...", ""))
            End If
        End Sub

        Private Function tryGetSupport(reference As ContextAlignmentReference,
                                       index As Integer,
                                       ByRef lower As Double,
                                       ByRef upper As Double) As Boolean
            If reference.SupportMin IsNot Nothing AndAlso reference.SupportMax IsNot Nothing AndAlso
               index < reference.SupportMin.Length AndAlso index < reference.SupportMax.Length Then

                lower = reference.SupportMin(index)
                upper = reference.SupportMax(index)

                Return Not (Double.IsNaN(lower) OrElse Double.IsNaN(upper)) AndAlso upper >= lower
            End If

            Dim population As Double() = reference.SortedPopulation

            If population Is Nothing OrElse index >= population.Length Then
                Return False
            End If

            Dim values As Double() = population(index)

            If values Is Nothing OrElse values.Length = 0 Then
                Return False
            End If

            lower = values(0)
            upper = values(values.Length - 1)

            Return upper >= lower
        End Function

        ' ==================== 分位工具函数 ====================

        ''' <summary>
        ''' 在已升序排序的数组上取 <paramref name="x"/> 所对应的分位（重复值用中位秩）。
        ''' </summary>
        ''' <param name="sorted">升序数组</param>
        ''' <param name="x">查询值（必须来自同一个数组）</param>
        ''' <returns>落在 (0, 1) 区间的分位</returns>
        Public Function PercentileOf(sorted As Double(), x As Double) As Double
            Dim n As Integer = sorted.Length

            If n = 0 Then
                Return 0.0R
            ElseIf n = 1 Then
                Return 0.5R
            End If

            Dim idx As Integer = Array.BinarySearch(sorted, x)
            Dim rank As Double

            If idx >= 0 Then
                Dim first As Integer = idx
                Dim last As Integer = idx

                While first > 0 AndAlso sorted(first - 1) = x
                    first -= 1
                End While
                While last < n - 1 AndAlso sorted(last + 1) = x
                    last += 1
                End While

                rank = (first + last) / 2.0R + 0.5R
            Else
                ' Array.BinarySearch 未命中时返回 ~insertionPoint，取反即可还原插入位置
                rank = Not idx
            End If

            Dim p As Double = rank / n

            If p <= 0.0R Then
                Return 1.0R / n
            ElseIf p >= 1.0R Then
                Return 1.0R - 1.0R / n
            Else
                Return p
            End If
        End Function

        ''' <summary>
        ''' 在升序数组的前 <paramref name="count"/> 个元素上取 <paramref name="p"/> 分位（线性插值）。
        ''' </summary>
        ''' <param name="sorted">升序数组</param>
        ''' <param name="count">有效元素数量</param>
        ''' <param name="p">分位，取值 [0, 1]</param>
        Public Function QuantileOf(sorted As Double(), count As Integer, p As Double) As Double
            If count <= 0 Then
                Return 0.0R
            ElseIf count = 1 Then
                Return sorted(0)
            End If

            If p <= 0.0R Then
                Return sorted(0)
            ElseIf p >= 1.0R Then
                Return sorted(count - 1)
            End If

            Dim h As Double = p * (count - 1)
            Dim lo As Integer = CInt(std.Floor(h))
            Dim frac As Double = h - lo

            If lo >= count - 1 Then
                Return sorted(count - 1)
            Else
                Return sorted(lo) + frac * (sorted(lo + 1) - sorted(lo))
            End If
        End Function

        ' ==================== 内部辅助 ====================

        ''' <summary>上下文的跨基因经验分布（<c>log1p</c> 域，升序）。</summary>
        Private Function contextDistribution(matched As List(Of MatchedGene)) As Double()
            Dim source(matched.Count - 1) As Double

            For i As Integer = 0 To matched.Count - 1
                Dim value As Double = matched(i).Value
                Dim logValue As Double = std.Log(1.0R + If(value < 0.0R, 0.0R, value))

                matched(i).LogValue = logValue
                source(i) = logValue
            Next

            Call Array.Sort(source)

            Return source
        End Function

        ''' <summary>训练均值向量的跨基因经验分布（升序，剔除无效值）。</summary>
        Private Function sortedMeans(reference As ContextAlignmentReference) As Double()
            Dim buffer(reference.NGene - 1) As Double
            Dim count As Integer = 0

            For i As Integer = 0 To reference.NGene - 1
                Dim v As Double = reference.Means(i)

                If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                    Continue For
                End If

                buffer(count) = v
                count += 1
            Next

            If count = 0 Then
                Return New Double() {}
            End If

            Call Array.Sort(buffer, 0, count)
            Call Array.Resize(buffer, count)

            Return buffer
        End Function

        ''' <summary>对齐前后的秩相关（仅命中基因，按模型下标排序）。</summary>
        Private Function spearmanOf(matched As List(Of MatchedGene),
                                    values As Dictionary(Of String, Double)) As Double
            If matched.Count < 3 Then
                Return Double.NaN
            End If

            Dim ordered As MatchedGene() = matched.ToArray()

            Call Array.Sort(ordered, Function(a, b) a.Index.CompareTo(b.Index))

            Dim before(ordered.Length - 1) As Double
            Dim after(ordered.Length - 1) As Double

            For i As Integer = 0 To ordered.Length - 1
                Dim aligned As Double

                If Not values.TryGetValue(ordered(i).Gene, aligned) Then
                    Return Double.NaN
                End If

                before(i) = ordered(i).Value
                after(i) = aligned
            Next

            Return pearson(ranks(before), ranks(after))
        End Function

        Private Function ranks(values As Double()) As Double()
            Dim n As Integer = values.Length
            Dim order(n - 1) As Integer

            For i As Integer = 0 To n - 1
                order(i) = i
            Next

            Call Array.Sort(order, Function(a, b) values(a).CompareTo(values(b)))

            Dim result(n - 1) As Double
            Dim i0 As Integer = 0

            While i0 < n
                Dim j As Integer = i0

                While j + 1 < n AndAlso values(order(j + 1)) = values(order(i0))
                    j += 1
                End While

                Dim average As Double = (i0 + j) / 2.0R + 1.0R

                For k As Integer = i0 To j
                    result(order(k)) = average
                Next

                i0 = j + 1
            End While

            Return result
        End Function

        Private Function pearson(a As Double(), b As Double()) As Double
            Dim n As Integer = a.Length

            If n < 2 Then
                Return Double.NaN
            End If

            Dim meanA As Double = 0.0R
            Dim meanB As Double = 0.0R

            For i As Integer = 0 To n - 1
                meanA += a(i)
                meanB += b(i)
            Next

            meanA /= n
            meanB /= n

            Dim cov As Double = 0.0R
            Dim va As Double = 0.0R
            Dim vb As Double = 0.0R

            For i As Integer = 0 To n - 1
                Dim da As Double = a(i) - meanA
                Dim db As Double = b(i) - meanB

                cov += da * db
                va += da * da
                vb += db * db
            Next

            If va <= 0.0R OrElse vb <= 0.0R Then
                Return Double.NaN
            End If

            Return cov / std.Sqrt(va * vb)
        End Function

        Private Function fillValue(reference As ContextAlignmentReference,
                                   index As Integer,
                                   fill As ContextMissingGeneFill) As Double
            Select Case fill
                Case ContextMissingGeneFill.Mean
                    If reference.Means IsNot Nothing AndAlso index < reference.Means.Length Then
                        Return reference.Means(index)
                    End If
                Case ContextMissingGeneFill.Minimum
                    Dim population As Double() = reference.SortedPopulation

                    If population IsNot Nothing AndAlso index < population.Length AndAlso
                       population(index) IsNot Nothing AndAlso population(index).Length > 0 Then

                        Return population(index)(0)
                    End If
                Case Else
                    Return 0.0R
            End Select

            Return 0.0R
        End Function

        ''' <summary>匹配条目：基因名、模型下标与原始取值。</summary>
        Private Class MatchedGene
            Public Gene As String
            Public Index As Integer
            Public Value As Double
            Public LogValue As Double
        End Class

    End Module

End Namespace
