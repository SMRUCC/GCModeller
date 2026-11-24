' ============================================================================
' GraphBatch.vb - GraphVAE 的前向缓存、扩展选项与稀疏邻接容器
'
' 三个目标：
'   1. GraphForwardCache：把原先散落在 GraphVAE 上的约 20 个可变字段收敛成一个
'      缓存对象，使前向可以重入、可批处理，并让「评估」与「训练」走同一条通路
'      而不必共享可变状态；
'   2. GraphVAEOptions：变长图（填充 + 掩码）所需的扩展配置。GraphVAEConfig 是
'      公开 Structure，追加字段会破坏二进制布局，因此绝不修改它；
'   3. SparseAdjacency：CSR 稀疏邻接，用于替代编码器侧的稠密 O(N²) 传播。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' GraphVAE 单次前向的中间量缓存。
''' </summary>
''' <remarks>
''' 之前这些量以字段形式挂在 <see cref="GraphVAE"/> 上，导致同一个模型实例不可重入、
''' 无法并行评估，且 <c>Evaluate</c> 会顺带改写模型状态。
''' </remarks>
Public Class GraphForwardCache

    Public NumNodes As Integer
    Public InputDim As Integer
    Public HiddenDim1 As Integer
    Public HiddenDim2 As Integer
    Public DecoderHidden As Integer
    Public LatentDim As Integer

    ''' <summary>节点掩码 [N]，1 表示有效节点，0 表示填充节点（变长图）。</summary>
    Public NodeMask As Double()

    Public Adjacency As Tensor
    Public Features As Tensor
    Public NormAdjacency As Tensor

    Public Hidden1 As Tensor
    Public Hidden2 As Tensor
    Public GraphEmbedding As Tensor

    Public Mu As Tensor
    Public LogVar As Tensor
    Public Std As Tensor
    Public Epsilon As Tensor
    Public Sample As Tensor

    Public DecoderH As Tensor
    Public NodeEmbedding As Tensor
    Public NodeEmbeddingActivated As Tensor

    Public AdjacencyLogits As Tensor
    Public ReconAdjacency As Tensor
    Public FeatureLogits As Tensor
    Public ReconFeatures As Tensor

    ''' <summary>有效节点数量（无掩码时等于 <see cref="NumNodes"/>）。</summary>
    Public Function ValidNodeCount() As Integer
        If NodeMask Is Nothing Then
            Return NumNodes
        End If

        Dim count As Integer = 0

        For i As Integer = 0 To NodeMask.Length - 1
            If NodeMask(i) > 0 Then
                count += 1
            End If
        Next

        Return count
    End Function

    Public Function HasMask() As Boolean
        Return NodeMask IsNot Nothing
    End Function

End Class

''' <summary>
''' GraphVAE 的扩展选项（变长图 / 有向图 / 种子）。
''' </summary>
Public Class GraphVAEOptions

    ''' <summary>节点数量上界；变长图按该值填充。</summary>
    Public Property MaxNodes As Integer

    ''' <summary>节点特征维度。</summary>
    Public Property InputDim As Integer

    Public Property Hidden1 As Integer = 32

    Public Property Hidden2 As Integer = 16

    Public Property LatentDim As Integer = 8

    Public Property DecHidden As Integer = 16

    Public Property Seed As Integer?

    ''' <summary>邻接矩阵是否对称（无向图）。有向图必须设为 <c>False</c>。</summary>
    Public Property AssumeSymmetricAdjacency As Boolean = True

    ''' <summary>转换为既有的 <see cref="GraphVAEConfig"/>。</summary>
    Public Function ToConfig() As GraphVAEConfig
        Return New GraphVAEConfig With {
            .NumNodes = MaxNodes,
            .InputDim = InputDim,
            .Hidden1 = Hidden1,
            .Hidden2 = Hidden2,
            .LatentDim = LatentDim,
            .DecHidden = DecHidden
        }
    End Function

    Public Overrides Function ToString() As String
        Return $"GraphVAEOptions(maxNodes={MaxNodes}, input={InputDim}, latent={LatentDim}, dec={DecHidden})"
    End Function

