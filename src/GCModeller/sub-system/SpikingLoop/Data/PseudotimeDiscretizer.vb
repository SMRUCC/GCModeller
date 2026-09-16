' ============================================================================
' PseudotimeDiscretizer.vb — readme 一.2 prepare_training_data 的实现
'
' 把"细胞 × 基因"的连续表达矩阵压缩成 SNN 可用的离散时间轨迹 U_seq[T, N]：
'
'   1. 确定伪时间
'        优先级：外部伪时间（Monocle3 结果） → 表达矩阵自带的时间点标签
'                → 从样本名解析时间标签（T1.2h → 1.2）
'      三条路径都拿不到有序时间时直接报错（绝不静默地按原始顺序当成时间轴）。
'   2. 沿伪时间排序样本
'   3. 把伪时间等分为 T 个时间窗（默认分位数分箱，保证各窗样本数均衡）
'   4. 每个时间窗内对全部生物学重复取均值 → U[t, :]
'        空窗（分位数边界上的并列值导致）按前一窗前向填充并记录诊断
'   5. 沿时间做滑动平均平滑（缓解单细胞 Dropout 造成的虚假零值）
'   6. MinMax 归一到 [0,1]（便于编码为输入电流），并保留 min/max 供反变换
'
' SNN 的膜时间常数与离散步长必须匹配（readme 一.2 关键点），
' 因此 NumBins（窗口长度）与 Beta(λ) 在配置中同时暴露，便于联合扫描。
'
' 注意：伪时间是相对量而非真实时间，结果解释时不应做绝对时间的断言（readme 六）。
' ============================================================================

Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports std = System.Math

