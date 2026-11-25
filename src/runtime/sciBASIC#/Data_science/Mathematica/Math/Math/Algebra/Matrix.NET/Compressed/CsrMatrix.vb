Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

Namespace LinearAlgebra.Matrix.Compressed

    ''' <summary>
    ''' 压缩行存储（Compressed Sparse Row, CSR）稀疏矩阵。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 内部由三个平行数组构成：<c>indptr</c>（长度 <c>rows + 1</c>）、<c>indices</c>（列下标）、
    ''' <c>values</c>（元素值）。第 <c>i</c> 行的非零元素位于
    ''' <c>indices(indptr(i)) .. indices(indptr(i + 1) - 1)</c>，且同一行内的列下标严格升序，
    ''' 因此单行的列定位是 <c>O(log nnz_row)</c> 的二分查找。
    ''' </para>
    ''' <para>
    ''' 与 <see cref="SparseMatrix"/>（字典套字典）不同，本类型是"可计算的矩阵"：
    ''' 三次稀疏矩阵-向量乘法、稀疏×稀疏、转置、行/列子集都只依赖 <c>nnz</c> 的单遍扫描，
    ''' 不会把内部结构稠密化。适用于单细胞计数矩阵（1e7 以上非零元）这类规模。
    ''' </para>
    ''' <para>
    ''' 本类型是不可变容器：所有"修改类"操作（<see cref="EliminateZeros"/>、<see cref="ScaleRows(Double())"/>、
    ''' <see cref="Slice(IEnumerable(Of Integer))"/> 等）都返回新实例，不会改写当前实例，
    ''' 因此可以安全地被多个线程共享读取（批量统计算子的并行只读前提）。
    ''' </para>
    ''' </remarks>
    Public Class CsrMatrix
        Implements GeneralMatrix

#Region "Class variables"

        ''' <summary>
        ''' 行指针数组，长度为 <c>rows + 1</c>
        ''' </summary>
        Private ReadOnly m_indptr As Integer()
        ''' <summary>
        ''' 列下标数组，长度为 <c>nnz</c>
        ''' </summary>
        Private ReadOnly m_indices As Integer()
        ''' <summary>
        ''' 元素值数组，长度为 <c>nnz</c>
        ''' </summary>
        Private ReadOnly m_values As Double()
        Private ReadOnly m_rows As Integer
        Private ReadOnly m_columns As Integer

#End Region

