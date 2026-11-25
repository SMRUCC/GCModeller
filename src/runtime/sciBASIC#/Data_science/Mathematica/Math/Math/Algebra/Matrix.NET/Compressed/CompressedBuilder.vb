Imports std = System.Math

Namespace LinearAlgebra.Matrix.Compressed

    ''' <summary>
    ''' 压缩稀疏矩阵的构建器：把稠密矩阵、三元组或分块数据转换为
    ''' <see cref="CsrMatrix"/> / <see cref="CscMatrix"/>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 所有构建路径都是"先计数、再填充"的两遍扫描：第一遍只统计每行的非零元个数，
    ''' 第二遍按前缀和定位写入，因此不需要 <c>List(Of T)</c> 的动态扩容，
    ''' 也不会产生中间稠密副本（<see cref="FromDense(Double()())"/> 的输入本身就是稠密的）。
    ''' </para>
    ''' <para>
    ''' 三元组入口会对重复坐标做<b>求和</b>并保持每行列下标升序；
    ''' 这与 scipy 的 <c>coo_matrix.tocsr()</c> 语义一致。
    ''' </para>
    ''' </remarks>
    Public NotInheritable Class CompressedBuilder

        Private Sub New()
        End Sub

        ''' <summary>
        ''' 由一维锯齿数组（行优先）构建 CSR
        ''' </summary>
        ''' <param name="dense">稠密矩阵，允许为 Nothing 表示空矩阵</param>
        ''' <returns></returns>
        Public Shared Function FromDense(dense As Double()()) As CsrMatrix
            If dense Is Nothing Then Return CsrMatrix.Empty()

            Dim rows As Integer = dense.Length
            Dim columns As Integer = If(rows = 0, 0, If(dense(0) Is Nothing, 0, dense(0).Length))
            Dim indptr As Integer() = New Integer(rows) {}
            Dim nnz As Integer = 0

            For i As Integer = 0 To rows - 1
                Dim row As Double() = dense(i)

                If row IsNot Nothing Then
                    For j As Integer = 0 To row.Length - 1
                        If row(j) <> 0.0 Then nnz += 1
                    Next
                End If

                indptr(i + 1) = nnz
            Next

            Dim indices As Integer() = New Integer(nnz - 1) {}
            Dim values As Double() = New Double(nnz - 1) {}
            Dim n As Integer = 0

            For i As Integer = 0 To rows - 1
                Dim row As Double() = dense(i)

                If row Is Nothing Then Continue For

                For j As Integer = 0 To row.Length - 1
                    If row(j) <> 0.0 Then
                        indices(n) = j
                        values(n) = row(j)
                        n += 1
                    End If
                Next
            Next

            Return New CsrMatrix(rows, columns, indptr, indices, values)
        End Function

        ''' <summary>
        ''' 由二维数组构建 CSR
        ''' </summary>
        ''' <param name="dense">稠密矩阵</param>
        ''' <returns></returns>
        Public Shared Function FromDense(dense As Double(,)) As CsrMatrix
            If dense Is Nothing Then Return CsrMatrix.Empty()

            Dim rows As Integer = dense.GetLength(0)
            Dim columns As Integer = dense.GetLength(1)
            Dim indptr As Integer() = New Integer(rows) {}
            Dim nnz As Integer = 0

            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To columns - 1
                    If dense(i, j) <> 0.0 Then nnz += 1
                Next

                indptr(i + 1) = nnz
            Next

            Dim indices As Integer() = New Integer(nnz - 1) {}
            Dim values As Double() = New Double(nnz - 1) {}
            Dim n As Integer = 0

            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To columns - 1
                    Dim v As Double = dense(i, j)

                    If v <> 0.0 Then
                        indices(n) = j
                        values(n) = v
                        n += 1
                    End If
                Next
            Next

            Return New CsrMatrix(rows, columns, indptr, indices, values)
        End Function

        ''' <summary>
        ''' 由任意 <see cref="GeneralMatrix"/> 构建 CSR
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <returns></returns>
        Public Shared Function FromGeneral(matrix As GeneralMatrix) As CsrMatrix
            If matrix Is Nothing Then Return CsrMatrix.Empty()
            If TypeOf matrix Is CsrMatrix Then Return DirectCast(matrix, CsrMatrix)

            Return FromDense(matrix.ArrayPack())
        End Function

        ''' <summary>
        ''' 由三元组（COO）构建 CSR；重复坐标按求和合并，每行列下标升序
        ''' </summary>
        ''' <param name="rows">行下标</param>
        ''' <param name="columns">列下标</param>
        ''' <param name="values">元素值</param>
        ''' <param name="rowCount">完整行数（未出现在 <paramref name="rows"/> 中的行按全零处理）</param>
        ''' <param name="columnCount">完整列数</param>
        ''' <returns></returns>
        ''' <exception cref="ArgumentException">三个数组长度不一致或下标越界</exception>
        Public Shared Function FromTriplets(rows As Integer(),
                                           columns As Integer(),
                                           values As Double(),
                                           rowCount As Integer,
                                           columnCount As Integer) As CsrMatrix
            If rows Is Nothing OrElse columns Is Nothing OrElse values Is Nothing Then
                Throw New ArgumentNullException(NameOf(rows), "triplet arrays must not be nothing")
            End If
            If rows.Length <> columns.Length OrElse rows.Length <> values.Length Then
                Throw New ArgumentException(
                    $"triplet arrays must have the same length: rows={rows.Length}, columns={columns.Length}, values={values.Length}")
            End If
            If rowCount < 0 OrElse columnCount < 0 Then
                Throw New ArgumentException($"matrix dimensions must not be negative: [{rowCount}, {columnCount}]")
            End If

            Dim indptr As Integer() = New Integer(rowCount) {}

            For k As Integer = 0 To rows.Length - 1
                Dim r As Integer = rows(k)

                If r < 0 OrElse r >= rowCount Then
                    Throw New ArgumentException($"row index {r} at entry #{k} is out of range [0, {rowCount})", NameOf(rows))
                End If
                If columns(k) < 0 OrElse columns(k) >= columnCount Then
                    Throw New ArgumentException(
                        $"column index {columns(k)} at entry #{k} is out of range [0, {columnCount})", NameOf(columns))
                End If

                indptr(r + 1) += 1
            Next

            For i As Integer = 0 To rowCount - 1
                indptr(i + 1) += indptr(i)
            Next

            Dim cursor As Integer() = DirectCast(indptr.Clone(), Integer())
            Dim indices As Integer() = New Integer(rows.Length - 1) {}
            Dim entries As Double() = New Double(rows.Length - 1) {}

            For k As Integer = 0 To rows.Length - 1
                Dim r As Integer = rows(k)
                Dim pos As Integer = cursor(r)

                indices(pos) = columns(k)
                entries(pos) = values(k)
                cursor(r) = pos + 1
            Next

            ' 每行排序并合并重复列（求和）
            Dim mergedColumns As New List(Of Integer)(rows.Length)
            Dim mergedValues As New List(Of Double)(rows.Length)
            Dim mergedIndptr As Integer() = New Integer(rowCount) {}

            For i As Integer = 0 To rowCount - 1
                Dim s As Integer = indptr(i)
                Dim n As Integer = indptr(i + 1) - s

                If n > 0 Then
                    Dim cols As Integer() = New Integer(n - 1) {}
                    Dim vals As Double() = New Double(n - 1) {}

                    Call Array.ConstrainedCopy(indices, s, cols, 0, n)
                    Call Array.ConstrainedCopy(entries, s, vals, 0, n)
                    Call Array.Sort(cols, vals)

                    Dim prev As Integer = cols(0)
                    Dim acc As Double = vals(0)

                    For t As Integer = 1 To n - 1
                        If cols(t) = prev Then
                            acc += vals(t)
                        Else
                            Call mergedColumns.Add(prev)
                            Call mergedValues.Add(acc)
                            prev = cols(t)
                            acc = vals(t)
                        End If
                    Next

                    Call mergedColumns.Add(prev)
                    Call mergedValues.Add(acc)
                End If

                mergedIndptr(i + 1) = mergedColumns.Count
            Next

            Return New CsrMatrix(rowCount, columnCount, mergedIndptr, mergedColumns.ToArray, mergedValues.ToArray)
        End Function

        ''' <summary>
        ''' 分块流式构建：把若干行块依次纵向拼接为一个 CSR。
        ''' </summary>
        ''' <param name="chunks">行块序列，每块的列数必须一致</param>
        ''' <param name="columnCount">可选：显式指定列数；缺省时取第一块的列数</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 用于"边读边转"的场景（例如按块读取单细胞计数矩阵）：调用方只需要提供一个
        ''' <c>IEnumerable(Of Double(,))</c>，本方法不会把所有块同时驻留在内存中。
        ''' </remarks>
        Public Shared Function FromChunks(chunks As IEnumerable(Of Double(,)), Optional columnCount As Integer = -1) As CsrMatrix
            If chunks Is Nothing Then Throw New ArgumentNullException(NameOf(chunks))

            Dim indptr As New List(Of Integer)(1024) From {0}
            Dim indices As New List(Of Integer)(65536)
            Dim values As New List(Of Double)(65536)
            Dim columns As Integer = columnCount
            Dim rows As Integer = 0

            For Each chunk As Double(,) In chunks
                If chunk Is Nothing Then Continue For

                Dim cn As Integer = chunk.GetLength(1)

                If columns < 0 Then
                    columns = cn
                ElseIf cn <> columns Then
                    Throw New ArgumentException($"chunk {rows} has {cn} column(s) but {columns} is required", NameOf(chunks))
                End If

                For i As Integer = 0 To chunk.GetLength(0) - 1
                    For j As Integer = 0 To cn - 1
                        Dim v As Double = chunk(i, j)

                        If v <> 0.0 Then
                            Call indices.Add(j)
                            Call values.Add(v)
                        End If
                    Next

                    Call indptr.Add(indices.Count)
                    rows += 1
                Next
            Next

            If columns < 0 Then columns = 0

            Return New CsrMatrix(rows, columns, indptr.ToArray, indices.ToArray, values.ToArray)
        End Function

    End Class
End Namespace
