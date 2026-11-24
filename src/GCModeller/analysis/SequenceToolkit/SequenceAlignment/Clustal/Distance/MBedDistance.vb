Imports System.Runtime.CompilerServices

Namespace Clustal

    ''' <summary>
    ''' mBed 距离估计：把每条序列投影到由少量"种子序列"张成的低维空间，
    ''' 把 O(N^2) 的两两距离计算压缩到 O(N x |seeds|)。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 每个坐标值的含义是"该序列到某条种子序列的 k-mer 距离"，因此低维空间中的欧氏距离
    ''' 是两两 k-mer 距离的一个近似。k-means 分簇与簇内的精确距离都在此基础上完成。
    ''' </para>
    ''' <para>
    ''' 当 <see cref="ClustalOmegaOptions.FullDistanceMatrix"/> 打开且序列条数不超过
    ''' <see cref="ClustalOmegaOptions.MaxFullDistanceSequences"/> 时，直接计算完整的 N x N 距离矩阵，
    ''' 这与 clustalo 的 <c>--full</c> 语义一致。
    ''' </para>
    ''' </remarks>
    Public Class MBedDistance

        ''' <summary>
        ''' 序列条数
        ''' </summary>
        Public ReadOnly Property SequenceCount As Integer
        ''' <summary>
        ''' N x |seeds| 的低维坐标(全距离矩阵模式下为 Nothing)
        ''' </summary>
        Public ReadOnly Property Feature As Double()()
        ''' <summary>
        ''' 被选为种子的序列索引
        ''' </summary>
        Public ReadOnly Property Seeds As Integer()
        ''' <summary>
        ''' 是否直接计算了完整的距离矩阵
        ''' </summary>
        Public ReadOnly Property IsFull As Boolean
        ''' <summary>
        ''' 完整的 N x N 距离矩阵(仅在 <see cref="IsFull"/> 为真时有效)
        ''' </summary>
        Public ReadOnly Property Matrix As Double()()
        ''' <summary>
        ''' 使用的 k-mer 长度
        ''' </summary>
        Public ReadOnly Property K As Integer

        Private Sub New(sequenceCount As Integer, feature As Double()(), seeds As Integer(),
                        isFull As Boolean, matrix As Double()(), k As Integer)

            Me.SequenceCount = sequenceCount
            Me.Feature = feature
            Me.Seeds = seeds
            Me.IsFull = isFull
            Me.Matrix = matrix
            Me.K = k
        End Sub

        ''' <summary>
        ''' 计算 mBed 投影(或者完整的距离矩阵)
        ''' </summary>
        Public Shared Function Compute(seqs As String(), alphabet As SequenceAlphabet,
                                       k As Integer, options As ClustalOmegaOptions) As MBedDistance

            Dim n As Integer = If(seqs Is Nothing, 0, seqs.Length)

            If n = 0 Then
                Return New MBedDistance(0, New Double()() {}, New Integer() {}, False, Nothing, k)
            End If

            If n = 1 Then
                Dim probe As Double()() = New Double()() {New Double() {}}
                Return New MBedDistance(1, probe, New Integer() {0}, False, Nothing, k)
            End If

            If options.FullDistanceMatrix AndAlso n <= options.MaxFullDistanceSequences Then
                Return ComputeFull(seqs, alphabet, k)
            End If

            ' 1. 确定性地挑选种子序列：按索引等距抽样，保证同一种子下结果可复现
            Dim seedCount As Integer = options.EffectiveSeedCount(n)
            Dim seeds As New List(Of Integer)(seedCount)
            Dim seen As New HashSet(Of Integer)()

            For t As Integer = 0 To seedCount - 1
                Dim idx As Integer = CInt(Math.Floor(CDbl(t) * n / seedCount))
                idx = Math.Min(Math.Max(idx, 0), n - 1)

                If seen.Add(idx) Then
                    Call seeds.Add(idx)
                End If
            Next

            Dim seedIndex As Integer() = seeds.ToArray()
            Dim seedCounters As New List(Of KmerCounter)(seedIndex.Length)

            For Each i As Integer In seedIndex
                Call seedCounters.Add(KmerCounter.Build(seqs(i), alphabet, k))
            Next

            ' 2. 逐条序列投影：只需要 O(N x |seeds|) 次 k-mer 比较
            Dim feature As Double()() = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                Dim counter As KmerCounter = KmerCounter.Build(seqs(i), alphabet, k)
                Dim row As Double() = New Double(seedIndex.Length - 1) {}

                For t As Integer = 0 To seedIndex.Length - 1
                    row(t) = counter.Distance(seedCounters(t))
                Next

                feature(i) = row
            Next

            Return New MBedDistance(n, feature, seedIndex, False, Nothing, k)
        End Function

        ''' <summary>
        ''' 完整距离矩阵分支(对应 clustalo 的 <c>--full</c>)
        ''' </summary>
        Private Shared Function ComputeFull(seqs As String(), alphabet As SequenceAlphabet, k As Integer) As MBedDistance
            Dim n As Integer = seqs.Length
            Dim counters As KmerCounter() = New KmerCounter(n - 1) {}

            For i As Integer = 0 To n - 1
                counters(i) = KmerCounter.Build(seqs(i), alphabet, k)
            Next

            Dim matrix(n - 1)() As Double

            For i As Integer = 0 To n - 1
                matrix(i) = New Double(n - 1) {}
            Next

            For i As Integer = 0 To n - 1
                For j As Integer = i + 1 To n - 1
                    Dim d As Double = counters(i).Distance(counters(j))
                    matrix(i)(j) = d
                    matrix(j)(i) = d
                Next
            Next

            Return New MBedDistance(n, Nothing, Nothing, True, matrix, k)
        End Function

        ''' <summary>
        ''' 取两条序列之间的距离：全矩阵模式直接查表，否则用低维坐标的欧氏距离近似。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PairDistance(i As Integer, j As Integer) As Double
            If i = j Then
                Return 0
            End If

            If IsFull Then
                Return Matrix(i)(j)
            End If

            Dim a As Double() = Feature(i)
            Dim b As Double() = Feature(j)

            If a.Length = 0 Then
                Return 0
            End If

            Dim acc As Double = 0

            For t As Integer = 0 To a.Length - 1
                Dim d As Double = a(t) - b(t)
                acc += d * d
            Next

            Return Math.Sqrt(acc / a.Length)
        End Function

        ''' <summary>
        ''' 直接比较两条序列的 k-mer 距离
        ''' </summary>
        Public Shared Function PairDistance(a As String, b As String,
                                            alphabet As SequenceAlphabet, k As Integer) As Double

            Return KmerCounter.PairDistance(a, b, alphabet, k)
        End Function

        Public Overrides Function ToString() As String
            Return $"mBed(N={SequenceCount}, seeds={If(Seeds Is Nothing, 0, Seeds.Length)}, k={K}, full={IsFull})"
        End Function
    End Class
End Namespace