#Region "Constructor"

        ''' <summary>
        ''' 以已有的压缩结构直接构造（不做任何拷贝）。
        ''' </summary>
        ''' <param name="rows">行数</param>
        ''' <param name="columns">列数</param>
        ''' <param name="indptr">行指针，长度必须等于 <paramref name="rows"/> + 1</param>
        ''' <param name="indices">列下标，必须落在 <c>[0, columns)</c> 且每行内升序</param>
        ''' <param name="values">元素值，长度必须等于 <paramref name="indices"/></param>
        ''' <exception cref="ArgumentException">压缩结构不合法（长度、单调性、下标越界）</exception>
        Public Sub New(rows As Integer, columns As Integer, indptr As Integer(), indices As Integer(), values As Double())
            If rows < 0 Then Throw New ArgumentException($"row dimension must not be negative: {rows}", NameOf(rows))
            If columns < 0 Then Throw New ArgumentException($"column dimension must not be negative: {columns}", NameOf(columns))
            If indptr Is Nothing Then Throw New ArgumentNullException(NameOf(indptr))
            If indices Is Nothing Then Throw New ArgumentNullException(NameOf(indices))
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
            If indptr.Length <> rows + 1 Then
                Throw New ArgumentException($"indptr length {indptr.Length} does not match rows + 1 = {rows + 1}", NameOf(indptr))
            End If
            If indices.Length <> values.Length Then
                Throw New ArgumentException($"indices length {indices.Length} does not match values length {values.Length}", NameOf(values))
            End If
            If indptr(0) <> 0 Then
                Throw New ArgumentException($"indptr(0) must be zero but is {indptr(0)}", NameOf(indptr))
            End If
            If indptr(rows) <> indices.Length Then
                Throw New ArgumentException(
                    $"indptr(last) = {indptr(rows)} does not match the non-zero count {indices.Length}", NameOf(indptr))
            End If

            For i As Integer = 0 To rows - 1
                If indptr(i + 1) < indptr(i) Then
                    Throw New ArgumentException($"indptr is not monotone at row {i}: {indptr(i)} -> {indptr(i + 1)}", NameOf(indptr))
                End If
            Next
            For k As Integer = 0 To indices.Length - 1
                Dim col As Integer = indices(k)

                If col < 0 OrElse col >= columns Then
                    Throw New ArgumentException(
                        $"column index {col} at non-zero entry #{k} is out of range [0, {columns})", NameOf(indices))
                End If
            Next

            m_rows = rows
            m_columns = columns
            m_indptr = indptr
            m_indices = indices
            m_values = values
        End Sub

#End Region

#Region "Size and raw storage"

        ''' <summary>
        ''' 行数（m）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Rows As Integer
            Get
                Return m_rows
            End Get
        End Property

        ''' <summary>
        ''' 列数（n）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Columns As Integer
            Get
                Return m_columns
            End Get
        End Property

        ''' <summary>
        ''' 非零元素个数（= <c>values.Length</c>）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NonZeros As Long
            Get
                Return CLng(m_values.Length)
            End Get
        End Property

        ''' <summary>
        ''' 行指针数组（长度 = <see cref="Rows"/> + 1）。
        ''' </summary>
        ''' <remarks>
        ''' 直接暴露内部数组以避免整表拷贝，**调用方不得修改**。
        ''' </remarks>
        Public ReadOnly Property Indptr As Integer()
            Get
                Return m_indptr
            End Get
        End Property

        ''' <summary>
        ''' 列下标数组（长度 = <see cref="NonZeros"/>），每行内升序。
        ''' </summary>
        ''' <remarks>直接暴露内部数组以避免整表拷贝，**调用方不得修改**。</remarks>
        Public ReadOnly Property Indices As Integer()
            Get
                Return m_indices
            End Get
        End Property

        ''' <summary>
        ''' 元素值数组（长度 = <see cref="NonZeros"/>）。
        ''' </summary>
        ''' <remarks>直接暴露内部数组以避免整表拷贝，**调用方不得修改**。</remarks>
        Public ReadOnly Property Values As Double()
            Get
                Return m_values
            End Get
        End Property

        Public ReadOnly Property RowDimension As Integer Implements GeneralMatrix.RowDimension
            Get
                Return m_rows
            End Get
        End Property

        Public ReadOnly Property ColumnDimension As Integer Implements GeneralMatrix.ColumnDimension
            Get
                Return m_columns
            End Get
        End Property

        ''' <summary>
        ''' 创建一个指定尺寸的空稀疏矩阵（0 个非零元）
        ''' </summary>
        ''' <param name="rows"></param>
        ''' <param name="columns"></param>
        ''' <returns></returns>
        Public Shared Function Empty(Optional rows As Integer = 0, Optional columns As Integer = 0) As CsrMatrix
            Return New CsrMatrix(rows, columns, New Integer(rows) {}, New Integer() {}, New Double() {})
        End Function

#End Region

#Region "Element access"

        ''' <summary>
        ''' 在指定行内二分定位列下标
        ''' </summary>
        ''' <param name="row">行下标</param>
        ''' <param name="column">列下标</param>
        ''' <returns>非零元在 <see cref="Indices"/> 中的位置；不存在返回 -1</returns>
        Public Function IndexOf(row As Integer, column As Integer) As Integer
            CheckRow(row)
            CheckColumn(column)

            Dim s As Integer = m_indptr(row)
            Dim e As Integer = m_indptr(row + 1) - 1

            Do While s <= e
                Dim mid As Integer = (s + e) \ 2
                Dim c As Integer = m_indices(mid)

                If c = column Then
                    Return mid
                ElseIf c < column Then
                    s = mid + 1
                Else
                    e = mid - 1
                End If
            Loop

            Return -1
        End Function

        ''' <summary>
        ''' 读取一个元素，缺失位置按 0 处理
        ''' </summary>
        ''' <param name="row">行下标</param>
        ''' <param name="column">列下标</param>
        ''' <returns></returns>
        Public Function ValueOf(row As Integer, column As Integer) As Double
            Dim i As Integer = IndexOf(row, column)

            If i < 0 Then
                Return 0.0
            Else
                Return m_values(i)
            End If
        End Function

        ''' <summary>
        ''' 读取/写入一个元素。读取缺失位置返回 0；
        ''' </summary>
        ''' <remarks>
        ''' <b>本类型是不可变容器，写入操作会抛出 <see cref="NotSupportedException"/>。</b>
        ''' 需要构造新值请使用 <see cref="Slice(Integer(), Integer())"/> 或先用 <see cref="ToDense"/> 转为稠密矩阵。
        ''' </remarks>
        Default Public Property X(i As Integer, j As Integer) As Double Implements GeneralMatrix.X
            Get
                Return ValueOf(i, j)
            End Get
            Set(value As Double)
                Throw New NotSupportedException(
                    "CsrMatrix is an immutable container: assign through a builder or ToDense() instead.")
            End Set
        End Property

        Default Public Property X(i As Integer, Optional byRow As Boolean = True) As Vector Implements GeneralMatrix.X
            Get
                If byRow Then
                    Return New Vector(Row(i))
                Else
                    Return New Vector(ColumnVector(i))
                End If
            End Get
            Set(value As Vector)
                Throw New NotSupportedException(
                    "CsrMatrix is an immutable container: assign through a builder or ToDense() instead.")
            End Set
        End Property

        ''' <summary>
        ''' 列投影：按给定的列下标顺序抽取子矩阵（同名列只保留首次出现）
        ''' </summary>
        ''' <param name="indices">目标列下标序列</param>
        ''' <returns>行数不变、列数为 <paramref name="indices"/> 元素个数的新矩阵</returns>
        Default Public ReadOnly Property X(indices As IEnumerable(Of Integer)) As GeneralMatrix Implements GeneralMatrix.X
            Get
                Return SliceColumns(indices)
            End Get
        End Property

        ''' <summary>
        ''' 行投影：按布尔掩码抽取行子集（<c>True</c> 保留）
        ''' </summary>
        ''' <param name="rowIdx">长度必须等于 <see cref="Rows"/> 的掩码</param>
        ''' <returns></returns>
        Default Public ReadOnly Property X(rowIdx As BooleanVector) As GeneralMatrix Implements GeneralMatrix.X
            Get
                If rowIdx Is Nothing Then Throw New ArgumentNullException(NameOf(rowIdx))

                Dim mask As Boolean() = rowIdx.ToArray()

                If mask.Length <> m_rows Then
                    Throw New ArgumentException(
                        $"row mask length {mask.Length} does not match the row dimension {m_rows}", NameOf(rowIdx))
                End If

                Dim take As New List(Of Integer)

                For i As Integer = 0 To mask.Length - 1
                    If mask(i) Then Call take.Add(i)
                Next

                Return Slice(take)
            End Get
        End Property

        ''' <summary>
        ''' 抽取一整行（稠密）
        ''' </summary>
        ''' <param name="row">行下标</param>
        ''' <returns>长度为 <see cref="Columns"/> 的数组</returns>
        Public Function Row(row As Integer) As Double()
            CheckRow(row)

            Dim out As Double() = New Double(m_columns - 1) {}

            For k As Integer = m_indptr(row) To m_indptr(row + 1) - 1
                out(m_indices(k)) = m_values(k)
            Next

            Return out
        End Function

        ''' <summary>
        ''' 抽取一整列（稠密）
        ''' </summary>
        ''' <param name="column">列下标</param>
        ''' <returns>长度为 <see cref="Rows"/> 的数组</returns>
        ''' <remarks>复杂度 <c>O(nnz)</c>；需要反复按列访问时请先 <see cref="ToCsc"/></remarks>
        Public Function ColumnVector(column As Integer) As Double()
            CheckColumn(column)

            Dim out As Double() = New Double(m_rows - 1) {}

            For i As Integer = 0 To m_rows - 1
                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    If m_indices(k) = column Then
                        out(i) = m_values(k)
                        Exit For
                    ElseIf m_indices(k) > column Then
                        Exit For
                    End If
                Next
            Next

            Return out
        End Function

        ''' <summary>
        ''' 抽取一行的非零元（列下标与值成对返回，按列升序）
        ''' </summary>
        ''' <param name="row">行下标</param>
        ''' <returns></returns>
        Public Function RowNonZero(row As Integer) As (columns As Integer(), values As Double())
            CheckRow(row)

            Dim s As Integer = m_indptr(row)
            Dim n As Integer = m_indptr(row + 1) - s
            Dim cols As Integer() = New Integer(n - 1) {}
            Dim vals As Double() = New Double(n - 1) {}

            Call Array.ConstrainedCopy(m_indices, s, cols, 0, n)
            Call Array.ConstrainedCopy(m_values, s, vals, 0, n)

            Return (cols, vals)
        End Function

