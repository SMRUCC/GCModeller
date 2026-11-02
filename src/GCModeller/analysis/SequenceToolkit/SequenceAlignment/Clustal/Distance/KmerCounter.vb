Imports System.Runtime.CompilerServices

Namespace Clustal

    ''' <summary>
    ''' k-mer 计数表：把一条序列分解为长度为 k 的重叠子串并统计每个子串的出现次数。
    ''' </summary>
    ''' <remarks>
    ''' 当 <c>字母表大小^k</c> 落在较小的范围内时使用稠密数组(最快的路径)，否则退化为
    ''' 稀疏的 <see cref="Dictionary(Of Long, Integer)"/>。蛋白 k=2 与核酸 k=5 都属于稠密分支，
    ''' 因此在 mBed 阶段不会产生任何字典操作。
    ''' </remarks>
    Public Class KmerCounter

        ''' <summary>
        ''' 稠密计数表所能容忍的编码空间上限
        ''' </summary>
        Private Const DenseLimit As Long = 262144

        ''' <summary>
        ''' k-mer 长度
        ''' </summary>
        Public ReadOnly Property K As Integer
        ''' <summary>
        ''' 字母表大小
        ''' </summary>
        Public ReadOnly Property AlphabetSize As Integer
        ''' <summary>
        ''' k-mer 的总数(即序列长度 - k + 1)
        ''' </summary>
        Public ReadOnly Property Total As Integer

        Private ReadOnly dense As Integer()
        Private ReadOnly sparse As Dictionary(Of Long, Integer)

        ''' <summary>
        ''' 是否使用了稠密计数表
        ''' </summary>
        Public ReadOnly Property IsDense As Boolean
            Get
                Return dense IsNot Nothing
            End Get
        End Property

        Private Sub New(k As Integer, alphabetSize As Integer, dense As Integer(), sparse As Dictionary(Of Long, Integer), total As Integer)
            Me.K = k
            Me.AlphabetSize = alphabetSize
            Me.dense = dense
            Me.sparse = sparse
            Me.Total = total
        End Sub

        ''' <summary>
        ''' 统计一条序列的 k-mer
        ''' </summary>
        Public Shared Function Build(seq As String, alphabet As SequenceAlphabet, k As Integer) As KmerCounter
            Dim A As Integer = alphabet.Size
            k = Math.Max(1, k)

            ' k 保持固定(不做截断)，否则不同长度的序列之间无法共享计数表比较；
            ' 序列短于 k 时直接得到空的计数表(距离退化为 1)
            Dim total As Integer = Math.Max(0, seq.Length - k + 1)

            Dim space As Long = 1

            For i As Integer = 1 To k
                space *= A

                If space > DenseLimit Then
                    Exit For
                End If
            Next

            If space <= DenseLimit Then
                Dim counts As Integer() = New Integer(CInt(space) - 1) {}

                For start As Integer = 0 To total - 1
                    Dim code As Long = 0

                    For offset As Integer = 0 To k - 1
                        code = code * A + alphabet.CodeOf(seq(start + offset))
                    Next

                    counts(CInt(code)) += 1
                Next

                Return New KmerCounter(k, A, counts, Nothing, total)
            Else
                Dim counts As New Dictionary(Of Long, Integer)()

                For start As Integer = 0 To total - 1
                    Dim code As Long = 0

                    For offset As Integer = 0 To k - 1
                        code = code * A + alphabet.CodeOf(seq(start + offset))
                    Next

                    Dim c As Integer = 0

                    If counts.TryGetValue(code, c) Then
                        counts(code) = c + 1
                    Else
                        counts.Add(code, 1)
                    End If
                Next

                Return New KmerCounter(k, A, Nothing, counts, total)
            End If
        End Function

        ''' <summary>
        ''' 两个计数表之间共享的 k-mer 个数：sum(min(countA, countB))
        ''' </summary>
        Public Function Shared(other As KmerCounter) As Integer
            If K <> other.K Then
                Throw New ArgumentException($"k-mer 长度不一致: {K} vs {other.K}")
            End If

            Dim acc As Long = 0

            If dense IsNot Nothing AndAlso other.dense IsNot Nothing Then
                Dim a As Integer() = dense
                Dim b As Integer() = other.dense
                Dim n As Integer = Math.Min(a.Length, b.Length)

                For i As Integer = 0 To n - 1
                    acc += Math.Min(a(i), b(i))
                Next
            ElseIf sparse IsNot Nothing AndAlso other.sparse IsNot Nothing Then
                Dim small As Dictionary(Of Long, Integer) = If(sparse.Count <= other.sparse.Count, sparse, other.sparse)
                Dim large As Dictionary(Of Long, Integer) = If(sparse.Count <= other.sparse.Count, other.sparse, sparse)

                For Each item As KeyValuePair(Of Long, Integer) In small
                    Dim c As Integer = 0

                    If large.TryGetValue(item.Key, c) Then
                        acc += Math.Min(item.Value, c)
                    End If
                Next
            Else
                ' 一边稠密一边稀疏：把稀疏的一边逐项按编码投影到稠密数组
                Dim a As KmerCounter = If(dense IsNot Nothing, Me, other)
                Dim b As KmerCounter = If(dense IsNot Nothing, other, Me)

                For Each item As KeyValuePair(Of Long, Integer) In b.sparse
                    If item.Key < a.dense.Length Then
                        acc += Math.Min(a.dense(CInt(item.Key)), item.Value)
                    End If
                Next
            End If

            Return CInt(Math.Min(acc, Integer.MaxValue))
        End Function

        ''' <summary>
        ''' 基于共享 k-mer 的归一化距离，取值区间 [0, 1]
        ''' </summary>
        Public Function Distance(other As KmerCounter) As Double
            Dim sharedKmers As Integer = Shared(other)
            Dim union As Integer = Total + other.Total - sharedKmers

            If union <= 0 Then
                Return 0
            Else
                Return 1.0R - CDbl(sharedKmers) / union
            End If
        End Function

        ''' <summary>
        ''' 直接比较两条序列的 k-mer 距离
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function PairDistance(a As String, b As String,
                                            alphabet As SequenceAlphabet, k As Integer) As Double

            If String.IsNullOrEmpty(a) OrElse String.IsNullOrEmpty(b) Then
                Return 0
            End If
            If a = b Then
                Return 0
            End If

            Return Build(a, alphabet, k).Distance(Build(b, alphabet, k))
        End Function

        Public Overrides Function ToString() As String
            Return $"k={K}, total={Total}, dense={IsDense}"
        End Function
    End Class
End Namespace