Namespace Data

    ''' <summary>伪时间来源</summary>
    Public Enum PseudotimeSource
        ''' <summary>调用方显式提供的伪时间（如 Monocle3 结果）</summary>
        External
        ''' <summary>表达矩阵自带的时间点标签（GeneExpressionData.TimePoints）</summary>
        SampleTimePoints
        ''' <summary>由样本名解析出的时间标签</summary>
        SampleName
    End Enum

    ''' <summary>离散化后的训练轨迹</summary>
    Public Class PseudotimeResult

        ''' <summary>基因名（顺序与 <see cref="U"/> 的列严格一致）</summary>
        Public Property GeneNames As String()

        ''' <summary>离散化后的表达轨迹 U_seq [T, N]（行 = 伪时间窗，列 = 基因）</summary>
        Public Property U As Tensor

        ''' <summary>与 <see cref="U"/> 同内容的嵌套数组（行 = 伪时间窗）</summary>
        Public Property Values As Double()()

        ''' <summary>时间窗的分箱边界（内部边界，长度 = NumBins − 1）</summary>
        Public Property BinEdges As Double()

        ''' <summary>每个时间窗内的样本数</summary>
        Public Property BinCounts As Integer()

        ''' <summary>每个样本的伪时间值（原始样本顺序）</summary>
        Public Property Pseudotime As Double()

        ''' <summary>按伪时间排序后的样本索引</summary>
        Public Property SampleOrder As Integer()

        ''' <summary>伪时间的来源</summary>
        Public Property Source As PseudotimeSource

        ''' <summary>MinMax 归一化前的最小值（反变换用）</summary>
        Public Property GlobalMin As Double

        ''' <summary>MinMax 归一化前的最大值（反变换用）</summary>
        Public Property GlobalMax As Double

        ''' <summary>数据质量诊断信息（空窗填充、平滑幅度、静默基因等）</summary>
        Public Property Diagnostics As List(Of String)

        Public ReadOnly Property NumBins As Integer
            Get
                Return If(Values Is Nothing, 0, Values.Length)
            End Get
        End Property

        Public ReadOnly Property NumGenes As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>每个时间窗平均包含的样本数</summary>
        Public ReadOnly Property MeanSamplesPerBin As Double
            Get
                If BinCounts Is Nothing OrElse BinCounts.Length = 0 Then Return 0.0
                Dim s = 0
                For Each c In BinCounts
                    s += c
                Next
                Return s / CDbl(BinCounts.Length)
            End Get
        End Property

        ''' <summary>把归一化到 [0,1] 的预测值反变换回原始表达量尺度</summary>
        Public Function Denormalize(t As Tensor) As Tensor
            Dim scale = GlobalMax - GlobalMin
            If scale <= 0.0 Then Return New Tensor(t.ToDoubleArray(), t.Shape)
            Return t.Apply(Function(v As Double) GlobalMin + v * scale)
        End Function

        ''' <summary>
        ''' 把表达轨迹渲染成 ASCII 强度图（前若干基因），便于在控制台直观判断
        ''' "平滑是否足够 / 是否存在剧烈跳变 / 是否有基因全程静默"。
        ''' </summary>
        Public Function TrajectoryText(Optional maxGenes As Integer = 8,
                                       Optional ramp As String = " ·:-=+*#%@") As String
            If Values Is Nothing OrElse Values.Length = 0 Then
                Return "(empty trajectory)"
            End If

            Dim genes = std.Min(maxGenes, NumGenes)
            Dim sb As New StringBuilder()
            For g = 0 To genes - 1
                sb.Append($"    {GeneNames(g).PadRight(14)}|")

                Dim lo = Double.MaxValue
                Dim hi = Double.MinValue
                For t = 0 To NumBins - 1
                    lo = std.Min(lo, Values(t)(g))
                    hi = std.Max(hi, Values(t)(g))
                Next
                Dim span = hi - lo

                For t = 0 To NumBins - 1
                    Dim v = If(span > 0.0, (Values(t)(g) - lo) / span, 0.0)
                    Dim idx = CInt(std.Round(v * (ramp.Length - 1)))
                    sb.Append(ramp(std.Max(0, std.Min(ramp.Length - 1, idx))))
                Next

                sb.AppendLine($"| {lo:F3}~{hi:F3} ({GeneNames(g)})")
            Next

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"PseudotimeResult(T={NumBins}, N={NumGenes}, source={Source}, " &
                   $"samples/bin={MeanSamplesPerBin:F1})"
        End Function

    End Class

    ''' <summary>
    ''' 把连续表达矩阵按伪时间离散化为 SNN 训练轨迹（readme 一.2）。
    ''' </summary>
    Public Class PseudotimeDiscretizer

        Private ReadOnly _config As SpikingLoopConfig

        Public Sub New(config As SpikingLoopConfig)
            If config Is Nothing Then
                Throw New ArgumentNullException(NameOf(config))
            End If
            _config = config
        End Sub

        ''' <summary>
        ''' 构建训练轨迹。
        ''' </summary>
        ''' <param name="expr">表达矩阵（[gene, sample]）</param>
        ''' <param name="externalPseudotime">
        ''' 外部伪时间（长度必须等于样本数）；为 Nothing 时依次回退到
        ''' 表达矩阵的时间点标签、样本名中的时间标签。
        ''' </param>
        Public Function Build(expr As GeneExpressionData,
                              Optional externalPseudotime As Double() = Nothing) As PseudotimeResult

            If expr Is Nothing Then
                Throw New ArgumentNullException(NameOf(expr))
            End If

            Dim diagnostics As New List(Of String)()
            Dim pseudo = ResolvePseudotime(expr, externalPseudotime, diagnostics)

            ' ---- 沿伪时间排序样本 ----
            Dim order = Enumerable.Range(0, pseudo.Length) _
                .OrderBy(Function(i) pseudo(i)) _
                .ToArray()

            ' ---- 分箱边界 ----
            Dim numBins = _config.NumBins
            Dim edges = ComputeBinEdges(pseudo, order, numBins, _config.Binning)

            ' ---- 每个时间窗取均值 ----
            Dim bins = AssignBins(pseudo, edges, numBins)
            Dim counts(numBins - 1) As Integer
            For Each b In bins
                counts(b) += 1
            Next

            Dim n = expr.NGene
            Dim values(numBins - 1)() As Double
            For b = 0 To numBins - 1
                values(b) = New Double(n - 1) {}
                If counts(b) = 0 Then Continue For

                For g = 0 To n - 1
                    Dim sum = 0.0
                    For s = 0 To order.Length - 1
                        If bins(s) = b Then
                            sum += expr.Matrix(g, order(s))
                        End If
                    Next
                    values(b)(g) = sum / counts(b)
                Next
            Next

            FillEmptyBins(values, counts, diagnostics)

            ' ---- 平滑与归一化 ----
            Smooth(values, _config.SmoothingWindow, diagnostics)
            Dim rawMin = 0.0
            Dim rawMax = 0.0
            Normalize(values, _config.NormalizeExpression, rawMin, rawMax)

            ' ---- 打包 ----
            Dim flat(numBins * n - 1) As Double
            For b = 0 To numBins - 1
                Array.Copy(values(b), 0, flat, b * n, n)
            Next

            Return New PseudotimeResult With {
                .GeneNames = CType(expr.GeneNames.Clone(), String()),
                .U = Tensor.Wrap(flat, numBins, n),
                .Values = values,
                .BinEdges = edges,
                .BinCounts = counts,
                .Pseudotime = pseudo,
                .SampleOrder = order,
                .Source = _lastSource,
                .GlobalMin = rawMin,
                .GlobalMax = rawMax,
                .Diagnostics = diagnostics
            }
        End Function

        Private _lastSource As PseudotimeSource

