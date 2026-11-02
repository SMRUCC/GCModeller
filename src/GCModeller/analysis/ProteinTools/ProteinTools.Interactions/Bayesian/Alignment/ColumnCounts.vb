Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace Bayesian.Alignment

    ''' <summary>
    ''' 联合比对的列计数统计：单列计数 <c>n_i(α)</c> 与成对列计数 <c>n_ij(αβ)</c>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是论文之中所有统计量的唯一输入：Dirichlet-多项边际似然、
    ''' 似然比 <c>R_ij</c>、互信息以及静态打分 <c>X(S_k, S_r)</c> 都只依赖于这些计数。
    ''' </para>
    ''' <para>
    ''' 成对计数非常稀疏：21 个符号的理论组合有 441 种，但一条比对列通常只出现少数几种符号，
    ''' 因此每一对列实际非零的 <c>(α, β)</c> 组合通常只有几十个。这里使用
    ''' <c>Dictionary(Of Integer, Integer)</c> 以稀疏方式存放，键为 <c>α · K + β</c>。
    ''' </para>
    ''' </remarks>
    Public Class ColumnCounts

        Private _alignment As JointAlignment
        Private ReadOnly _single As Integer()()
        Private ReadOnly _pairs As Dictionary(Of Integer, Dictionary(Of Integer, Integer))
        Private ReadOnly _pairSums As Dictionary(Of Integer, Integer)

        ''' <summary>
        ''' 统计整个联合比对
        ''' </summary>
        ''' <param name="alignment">联合比对</param>
        ''' <param name="includeWithinBlockPairs">
        ''' 是否统计同家族内部的列对。Gibbs 采样只需要跨家族的列对(它们是唯一随分配变化的部分)，
        ''' 因此采样阶段可以关闭该项以显著降低计算量；但在计算完整的依赖树 <c>T*</c> 时必须打开。
        ''' </param>
        Public Sub New(alignment As JointAlignment, Optional includeWithinBlockPairs As Boolean = True)
            If alignment Is Nothing Then
                Throw New ArgumentNullException(NameOf(alignment))
            End If

            _alignment = alignment
            _pairs = New Dictionary(Of Integer, Dictionary(Of Integer, Integer))
            _pairSums = New Dictionary(Of Integer, Integer)
            _single = New Integer(alignment.Length - 1)() {}

            For c As Integer = 0 To alignment.Length - 1
                _single(c) = New Integer(alignment.Alphabet.Size - 1) {}
            Next

            Call CountSingles()
            Call CountPairs(includeWithinBlockPairs)
        End Sub

        ''' <summary>
        ''' 被统计的联合比对
        ''' </summary>
        Public ReadOnly Property Alignment As JointAlignment
            Get
                Return _alignment
            End Get
        End Property

        ''' <summary>
        ''' 残基字母表的符号个数 K
        ''' </summary>
        Public ReadOnly Property AlphabetSize As Integer
            Get
                Return _alignment.Alphabet.Size
            End Get
        End Property

        ''' <summary>
        ''' 列数 L
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                Return _alignment.Length
            End Get
        End Property

        ''' <summary>
        ''' 序列(基因组)条数 N
        ''' </summary>
        Public ReadOnly Property SampleSize As Integer
            Get
                Return _alignment.Rows
            End Get
        End Property

        ''' <summary>
        ''' 读取某一列的单列计数 <c>n_i(α)</c>
        ''' </summary>
        Public Function SingleCounts(column As Integer) As Integer()
            Return _single(column)
        End Function

        ''' <summary>
        ''' 某一列的总计数 <c>n_i</c>(等于样本数)
        ''' </summary>
        Public Function SingleTotal(column As Integer) As Integer
            Dim total As Integer = 0

            For Each n As Integer In _single(column)
                total += n
            Next

            Return total
        End Function

        ''' <summary>
        ''' 判断某一对列的计数是否已经被统计
        ''' </summary>
        Public Function HasPair(i As Integer, j As Integer) As Boolean
            Return _pairs.ContainsKey(PairKey(i, j))
        End Function

        ''' <summary>
        ''' 读取某一对列的联合计数 <c>n_ij(αβ)</c>，键为 <c>α · K + β</c>。
        ''' 未被统计的列对返回一个空的字典。
        ''' </summary>
        Public Function PairCounts(i As Integer, j As Integer) As Dictionary(Of Integer, Integer)
            Dim counts As Dictionary(Of Integer, Integer) = Nothing

            If _pairs.TryGetValue(PairKey(i, j), counts) Then
                Return counts
            End If

            Return New Dictionary(Of Integer, Integer)
        End Function

        ''' <summary>
        ''' 某一对列的有效计数总数(所有 <c>n_ij(αβ)</c> 之和，等于样本数)
        ''' </summary>
        Public Function PairTotal(i As Integer, j As Integer) As Integer
            Dim total As Integer = 0

            If _pairSums.TryGetValue(PairKey(i, j), total) Then
                Return total
            End If

            Return 0
        End Function

        ''' <summary>
        ''' 列对的键值，保证 <c>(i, j)</c> 与 <c>(j, i)</c> 映射到同一个键
        ''' </summary>
        Public Shared Function PairKey(i As Integer, j As Integer) As Integer
            Return If(i < j, i * 100000 + j, j * 100000 + i)
        End Function

        ''' <summary>
        ''' 已经统计的列对个数
        ''' </summary>
        Public ReadOnly Property PairCount As Integer
            Get
                Return _pairs.Count
            End Get
        End Property

        Private Sub CountSingles()
            Dim rows As Integer = _alignment.Rows

            For c As Integer = 0 To _alignment.Length - 1
                Dim counts As Integer() = _single(c)

                For r As Integer = 0 To rows - 1
                    counts(_alignment(r, c)) += 1
                Next
            Next
        End Sub

        Private Sub CountPairs(includeWithinBlock As Boolean)
            Dim rows As Integer = _alignment.Rows
            Dim k As Integer = AlphabetSize

            For i As Integer = 0 To _alignment.Length - 2
                For j As Integer = i + 1 To _alignment.Length - 1
                    Dim crossBlock As Boolean = _alignment.IsBlockA(i) <> _alignment.IsBlockA(j)

                    If Not crossBlock AndAlso Not includeWithinBlock Then
                        Continue For
                    End If

                    Dim counts As New Dictionary(Of Integer, Integer)

                    For r As Integer = 0 To rows - 1
                        Dim code As Integer = _alignment(r, i) * k + _alignment(r, j)
                        Dim n As Integer = 0

                        If counts.TryGetValue(code, n) Then
                            counts(code) = n + 1
                        Else
                            counts(code) = 1
                        End If
                    Next

                    Dim key As Integer = PairKey(i, j)
                    _pairs(key) = counts
                    _pairSums(key) = rows
                Next
            Next
        End Sub

        ''' <summary>
        ''' 重新统计跨家族的列对，用于 Gibbs 采样在交换伙伴之后更新似然。
        ''' </summary>
        ''' <param name="alignment">新的联合比对(行顺序必须与原来一致)</param>
        Public Sub RefreshCrossBlockPairs(alignment As JointAlignment)
            Dim rows As Integer = alignment.Rows
            Dim k As Integer = AlphabetSize

            For i As Integer = 0 To alignment.LengthA - 1
                For j As Integer = alignment.LengthA To alignment.Length - 1
                    Dim counts As New Dictionary(Of Integer, Integer)

                    For r As Integer = 0 To rows - 1
                        Dim code As Integer = alignment(r, i) * k + alignment(r, j)
                        Dim n As Integer = 0

                        If counts.TryGetValue(code, n) Then
                            counts(code) = n + 1
                        Else
                            counts(code) = 1
                        End If
                    Next

                    Dim key As Integer = PairKey(i, j)
                    _pairs(key) = counts
                    _pairSums(key) = rows
                Next
            Next
        End Sub

        ''' <summary>
        ''' 在同一个基因组之内交换两个成员的家族 B 段，并且<b>增量</b>更新跨家族的列对计数。
        ''' </summary>
        ''' <param name="row1">第一个成员所在的行</param>
        ''' <param name="row2">第二个成员所在的行</param>
        ''' <returns>交换之后的联合比对</returns>
        ''' <remarks>
        ''' <para>
        ''' 一次交换只是把两行之中的 B 段互换，因此只有跨家族的列对计数会改变；
        ''' 单列计数、家族内部的列对计数(包括 B-B 列对)在这个置换之下都是不变量，
        ''' 于是完全不需要重新统计。
        ''' </para>
        ''' <para>
        ''' 这一点是 Gibbs 采样器能够高效运行的关键：每次移动只需要 <c>O(L_A · L_B)</c> 次
        ''' 字典更新，而不是 <c>O(L² · N)</c> 次全量统计。
        ''' </para>
        ''' </remarks>
        Public Function ApplySwap(row1 As Integer, row2 As Integer) As JointAlignment
            If row1 = row2 Then
                Return _alignment
            End If

            Dim updated As JointAlignment = _alignment.SwapPartners(row1, row2)
            Dim k As Integer = AlphabetSize

            For i As Integer = 0 To _alignment.LengthA - 1
                For j As Integer = _alignment.LengthA To _alignment.Length - 1
                    Dim counts As Dictionary(Of Integer, Integer) = _pairs(PairKey(i, j))

                    Call Decrement(counts, _alignment(row1, i) * k + _alignment(row1, j))
                    Call Decrement(counts, _alignment(row2, i) * k + _alignment(row2, j))
                    Call Increment(counts, updated(row1, i) * k + updated(row1, j))
                    Call Increment(counts, updated(row2, i) * k + updated(row2, j))
                Next
            Next

            _alignment = updated

            Return updated
        End Function

        Private Shared Sub Increment(counts As Dictionary(Of Integer, Integer), code As Integer)
            Dim n As Integer = 0

            If counts.TryGetValue(code, n) Then
                counts(code) = n + 1
            Else
                counts(code) = 1
            End If
        End Sub

        Private Shared Sub Decrement(counts As Dictionary(Of Integer, Integer), code As Integer)
            Dim n As Integer = 0

            If counts.TryGetValue(code, n) Then
                If n <= 1 Then
                    Call counts.Remove(code)
                Else
                    counts(code) = n - 1
                End If
            End If
        End Sub
    End Class
End Namespace
