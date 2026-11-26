Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Statistics.Copula
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports std = System.Math

Namespace Evidence

    ''' <summary>
    ''' 池化所需的"细胞系因子"：每个训练行在基因轴、扰动轴、整行三个方向上的权重。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对应上游 <c>pooling/weights.py</c> 的 <c>Features</c> 与 <c>CellLineFactors</c>。
    ''' 相似度、配对、噪声比这类因子定义在"主视图训练行的基因并集"上
    ''' （因为相似度与噪声比要用训练数据的完整谱），取到任务基因轴时通过 <see cref="Gidx"/> 映射；
    ''' 映射不到时权重按 1 处理（与上游 <c>block</c> 把 NaN 归一为 1 的行为一致）。
    ''' </para>
    ''' <para>
    ''' 因子含义：<b>nr</b> 噪声比、<b>sef</b> 尺度不确定性、<b>kd</b> 敲低效率、
    ''' <b>sim</b> 对照表达谱相似度、<b>pair</b> 配对降权、<b>auxiliary</b> 辅助行倍数。
    ''' </para>
    ''' </remarks>
    Public Class PoolingFactors

        Private Const VarianceFloor As Double = 0.00000001

        ''' <summary>主视图训练行名（相似度/配对因子的下标空间）。</summary>
        Public ReadOnly Property Lines As String()

        ''' <summary>训练基因并集（字典序）。</summary>
        Public ReadOnly Property Union As String()

        ''' <summary>任务扰动轴。</summary>
        Public ReadOnly Property Perts As String()

        ''' <summary>任务基因 → 并集下标（-1 表示该基因不在训练并集里）。</summary>
        Public ReadOnly Property Gidx As Integer()

        ''' <summary>池化行 → <see cref="Lines"/> 下标（-1 表示该行没有主视图因子，例如辅助行）。</summary>
        Public ReadOnly Property Index As Integer()

        ''' <summary>噪声比因子（行数×并集）。</summary>
        Public ReadOnly Property NoiseRatio As Double()()

        ''' <summary>尺度误差因子（行数×P）。</summary>
        Public ReadOnly Property SeFactor As Double()()

        ''' <summary>敲低因子（行数×P）。</summary>
        Public ReadOnly Property Knockdown As Double()()

        ''' <summary>相似度因子（主视图行数）。</summary>
        Public ReadOnly Property SimFactor As Double()

        ''' <summary>配对降权因子（主视图行数）。</summary>
        Public ReadOnly Property PairFactor As Double()

        Private Sub New(lines As String(), union As String(), perts As String(), gidx As Integer(),
                        index As Integer(), noiseRatio As Double()(), se As Double()(), kd As Double()(),
                        sim As Double(), pair As Double())
            Me.Lines = lines
            Me.Union = union
            Me.Perts = perts
            Me.Gidx = gidx
            Me.Index = index
            Me.NoiseRatio = noiseRatio
            Me.SeFactor = se
            Me.Knockdown = kd
            Me.SimFactor = sim
            Me.PairFactor = pair
        End Sub

        ''' <summary>主视图行数。</summary>
        Public ReadOnly Property LineCount As Integer
            Get
                Return If(Lines Is Nothing, 0, Lines.Length)
            End Get
        End Property