#End Region

#Region "Row and column statistics"

        ''' <summary>
        ''' 每一行的和
        ''' </summary>
        ''' <returns>长度 = <see cref="Rows"/></returns>
        Public Function RowSums() As Double()
            Dim out As Double() = New Double(m_rows - 1) {}

            For i As Integer = 0 To m_rows - 1
                Dim sum As Double = 0.0

                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    sum += m_values(k)
                Next

                out(i) = sum
            Next

            Return out
        End Function

        ''' <summary>
        ''' 单行的和
        ''' </summary>
        ''' <param name="row">行下标</param>
        ''' <returns></returns>
        Public Function RowSum(row As Integer) As Double
            CheckRow(row)

            Dim sum As Double = 0.0

            For k As Integer = m_indptr(row) To m_indptr(row + 1) - 1
                sum += m_values(k)
            Next

            Return sum
        End Function

        ''' <summary>
        ''' 每一列的和
        ''' </summary>
        ''' <returns>长度 = <see cref="Columns"/></returns>
        Public Function ColumnSums() As Double()
            Dim out As Double() = New Double(m_columns - 1) {}

            For k As Integer = 0 To m_values.Length - 1
                out(m_indices(k)) += m_values(k)
            Next

            Return out
        End Function

        ''' <summary>
        ''' 非零元的绝对值和
        ''' </summary>
        ''' <returns></returns>
        Public Function AbsSum() As Double
            Dim sum As Double = 0.0

            For k As Integer = 0 To m_values.Length - 1
                sum += std.Abs(m_values(k))
            Next

            Return sum
        End Function

#End Region

#Region "Sparse arithmetic"

        ''' <summary>
        ''' 稀疏矩阵乘稠密向量：<c>result = A · x</c>，复杂度 <c>O(nnz)</c>
        ''' </summary>
        ''' <param name="x">长度必须等于 <see cref="Columns"/></param>
        ''' <returns>长度 = <see cref="Rows"/></returns>
        Public Function Multiply(x As Double()) As Double()
            If x Is Nothing Then Throw New ArgumentNullException(NameOf(x))
            If x.Length <> m_columns Then
                Throw New ArgumentException($"vector length {x.Length} does not match the column dimension {m_columns}", NameOf(x))
            End If

            Dim out As Double() = New Double(m_rows - 1) {}

            For i As Integer = 0 To m_rows - 1
                Dim sum As Double = 0.0
                Dim s As Integer = m_indptr(i)
                Dim e As Integer = m_indptr(i + 1)

                For k As Integer = s To e - 1
                    sum += m_values(k) * x(m_indices(k))
                Next

                out(i) = sum
            Next

            Return out
        End Function

        ''' <summary>
        ''' 稀疏矩阵转置乘稠密向量：<c>result = Aᵀ · y</c>，复杂度 <c>O(nnz)</c>
        ''' </summary>
        ''' <param name="y">长度必须等于 <see cref="Rows"/></param>
        ''' <returns>长度 = <see cref="Columns"/></returns>
        Public Function MultiplyTranspose(y As Double()) As Double()
            If y Is Nothing Then Throw New ArgumentNullException(NameOf(y))
            If y.Length <> m_rows Then
                Throw New ArgumentException($"vector length {y.Length} does not match the row dimension {m_rows}", NameOf(y))
            End If

            Dim out As Double() = New Double(m_columns - 1) {}

            For i As Integer = 0 To m_rows - 1
                Dim yi As Double = y(i)

                If yi = 0.0 Then Continue For

                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    out(m_indices(k)) += m_values(k) * yi
                Next
            Next

            Return out
        End Function

        ''' <summary>
        ''' 稀疏矩阵乘稀疏矩阵：<c>C = A · B</c>
        ''' </summary>
        ''' <param name="other">右矩阵，行数必须等于 <see cref="Columns"/></param>
        ''' <returns>按行升序、每行列下标升序的标准 CSR</returns>
        ''' <remarks>
        ''' 采用 SMMP 的"标记数组 + 累加器"单遍算法：每个输出行只遍历一次
        ''' <c>A</c> 的非零元所对应的 <c>B</c> 行，复杂度约为 <c>Σ_i nnz(A_i)·avg(nnz(B_row))</c>，
        ''' 不需要预先做符号分解。
        ''' </remarks>
        Public Function Multiply(other As CsrMatrix) As CsrMatrix
            If other Is Nothing Then Throw New ArgumentNullException(NameOf(other))
            If other.m_rows <> m_columns Then
                Throw New ArgumentException(
                    $"inner dimensions do not agree: [{m_rows}x{m_columns}] · [{other.m_rows}x{other.m_columns}]", NameOf(other))
            End If

            Dim outRows As Integer = m_rows
            Dim outCols As Integer = other.m_columns
            Dim indptr As Integer() = New Integer(outRows) {}
            Dim acc As Double() = New Double(outCols - 1) {}
            Dim marker As Integer() = New Integer(outCols - 1) {}

            For j As Integer = 0 To outCols - 1
                marker(j) = -1
            Next

            Dim outIndices As New List(Of Integer)(1024)
            Dim outValues As New List(Of Double)(1024)
            Dim rowCols As New List(Of Integer)(256)

            For i As Integer = 0 To outRows - 1
                Call rowCols.Clear()

                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    Dim aVal As Double = m_values(k)
                    Dim inner As Integer = m_indices(k)

                    For k2 As Integer = other.m_indptr(inner) To other.m_indptr(inner + 1) - 1
                        Dim col As Integer = other.m_indices(k2)

                        If marker(col) <> i Then
                            marker(col) = i
                            acc(col) = aVal * other.m_values(k2)
                            Call rowCols.Add(col)
                        Else
                            acc(col) += aVal * other.m_values(k2)
                        End If
                    Next
                Next

                ' 输出必须保持每行列下标升序，便于下游二分与切片
                Call rowCols.Sort()

                For Each col As Integer In rowCols
                    Call outIndices.Add(col)
                    Call outValues.Add(acc(col))
                Next

                indptr(i + 1) = outIndices.Count
            Next

            Return New CsrMatrix(outRows, outCols, indptr, outIndices.ToArray, outValues.ToArray)
        End Function

        ''' <summary>
        ''' 乘以标量
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Function ScalarMultiply(s As Double) As CsrMatrix
            Dim out As Double() = New Double(m_values.Length - 1) {}

            For k As Integer = 0 To m_values.Length - 1
                out(k) = m_values(k) * s
            Next

            Return New CsrMatrix(m_rows, m_columns, DirectCast(m_indptr.Clone(), Integer()), m_indices, out)
        End Function

        ''' <summary>
        ''' 按行缩放：<c>A(i, j) = A(i, j) * factors(i)</c>
        ''' </summary>
        ''' <param name="factors">长度必须等于 <see cref="Rows"/></param>
        ''' <returns></returns>
        Public Function ScaleRows(factors As Double()) As CsrMatrix
            If factors Is Nothing Then Throw New ArgumentNullException(NameOf(factors))
            If factors.Length <> m_rows Then
                Throw New ArgumentException($"factor length {factors.Length} does not match the row dimension {m_rows}", NameOf(factors))
            End If

            Dim out As Double() = New Double(m_values.Length - 1) {}

            For i As Integer = 0 To m_rows - 1
                Dim f As Double = factors(i)

                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    out(k) = m_values(k) * f
                Next
            Next

            Return New CsrMatrix(m_rows, m_columns, DirectCast(m_indptr.Clone(), Integer()), m_indices, out)
        End Function

        ''' <summary>
        ''' 按列缩放：<c>A(i, j) = A(i, j) * factors(j)</c>
        ''' </summary>
        ''' <param name="factors">长度必须等于 <see cref="Columns"/></param>
        ''' <returns></returns>
        Public Function ScaleColumns(factors As Double()) As CsrMatrix
            If factors Is Nothing Then Throw New ArgumentNullException(NameOf(factors))
            If factors.Length <> m_columns Then
                Throw New ArgumentException(
                    $"factor length {factors.Length} does not match the column dimension {m_columns}", NameOf(factors))
            End If

            Dim out As Double() = New Double(m_values.Length - 1) {}

            For k As Integer = 0 To m_values.Length - 1
                out(k) = m_values(k) * factors(m_indices(k))
            Next

            Return New CsrMatrix(m_rows, m_columns, DirectCast(m_indptr.Clone(), Integer()), m_indices, out)
        End Function

        ''' <summary>
        ''' 转置，返回按列压缩的等价矩阵（<c>O(nnz)</c>，不做稠密化）
        ''' </summary>
        ''' <returns></returns>
        Public Function ToCsc() As CscMatrix
            Dim indptr As Integer() = New Integer(m_columns) {}
            Dim indices As Integer() = New Integer(m_values.Length - 1) {}
            Dim values As Double() = New Double(m_values.Length - 1) {}

            ' 计数排序：先统计每一列的非零元个数，再前缀和定位
            For k As Integer = 0 To m_values.Length - 1
                indptr(m_indices(k) + 1) += 1
            Next
            For j As Integer = 0 To m_columns - 1
                indptr(j + 1) += indptr(j)
            Next

            Dim cursor As Integer() = DirectCast(indptr.Clone(), Integer())

            For i As Integer = 0 To m_rows - 1
                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    Dim col As Integer = m_indices(k)
                    Dim pos As Integer = cursor(col)

                    indices(pos) = i
                    values(pos) = m_values(k)
                    cursor(col) = pos + 1
                Next
            Next

            Return New CscMatrix(m_columns, m_rows, indptr, indices, values)
        End Function

#End Region

#Region "Sub-matrix and stacking"

        ''' <summary>
        ''' 按给定的行下标顺序抽取行子集（行可以重复、可以乱序）
        ''' </summary>
        ''' <param name="rows">目标行下标序列</param>
        ''' <returns></returns>
        Public Function Slice(rows As IEnumerable(Of Integer)) As CsrMatrix
            If rows Is Nothing Then Throw New ArgumentNullException(NameOf(rows))

            Dim take As Integer() = rows.ToArray()
            Dim indptr As Integer() = New Integer(take.Length) {}
            Dim indices As Integer() = New Integer(m_values.Length - 1) {}
            Dim values As Double() = New Double(m_values.Length - 1) {}
            Dim n As Integer = 0

            For r As Integer = 0 To take.Length - 1
                Dim row As Integer = take(r)
                CheckRow(row)

                For k As Integer = m_indptr(row) To m_indptr(row + 1) - 1
                    indices(n) = m_indices(k)
                    values(n) = m_values(k)
                    n += 1
                Next

                indptr(r + 1) = n
            Next

            If n <> indices.Length Then
                Call Array.Resize(indices, n)
                Call Array.Resize(values, n)
            End If

            Return New CsrMatrix(take.Length, m_columns, indptr, indices, values)
        End Function

        ''' <summary>
        ''' 抽取子矩阵（行下标 × 列下标，列闭区间）
        ''' </summary>
        ''' <param name="rows">目标行下标序列</param>
        ''' <param name="j0">起始列（含）</param>
        ''' <param name="j1">结束列（含）</param>
        ''' <returns></returns>
        Public Function Slice(rows As Integer(), j0 As Integer, j1 As Integer) As CsrMatrix
            If rows Is Nothing Then Throw New ArgumentNullException(NameOf(rows))
            If j0 < 0 OrElse j1 >= m_columns OrElse j0 > j1 Then
                Throw New ArgumentException($"invalid column range [{j0}, {j1}] for a matrix with {m_columns} column(s)")
            End If

            Dim indptr As Integer() = New Integer(rows.Length) {}
            Dim indices As New List(Of Integer)(1024)
            Dim values As New List(Of Double)(1024)

            For r As Integer = 0 To rows.Length - 1
                Dim row As Integer = rows(r)
                CheckRow(row)

                For k As Integer = m_indptr(row) To m_indptr(row + 1) - 1
                    Dim col As Integer = m_indices(k)

                    If col >= j0 AndAlso col <= j1 Then
                        Call indices.Add(col - j0)
                        Call values.Add(m_values(k))
                    End If
                Next

                indptr(r + 1) = indices.Count
            Next

            Return New CsrMatrix(rows.Length, j1 - j0 + 1, indptr, indices.ToArray, values.ToArray)
        End Function

        ''' <summary>
        ''' 列投影：按给定顺序保留列（重复列只保留首次出现）
        ''' </summary>
        ''' <param name="columns">目标列下标序列</param>
        ''' <returns></returns>
        Public Function SliceColumns(columns As IEnumerable(Of Integer)) As CsrMatrix
            If columns Is Nothing Then Throw New ArgumentNullException(NameOf(columns))

            Dim map As New Dictionary(Of Integer, Integer)
            Dim keep As New List(Of Integer)

            For Each col As Integer In columns
                CheckColumn(col)

                If Not map.ContainsKey(col) Then
                    map(col) = keep.Count
                    Call keep.Add(col)
                End If
            Next

            Dim outCols As Integer = keep.Count
            Dim indptr As Integer() = New Integer(m_rows) {}
            Dim indices As New List(Of Integer)(m_values.Length)
            Dim values As New List(Of Double)(m_values.Length)

            For i As Integer = 0 To m_rows - 1
                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    Dim target As Integer

                    If map.TryGetValue(m_indices(k), target) Then
                        Call indices.Add(target)
                        Call values.Add(m_values(k))
                    End If
                Next

                ' 目标列顺序由调用方给定，逐行需要重新按新列号升序排列
                SortLastRow(indices, values, indptr(i), indices.Count - 1)
                indptr(i + 1) = indices.Count
            Next

            Return New CsrMatrix(m_rows, outCols, indptr, indices.ToArray, values.ToArray)
        End Function

        ''' <summary>
        ''' 纵向堆叠多个稀疏矩阵（行数相加，列数必须一致）
        ''' </summary>
        ''' <param name="parts">按从上到下的顺序</param>
        ''' <returns></returns>
        Public Shared Function Stack(parts As IEnumerable(Of CsrMatrix)) As CsrMatrix
            If parts Is Nothing Then Throw New ArgumentNullException(NameOf(parts))

            Dim blocks As CsrMatrix() = parts.ToArray()

            If blocks.Length = 0 Then Return Empty()

            Dim columns As Integer = blocks(0).m_columns
            Dim rows As Integer = 0
            Dim nnz As Long = 0

            For i As Integer = 0 To blocks.Length - 1
                If blocks(i) Is Nothing Then Throw New ArgumentException($"stack block #{i} is nothing", NameOf(parts))
                If blocks(i).m_columns <> columns Then
                    Throw New ArgumentException(
                        $"stack block #{i} has {blocks(i).m_columns} column(s) but {columns} is required", NameOf(parts))
                End If

                rows += blocks(i).m_rows
                nnz += blocks(i).NonZeros
            Next

            If nnz > Integer.MaxValue Then
                Throw New ArgumentException($"stacked matrix has {nnz} non-zero entries which exceeds the Int32 index capacity")
            End If

            Dim indptr As Integer() = New Integer(rows) {}
            Dim indices As Integer() = New Integer(CInt(nnz) - 1) {}
            Dim values As Double() = New Double(CInt(nnz) - 1) {}
            Dim rowOffset As Integer = 0
            Dim n As Integer = 0

            For i As Integer = 0 To blocks.Length - 1
                Dim block As CsrMatrix = blocks(i)

                Call Array.ConstrainedCopy(block.m_indices, 0, indices, n, block.m_indices.Length)
                Call Array.ConstrainedCopy(block.m_values, 0, values, n, block.m_values.Length)

                For r As Integer = 0 To block.m_rows - 1
                    indptr(rowOffset + r + 1) = n + block.m_indptr(r + 1)
                Next

                n += block.m_values.Length
                rowOffset += block.m_rows
            Next

            Return New CsrMatrix(rows, columns, indptr, indices, values)
        End Function

        ''' <summary>
        ''' 在末尾追加行（等价于 <c>Stack({Me, other})</c>，但省掉一次数组分配）
        ''' </summary>
        ''' <param name="other"></param>
        ''' <returns></returns>
        Public Function Append(other As CsrMatrix) As CsrMatrix
            Return Stack({Me, other})
        End Function