#Region "伪时间解析"

        ''' <summary>
        ''' 按优先级解析伪时间：外部输入 → 样本时间点标签 → 样本名时间标签。
        ''' </summary>
        Private Function ResolvePseudotime(expr As GeneExpressionData,
                                           external As Double(),
                                           diagnostics As List(Of String)) As Double()

            Dim ns = expr.NSample

            ' ---- ① 外部伪时间（最高优先级） ----
            If external IsNot Nothing Then
                If external.Length <> ns Then
                    Throw New ArgumentException(
                        $"外部伪时间长度({external.Length})与样本数({ns})不一致")
                End If
                _lastSource = PseudotimeSource.External
                Return CType(external.Clone(), Double())
            End If

            ' ---- ② 表达矩阵自带的时间点标签 ----
            If expr.TimePoints IsNot Nothing AndAlso expr.TimePoints.Length = ns Then
                If HasVariation(expr.TimePoints) Then
                    _lastSource = PseudotimeSource.SampleTimePoints
                    Return CType(expr.TimePoints.Clone(), Double())
                End If
                diagnostics.Add("表达矩阵的 TimePoints 全为相同值（常见于未传递 SampleInfo），" &
                                "回退到从样本名解析时间标签")
            End If

            ' ---- ③ 从样本名解析 ----
            Dim parsed = SampleNameTimeParser.ParseAll(expr.SampleNames)
            If parsed.Parsed > 0 AndAlso HasVariation(parsed.TimePoints) Then
                _lastSource = PseudotimeSource.SampleName

                If Not parsed.AllParsed Then
                    diagnostics.Add($"{parsed.Failed}/{ns} 个样本名无法解析出时间标签，" &
                                    $"这些样本的伪时间被填为已解析样本的中位数：" &
                                    $"{String.Join(", ", parsed.FailedSamples.Take(5))}")
                    FillFailedWithMedian(parsed)
                End If

                Return parsed.TimePoints
            End If

            Throw New InvalidOperationException(
                "无法确定伪时间：请提供外部伪时间数组（Monocle3 结果），" &
                "或在表达矩阵中携带时间点标签，或让样本名包含可识别的时间（如 T1.2h_Rep1）")
        End Function

        Private Shared Function HasVariation(values As Double()) As Boolean
            If values Is Nothing OrElse values.Length < 2 Then Return False

            Dim lo = Double.MaxValue
            Dim hi = Double.MinValue
            For Each v In values
                If Double.IsNaN(v) Then Continue For
                lo = std.Min(lo, v)
                hi = std.Max(hi, v)
            Next

            Return hi > lo
        End Function

        ''' <summary>把解析失败（NaN）的样本伪时间填为已解析样本的中位数</summary>
        Private Shared Sub FillFailedWithMedian(parsed As SampleTimeParseResult)
            Dim valid = parsed.TimePoints.Where(Function(v) Not Double.IsNaN(v)).OrderBy(Function(v) v).ToArray()
            If valid.Length = 0 Then Return

            Dim median = valid(valid.Length \ 2)
            For i = 0 To parsed.TimePoints.Length - 1
                If Double.IsNaN(parsed.TimePoints(i)) Then parsed.TimePoints(i) = median
            Next
        End Sub

#End Region