#Region "factory"

        ''' <summary>
        ''' 计算全部细胞系因子。
        ''' </summary>
        ''' <param name="bundle">训练摘要（内存版）</param>
        ''' <param name="genes">任务基因轴</param>
        ''' <param name="perturbations">任务扰动轴</param>
        ''' <param name="targetCpm">新上下文对照细胞的平均 CPM（按任务基因轴），用于相似度</param>
        ''' <param name="model">模型参数</param>
        ''' <param name="rowLines">池化行对应的细胞系名（顺序与池化栈一致）</param>
        ''' <param name="exclude">需要排除的细胞系</param>
        Public Shared Function Compute(bundle As SummaryBundle,
                                       genes As String(),
                                       perturbations As String(),
                                       targetCpm As Double(),
                                       model As CellularAiModel,
                                       rowLines As String(),
                                       Optional exclude As IEnumerable(Of String) = Nothing) As PoolingFactors

            If bundle Is Nothing Then
                Throw New ArgumentNullException(NameOf(bundle))
            End If

            Dim stacked = bundle.StackEffects(perturbations, exclude)
            Dim lines As String() = stacked.Lines
            Dim union As String() = stacked.Union
            Dim nUnion As Integer = union.Length
            Dim nPerts As Integer = perturbations.Length
            Dim nLines As Integer = lines.Length

            ' 任务基因 → 并集下标
            Dim position As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To nUnion - 1
                position(union(i)) = i
            Next

            Dim gidx(If(genes Is Nothing, -1, genes.Length - 1)) As Integer

            For i As Integer = 0 To gidx.Length - 1
                Dim at As Integer = -1

                Call position.TryGetValue(genes(i), at)
                gidx(i) = at
            Next

            ' 池化行 → 主视图行下标
            Dim lineIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To nLines - 1
                lineIndex(lines(i)) = i
            Next

            Dim index(If(rowLines Is Nothing, -1, rowLines.Length - 1)) As Integer

            For i As Integer = 0 To index.Length - 1
                Dim at As Integer = -1

                Call lineIndex.TryGetValue(rowLines(i), at)
                index(i) = at
            Next

            ' 控制剖面 CPM（主视图行 × 并集）
            Dim profiles As Dictionary(Of String, ControlProfile) = bundle.ControlProfiles(exclude)
            Dim cpm As Double()() = RectangularArray.Matrix(Of Double)(nLines, nUnion)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    cpm(t)(j) = Double.NaN
                Next
            Next

            For t As Integer = 0 To nLines - 1
                Dim profile As ControlProfile = Nothing

                If Not profiles.TryGetValue(lines(t), profile) Then
                    Continue For
                End If

                Dim lookup As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For i As Integer = 0 To profile.Count - 1
                    lookup(profile.Genes(i)) = i
                Next

                Dim total As Double = profile.Total

                If total <= 0.0 Then
                    total = 1.0
                End If

                For j As Integer = 0 To nUnion - 1
                    Dim at As Integer = -1

                    If lookup.TryGetValue(union(j), at) Then
                        cpm(t)(j) = 1000000.0 * profile.Counts(at) / total
                    End If
                Next
            Next

            ' 新上下文的目标 CPM 映射到并集（不在任务轴上的基因为 NaN）
            Dim tcpm As Double() = New Double(If(nUnion = 0, -1, nUnion - 1)) {}

            For j As Integer = 0 To nUnion - 1
                tcpm(j) = Double.NaN
            Next

            If targetCpm IsNot Nothing AndAlso genes IsNot Nothing Then
                Dim target As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For i As Integer = 0 To genes.Length - 1
                    target(genes(i)) = i
                Next

                For j As Integer = 0 To nUnion - 1
                    Dim at As Integer = -1

                    If target.TryGetValue(union(j), at) Then
                        tcpm(j) = targetCpm(at)
                    End If
                Next
            End If

            ' 敲低因子（池化行 × P）
            Dim rowCount As Integer = index.Length
            Dim knockdown As Double()() = RectangularArray.Matrix(Of Double)(rowCount, nPerts)

            For t As Integer = 0 To rowCount - 1
                For p As Integer = 0 To nPerts - 1
                    Dim own As Double = Double.NaN
                    Dim at As Integer = -1
                    Dim line As Integer = index(t)

                    If line >= 0 AndAlso line < nLines AndAlso position.TryGetValue(perturbations(p), at) Then
                        own = stacked.Stack(line)(at)(p)
                    End If

                    If Double.IsNaN(own) Then
                        knockdown(t)(p) = 1.0
                    Else
                        Dim kd As Double = 1.0 - std.Pow(2.0, own)
                        Dim scaled As Double = kd / model.Pooling.KdRef

                        knockdown(t)(p) = std.Max(std.Min(scaled, 1.0), model.Pooling.KdFloor)
                    End If
                Next
            Next

            Dim se As Double()() = ComputeSeFactor(bundle, lines, perturbations, model.Pooling)
            Dim sim As Double() = ComputeSimFactor(cpm, tcpm, model.Pooling, model.Guards)
            Dim pair As Double() = ComputePairFactor(bundle, lines, model.Pooling.PairWeight)

            Dim nullVariance As Double()() = bundle.NullVariance(lines, union)
            Dim noiseRatio As Double()() = ComputeNoiseRatio(nullVariance, stacked.Stack, model.Pooling.Noise, model.Guards.NrClip)

            Call Guards.Share("pooling", sim, model.Guards)

            Return New PoolingFactors(lines, union, perturbations, gidx, index,
                                      noiseRatio, se, knockdown, sim, pair)
        End Function

        ''' <summary>
        ''' 全 1 因子：用于"关闭所有细胞系加权"的基线对照（例如与 <c>EvidenceCombination.Stouffer</c> 对拍）。
        ''' </summary>
        Public Shared Function Identity(lines As String(), union As String(),
                                        perturbations As String(), genes As String()) As PoolingFactors
            Dim nLines As Integer = If(lines Is Nothing, 0, lines.Length)
            Dim nUnion As Integer = If(union Is Nothing, 0, union.Length)
            Dim nPerts As Integer = If(perturbations Is Nothing, 0, perturbations.Length)

            Dim onesRow As Double()() = RectangularArray.Matrix(Of Double)(nLines, nUnion)
            Dim onesCol As Double()() = RectangularArray.Matrix(Of Double)(nLines, nPerts)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    onesRow(t)(j) = 1.0
                Next
                For p As Integer = 0 To nPerts - 1
                    onesCol(t)(p) = 1.0
                Next
            Next

            Dim sim(nLines - 1) As Double
            Dim pair(nLines - 1) As Double

            For t As Integer = 0 To nLines - 1
                sim(t) = 1.0
                pair(t) = 1.0
            Next

            Dim position As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To nUnion - 1
                position(union(i)) = i
            Next

            Dim gidx(If(genes Is Nothing, -1, genes.Length - 1)) As Integer

            For i As Integer = 0 To gidx.Length - 1
                Dim at As Integer = -1

                Call position.TryGetValue(genes(i), at)
                gidx(i) = at
            Next

            Dim index(nLines - 1) As Integer

            For t As Integer = 0 To nLines - 1
                index(t) = t
            Next

            Return New PoolingFactors(lines, union, perturbations, gidx, index,
                                      onesRow, onesCol, onesCol, sim, pair)
        End Function

#End Region

#Region "weights"

        ''' <summary>
        ''' 行方向权重：<paramref name="names"/> 里的因子在 (行, 基因) 上的乘积；映射不到时按 1 处理。
        ''' </summary>
        Public Function RowWeight(row As Integer, names As String(), gene As Integer) As Double
            If names Is Nothing OrElse Gidx Is Nothing OrElse Index Is Nothing Then
                Return 1.0
            End If

            Dim at As Integer = Gidx(gene)
            Dim line As Integer = If(row < Index.Length, Index(row), -1)

            If at < 0 OrElse line < 0 Then
                Return 1.0
            End If

            Dim w As Double = 1.0

            For Each name As String In names
                Select Case name
                    Case FactorExpression.LocalVarianceFactor
                        Continue For
                    Case "nr"
                        w *= Sanitize(NoiseRatio(line)(at))
                    Case Else
                        Continue For
                End Select
            Next

            Return w
        End Function

        ''' <summary>列方向权重：<paramref name="names"/> 里的因子在 (行, 扰动) 上的乘积。</summary>
        Public Function ColWeight(row As Integer, p As Integer, names As String()) As Double
            If names Is Nothing Then
                Return 1.0
            End If

            Dim w As Double = 1.0

            For Each name As String In names
                Select Case name
                    Case "kd"
                        w *= Sanitize(Knockdown(row)(p))
                    Case "sef"
                        w *= Sanitize(SeFactor(row)(p))
                    Case Else
                        Continue For
                End Select
            Next

            Return w
        End Function

        ''' <summary>整行权重：<paramref name="names"/> 里的因子在主视图行维度上的乘积。</summary>
        Public Function MatWeight(row As Integer, names As String()) As Double
            If names Is Nothing Then
                Return 1.0
            End If

            Dim line As Integer = If(Index Is Nothing OrElse row >= Index.Length, -1, Index(row))
            Dim w As Double = 1.0

            For Each name As String In names
                Select Case name
                    Case "sim"
                        w *= Sanitize(If(line >= 0, SimFactor(line), 1.0))
                    Case "pair"
                        w *= Sanitize(If(line >= 0, PairFactor(line), 1.0))
                    Case Else
                        ' auxiliary 由调用方按行加权，不在这里重复
                        Continue For
                End Select
            Next

            Return w
        End Function

        Private Shared Function Sanitize(value As Double) As Double
            Return If(Double.IsNaN(value) OrElse Double.IsInfinity(value), 1.0, value)
        End Function

