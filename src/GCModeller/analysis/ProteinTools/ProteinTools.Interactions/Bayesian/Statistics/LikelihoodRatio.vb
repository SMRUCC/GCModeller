Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment

Namespace Bayesian.Statistics

    ''' <summary>
    ''' 列与列之间「依赖度」的似然比矩阵 <c>R_ij</c> 及其对数值。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <c>R_ij = P(D_ij) / (P(D_i) P(D_j))</c> 是论文的核心统计量：
    ''' <c>R_ij &gt; 1</c>(即 <c>ln R_ij &gt; 0</c>)被解释为「位置 i 与位置 j 之间存在依赖」的正证据。
    ''' </para>
    ''' <para>
    ''' 计算是惰性的：只有在第一次访问 <c>(i, j)</c> 的时候才去统计该列对的联合计数并求 Gamma 函数，
    ''' 结果随后被缓存。这样在只需要少数几列(例如最大生成树之上的 <c>L-1</c> 条边)的时候，
    ''' 可以避免 <c>O(L²)</c> 次完整统计。
    ''' </para>
    ''' <para>
    ''' 所有数值都以<b>对数</b>形式对外提供，因为 <c>R_ij</c> 会随着样本数增长到
    ''' <c>e^100</c> 量级，直接用双精度浮点数的线性值会溢出。
    ''' </para>
    ''' </remarks>
    Public Class LikelihoodRatioMatrix

        Private ReadOnly _counts As ColumnCounts
        Private ReadOnly _model As DirichletMultinomial
        Private ReadOnly _logR As Double(,)

        ''' <summary>
        ''' 为给定的列计数构造似然比矩阵
        ''' </summary>
        Public Sub New(counts As ColumnCounts, Optional prior As DirichletMultinomial = Nothing)
            If counts Is Nothing Then
                Throw New ArgumentNullException(NameOf(counts))
            End If

            _counts = counts
            _model = If(prior, New DirichletMultinomial(counts.AlphabetSize))
            _logR = New Double(counts.Length - 1, counts.Length - 1) {}

            For i As Integer = 0 To counts.Length - 1
                For j As Integer = 0 To counts.Length - 1
                    _logR(i, j) = Double.NaN
                Next
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
        ''' 底层的列计数
        ''' </summary>
        Public ReadOnly Property Counts As ColumnCounts
            Get
                Return _counts
            End Get
        End Property

        ''' <summary>
        ''' Dirichlet-多项模型
        ''' </summary>
        Public ReadOnly Property Prior As DirichletMultinomial
            Get
                Return _model
            End Get
        End Property

        ''' <summary>
        ''' 读取 <c>ln R_ij</c>，必要时现算并缓存
        ''' </summary>
        Public Function LogR(i As Integer, j As Integer) As Double
            If i = j Then
                Return Double.PositiveInfinity
            End If

            Dim cached As Double = _logR(i, j)

            If Not Double.IsNaN(cached) Then
                Return cached
            End If

            Dim value As Double = _model.LogLikelihoodRatio(_counts, i, j)

            _logR(i, j) = value
            _logR(j, i) = value

            Return value
        End Function

        ''' <summary>
        ''' 读取 <c>R_ij</c> 的线性值
        ''' </summary>
        Public Function R(i As Integer, j As Integer) As Double
            Dim logValue As Double = LogR(i, j)

            If Double.IsPositiveInfinity(logValue) Then
                Return Double.PositiveInfinity
            ElseIf Double.IsNegativeInfinity(logValue) Then
                Return 0.0
            Else
                Return Math.Exp(logValue)
            End If
        End Function

        ''' <summary>
        ''' 读取一整行的 <c>ln R_ij</c>，同时把所有列对都提前算好
        ''' </summary>
        Public Function Row(i As Integer) As Double()
            Dim values As Double() = New Double(Size - 1) {}

            For j As Integer = 0 To Size - 1
                values(j) = LogR(i, j)
            Next

            Return values
        End Function

        ''' <summary>
        ''' 把所有列对都计算一遍，用于需要完整矩阵的场合(例如最大生成树)
        ''' </summary>
        Public Sub ComputeAll()
            For i As Integer = 0 To Size - 2
                For j As Integer = i + 1 To Size - 1
                    Call LogR(i, j)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 已经计算过的列对个数
        ''' </summary>
        Public ReadOnly Property ComputedPairs As Integer
            Get
                Dim count As Integer = 0

                For i As Integer = 0 To Size - 2
                    For j As Integer = i + 1 To Size - 1
                        If Not Double.IsNaN(_logR(i, j)) Then
                            count += 1
                        End If
                    Next
                Next

                Return count
            End Get
        End Property

        ''' <summary>
        ''' 丢弃全部缓存，在底层计数发生变化(例如 Gibbs 采样交换伙伴)之后必须调用
        ''' </summary>
        Public Sub Invalidate()
            For i As Integer = 0 To Size - 1
                For j As Integer = 0 To Size - 1
                    _logR(i, j) = Double.NaN
                Next
            Next
        End Sub

        ''' <summary>
        ''' 只丢弃「跨家族」的列对缓存。
        ''' </summary>
        ''' <param name="lengthA">家族 A 的列数，前 <paramref name="lengthA"/> 列属于家族 A</param>
        ''' <remarks>
        ''' Gibbs 采样交换两个成员的家族 B 段之后，只有跨家族的列对统计量会改变；
        ''' 家族内部(包括 B-B)的列对在这个行置换之下是不变量。因此只需清掉跨家族的缓存，
        ''' 采样过程中就可以避免重复计算占绝大多数的内部列对。
        ''' </remarks>
        Public Sub InvalidateCrossBlock(lengthA As Integer)
            For i As Integer = 0 To Size - 1
                For j As Integer = 0 To Size - 1
                    If (i < lengthA) <> (j < lengthA) Then
                        _logR(i, j) = Double.NaN
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 计算并返回完整的 <c>ln R</c> 矩阵的副本
        ''' </summary>
        Public Function ToMatrix() As Double(,)
            Call ComputeAll()
            Return DirectCast(_logR.Clone(), Double(,))
        End Function

        ''' <summary>
        ''' 统计 <c>ln R_ij &gt; 0</c> 的列对比例，用作「配对比对是否具有共进化信号」的整体诊断
        ''' </summary>
        Public Function PositiveFraction(Optional threshold As Double = 0.0) As Double
            Dim positive As Integer = 0
            Dim total As Integer = 0

            For i As Integer = 0 To Size - 2
                For j As Integer = i + 1 To Size - 1
                    total += 1

                    If LogR(i, j) > threshold Then
                        positive += 1
                    End If
                Next
            Next

            If total = 0 Then
                Return 0.0
            Else
                Return positive / CDbl(total)
            End If
        End Function
    End Class
End Namespace