#End Region

#Region "Cleaning and conversion"

        ''' <summary>
        ''' 去掉绝对值不超过阈值的元素（默认只去掉精确的 0）
        ''' </summary>
        ''' <param name="threshold">保留条件为 <c>|v| &gt; threshold</c></param>
        ''' <returns></returns>
        Public Function EliminateZeros(Optional threshold As Double = 0.0) As CsrMatrix
            Dim indptr As Integer() = New Integer(m_rows) {}
            Dim indices As New List(Of Integer)(m_values.Length)
            Dim values As New List(Of Double)(m_values.Length)

            For i As Integer = 0 To m_rows - 1
                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    If std.Abs(m_values(k)) > threshold Then
                        Call indices.Add(m_indices(k))
                        Call values.Add(m_values(k))
                    End If
                Next

                indptr(i + 1) = indices.Count
            Next

            If indices.Count = m_values.Length Then
                ' 没有元素被移除，直接复用原结构
                Return Me
            End If

            Return New CsrMatrix(m_rows, m_columns, indptr, indices.ToArray, values.ToArray)
        End Function

        ''' <summary>
        ''' 调整尺寸：保留左上角 <c>min(rows, M) × min(columns, N)</c> 区域内的元素
        ''' </summary>
        ''' <param name="M">新行数</param>
        ''' <param name="N">新列数</param>
        ''' <returns>新实例（不修改当前实例）</returns>
        Public Function Resize(M As Integer, N As Integer) As GeneralMatrix Implements GeneralMatrix.Resize
            If M < 0 OrElse N < 0 Then
                Throw New ArgumentException($"resize dimensions must not be negative: [{M}, {N}]")
            End If

            Dim rowsToCopy As Integer = std.Min(M, m_rows)
            Dim indptr As Integer() = New Integer(M) {}
            Dim indices As New List(Of Integer)(m_values.Length)
            Dim values As New List(Of Double)(m_values.Length)

            For i As Integer = 0 To rowsToCopy - 1
                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    If m_indices(k) < N Then
                        Call indices.Add(m_indices(k))
                        Call values.Add(m_values(k))
                    End If
                Next

                indptr(i + 1) = indices.Count
            Next

            For i As Integer = rowsToCopy To M - 1
                indptr(i + 1) = indices.Count
            Next

            Return New CsrMatrix(M, N, indptr, indices.ToArray, values.ToArray)
        End Function

        ''' <summary>
        ''' 转换为一维锯齿数组（稠密）
        ''' </summary>
        ''' <param name="deepcopy">本方法总是返回新分配的内存，该参数仅为接口兼容保留</param>
        ''' <returns></returns>
        Public Function ArrayPack(Optional deepcopy As Boolean = False) As Double()() Implements GeneralMatrix.ArrayPack
            Return ToDense()
        End Function

        ''' <summary>
        ''' 转换为一维锯齿数组（稠密，行优先）
        ''' </summary>
        ''' <returns></returns>
        Public Function ToDense() As Double()()
            Dim dense As Double()() = RectangularArray.Matrix(Of Double)(m_rows, m_columns)

            For i As Integer = 0 To m_rows - 1
                Dim row As Double() = dense(i)

                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    row(m_indices(k)) = m_values(k)
                Next
            Next

            Return dense
        End Function

        ''' <summary>
        ''' 逐行枚举（每行是一个独立的稠密向量，惰性生成）
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function RowVectors() As IEnumerable(Of Vector) Implements GeneralMatrix.RowVectors
            For i As Integer = 0 To m_rows - 1
                Yield New Vector(Row(i))
            Next
        End Function

        ''' <summary>
        ''' 抽取子矩阵（行下标 × 列闭区间）
        ''' </summary>
        ''' <param name="r">行下标数组</param>
        ''' <param name="j0">起始列（含）</param>
        ''' <param name="j1">结束列（含）</param>
        ''' <returns></returns>
        Public Function GetMatrix(r As Integer(), j0 As Integer, j1 As Integer) As GeneralMatrix Implements GeneralMatrix.GetMatrix
            Return Slice(r, j0, j1)
        End Function

        ''' <summary>
        ''' 矩阵乘法；右矩阵为 <see cref="CsrMatrix"/> 时保持稀疏，否则退化为稠密乘法
        ''' </summary>
        ''' <param name="m2"></param>
        ''' <returns></returns>
        Public Function Dot(m2 As GeneralMatrix) As GeneralMatrix Implements GeneralMatrix.Dot
            If m2 Is Nothing Then Throw New ArgumentNullException(NameOf(m2))
            If m2.RowDimension <> m_columns Then
                Throw New ArgumentException(
                    $"inner dimensions do not agree: [{m_rows}x{m_columns}] · [{m2.RowDimension}x{m2.ColumnDimension}]", NameOf(m2))
            End If

            If TypeOf m2 Is CsrMatrix Then
                Return Multiply(DirectCast(m2, CsrMatrix))
            End If

            ' 稠密右矩阵：逐行展开，避免把左矩阵整体稠密化
            Dim right As Double()() = m2.ArrayPack()

            If right.Length <> m_columns Then
                Throw New ArgumentException(
                    $"inner dimensions do not agree: [{m_rows}x{m_columns}] · [{right.Length}x{m2.ColumnDimension}]", NameOf(m2))
            End If

            Dim n As Integer = m2.ColumnDimension
            Dim out As Double()() = RectangularArray.Matrix(Of Double)(m_rows, n)

            For i As Integer = 0 To m_rows - 1
                Dim acc As Double() = out(i)

                For k As Integer = m_indptr(i) To m_indptr(i + 1) - 1
                    Dim aVal As Double = m_values(k)
                    Dim bRow As Double() = right(m_indices(k))

                    For j As Integer = 0 To n - 1
                        acc(j) += aVal * bRow(j)
                    Next
                Next
            Next

            Return New NumericMatrix(out)
        End Function

        ''' <summary>
        ''' 深拷贝（数组整体复制）
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As CsrMatrix
            Return New CsrMatrix(
                m_rows,
                m_columns,
                DirectCast(m_indptr.Clone(), Integer()),
                DirectCast(m_indices.Clone(), Integer()),
                DirectCast(m_values.Clone(), Double()))
        End Function

        Public Overrides Function ToString() As String
            Return $"[{m_rows}x{m_columns}] csr, nnz={m_values.Length}, density={DensityText()}"
        End Function

        Private Function DensityText() As String
            Dim total As Long = CLng(m_rows) * CLng(m_columns)

            If total = 0 Then Return "0"

            Return (m_values.Length / total).ToString("G4")
        End Function