End Class

''' <summary>
''' CSR（压缩稀疏行）格式的邻接矩阵。
''' </summary>
''' <remarks>
''' GraphVAE 的内积解码器输出的是稠密 N×N 邻接，这一部分无法避免 O(N²)；
''' 但编码器的图传播 <c>Â @ X</c> 完全可以在稀疏结构上完成。
''' 对规模较大的图，用本容器替代稠密矩阵可以把编码器侧的峰值内存从
''' O(N²) 降到 O(E)，其中 E 为边数。
''' </remarks>
Public Class SparseAdjacency

    Private ReadOnly _rowOffsets As Integer()
    Private ReadOnly _columnIndices As Integer()
    Private ReadOnly _values As Double()

    Public Sub New(numNodes As Integer, rowOffsets As Integer(), columnIndices As Integer(), values As Double())
        If numNodes <= 0 Then
            Throw New ArgumentException("节点数量必须大于 0", NameOf(numNodes))
        End If

        If rowOffsets Is Nothing OrElse rowOffsets.Length <> numNodes + 1 Then
            Throw New ArgumentException($"行偏移数组长度必须为 {numNodes + 1}", NameOf(rowOffsets))
        End If

        If columnIndices Is Nothing OrElse values Is Nothing Then
            Throw New ArgumentNullException(NameOf(columnIndices))
        End If

        If columnIndices.Length <> values.Length Then
            Throw New ArgumentException("列索引与数值数组长度必须一致")
        End If

        Me.NumNodes = numNodes
        _rowOffsets = rowOffsets
        _columnIndices = columnIndices
        _values = values
    End Sub

    ''' <summary>节点数量。</summary>
    Public ReadOnly Property NumNodes As Integer

    ''' <summary>非零元素数量（边数）。</summary>
    Public ReadOnly Property NonZeroCount As Integer
        Get
            Return _values.Length
        End Get
    End Property

    Public ReadOnly Property RowOffsets As Integer()
        Get
            Return _rowOffsets
        End Get
    End Property

    ''' <summary>把稠密邻接矩阵转换为 CSR，忽略绝对值不超过阈值的元素。</summary>
    Public Shared Function FromDense(adjacency As Tensor, Optional threshold As Double = 0) As SparseAdjacency
        If adjacency Is Nothing Then
            Throw New ArgumentNullException(NameOf(adjacency))
        End If

        If adjacency.Rank <> 2 OrElse adjacency.Shape(0) <> adjacency.Shape(1) Then
            Throw New ArgumentException("邻接矩阵必须是方阵", NameOf(adjacency))
        End If

        Dim nodes As Integer = adjacency.Shape(0)
        Dim offsets As Integer() = New Integer(nodes) {}
        Dim columns As New List(Of Integer)
        Dim values As New List(Of Double)

        For i As Integer = 0 To nodes - 1
            offsets(i) = columns.Count

            For j As Integer = 0 To nodes - 1
                Dim value As Double = adjacency(i, j)

                If std.Abs(value) > threshold Then
                    columns.Add(j)
                    values.Add(value)
                End If
            Next
        Next

        offsets(nodes) = columns.Count

        Return New SparseAdjacency(nodes, offsets, columns.ToArray(), values.ToArray())
    End Function

    ''' <summary>还原为稠密矩阵（O(N²)，仅用于调试与小规模场景）。</summary>
    Public Function ToDense() As Tensor
        Dim result As New Tensor(NumNodes, NumNodes)
        Dim buffer As Double() = result.Data

        For i As Integer = 0 To NumNodes - 1
            For p As Integer = _rowOffsets(i) To _rowOffsets(i + 1) - 1
                buffer(i * NumNodes + _columnIndices(p)) = _values(p)
            Next
        Next

        Return result
    End Function

    ''' <summary>第 i 行的元素之和（即加自环后的度数）。</summary>
    Public Function RowSum(row As Integer) As Double
        Dim acc As Double = 0

        For p As Integer = _rowOffsets(row) To _rowOffsets(row + 1) - 1
            acc += _values(p)
        Next

        Return acc
    End Function

    ''' <summary>
    ''' 构造 GCN 的标准归一化稀疏邻接：Â = D^(-1/2)·(A + I)·D^(-1/2)。
    ''' </summary>
    Public Function NormalizedWithSelfLoops() As SparseAdjacency
        Dim nodes As Integer = NumNodes
        Dim degree As Double() = New Double(nodes - 1) {}

        For i As Integer = 0 To nodes - 1
            degree(i) = RowSum(i) + 1.0
        Next

        Dim inverseSqrt As Double() = New Double(nodes - 1) {}

        For i As Integer = 0 To nodes - 1
            inverseSqrt(i) = If(degree(i) > 0, 1.0 / std.Sqrt(degree(i)), 0)
        Next

        Dim offsets As Integer() = New Integer(nodes) {}
        Dim columns As New List(Of Integer)
        Dim values As New List(Of Double)

        For i As Integer = 0 To nodes - 1
            offsets(i) = columns.Count

            ' 自环
            columns.Add(i)
            values.Add(1.0 * inverseSqrt(i) * inverseSqrt(i))

            ' 原始边
            For p As Integer = _rowOffsets(i) To _rowOffsets(i + 1) - 1
                Dim j As Integer = _columnIndices(p)

                If j = i Then
                    Continue For
                End If

                columns.Add(j)
                values.Add(_values(p) * inverseSqrt(i) * inverseSqrt(j))
            Next
        Next

        offsets(nodes) = columns.Count

        Return New SparseAdjacency(nodes, offsets, columns.ToArray(), values.ToArray())
    End Function

    ''' <summary>稀疏矩阵乘法：返回本矩阵 × <paramref name="x"/>，形状 [N, F]。</summary>
    Public Function Multiply(x As Tensor) As Tensor
        If x Is Nothing Then
            Throw New ArgumentNullException(NameOf(x))
        End If

        If x.Shape(0) <> NumNodes Then
            Throw New ArgumentException($"特征矩阵行数应为 {NumNodes}，实际 {x.Shape(0)}")
        End If

        Dim features As Integer = x.Shape(1)
        Dim source As Double() = x.Data
        Dim result As New Tensor(NumNodes, features)
        Dim output As Double() = result.Data

        For i As Integer = 0 To NumNodes - 1
            Dim rowBase As Integer = i * features

            For p As Integer = _rowOffsets(i) To _rowOffsets(i + 1) - 1
                Dim weight As Double = _values(p)
                Dim sourceBase As Integer = _columnIndices(p) * features

                For f As Integer = 0 To features - 1
                    output(rowBase + f) += weight * source(sourceBase + f)
                Next
            Next
        Next

        Return result
    End Function

    ''' <summary>稀疏矩阵转置乘法：返回本矩阵ᵀ × <paramref name="x"/>。</summary>
    Public Function TransposeMultiply(x As Tensor) As Tensor
        If x Is Nothing Then
            Throw New ArgumentNullException(NameOf(x))
        End If

        If x.Shape(0) <> NumNodes Then
            Throw New ArgumentException($"特征矩阵行数应为 {NumNodes}，实际 {x.Shape(0)}")
        End If

        Dim features As Integer = x.Shape(1)
        Dim source As Double() = x.Data
        Dim result As New Tensor(NumNodes, features)
        Dim output As Double() = result.Data

        For i As Integer = 0 To NumNodes - 1
            Dim rowBase As Integer = i * features

            For p As Integer = _rowOffsets(i) To _rowOffsets(i + 1) - 1
                Dim weight As Double = _values(p)
                Dim targetBase As Integer = _columnIndices(p) * features

                For f As Integer = 0 To features - 1
                    output(targetBase + f) += weight * source(rowBase + f)
                Next
            Next
        Next

        Return result
    End Function

    Public Overrides Function ToString() As String
        Return $"SparseAdjacency(nodes={NumNodes}, edges={NonZeroCount}, density={NonZeroCount / CDbl(NumNodes * NumNodes):P3})"
    End Function

End Class