#Region "分箱"

        ''' <summary>计算分箱的内部边界（长度 = numBins − 1，非降序）</summary>
        Private Shared Function ComputeBinEdges(pseudo As Double(), order As Integer(),
                                                numBins As Integer,
                                                mode As PseudotimeBinning) As Double()
            Dim sorted(order.Length - 1) As Double
            For k = 0 To order.Length - 1
                sorted(k) = pseudo(order(k))
            Next

            Dim edges(numBins - 2) As Double

            If mode = PseudotimeBinning.Uniform Then
                Dim lo = sorted(0)
                Dim hi = sorted(sorted.Length - 1)
                For b = 1 To numBins - 1
                    edges(b - 1) = lo + (hi - lo) * b / numBins
                Next
            Else
                ' 分位数分箱：第 b 个边界取第 b/numBins 分位处（相邻样本取中点，更稳健）
                For b = 1 To numBins - 1
                    Dim pos = (sorted.Length - 1) * b / CDbl(numBins)
                    Dim i0 = CInt(std.Floor(pos))
                    Dim i1 = CInt(std.Ceiling(pos))
                    edges(b - 1) = (sorted(i0) + sorted(i1)) / 2.0
                Next
            End If

            Return edges
        End Function

        ''' <summary>把每个样本映射到时间窗：bin = 小于该样本伪时间的边界个数</summary>
        Private Shared Function AssignBins(pseudo As Double(), edges As Double(), numBins As Integer) As Integer()
            Dim bins(pseudo.Length - 1) As Integer
            For s = 0 To pseudo.Length - 1
                Dim b = 0
                While b < edges.Length AndAlso edges(b) < pseudo(s)
                    b += 1
                End While
                bins(s) = std.Min(b, numBins - 1)
            Next
            Return bins
        End Function

        ''' <summary>
        ''' 空窗填充：伪时间并列值较多时，分位数边界可能切出空窗。
        ''' 前向填充（用前一窗的均值），首窗为空则用其后第一个非空窗。
        ''' </summary>
        Private Shared Sub FillEmptyBins(values As Double()(), counts As Integer(),
                                         diagnostics As List(Of String))
            Dim empty As New List(Of Integer)()
            For b = 0 To counts.Length - 1
                If counts(b) = 0 Then empty.Add(b)
            Next

            If empty.Count = 0 Then Return
            If empty.Count = counts.Length Then
                Throw New InvalidOperationException("所有时间窗都为空，无法构造训练轨迹")
            End If

            ' 找到第一个非空窗
            Dim firstNonEmpty = 0
            While counts(firstNonEmpty) = 0
                firstNonEmpty += 1
            End While

            ' 首窗之前的全部用 firstNonEmpty 填充
            For b = 0 To firstNonEmpty - 1
                Array.Copy(values(firstNonEmpty), values(b), values(b).Length)
            Next

            ' 其余空窗用前一窗填充
            For b = firstNonEmpty + 1 To counts.Length - 1
                If counts(b) = 0 Then
                    Array.Copy(values(b - 1), values(b), values(b).Length)
                End If
            Next

            diagnostics.Add($"{empty.Count}/{counts.Length} 个时间窗为空（伪时间并列值导致），" &
                            $"已按相邻窗均值填充；建议减少分箱数或改用 Uniform 分箱")
        End Sub

#End Region

#Region "平滑与归一化"

        ''' <summary>
        ''' 沿时间做滑动平均（窗口为奇数，边界按可用样本数取均值）。
        ''' readme 一.2 强调这一步用于缓解 Dropout 造成的虚假零值。
        ''' </summary>
        Private Shared Sub Smooth(values As Double()(), window As Integer, diagnostics As List(Of String))
            If window <= 1 Then Return

            If window Mod 2 = 0 Then
                window += 1
                diagnostics.Add($"平滑窗口已调整为奇数 {window}")
            End If

            Dim bins = values.Length
            Dim half = window \ 2
            Dim n = values(0).Length
            Dim smoothed(bins - 1)() As Double

            For b = 0 To bins - 1
                smoothed(b) = New Double(n - 1) {}
                Dim lo = std.Max(0, b - half)
                Dim hi = std.Min(bins - 1, b + half)
                Dim cnt = hi - lo + 1

                For g = 0 To n - 1
                    Dim sum = 0.0
                    For k = lo To hi
                        sum += values(k)(g)
                    Next
                    smoothed(b)(g) = sum / cnt
                Next
            Next

            Array.Copy(smoothed, values, bins)
        End Sub

        ''' <summary>
        ''' 全局 MinMax 归一化到配置的目标区间；记录原始 min/max 供反变换。
        ''' 若所有值相同（零极差），则统一映射到目标区间的下界并给出诊断。
        ''' </summary>
        Private Sub Normalize(values As Double()(), enabled As Boolean,
                              ByRef rawMin As Double, ByRef rawMax As Double)
            Dim lo = Double.MaxValue
            Dim hi = Double.MinValue
            For Each row In values
                For Each v In row
                    lo = std.Min(lo, v)
                    hi = std.Max(hi, v)
                Next
            Next

            rawMin = lo
            rawMax = hi

            If Not enabled Then Return

            Dim targetLo = _config.ExpressionMin
            Dim targetHi = _config.ExpressionMax
            Dim span = hi - lo

            For b = 0 To values.Length - 1
                For g = 0 To values(b).Length - 1
                    values(b)(g) = If(span > 0.0,
                                      targetLo + (values(b)(g) - lo) / span * (targetHi - targetLo),
                                      targetLo)
                Next
            Next
        End Sub

#End Region

    End Class

End Namespace