#End Region

#Region "Validation helpers"

        Private Sub CheckRow(row As Integer)
            If row < 0 OrElse row >= m_rows Then
                Throw New ArgumentOutOfRangeException(NameOf(row), row, $"row index must be within [0, {m_rows})")
            End If
        End Sub

        Private Sub CheckColumn(column As Integer)
            If column < 0 OrElse column >= m_columns Then
                Throw New ArgumentOutOfRangeException(NameOf(column), column, $"column index must be within [0, {m_columns})")
            End If
        End Sub

        ''' <summary>
        ''' 对一段"行内区间"按列下标升序做原地排序（列投影后恢复 CSR 的升序不变式）
        ''' </summary>
        Private Shared Sub SortLastRow(indices As List(Of Integer), values As List(Of Double), from As Integer, [to] As Integer)
            Dim n As Integer = [to] - from + 1

            If n <= 1 Then Return

            Dim cols As Integer() = New Integer(n - 1) {}
            Dim vals As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                cols(i) = indices(from + i)
                vals(i) = values(from + i)
            Next

            Call Array.Sort(cols, vals)

            For i As Integer = 0 To n - 1
                indices(from + i) = cols(i)
                values(from + i) = vals(i)
            Next
        End Sub

#End Region

    End Class
End Namespace
