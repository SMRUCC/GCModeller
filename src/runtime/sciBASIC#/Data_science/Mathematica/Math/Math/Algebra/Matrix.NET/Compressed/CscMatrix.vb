Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports sysmath = System.Math

Namespace LinearAlgebra.Matrix.Compressed

    ''' <summary>
    ''' 压缩列存储（Compressed Sparse Column, CSC）稀疏矩阵。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 内部由三个平行数组构成：<c>indptr</c>（长度 <c>columns + 1</c>）、<c>indices</c>（行下标）、
    ''' <c>values</c>（元素值）。第 <c>j</c> 列的非零元素位于
    ''' <c>indices(indptr(j)) .. indices(indptr(j + 1) - 1)</c>，且同一列内的行下标严格升序。
    ''' </para>
    ''' <para>
    ''' 这是 <see cref="CsrMatrix.ToCsc"/> 的返回类型，也是"按列访问"算子的载体
    ''' （例如 GLM 类算法按列取特征、按列点积）。需要按行扫描时请先 <see cref="ToCsr"/>
    ''' —— 两者互转都是 <c>O(nnz)</c> 的计数排序，不做稠密化。
    ''' </para>
    ''' </remarks>
    Public Class CscMatrix
        Implements GeneralMatrix

#Region "Class variables"

        Private ReadOnly m_indptr As Integer()
        Private ReadOnly m_indices As Integer()
        Private ReadOnly m_values As Double()
        Private ReadOnly m_rows As Integer
        Private ReadOnly m_columns As Integer

#End Region

#Region "Constructor"

        ''' <summary>
        ''' 以已有的压缩结构直接构造（不做任何拷贝）
        ''' </summary>
        ''' <param name="rows">行数</param>
        ''' <param name="columns">列数</param>
        ''' <param name="indptr">列指针，长度必须等于 <paramref name="columns"/> + 1</param>
        ''' <param name="indices">行下标，必须落在 <c>[0, rows)</c> 且每列内升序</param>
        ''' <param name="values">元素值</param>
        ''' <exception cref="ArgumentException">压缩结构不合法</exception>
        Public Sub New(rows As Integer, columns As Integer, indptr As Integer(), indices As Integer(), values As Double())
            If rows < 0 Then Throw New ArgumentException($"row dimension must not be negative: {rows}", NameOf(rows))
            If columns < 0 Then Throw New ArgumentException($"column dimension must not be negative: {columns}", NameOf(columns))
            If indptr Is Nothing Then Throw New ArgumentNullException(NameOf(indptr))
            If indices Is Nothing Then Throw New ArgumentNullException(NameOf(indices))
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
            If indptr.Length <> columns + 1 Then
                Throw New ArgumentException(
                    $"indptr length {indptr.Length} does not match columns + 1 = {columns + 1}", NameOf(indptr))
            End If
            If indices.Length <> values.Length Then
                Throw New ArgumentException($"indices length {indices.Length} does not match values length {values.Length}", NameOf(values))
            End If
            If indptr(0) <> 0 Then
                Throw New ArgumentException($"indptr(0) must be zero but is {indptr(0)}", NameOf(indptr))
            End If
            If indptr(columns) <> indices.Length Then
                Throw New ArgumentException(
                    $"indptr(last) = {indptr(columns)} does not match the non-zero count {indices.Length}", NameOf(indptr))
            End If

            For j As Integer = 0 To columns - 1
                If indptr(j + 1) < indptr(j) Then
                    Throw New ArgumentException($"indptr is not monotone at column {j}: {indptr(j)} -> {indptr(j + 1)}", NameOf(indptr))
                End If
            Next
            For k As Integer = 0 To indices.Length - 1
                Dim row As Integer = indices(k)

                If row < 0 OrElse row >= rows Then
                    Throw New ArgumentException(
                        $"row index {row} at non-zero entry #{k} is out of range [0, {rows})", NameOf(indices))
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

        Public ReadOnly Property Rows As Integer
            Get
                Return m_rows
            End Get
        End Property

        Public ReadOnly Property Columns As Integer
            Get
                Return m_columns
            End Get
        End Property

        Public ReadOnly Property NonZeros As Long
            Get
                Return CLng(m_values.Length)
            End Get
        End Property

        ''' <summary>
        ''' 列指针数组（长度 = <see cref="Columns"/> + 1）。**调用方不得修改**。
        ''' </summary>
        Public ReadOnly Property Indptr As Integer()
            Get
                Return m_indptr
            End Get
        End Property

        ''' <summary>
        ''' 行下标数组（长度 = <see cref="NonZeros"/>），每列内升序。**调用方不得修改**。
        ''' </summary>
        Public ReadOnly Property Indices As Integer()
            Get
                Return m_indices
            End Get
        End Property

        ''' <summary>
        ''' 元素值数组（长度 = <see cref="NonZeros"/>）。**调用方不得修改**。
        ''' </summary>
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

        Public Shared Function Empty(Optional rows As Integer = 0, Optional columns As Integer = 0) As CscMatrix
            Return New CscMatrix(rows, columns, New Integer(columns) {}, New Integer() {}, New Double() {})
        End Function

#End Region

#Region "Element access"

        ''' <summary>
        ''' 在指定列内二分定位行下标
        ''' </summary>
        ''' <param name="column">列下标</param>
        ''' <param name="row">行下标</param>
        ''' <returns>非零元在 <see cref="Indices"/> 中的位置；不存在返回 -1</returns>
        Public Function IndexOfInColumn(column As Integer, row As Integer) As Integer
            CheckColumn(column)
            CheckRow(row)

            Dim s As Integer = m_indptr(column)
            Dim e As Integer = m_indptr(column + 1) - 1

            Do While s <= e
                Dim mid As Integer = (s + e) \ 2
                Dim r As Integer = m_indices(mid)

                If r = row Then
                    Return mid
                ElseIf r < row Then
                    s = mid + 1
                Else
                    e = mid - 1
                End If
            Loop

            Return -1
        End Function

        Public Function ValueOf(row As Integer, column As Integer) As Double
            Dim i As Integer = IndexOfInColumn(column, row)

            If i < 0 Then
                Return 0.0
            Else
                Return m_values(i)
            End If
        End Function

        Default Public Property X(i As Integer, j As Integer) As Double Implements GeneralMatrix.X
            Get
                Return ValueOf(i, j)
            End Get
            Set(value As Double)
                Throw New NotSupportedException(
                    "CscMatrix is an immutable container: assign through a builder or ToDense() instead.")
            End Set
        End Property

        Default Public Property X(i As Integer, Optional byRow As Boolean = True) As Vector Implements GeneralMatrix.X
            Get
                If byRow Then
                    Return New Vector(Row(i))
                Else
                    Return New Vector(Column(i))
                End If
            End Get
            Set(value As Vector)
                Throw New NotSupportedException(
                    "CscMatrix is an immutable container: assign through a builder or ToDense() instead.")
            End Set
        End Property

        Default Public ReadOnly Property X(indices As IEnumerable(Of Integer)) As GeneralMatrix Implements GeneralMatrix.X
            Get
                Return SliceColumns(indices)
            End Get
        End Property

        Default Public ReadOnly Property X(rowIdx As BooleanVector) As GeneralMatrix Implements GeneralMatrix.X
            Get
                If rowIdx Is Nothing Then Throw New ArgumentNullException(NameOf(rowIdx))

                ' 行掩码在 CSC 下需要重建行指针，走一次 CSR 转换最省事且只多一次 O(nnz) 扫描
                Return ToCsr().X(rowIdx)
            End Get
        End Property

        ''' <summary>
        ''' 抽取一整列（稠密）
        ''' </summary>
        ''' <param name="columnIndex">列下标</param>
        ''' <returns>长度 = <see cref="Rows"/></returns>
        Public Function Column(columnIndex As Integer) As Double()
            CheckColumn(columnIndex)

            Dim out As Double() = New Double(m_rows - 1) {}

            For k As Integer = m_indptr(columnIndex) To m_indptr(columnIndex + 1) - 1
                out(m_indices(k)) = m_values(k)
            Next

            Return out
        End Function

        ''' <summary>
        ''' 抽取一整行（稠密）
        ''' </summary>
        ''' <param name="rowIndex">行下标</param>
        ''' <returns>长度 = <see cref="Columns"/></returns>
        ''' <remarks>复杂度 <c>O(nnz)</c>；需要反复按行访问时请先 <see cref="ToCsr"/></remarks>
        Public Function Row(rowIndex As Integer) As Double()
            CheckRow(rowIndex)

            Dim out As Double() = New Double(m_columns - 1) {}

            For j As Integer = 0 To m_columns - 1
                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    If m_indices(k) = rowIndex Then
                        out(j) = m_values(k)
                        Exit For
                    ElseIf m_indices(k) > rowIndex Then
                        Exit For
                    End If
                Next
            Next

            Return out
        End Function

        ''' <summary>
        ''' 抽取一列的非零元（行下标与值成对返回，按行升序）
        ''' </summary>
        ''' <param name="column">列下标</param>
        ''' <returns></returns>
        Public Function ColumnNonZero(column As Integer) As (rows As Integer(), values As Double())
            CheckColumn(column)

            Dim s As Integer = m_indptr(column)
            Dim n As Integer = m_indptr(column + 1) - s
            Dim rows As Integer() = New Integer(n - 1) {}
            Dim vals As Double() = New Double(n - 1) {}

            Call Array.ConstrainedCopy(m_indices, s, rows, 0, n)
            Call Array.ConstrainedCopy(m_values, s, vals, 0, n)

            Return (rows, vals)
        End Function

#End Region

#Region "Row and column statistics"

        ''' <summary>
        ''' 每一列的和
        ''' </summary>
        ''' <returns>长度 = <see cref="Columns"/></returns>
        Public Function ColumnSums() As Double()
            Dim out As Double() = New Double(m_columns - 1) {}

            For j As Integer = 0 To m_columns - 1
                Dim sum As Double = 0.0

                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    sum += m_values(k)
                Next

                out(j) = sum
            Next

            Return out
        End Function

        ''' <summary>
        ''' 每一行的和
        ''' </summary>
        ''' <returns>长度 = <see cref="Rows"/></returns>
        Public Function RowSums() As Double()
            Dim out As Double() = New Double(m_rows - 1) {}

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
                sum += sysmath.Abs(m_values(k))
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

            For j As Integer = 0 To m_columns - 1
                Dim xj As Double = x(j)

                If xj = 0.0 Then Continue For

                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    out(m_indices(k)) += m_values(k) * xj
                Next
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

            For j As Integer = 0 To m_columns - 1
                Dim sum As Double = 0.0

                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    sum += m_values(k) * y(m_indices(k))
                Next

                out(j) = sum
            Next

            Return out
        End Function

        ''' <summary>
        ''' 乘以标量
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Function ScalarMultiply(s As Double) As CscMatrix
            Dim out As Double() = New Double(m_values.Length - 1) {}

            For k As Integer = 0 To m_values.Length - 1
                out(k) = m_values(k) * s
            Next

            Return New CscMatrix(m_rows, m_columns, DirectCast(m_indptr.Clone(), Integer()), m_indices, out)
        End Function

        ''' <summary>
        ''' 按列缩放：<c>A(i, j) = A(i, j) * factors(j)</c>
        ''' </summary>
        ''' <param name="factors">长度必须等于 <see cref="Columns"/></param>
        ''' <returns></returns>
        Public Function ScaleColumns(factors As Double()) As CscMatrix
            If factors Is Nothing Then Throw New ArgumentNullException(NameOf(factors))
            If factors.Length <> m_columns Then
                Throw New ArgumentException(
                    $"factor length {factors.Length} does not match the column dimension {m_columns}", NameOf(factors))
            End If

            Dim out As Double() = New Double(m_values.Length - 1) {}

            For j As Integer = 0 To m_columns - 1
                Dim f As Double = factors(j)

                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    out(k) = m_values(k) * f
                Next
            Next

            Return New CscMatrix(m_rows, m_columns, DirectCast(m_indptr.Clone(), Integer()), m_indices, out)
        End Function

        ''' <summary>
        ''' 按行缩放：<c>A(i, j) = A(i, j) * factors(i)</c>
        ''' </summary>
        ''' <param name="factors">长度必须等于 <see cref="Rows"/></param>
        ''' <returns></returns>
        Public Function ScaleRows(factors As Double()) As CscMatrix
            If factors Is Nothing Then Throw New ArgumentNullException(NameOf(factors))
            If factors.Length <> m_rows Then
                Throw New ArgumentException($"factor length {factors.Length} does not match the row dimension {m_rows}", NameOf(factors))
            End If

            Dim out As Double() = New Double(m_values.Length - 1) {}

            For k As Integer = 0 To m_values.Length - 1
                out(k) = m_values(k) * factors(m_indices(k))
            Next

            Return New CscMatrix(m_rows, m_columns, DirectCast(m_indptr.Clone(), Integer()), m_indices, out)
        End Function

        ''' <summary>
        ''' 转为按行压缩的等价矩阵（<c>O(nnz)</c> 计数排序）
        ''' </summary>
        ''' <returns></returns>
        Public Function ToCsr() As CsrMatrix
            Dim indptr As Integer() = New Integer(m_rows) {}
            Dim indices As Integer() = New Integer(m_values.Length - 1) {}
            Dim values As Double() = New Double(m_values.Length - 1) {}

            For k As Integer = 0 To m_values.Length - 1
                indptr(m_indices(k) + 1) += 1
            Next
            For i As Integer = 0 To m_rows - 1
                indptr(i + 1) += indptr(i)
            Next

            Dim cursor As Integer() = DirectCast(indptr.Clone(), Integer())

            For j As Integer = 0 To m_columns - 1
                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    Dim row As Integer = m_indices(k)
                    Dim pos As Integer = cursor(row)

                    indices(pos) = j
                    values(pos) = m_values(k)
                    cursor(row) = pos + 1
                Next
            Next

            Return New CsrMatrix(m_rows, m_columns, indptr, indices, values)
        End Function

#End Region

#Region "Sub-matrix and stacking"

        ''' <summary>
        ''' 按给定顺序保留列（重复列只保留首次出现）
        ''' </summary>
        ''' <param name="columns">目标列下标序列</param>
        ''' <returns></returns>
        Public Function SliceColumns(columns As IEnumerable(Of Integer)) As CscMatrix
            If columns Is Nothing Then Throw New ArgumentNullException(NameOf(columns))

            Dim keep As New List(Of Integer)
            Dim seen As New HashSet(Of Integer)

            For Each col As Integer In columns
                CheckColumn(col)

                If seen.Add(col) Then Call keep.Add(col)
            Next

            Dim indptr As Integer() = New Integer(keep.Count) {}
            Dim indices As New List(Of Integer)(m_values.Length)
            Dim values As New List(Of Double)(m_values.Length)

            For t As Integer = 0 To keep.Count - 1
                Dim src As Integer = keep(t)

                For k As Integer = m_indptr(src) To m_indptr(src + 1) - 1
                    Call indices.Add(m_indices(k))
                    Call values.Add(m_values(k))
                Next

                indptr(t + 1) = indices.Count
            Next

            Return New CscMatrix(m_rows, keep.Count, indptr, indices.ToArray, values.ToArray)
        End Function

        ''' <summary>
        ''' 纵向堆叠多个矩阵（行数相加，列数必须一致）
        ''' </summary>
        ''' <param name="parts">按从上到下的顺序</param>
        ''' <returns></returns>
        Public Shared Function Stack(parts As IEnumerable(Of CscMatrix)) As CscMatrix
            If parts Is Nothing Then Throw New ArgumentNullException(NameOf(parts))

            Dim blocks As CscMatrix() = parts.ToArray()

            If blocks.Length = 0 Then Return Empty()

            Dim csr As CsrMatrix() = New CsrMatrix(blocks.Length - 1) {}

            For i As Integer = 0 To blocks.Length - 1
                If blocks(i) Is Nothing Then Throw New ArgumentException($"stack block #{i} is nothing", NameOf(parts))
                csr(i) = blocks(i).ToCsr()
            Next

            Return CsrMatrix.Stack(csr).ToCsc()
        End Function

#End Region

#Region "Cleaning and conversion"

        ''' <summary>
        ''' 去掉绝对值不超过阈值的元素
        ''' </summary>
        ''' <param name="threshold">保留条件为 <c>|v| &gt; threshold</c></param>
        ''' <returns></returns>
        Public Function EliminateZeros(Optional threshold As Double = 0.0) As CscMatrix
            Dim indptr As Integer() = New Integer(m_columns) {}
            Dim indices As New List(Of Integer)(m_values.Length)
            Dim values As New List(Of Double)(m_values.Length)

            For j As Integer = 0 To m_columns - 1
                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    If sysmath.Abs(m_values(k)) > threshold Then
                        Call indices.Add(m_indices(k))
                        Call values.Add(m_values(k))
                    End If
                Next

                indptr(j + 1) = indices.Count
            Next

            If indices.Count = m_values.Length Then Return Me

            Return New CscMatrix(m_rows, m_columns, indptr, indices.ToArray, values.ToArray)
        End Function

        Public Function Resize(M As Integer, N As Integer) As GeneralMatrix Implements GeneralMatrix.Resize
            If M < 0 OrElse N < 0 Then
                Throw New ArgumentException($"resize dimensions must not be negative: [{M}, {N}]")
            End If

            Dim colsToCopy As Integer = sysmath.Min(N, m_columns)
            Dim indptr As Integer() = New Integer(N) {}
            Dim indices As New List(Of Integer)(m_values.Length)
            Dim values As New List(Of Double)(m_values.Length)

            For j As Integer = 0 To colsToCopy - 1
                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    If m_indices(k) < M Then
                        Call indices.Add(m_indices(k))
                        Call values.Add(m_values(k))
                    End If
                Next

                indptr(j + 1) = indices.Count
            Next

            For j As Integer = colsToCopy To N - 1
                indptr(j + 1) = indices.Count
            Next

            Return New CscMatrix(M, N, indptr, indices.ToArray, values.ToArray)
        End Function

        Public Function ArrayPack(Optional deepcopy As Boolean = False) As Double()() Implements GeneralMatrix.ArrayPack
            Return ToDense()
        End Function

        ''' <summary>
        ''' 转换为一维锯齿数组（稠密，行优先）
        ''' </summary>
        ''' <returns></returns>
        Public Function ToDense() As Double()()
            Dim dense As Double()() = RectangularArray.Matrix(Of Double)(m_rows, m_columns)

            For j As Integer = 0 To m_columns - 1
                For k As Integer = m_indptr(j) To m_indptr(j + 1) - 1
                    dense(m_indices(k))(j) = m_values(k)
                Next
            Next

            Return dense
        End Function

        Public Iterator Function RowVectors() As IEnumerable(Of Vector) Implements GeneralMatrix.RowVectors
            Dim csr As CsrMatrix = ToCsr()

            For i As Integer = 0 To m_rows - 1
                Yield New Vector(csr.Row(i))
            Next
        End Function

        Public Function GetMatrix(r As Integer(), j0 As Integer, j1 As Integer) As GeneralMatrix Implements GeneralMatrix.GetMatrix
            Return ToCsr().Slice(r, j0, j1)
        End Function

        ''' <summary>
        ''' 转置：返回转置矩阵的 CSR 形式（维度互换）。
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CSC 的第 <c>j</c> 列就是转置矩阵的第 <c>j</c> 行，且列内行下标已经升序，
        ''' 因此转置只是"把列指针当成行指针"——两个数据数组可以整体复用，<c>O(columns)</c>。
        ''' </remarks>
        Public Function Transpose() As GeneralMatrix Implements GeneralMatrix.Transpose
            Dim indptr As Integer() = New Integer(m_columns) {}
            Dim indices As Integer() = New Integer(m_values.Length - 1) {}
            Dim values As Double() = New Double(m_values.Length - 1) {}

            For j As Integer = 0 To m_columns - 1
                indptr(j + 1) = indptr(j) + (m_indptr(j + 1) - m_indptr(j))
            Next

            Call Array.ConstrainedCopy(m_indices, 0, indices, 0, m_indices.Length)
            Call Array.ConstrainedCopy(m_values, 0, values, 0, m_values.Length)

            Return New CsrMatrix(m_columns, m_rows, indptr, indices, values)
        End Function

        Public Function Dot(m2 As GeneralMatrix) As GeneralMatrix Implements GeneralMatrix.Dot
            Return ToCsr().Dot(m2)
        End Function

        Public Function Clone() As CscMatrix
            Return New CscMatrix(
                m_rows,
                m_columns,
                DirectCast(m_indptr.Clone(), Integer()),
                DirectCast(m_indices.Clone(), Integer()),
                DirectCast(m_values.Clone(), Double()))
        End Function

        Public Overrides Function ToString() As String
            Return $"[{m_rows}x{m_columns}] csc, nnz={m_values.Length}"
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

#End Region

    End Class
End Namespace
