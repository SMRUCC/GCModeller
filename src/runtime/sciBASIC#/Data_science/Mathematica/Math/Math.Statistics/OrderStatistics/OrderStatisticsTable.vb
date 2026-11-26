Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports std = System.Math

Namespace OrderStatistics

    ''' <summary>
    ''' 序统计表的数值存储精度。
    ''' </summary>
    ''' <remarks>
    ''' 单细胞计数矩阵的序统计表规模是"基因数 × 细胞数"（例如 18000 × 18000 = 3.24 亿个元素），
    ''' 用 <see cref="Float32"/> 存储约 1.3 GB，用 <see cref="Float64"/> 约 2.6 GB。
    ''' 选择 <see cref="Float32"/> 时与上游"按 float32 栅格比较并列"的语义一致。
    ''' </remarks>
    Public Enum ValuePrecision
        ''' <summary>
        ''' 32 位（4 字节/元素），与 R/scipy 的 float32 栅格语义一致
        ''' </summary>
        Float32 = 0
        ''' <summary>
        ''' 64 位（8 字节/元素），精度更高但内存翻倍
        ''' </summary>
        Float64 = 1
    End Enum

    ''' <summary>
    ''' 逐基因升序序统计表：把"每个基因在所有细胞上的取值"排序后缓存下来，
    ''' 用于分位数映射与秩和检验。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 一旦构建完成，本对象就是<b>只读</b>的：批量统计算子可以在多个线程上共享它，
    ''' 每个线程只读自己正在处理的基因行，不存在写竞争。
    ''' </para>
    ''' <para>
    ''' 构建复杂度 <c>O(G·m·log m)</c>（G 基因、m 细胞），内存 <c>G·m</c> 个元素。
    ''' 内部按基因分块为独立数组（而不是一个巨大的二维数组），这样排序可以原地进行，
    ''' 也便于"只取需要的基因"时按需构建。
    ''' </para>
    ''' </remarks>
    Public Class OrderStatisticsTable

        Private ReadOnly m_single As Single()()
        Private ReadOnly m_double As Double()()
        Private ReadOnly m_meanCpm As Double()
        Private ReadOnly m_tieSum As Double()
        Private ReadOnly m_geneCount As Integer
        Private ReadOnly m_cellCount As Integer
        Private ReadOnly m_precision As ValuePrecision

#Region "Construction"

        Private Sub New(singleValues As Single()(),
                        doubleValues As Double()(),
                        meanCpm As Double(),
                        tieSum As Double(),
                        geneCount As Integer,
                        cellCount As Integer,
                        precision As ValuePrecision)

            m_single = singleValues
            m_double = doubleValues
            m_meanCpm = meanCpm
            m_tieSum = tieSum
            m_geneCount = geneCount
            m_cellCount = cellCount
            m_precision = precision
        End Sub

        ''' <summary>
        ''' 由单细胞计数矩阵（细胞 × 基因，CSR）构建序统计表。
        ''' </summary>
        ''' <param name="counts">计数矩阵，非负；行是细胞、列是基因</param>
        ''' <param name="precision">存储精度，默认 <see cref="ValuePrecision.Float32"/></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 每个值先转成"每细胞 CPM"（<c>1e6 · count / max(library size, 1)</c>），
        ''' 这与单细胞差异表达的标准口径一致；库大小为 0 的空细胞按分母 1 处理（全部为 0）。
        ''' </remarks>
        Public Shared Function CreateFromCounts(counts As CsrMatrix,
                                               Optional precision As ValuePrecision = ValuePrecision.Float32) As OrderStatisticsTable

            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))

            Dim cells As Integer = counts.Rows
            Dim genes As Integer = counts.Columns
            Dim indptr As Integer() = counts.Indptr
            Dim indices As Integer() = counts.Indices
            Dim values As Double() = counts.Values
            Dim meanSum As Double() = New Double(genes - 1) {}

            If precision = ValuePrecision.Float64 Then
                Dim table As Double()() = New Double(genes - 1)() {}

                For g As Integer = 0 To genes - 1
                    table(g) = New Double(cells - 1) {}
                Next

                For cell As Integer = 0 To cells - 1
                    Dim total As Double = 0.0

                    For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                        total += values(k)
                    Next

                    total = std.Max(total, 1.0)

                    For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                        Dim cpm As Double = 1000000.0 * values(k) / total

                        table(indices(k))(cell) = cpm
                        meanSum(indices(k)) += cpm
                    Next
                Next

                Return Finish(table, Nothing, meanSum, genes, cells, precision)
            Else
                Dim table As Single()() = New Single(genes - 1)() {}

                For g As Integer = 0 To genes - 1
                    table(g) = New Single(cells - 1) {}
                Next

                For cell As Integer = 0 To cells - 1
                    Dim total As Double = 0.0

                    For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                        total += values(k)
                    Next

                    total = std.Max(total, 1.0)

                    For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                        Dim cpm As Double = 1000000.0 * values(k) / total

                        table(indices(k))(cell) = CSng(cpm)
                        meanSum(indices(k)) += cpm
                    Next
                Next

                Return Finish(Nothing, table, meanSum, genes, cells, precision)
            End If
        End Function

        ''' <summary>
        ''' 由逐基因的原始取值构建序统计表（每个元素是一个基因在所有细胞上的取值）。
        ''' </summary>
        ''' <param name="perGene">长度 = 基因数；每个元素长度必须一致（= 细胞数）</param>
        ''' <param name="precision">存储精度</param>
        ''' <returns></returns>
        Public Shared Function CreateFromGenes(perGene As IEnumerable(Of Double()),
                                              Optional precision As ValuePrecision = ValuePrecision.Float32) As OrderStatisticsTable

            If perGene Is Nothing Then Throw New ArgumentNullException(NameOf(perGene))

            Dim rows As Double()() = perGene.ToArray()

            If rows.Length = 0 Then
                Return New OrderStatisticsTable(Nothing, Nothing, New Double() {}, New Double() {}, 0, 0, precision)
            End If

            Dim cells As Integer = If(rows(0) Is Nothing, 0, rows(0).Length)
            Dim meanSum As Double() = New Double(rows.Length - 1) {}

            If precision = ValuePrecision.Float64 Then
                Dim table As Double()() = New Double(rows.Length - 1)() {}

                For g As Integer = 0 To rows.Length - 1
                    Dim row As Double() = rows(g)

                    If row Is Nothing OrElse row.Length <> cells Then
                        Throw New ArgumentException($"gene #{g} has {If(row Is Nothing, 0, row.Length)} value(s) but {cells} is required", NameOf(perGene))
                    End If

                    table(g) = DirectCast(row.Clone(), Double())

                    For i As Integer = 0 To cells - 1
                        meanSum(g) += row(i)
                    Next
                Next

                Return Finish(table, Nothing, meanSum, rows.Length, cells, precision)
            Else
                Dim table As Single()() = New Single(rows.Length - 1)() {}

                For g As Integer = 0 To rows.Length - 1
                    Dim row As Double() = rows(g)

                    If row Is Nothing OrElse row.Length <> cells Then
                        Throw New ArgumentException($"gene #{g} has {If(row Is Nothing, 0, row.Length)} value(s) but {cells} is required", NameOf(perGene))
                    End If

                    Dim dense As Single() = New Single(cells - 1) {}

                    For i As Integer = 0 To cells - 1
                        dense(i) = CSng(row(i))
                        meanSum(g) += row(i)
                    Next

                    table(g) = dense
                Next

                Return Finish(Nothing, table, meanSum, rows.Length, cells, precision)
            End If
        End Function

        Private Shared Function Finish(doubleTable As Double()(),
                                       singleTable As Single()(),
                                       meanSum As Double(),
                                       genes As Integer,
                                       cells As Integer,
                                       precision As ValuePrecision) As OrderStatisticsTable

            Dim tieSum As Double() = New Double(genes - 1) {}
            Dim meanCpm As Double() = New Double(genes - 1) {}

            For g As Integer = 0 To genes - 1
                meanCpm(g) = If(cells = 0, 0.0, meanSum(g) / cells)

                If singleTable IsNot Nothing Then
                    Call Array.Sort(singleTable(g))
                    tieSum(g) = TieSumOfSorted(singleTable(g))
                Else
                    Call Array.Sort(doubleTable(g))
                    tieSum(g) = TieSumOfSorted(doubleTable(g))
                End If
            Next

            Return New OrderStatisticsTable(singleTable, doubleTable, meanCpm, tieSum, genes, cells, precision)
        End Function

        ''' <summary>
        ''' <c>Σ (t³ - t)</c>，t 为每个不同取值的重复次数
        ''' </summary>
        Private Shared Function TieSumOfSorted(values As Single()) As Double
            Dim n As Integer = values.Length
            Dim sum As Double = 0.0
            Dim i As Integer = 0

            While i < n
                Dim v As Single = values(i)
                Dim j As Integer = i

                While j + 1 < n AndAlso values(j + 1) = v
                    j += 1
                End While

                Dim t As Double = j - i + 1

                sum += t * t * t - t
                i = j + 1
            End While

            Return sum
        End Function

        Private Shared Function TieSumOfSorted(values As Double()) As Double
            Dim n As Integer = values.Length
            Dim sum As Double = 0.0
            Dim i As Integer = 0

            While i < n
                Dim v As Double = values(i)
                Dim j As Integer = i

                While j + 1 < n AndAlso values(j + 1) = v
                    j += 1
                End While

                Dim t As Double = j - i + 1

                sum += t * t * t - t
                i = j + 1
            End While

            Return sum
        End Function

#End Region

#Region "Shape"

        ''' <summary>
        ''' 基因数量（表的行数）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneCount As Integer
            Get
                Return m_geneCount
            End Get
        End Property

        ''' <summary>
        ''' 每个基因的观测数量（细胞数）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CellCount As Integer
            Get
                Return m_cellCount
            End Get
        End Property

        ''' <summary>
        ''' 数值存储精度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Precision As ValuePrecision
            Get
                Return m_precision
            End Get
        End Property

        ''' <summary>
        ''' 逐基因的平均每细胞 CPM
        ''' </summary>
        ''' <returns>长度 = <see cref="GeneCount"/></returns>
        Public ReadOnly Property MeanCpm As Double()
            Get
                Return m_meanCpm
            End Get
        End Property

        ''' <summary>
        ''' 逐基因的并列和 <c>Σ (t³ - t)</c>（秩和检验的并列校正用）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TieSum As Double()
            Get
                Return m_tieSum
            End Get
        End Property

        ''' <summary>
        ''' 取出一个基因的升序数值行（<see cref="ValuePrecision.Float32"/> 精度）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <returns>内部数组，**调用方不得修改**</returns>
        Public Function RowSingle(gene As Integer) As Single()
            CheckGene(gene)

            If m_single Is Nothing Then
                Throw New InvalidOperationException($"the table is stored as {m_precision}; use {NameOf(RowDouble)} instead")
            End If

            Return m_single(gene)
        End Function

        ''' <summary>
        ''' 取出一个基因的升序数值行（<see cref="ValuePrecision.Float64"/> 精度）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <returns>内部数组，**调用方不得修改**</returns>
        Public Function RowDouble(gene As Integer) As Double()
            CheckGene(gene)

            If m_double Is Nothing Then
                Throw New InvalidOperationException($"the table is stored as {m_precision}; use {NameOf(RowSingle)} instead")
            End If

            Return m_double(gene)
        End Function

        ''' <summary>
        ''' 复制一个基因的升序数值行（统一为 <see cref="Double"/>，用于诊断与测试）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <returns></returns>
        Public Function RowCopy(gene As Integer) As Double()
            CheckGene(gene)

            If m_single IsNot Nothing Then
                Dim src As Single() = m_single(gene)
                Dim out As Double() = New Double(src.Length - 1) {}

                For i As Integer = 0 To src.Length - 1
                    out(i) = src(i)
                Next

                Return out
            Else
                Return DirectCast(m_double(gene).Clone(), Double())
            End If
        End Function

#End Region

#Region "Rank lookup"

        ''' <summary>
        ''' 第一个 <c>&gt;= value</c> 的位置（升序数组的下界），等价于 <c>searchsorted(side="left")</c>
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="value"></param>
        ''' <returns>落在 <c>[0, CellCount]</c></returns>
        Public Function LowerBound(gene As Integer, value As Double) As Integer
            CheckGene(gene)

            Dim lo As Integer = 0
            Dim hi As Integer = m_cellCount

            If m_single IsNot Nothing Then
                Dim row As Single() = m_single(gene)
                Dim v As Single = CSng(value)

                While lo < hi
                    Dim mid As Integer = (lo + hi) \ 2

                    If row(mid) < v Then
                        lo = mid + 1
                    Else
                        hi = mid
                    End If
                End While
            Else
                Dim row As Double() = m_double(gene)

                While lo < hi
                    Dim mid As Integer = (lo + hi) \ 2

                    If row(mid) < value Then
                        lo = mid + 1
                    Else
                        hi = mid
                    End If
                End While
            End If

            Return lo
        End Function

        ''' <summary>
        ''' 第一个 <c>&gt; value</c> 的位置（上界），等价于 <c>searchsorted(side="right")</c>
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="value"></param>
        ''' <returns>落在 <c>[0, CellCount]</c></returns>
        Public Function UpperBound(gene As Integer, value As Double) As Integer
            CheckGene(gene)

            Dim lo As Integer = 0
            Dim hi As Integer = m_cellCount

            If m_single IsNot Nothing Then
                Dim row As Single() = m_single(gene)
                Dim v As Single = CSng(value)

                While lo < hi
                    Dim mid As Integer = (lo + hi) \ 2

                    If row(mid) <= v Then
                        lo = mid + 1
                    Else
                        hi = mid
                    End If
                End While
            Else
                Dim row As Double() = m_double(gene)

                While lo < hi
                    Dim mid As Integer = (lo + hi) \ 2

                    If row(mid) <= value Then
                        lo = mid + 1
                    Else
                        hi = mid
                    End If
                End While
            End If

            Return lo
        End Function

        ''' <summary>
        ''' 与 <paramref name="value"/> 相等的观测个数
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function CountEqual(gene As Integer, value As Double) As Integer
            Return UpperBound(gene, value) - LowerBound(gene, value)
        End Function

#End Region

#Region "Quantiles"

        ''' <summary>
        ''' 读取指定名次的值（越界自动截断到两端）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="index">0 基名次</param>
        ''' <returns></returns>
        Public Function ValueAtIndex(gene As Integer, index As Integer) As Double
            CheckGene(gene)

            If m_cellCount = 0 Then Return 0.0

            Dim i As Integer = index

            If i < 0 Then i = 0
            If i >= m_cellCount Then i = m_cellCount - 1

            If m_single IsNot Nothing Then
                Return m_single(gene)(i)
            Else
                Return m_double(gene)(i)
            End If
        End Function

        ''' <summary>
        ''' 直接按分位数取"最近名次"的值：<c>index = floor(q · m)</c>（越界截断）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="q">分位数，落在 <c>[0, 1]</c></param>
        ''' <returns></returns>
        Public Function IndexedQuantile(gene As Integer, q As Double) As Double
            CheckGene(gene)

            Dim index As Integer = CInt(std.Floor(q * m_cellCount))

            Return ValueAtIndex(gene, index)
        End Function

        ''' <summary>
        ''' 带平移量的分位数取值：<c>index = floor((shift + q - 0.5) · m)</c>
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="q">分位数，通常取 <c>(k + 0.5) / m</c></param>
        ''' <param name="shift">平移量（例如分位数搜索中的 <c>a</c> 参数）</param>
        ''' <returns>
        ''' 当 <c>shift + q - 0.5 &lt;= 0</c> 时返回 0（表示"低于控制组的最小观测"，
        ''' 对应计数为 0 的语义）；超过 1 时截断到最大值。
        ''' </returns>
        Public Function ShiftedQuantile(gene As Integer, q As Double, shift As Double) As Double
            CheckGene(gene)

            Dim position As Double = shift + q - 0.5

            If position <= 0.0 Then Return 0.0

            Return ValueAtIndex(gene, CInt(std.Floor(position * m_cellCount)))
        End Function

        ''' <summary>
        ''' 线性插值分位数（R 的 <c>type = 7</c> 语义：<c>h = (m-1)·q</c>）
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="q">分位数，落在 <c>[0, 1]</c></param>
        ''' <returns></returns>
        Public Function Type7Quantile(gene As Integer, q As Double) As Double
            CheckGene(gene)

            If m_cellCount = 0 Then Return 0.0
            If q <= 0.0 Then Return ValueAtIndex(gene, 0)
            If q >= 1.0 Then Return ValueAtIndex(gene, m_cellCount - 1)

            Dim h As Double = (m_cellCount - 1) * q
            Dim lo As Integer = CInt(std.Floor(h))

            If lo >= m_cellCount - 1 Then Return ValueAtIndex(gene, m_cellCount - 1)

            Dim fraction As Double = h - lo
            Dim lower As Double = ValueAtIndex(gene, lo)

            Return lower * (1.0 - fraction) + ValueAtIndex(gene, lo + 1) * fraction
        End Function

#End Region

        Private Sub CheckGene(gene As Integer)
            If gene < 0 OrElse gene >= m_geneCount Then
                Throw New ArgumentOutOfRangeException(NameOf(gene), gene, $"gene index must be within [0, {m_geneCount})")
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{m_geneCount} gene x {m_cellCount} cell] order statistics ({m_precision})"
        End Function

    End Class
End Namespace