#End Region

#Region "factor kernels"

        ''' <summary>
        ''' 尺度误差因子：<c>(median(medians)/median(scale))^(2α)</c>，裁剪到 <c>[1/clip, clip]</c>。
        ''' </summary>
        Private Shared Function ComputeSeFactor(bundle As SummaryBundle,
                                                lines As String(),
                                                perturbations As String(),
                                                pooling As PoolingOptions) As Double()()
            Dim nLines As Integer = lines.Length
            Dim nPerts As Integer = perturbations.Length
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nLines, nPerts)

            For t As Integer = 0 To nLines - 1
                For p As Integer = 0 To nPerts - 1
                    result(t)(p) = 1.0
                Next
            Next

            For t As Integer = 0 To nLines - 1
                Dim row As SummaryRow = Nothing

                Try
                    row = bundle.Catalog.PrimaryRow(lines(t))
                Catch ex As KeyNotFoundException
                    Continue For
                End Try

                If row.Scale Is Nothing Then
                    Continue For
                End If

                Dim medians(nPerts - 1) As Double
                Dim pertIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For p As Integer = 0 To row.EffectPerts.Length - 1
                    pertIndex(row.EffectPerts(p)) = p
                Next

                Dim allMedians As New List(Of Double)

                For p As Integer = 0 To nPerts - 1
                    Dim at As Integer = -1
                    Dim column As New List(Of Double)

                    If pertIndex.TryGetValue(perturbations(p), at) Then
                        For g As Integer = 0 To row.Scale.Length - 1
                            Dim v As Double = row.Scale(g)(at)

                            If Not Double.IsNaN(v) AndAlso v > 0.0 Then
                                Call column.Add(v)
                            End If
                        Next
                    End If

                    Dim median As Double = NanMedian(column.ToArray())

                    medians(p) = median

                    If Not Double.IsNaN(median) Then
                        Call allMedians.Add(median)
                    End If
                Next

                Dim overall As Double = NanMedian(allMedians.ToArray())

                If Double.IsNaN(overall) OrElse overall <= 0.0 Then
                    Continue For
                End If

                For p As Integer = 0 To nPerts - 1
                    Dim median As Double = medians(p)

                    If Double.IsNaN(median) OrElse median <= 0.0 Then
                        Continue For
                    End If

                    Dim ratio As Double = std.Pow(overall / median, 2.0 * pooling.Se)

                    result(t)(p) = If(Double.IsFinite(ratio), std.Max(std.Min(ratio, pooling.SeClip), 1.0 / pooling.SeClip), 1.0)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 相似度因子：对照 CPM 与目标 CPM 在高变基因上的秩相关，z 标准化后取 <c>exp(β·z)</c>。
        ''' </summary>
        Private Shared Function ComputeSimFactor(cpm As Double()(), tcpm As Double(),
                                                 pooling As PoolingOptions,
                                                 guards As GuardOptions) As Double()
            Dim nLines As Integer = cpm.Length

            If nLines = 0 Then
                Return New Double(-1) {}
            End If

            Dim nUnion As Integer = cpm(0).Length
            Dim logProfiles As Double()() = RectangularArray.Matrix(Of Double)(nLines, nUnion)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    Dim v As Double = cpm(t)(j)

                    logProfiles(t)(j) = If(Double.IsNaN(v), Double.NaN, std.Log(1.0 + v))
                Next
            Next

            Dim variance As Double() = New Double(If(nUnion = 0, -1, nUnion - 1)) {}
            Dim hvg As New List(Of Integer)

            For j As Integer = 0 To nUnion - 1
                Dim column(nLines - 1) As Double

                For t As Integer = 0 To nLines - 1
                    column(t) = logProfiles(t)(j)
                Next

                variance(j) = NanVariance(column)
            Next

            Dim order As Integer() = Enumerable.Range(0, nUnion).
                OrderBy(Function(j) If(Double.IsNaN(variance(j)), 0.0, variance(j))).
                ToArray

            Dim take As Integer = std.Min(std.Max(pooling.SimHvg, 1), nUnion)

            For i As Integer = order.Length - take To order.Length - 1
                Call hvg.Add(order(i))
            Next

            Dim correlations(nLines - 1) As Double
            Dim target As Double() = New Double(If(hvg.Count = 0, -1, hvg.Count - 1)) {}

            For i As Integer = 0 To hvg.Count - 1
                target(i) = std.Log(1.0 + tcpm(hvg(i)))
            Next

            For t As Integer = 0 To nLines - 1
                Dim sample As Double() = New Double(If(hvg.Count = 0, -1, hvg.Count - 1)) {}

                For i As Integer = 0 To hvg.Count - 1
                    sample(i) = logProfiles(t)(hvg(i))
                Next

                correlations(t) = SpearmanOmit(sample, target)
            Next

            Return SimWeight(correlations, pooling.Sim, guards.SimSdMin, guards.SimZClip)
        End Function

        ''' <summary><c>exp(β·clip((r − mean)/max(sd, sd_min), ±z_clip))</c>。</summary>
        Private Shared Function SimWeight(correlation As Double(), beta As Double, sdMin As Double, zClip As Double) As Double()
            Dim count As Integer = correlation.Length
            Dim result(count - 1) As Double
            Dim finite As Double() = correlation.Where(Function(v) Not Double.IsNaN(v)).ToArray

            If finite.Length = 0 Then
                For t As Integer = 0 To count - 1
                    result(t) = 1.0
                Next

                Return result
            End If

            Dim mean As Double = finite.Average
            Dim variance As Double = 0.0

            For i As Integer = 0 To finite.Length - 1
                Dim d As Double = finite(i) - mean

                variance += d * d
            Next

            Dim floor As Double = std.Max(std.Sqrt(variance / finite.Length), sdMin)
            Dim z As Double() = New Double(count - 1) {}

            For t As Integer = 0 To count - 1
                z(t) = If(Double.IsNaN(correlation(t)), 0.0, (correlation(t) - mean) / floor)
            Next

            Dim clipped As Double() = Guards.ClipAbs(z, zClip, "sim_z_clip")

            For t As Integer = 0 To count - 1
                result(t) = If(Double.IsNaN(correlation(t)), 1.0, std.Exp(beta * clipped(t)))
            Next

            Return result
        End Function

        ''' <summary>配对降权：交替记录或同组记录数 ≥ 2 时降权。</summary>
        Private Shared Function ComputePairFactor(bundle As SummaryBundle, lines As String(), weight As Double) As Double()
            Dim count As Integer = lines.Length
            Dim result(count - 1) As Double
            Dim groups As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim rowGroups(count - 1) As String
            Dim alternates(count - 1) As Boolean

            For t As Integer = 0 To count - 1
                For i As Integer = 0 To bundle.Catalog.Count - 1
                    Dim row As SummaryRow = bundle.Catalog.Rows(i)

                    If Not String.Equals(row.CellLine, lines(t), StringComparison.Ordinal) Then
                        Continue For
                    End If
                    If row.IsAuxiliary Then
                        Continue For
                    End If

                    rowGroups(t) = row.PairGroup
                    alternates(t) = row.PairAlternate
                    Exit For
                Next

                If rowGroups(t) IsNot Nothing Then
                    Dim seen As Integer = Nothing

                    Call groups.TryGetValue(rowGroups(t), seen)
                    groups(rowGroups(t)) = seen + 1
                End If
            Next

            For t As Integer = 0 To count - 1
                Dim down As Boolean = alternates(t)

                If Not down AndAlso rowGroups(t) IsNot Nothing Then
                    down = groups(rowGroups(t)) >= 2
                End If

                result(t) = If(down, weight, 1.0)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 噪声比：<c>(1/max(variance, FLOOR) / 逆方差权重)^α</c>，按中位数归一后裁剪到 <c>[1/bound, bound]</c>。
        ''' </summary>
        Private Shared Function ComputeNoiseRatio(nullVariance As Double()(),
                                                  stack As Double()()(),
                                                  alpha As Double,
                                                  bound As Double) As Double()()
            Dim nLines As Integer = nullVariance.Length

            If nLines = 0 Then
                Return New Double(-1)() {}
            End If

            Dim nUnion As Integer = nullVariance(0).Length
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nLines, nUnion)
            Dim geneMedian As Double() = New Double(If(nUnion = 0, -1, nUnion - 1)) {}

            For j As Integer = 0 To nUnion - 1
                Dim column(nLines - 1) As Double

                For t As Integer = 0 To nLines - 1
                    Dim v As Double = nullVariance(t)(j)

                    column(t) = If(Double.IsNaN(v) OrElse v <= 0.0, Double.NaN, v)
                Next

                geneMedian(j) = NanMedian(column)
            Next

            Dim overall As Double = NanMedian(geneMedian)

            For j As Integer = 0 To nUnion - 1
                If Double.IsNaN(geneMedian(j)) Then
                    geneMedian(j) = overall
                End If
            Next

            Dim ivw As Double()() = InverseVarianceWeights(stack, nUnion)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    Dim v As Double = nullVariance(t)(j)

                    If Double.IsNaN(v) OrElse v <= 0.0 Then
                        v = geneMedian(j)
                    End If

                    Dim ratio As Double = std.Pow(1.0 / std.Max(v, VarianceFloor) / ivw(t)(j), alpha)

                    result(t)(j) = If(Double.IsFinite(ratio), ratio, 1.0)
                Next
            Next

            Dim all As New List(Of Double)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    Call all.Add(result(t)(j))
                Next
            Next

            Dim center As Double = NanMedian(all.ToArray())

            If Double.IsFinite(center) AndAlso center > 0.0 Then
                For t As Integer = 0 To nLines - 1
                    For j As Integer = 0 To nUnion - 1
                        result(t)(j) /= center
                    Next
                Next
            End If

            Dim reciprocal As New List(Of Double)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    Call reciprocal.Add(std.Max(result(t)(j), 1.0 / result(t)(j)))
                Next
            Next

            Call Guards.Over(reciprocal.ToArray, bound, "nr_clip")

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    result(t)(j) = std.Max(std.Min(result(t)(j), bound), 1.0 / bound)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 逆方差权重（逐行逐基因）：对该行在扰动轴上的填零方差取倒数，
        ''' 没有实测的基因用该基因的跨行中位数回填。
        ''' </summary>
        Private Shared Function InverseVarianceWeights(stack As Double()()(), nUnion As Integer) As Double()()
            Dim nLines As Integer = stack.Length
            Dim nPerts As Integer = If(nLines = 0 OrElse stack(0).Length = 0, 0, stack(0)(0).Length)
            Dim variance As Double()() = RectangularArray.Matrix(Of Double)(nLines, nUnion)
            Dim measured As Boolean()() = RectangularArray.Matrix(Of Boolean)(nLines, nUnion)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    Dim acc As Double = 0.0
                    Dim acc2 As Double = 0.0
                    Dim any As Boolean = False

                    For p As Integer = 0 To nPerts - 1
                        Dim v As Double = stack(t)(j)(p)

                        If Double.IsNaN(v) Then
                            v = 0.0
                        Else
                            any = True
                        End If

                        acc += v
                        acc2 += v * v
                    Next

                    measured(t)(j) = any

                    If nPerts > 0 Then
                        Dim mean As Double = acc / nPerts

                        variance(t)(j) = acc2 / nPerts - mean * mean
                    Else
                        variance(t)(j) = 0.0
                    End If
                Next
            Next

            Dim typical As Double() = New Double(If(nUnion = 0, -1, nUnion - 1)) {}

            For j As Integer = 0 To nUnion - 1
                Dim column(nLines - 1) As Double

                For t As Integer = 0 To nLines - 1
                    column(t) = If(measured(t)(j), variance(t)(j), Double.NaN)
                Next

                typical(j) = NanMedian(column)
            Next

            Dim fallback As Double = NanMedian(typical)

            If Double.IsNaN(fallback) OrElse fallback <= 0.0 Then
                fallback = VarianceFloor
            End If

            For j As Integer = 0 To nUnion - 1
                If Double.IsNaN(typical(j)) Then
                    typical(j) = fallback
                End If
            Next

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nLines, nUnion)

            For t As Integer = 0 To nLines - 1
                For j As Integer = 0 To nUnion - 1
                    Dim v As Double = If(measured(t)(j), variance(t)(j), typical(j))

                    result(t)(j) = 1.0 / std.Max(v, VarianceFloor)
                Next
            Next

            Return result
        End Function

#End Region

#Region "statistics"

        ''' <summary>忽略 NaN 的中位数。</summary>
        Public Shared Function NanMedian(values As Double()) As Double
            If values Is Nothing Then
                Return Double.NaN
            End If

            Dim finite As Double() = values.Where(Function(v) Not Double.IsNaN(v)).OrderBy(Function(v) v).ToArray

            If finite.Length = 0 Then
                Return Double.NaN
            End If
            If finite.Length Mod 2 = 1 Then
                Return finite(finite.Length \ 2)
            End If

            Return (finite(finite.Length \ 2 - 1) + finite(finite.Length \ 2)) / 2.0
        End Function

        ''' <summary>忽略 NaN 的总体方差（ddof = 0，与 numpy 默认一致）。</summary>
        Public Shared Function NanVariance(values As Double()) As Double
            If values Is Nothing Then
                Return Double.NaN
            End If

            Dim finite As Double() = values.Where(Function(v) Not Double.IsNaN(v)).ToArray

            If finite.Length = 0 Then
                Return Double.NaN
            End If

            Dim mean As Double = finite.Average
            Dim acc As Double = 0.0

            For i As Integer = 0 To finite.Length - 1
                Dim d As Double = finite(i) - mean

                acc += d * d
            Next

            Return acc / finite.Length
        End Function

        ''' <summary>忽略 NaN 的 Spearman 秩相关；有效样本不足时返回 NaN。</summary>
        Public Shared Function SpearmanOmit(x As Double(), y As Double()) As Double
            Dim xs As New List(Of Double)
            Dim ys As New List(Of Double)

            For i As Integer = 0 To x.Length - 1
                If Double.IsNaN(x(i)) OrElse Double.IsNaN(y(i)) Then
                    Continue For
                End If

                Call xs.Add(x(i))
                Call ys.Add(y(i))
            Next

            If xs.Count < 3 Then
                Return Double.NaN
            End If

            Return GaussianCopula.Spearman(xs.ToArray, ys.ToArray)
        End Function

#End Region

        Public Overrides Function ToString() As String
            Return $"pooling factors: {LineCount} line(s), union {If(Union Is Nothing, 0, Union.Length)} gene(s), {If(Perts Is Nothing, 0, Perts.Length)} pert(s)"
        End Function

    End Class
End Namespace
