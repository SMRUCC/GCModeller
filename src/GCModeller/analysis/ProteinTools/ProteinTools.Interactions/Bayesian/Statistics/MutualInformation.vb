Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment

Namespace Bayesian.Statistics

    ''' <summary>
    ''' 互信息及其各种校正形式。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 互信息是共进化分析之中最常用的统计量(readme 第 4 节的特征 (1))：
    ''' </para>
    ''' <code>
    ''' MI(i,j) = Σ_ab f_ij(a,b) · ln[ f_ij(a,b) / (f_i(a) · f_j(b)) ]
    ''' </code>
    ''' <para>
    ''' 直接使用 MI 的问题在于它同时被「系统发育噪声」与「熵」污染：
    ''' 保守列(低熵)天然具有更小的 MI，而不相关但高变的列反而容易得到虚假的耦合信号。
    ''' 因此本模块同时提供三种校正：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' <b>APC 校正</b>(平均产物校正)：<c>MI_APC(i,j) = MI(i,j) - MI(i,·)·MI(·,j) / mean(MI)</c>，
    ''' 其中 <c>MI(i,·)</c> 是第 i 行的平均互信息。这是 DCA / EVcouplings 之中的标准做法。
    ''' </description></item>
    ''' <item><description><b>NMI</b>：<c>MI / sqrt(H_i · H_j)</c>，把互信息归一化到 <c>[0, 1]</c>。</description></item>
    ''' <item><description><b>熵加权</b>：<c>MI · (H_i · H_j)</c>，抑制低熵(高度保守)列的影响。</description></item>
    ''' </list>
    ''' <para>
    ''' 需要注意：论文的主路径并不使用 MI，而是使用自带有限样本修正的似然比 <c>R_ij</c>
    ''' (见 <see cref="DirichletMultinomial"/>)。本模块提供的 MI 系列统计量
    ''' 用于诊断、可视化以及 readme 之中描述的「可选特征层」。
    ''' </para>
    ''' </remarks>
    Public Class MutualInformation

        Private ReadOnly _counts As ColumnCounts
        Private ReadOnly _mi As Double(,)
        Private ReadOnly _entropy As Double()
        Private _apcApplied As Boolean = False

        ''' <summary>
        ''' 为给定的列计数构造互信息计算器
        ''' </summary>
        Public Sub New(counts As ColumnCounts)
            If counts Is Nothing Then
                Throw New ArgumentNullException(NameOf(counts))
            End If

            _counts = counts
            _mi = New Double(counts.Length - 1, counts.Length - 1) {}
            _entropy = New Double(counts.Length - 1) {}

            For i As Integer = 0 To counts.Length - 1
                _entropy(i) = SingleColumnEntropy(i)

                For j As Integer = 0 To counts.Length - 1
                    _mi(i, j) = Double.NaN
                Next

                _mi(i, i) = 0.0
            Next
        End Sub

        ''' <summary>
        ''' 列数 L
        ''' </summary>
        Public ReadOnly Property Size As Integer
            Get
                Return _counts.Length
            End Get
        End Property

        ''' <summary>
        ''' 读取第 i 列与第 j 列之间的互信息(单位 nat)
        ''' </summary>
        Public Function MI(i As Integer, j As Integer) As Double
            Dim cached As Double = _mi(i, j)

            If Not Double.IsNaN(cached) Then
                Return cached
            End If

            Dim counts As Dictionary(Of Integer, Integer) = _counts.PairCounts(i, j)

            If counts.Count = 0 Then
                Return 0.0
            End If

            Dim k As Integer = _counts.AlphabetSize
            Dim total As Double = _counts.PairTotal(i, j)
            Dim singleI As Integer() = _counts.SingleCounts(i)
            Dim singleJ As Integer() = _counts.SingleCounts(j)
            Dim totalI As Double = _counts.SingleTotal(i)
            Dim totalJ As Double = _counts.SingleTotal(j)
            Dim value As Double = 0.0

            For Each entry As KeyValuePair(Of Integer, Integer) In counts
                Dim a As Integer = entry.Key \ k
                Dim b As Integer = entry.Key Mod k
                Dim fab As Double = entry.Value / total
                Dim fa As Double = singleI(a) / totalI
                Dim fb As Double = singleJ(b) / totalJ

                If fab > 0 AndAlso fa > 0 AndAlso fb > 0 Then
                    value += fab * Math.Log(fab / (fa * fb))
                End If
            Next

            _mi(i, j) = value
            _mi(j, i) = value

            Return value
        End Function

        ''' <summary>
        ''' 第 i 列序列熵 <c>H_i = -Σ_α f_i(α) ln f_i(α)</c>
        ''' </summary>
        Public Function Entropy(i As Integer) As Double
            Return _entropy(i)
        End Function

        ''' <summary>
        ''' 归一化互信息 <c>NMI = MI / sqrt(H_i H_j)</c>，取值落在 <c>[0, 1]</c>
        ''' </summary>
        Public Function NMI(i As Integer, j As Integer) As Double
            Dim denominator As Double = Math.Sqrt(Math.Max(_entropy(i) * _entropy(j), 1.0E-12))

            Return MI(i, j) / denominator
        End Function

        ''' <summary>
        ''' 熵加权互信息 <c>MI · (H_i + H_j) / 2</c>
        ''' </summary>
        Public Function WeightedMI(i As Integer, j As Integer) As Double
            Return MI(i, j) * (_entropy(i) + _entropy(j)) / 2.0
        End Function

        ''' <summary>
        ''' 计算 APC(平均产物校正)之后的互信息矩阵。
        ''' </summary>
        ''' <remarks>
        ''' <c>MI_APC(i,j) = MI(i,j) - MI(i,·) · MI(·,j) / mean(MI)</c>。
        ''' 返回值是完整的 L×L 矩阵，对角线元素为 0。
        ''' </remarks>
        Public Function AverageProductCorrection() As Double(,)
            Call ComputeAll()

            Dim l As Integer = Size
            Dim rowMean As Double() = New Double(l - 1) {}
            Dim total As Double = 0.0
            Dim pairs As Integer = 0

            For i As Integer = 0 To l - 1
                Dim sum As Double = 0.0

                For j As Integer = 0 To l - 1
                    If i <> j Then
                        sum += _mi(i, j)
                    End If
                Next

                rowMean(i) = sum / Math.Max(1, l - 1)
                total += sum
                pairs += Math.Max(1, l - 1)
            Next

            Dim mean As Double = total / Math.Max(1, pairs)
            Dim result As Double(,) = New Double(l - 1, l - 1) {}

            If mean <= 0 Then
                Return result
            End If

            For i As Integer = 0 To l - 1
                For j As Integer = 0 To l - 1
                    If i = j Then
                        result(i, j) = 0.0
                    Else
                        result(i, j) = _mi(i, j) - rowMean(i) * rowMean(j) / mean
                    End If
                Next
            Next

            _apcApplied = True

            Return result
        End Function

        ''' <summary>
        ''' APC 校正是否已经被计算过
        ''' </summary>
        Public ReadOnly Property ApcComputed As Boolean
            Get
                Return _apcApplied
            End Get
        End Property

        ''' <summary>
        ''' 把所有列对的互信息都算一遍
        ''' </summary>
        Public Sub ComputeAll()
            For i As Integer = 0 To Size - 2
                For j As Integer = i + 1 To Size - 1
                    Call MI(i, j)
                Next
            Next
        End Sub

        Private Function SingleColumnEntropy(column As Integer) As Double
            Dim counts As Integer() = _counts.SingleCounts(column)
            Dim total As Double = 0.0
            Dim value As Double = 0.0

            For Each n As Integer In counts
                total += n
            Next

            If total <= 0 Then
                Return 0.0
            End If

            For Each n As Integer In counts
                If n > 0 Then
                    Dim f As Double = n / total
                    value -= f * Math.Log(f)
                End If
            Next

            Return value
        End Function
    End Class
End Namespace
