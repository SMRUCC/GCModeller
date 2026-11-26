Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics

Namespace SingleCell

    ''' <summary>
    ''' 单细胞计数矩阵（细胞 × 基因）的轻量包装：在 <see cref="CsrMatrix"/> 之上补充
    ''' 计数语义的校验与常用统计量。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 构造时强制校验"非负整数且小于 <c>2^24</c>"——因为上游把计数以 <c>float32</c> 存储，
    ''' 超过 <c>2^24</c> 的整数无法被精确表示，静默接受这种输入会让下游的整数守恒检查失效。
    ''' </para>
    ''' <para>
    ''' 库大小（library size）为 0 的空细胞是合法输入：它们的所有 CPM 都是 0，
    ''' 在计算中按分母 1 处理，不会被当成异常。
    ''' </para>
    ''' </remarks>
    Public Class SingleCellCounts

        Private ReadOnly m_matrix As CsrMatrix
        Private ReadOnly m_cellTotals As Double()
        Private ReadOnly m_geneSums As Double()

        ''' <summary>
        ''' 包装一个计数矩阵
        ''' </summary>
        ''' <param name="matrix">细胞 × 基因的稀疏计数矩阵</param>
        ''' <param name="matrixName">异常信息中使用的名称</param>
        ''' <exception cref="ArgumentException">存在负值、非整数或超过 <c>2^24</c> 的计数</exception>
        Public Sub New(matrix As CsrMatrix, Optional matrixName As String = "counts")
            If matrix Is Nothing Then Throw New ArgumentNullException(NameOf(matrix))

            Call matrix.AssertIntegerCounts(matrixName)

            m_matrix = matrix
            m_cellTotals = matrix.RowSums()
            m_geneSums = matrix.ColumnSums()
        End Sub

        ''' <summary>
        ''' 由稠密矩阵（细胞 × 基因）构造
        ''' </summary>
        ''' <param name="dense"></param>
        ''' <param name="matrixName"></param>
        ''' <returns></returns>
        Public Shared Function FromDense(dense As Double()(), Optional matrixName As String = "counts") As SingleCellCounts
            Return New SingleCellCounts(CompressedBuilder.FromDense(dense), matrixName)
        End Function

        ''' <summary>
        ''' 底层的压缩矩阵
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Matrix As CsrMatrix
            Get
                Return m_matrix
            End Get
        End Property

        ''' <summary>
        ''' 细胞数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Cells As Integer
            Get
                Return m_matrix.Rows
            End Get
        End Property

        ''' <summary>
        ''' 基因数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Genes As Integer
            Get
                Return m_matrix.Columns
            End Get
        End Property

        ''' <summary>
        ''' 每个细胞的测序深度（行和）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CellTotals As Double()
            Get
                Return m_cellTotals
            End Get
        End Property

        ''' <summary>
        ''' 每个基因的总计数（列和）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneSums As Double()
            Get
                Return m_geneSums
            End Get
        End Property

        ''' <summary>
        ''' 平均测序深度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MeanDepth As Double
            Get
                If m_cellTotals.Length = 0 Then Return 0.0

                Dim sum As Double = 0.0

                For i As Integer = 0 To m_cellTotals.Length - 1
                    sum += m_cellTotals(i)
                Next

                Return sum / m_cellTotals.Length
            End Get
        End Property

        ''' <summary>
        ''' 中位测序深度（全零矩阵返回 0）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MedianDepth As Double
            Get
                If m_cellTotals.Length = 0 Then Return 0.0

                Return OrderStatistics.Quantiles.Median(m_cellTotals)
            End Get
        End Property

        ''' <summary>
        ''' 每个基因的平均每细胞 CPM（零计数按 0 计入平均，与上游口径一致）
        ''' </summary>
        ''' <returns>长度 = <see cref="Genes"/></returns>
        Public Function MeanCpm() As Double()
            Return CountStatistics.ChunkedCpmMean(m_matrix)
        End Function

        ''' <summary>
        ''' 每个基因的"检出率"：CPM 超过阈值的细胞占比
        ''' </summary>
        ''' <param name="cpmThreshold">判定阈值，默认 5（与常见的单细胞表达过滤口径一致）</param>
        ''' <returns>长度 = <see cref="Genes"/>，取值 <c>[0, 1]</c></returns>
        Public Function DetectionRate(Optional cpmThreshold As Double = 5.0) As Double()
            Return CountStatistics.DetectionRate(m_matrix, cpmThreshold)
        End Function

        ''' <summary>
        ''' 按行（细胞）下标抽取子集
        ''' </summary>
        ''' <param name="rows">目标细胞下标</param>
        ''' <returns></returns>
        Public Function Subset(rows As Integer()) As SingleCellCounts
            Return New SingleCellCounts(m_matrix.Slice(rows))
        End Function

        ''' <summary>
        ''' 纵向拼接多个计数矩阵（细胞数相加，基因数必须一致）
        ''' </summary>
        ''' <param name="parts"></param>
        ''' <returns></returns>
        Public Shared Function Append(parts As IEnumerable(Of SingleCellCounts)) As SingleCellCounts
            If parts Is Nothing Then Throw New ArgumentNullException(NameOf(parts))

            Dim blocks As SingleCellCounts() = parts.ToArray()

            If blocks.Length = 0 Then Return New SingleCellCounts(CsrMatrix.Empty())

            Dim matrices As CsrMatrix() = New CsrMatrix(blocks.Length - 1) {}

            For i As Integer = 0 To blocks.Length - 1
                If blocks(i) Is Nothing Then Throw New ArgumentException($"block #{i} is nothing", NameOf(parts))

                matrices(i) = blocks(i).m_matrix
            Next

            Return New SingleCellCounts(CsrMatrix.Stack(matrices))
        End Function

        Public Overrides Function ToString() As String
            Return $"[{Cells} cell x {Genes} gene] counts, median depth={MedianDepth:G6}"
        End Function

    End Class
End Namespace
