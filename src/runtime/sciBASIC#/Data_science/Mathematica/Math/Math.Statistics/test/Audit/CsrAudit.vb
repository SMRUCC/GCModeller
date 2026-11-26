Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports sysmath = System.Math

''' <summary>
''' 压缩稀疏矩阵（CSR/CSC）的结构与算术验收测试：全部与稠密参考实现逐步对拍。
''' </summary>
Friend Module CsrAudit

    Private Const Section As String = "csr"

    ''' <summary>
    ''' 测试用稠密矩阵（4 行 × 5 列，含零与重复列位置）
    ''' </summary>
    Private ReadOnly Sample As Double()() = {
        New Double() {1.0, 0.0, 2.0, 0.0, 0.0},
        New Double() {0.0, 0.0, 0.0, 0.0, 0.0},
        New Double() {3.0, 4.0, 0.0, 5.0, 0.0},
        New Double() {0.0, 0.0, 6.0, 0.0, 7.0}
    }

    Friend Function Run() As Integer
        Dim failures As Integer = 0
        Dim dense As Double()() = Sample
        Dim matrix As CsrMatrix = CompressedBuilder.FromDense(dense)

        ' ---- 基本形状 ----
        failures += Assert("rows", matrix.Rows = 4, matrix.Rows.ToString)
        failures += Assert("columns", matrix.Columns = 5, matrix.Columns.ToString)
        failures += Assert("nnz", matrix.NonZeros = 7L, matrix.NonZeros.ToString)

        ' ---- 元素访问 ----
        For i As Integer = 0 To 3
            For j As Integer = 0 To 4
                failures += AssertClose($"X({i},{j})", matrix.X(i, j), dense(i)(j), 0.0, Section)
            Next
        Next

        ' ---- 行/列和 ----
        Dim rowSums As Double() = matrix.RowSums()
        Dim columnSums As Double() = matrix.ColumnSums()
        Dim rowExpected As Double() = {3.0, 0.0, 12.0, 13.0}
        Dim columnExpected As Double() = {4.0, 4.0, 8.0, 5.0, 7.0}

        For i As Integer = 0 To 3
            failures += AssertClose($"RowSums[{i}]", rowSums(i), rowExpected(i), 0.0, Section)
            failures += AssertClose($"RowSum({i})", matrix.RowSum(i), rowExpected(i), 0.0, Section)
        Next
        For j As Integer = 0 To 4
            failures += AssertClose($"ColumnSums[{j}]", columnSums(j), columnExpected(j), 0.0, Section)
        Next

        ' ---- 稀疏 × 稠密向量 ----
        Dim x As Double() = {1.0, 2.0, 3.0, 4.0, 5.0}
        Dim expectedProduct As Double() = DenseMultiplyVector(dense, x)
        Dim product As Double() = matrix.Multiply(x)

        For i As Integer = 0 To 3
            failures += AssertClose($"Multiply[{i}]", product(i), expectedProduct(i), 1.0E-12, Section)
        Next

        ' ---- 稀疏转置 × 稠密向量 ----
        Dim y As Double() = {1.0, 2.0, 3.0, 4.0}
        Dim expectedTransposeProduct As Double() = DenseMultiplyVector(DenseTranspose(dense), y)
        Dim transposeProduct As Double() = matrix.MultiplyTranspose(y)

        For j As Integer = 0 To 4
            failures += AssertClose($"MultiplyTranspose[{j}]", transposeProduct(j), expectedTransposeProduct(j), 1.0E-12, Section)
        Next

        ' ---- 稀疏 × 稀疏 ----
        Dim right As Double()() = {
            New Double() {1.0, 0.0},
            New Double() {0.0, 1.0},
            New Double() {2.0, 0.0},
            New Double() {0.0, 3.0},
            New Double() {1.0, 1.0}
        }
        Dim productMatrix As CsrMatrix = matrix.Multiply(CompressedBuilder.FromDense(right))
        Dim expectedMatrix As Double()() = DenseMultiply(dense, right)

        For i As Integer = 0 To 3
            For j As Integer = 0 To 1
                failures += AssertClose($"(A·B)({i},{j})", productMatrix.X(i, j), expectedMatrix(i)(j), 1.0E-12, Section)
            Next
        Next

        ' ---- 转置（CSC 往返）----
        Dim transposed As Double()() = matrix.Transpose().ArrayPack()
        Dim transposeExpected As Double()() = DenseTranspose(dense)

        For i As Integer = 0 To 4
            For j As Integer = 0 To 3
                failures += AssertClose($"transpose({i},{j})", transposed(i)(j), transposeExpected(i)(j), 0.0, Section)
            Next
        Next

        ' ToCsc() 的语义是"转置后按列压缩"，因此它给出的是 Aᵀ（5 行 × 4 列）；
        ' 再取一次转置才回到原矩阵 A（4 行 × 5 列）
        Dim transposedCsc As CscMatrix = matrix.ToCsc()

        failures += Assert("ToCsc shape is transposed", transposedCsc.Rows = 5 AndAlso transposedCsc.Columns = 4,
                           $"{transposedCsc.Rows}x{transposedCsc.Columns}")

        Dim roundTrip As Double()() = transposedCsc.Transpose().ArrayPack()

        For i As Integer = 0 To 3
            For j As Integer = 0 To 4
                failures += AssertClose($"csr->csc->csr ({i},{j})", roundTrip(i)(j), dense(i)(j), 0.0, Section)
            Next
        Next

        ' ---- 行切片 / 子矩阵 / 列投影 ----
        Dim sliced As CsrMatrix = matrix.Slice({3, 0})

        failures += Assert("slice rows", sliced.Rows = 2)
        failures += AssertClose("slice[0,0]", sliced.X(0, 0), 0.0, 0.0, Section)
        failures += AssertClose("slice[0,2]", sliced.X(0, 2), 6.0, 0.0, Section)
        failures += AssertClose("slice[1,0]", sliced.X(1, 0), 1.0, 0.0, Section)

        Dim subMatrix As CsrMatrix = matrix.Slice({2, 3}, 1, 3)

        failures += Assert("submatrix shape", subMatrix.Rows = 2 AndAlso subMatrix.Columns = 3)
        failures += AssertClose("submatrix(0,0)", subMatrix.X(0, 0), 4.0, 0.0, Section)
        failures += AssertClose("submatrix(0,2)", subMatrix.X(0, 2), 5.0, 0.0, Section)
        failures += AssertClose("submatrix(1,1)", subMatrix.X(1, 1), 6.0, 0.0, Section)

        Dim projected As CsrMatrix = matrix.SliceColumns({4, 2, 0})

        failures += Assert("column projection shape", projected.Rows = 4 AndAlso projected.Columns = 3)
        failures += AssertClose("projection(0,0)=A(0,4)", projected.X(0, 0), 0.0, 0.0, Section)
        failures += AssertClose("projection(0,1)=A(0,2)", projected.X(0, 1), 2.0, 0.0, Section)
        failures += AssertClose("projection(0,2)=A(0,0)", projected.X(0, 2), 1.0, 0.0, Section)
        failures += AssertClose("projection(3,0)=A(3,4)", projected.X(3, 0), 7.0, 0.0, Section)

        ' ---- 纵向堆叠 ----
        Dim stacked As CsrMatrix = CsrMatrix.Stack({matrix, matrix})

        failures += Assert("stack shape", stacked.Rows = 8 AndAlso stacked.Columns = 5)
        failures += AssertClose("stack(4,2)", stacked.X(4, 2), 2.0, 0.0, Section)
        failures += AssertClose("stack(6,0)", stacked.X(6, 0), 3.0, 0.0, Section)
        failures += AssertClose("stack(7,2)", stacked.X(7, 2), 6.0, 0.0, Section)
        failures += AssertClose("stack RowSums", stacked.RowSums()(7), 13.0, 0.0, Section)

        ' ---- 消零与缩放 ----
        Dim withZero As CsrMatrix = CompressedBuilder.FromTriplets({0, 0, 1}, {0, 1, 1}, {0.0, 0.0, 2.0}, 2, 2)

        failures += Assert("triplets keep explicit zeros", withZero.NonZeros = 3L, withZero.NonZeros.ToString)
        failures += Assert("EliminateZeros drops zeros", withZero.EliminateZeros().NonZeros = 1L)
        failures += AssertClose("triplet duplicate fusion", withZero.EliminateZeros().X(1, 1), 2.0, 0.0, Section)

        Dim duplicates As CsrMatrix = CompressedBuilder.FromTriplets({0, 0}, {1, 1}, {1.5, 2.5}, 1, 2)

        failures += Assert("duplicate coordinates are summed", duplicates.NonZeros = 1L)
        failures += AssertClose("duplicate sum value", duplicates.X(0, 1), 4.0, 0.0, Section)

        Dim scaled As CsrMatrix = matrix.ScaleRows({1.0, 2.0, 3.0, 4.0})
        Dim scaledColumns As CsrMatrix = matrix.ScaleColumns({1.0, 2.0, 3.0, 4.0, 5.0})

        failures += AssertClose("ScaleRows(2,2)", scaled.X(2, 2), 0.0, 0.0, Section)
        failures += AssertClose("ScaleRows(2,0)", scaled.X(2, 0), 9.0, 0.0, Section)
        failures += AssertClose("ScaleColumns(0,2)", scaledColumns.X(0, 2), 6.0, 0.0, Section)
        failures += AssertClose("ScaleColumns(3,4)", scaledColumns.X(3, 4), 35.0, 0.0, Section)

        ' ---- 二维数组入口 ----
        Dim flat As Double(,) = New Double(1, 2) {}
        flat(0, 0) = 1.0 : flat(0, 1) = 0.0 : flat(0, 2) = 2.0
        flat(1, 0) = 0.0 : flat(1, 1) = 3.0 : flat(1, 2) = 0.0

        Dim from2d As CsrMatrix = CompressedBuilder.FromDense(flat)

        failures += Assert("FromDense(2D) shape", from2d.Rows = 2 AndAlso from2d.Columns = 3)
        failures += Assert("FromDense(2D) nnz", from2d.NonZeros = 3L, from2d.NonZeros.ToString)
        failures += AssertClose("FromDense(2D)(1,1)", from2d.X(1, 1), 3.0, 0.0, Section)

        ' ---- 计数校验 ----
        failures += Assert("integer counts accepted",
                           CompressedBuilder.FromDense({{1.0, 2.0}, {0.0, 3.0}}).AssertIntegerCounts().NonZeros = 3L)
        failures += AssertThrows("negative counts rejected",
                                 Sub()
                                     Call CompressedBuilder.FromDense({{-1.0}}).AssertIntegerCounts()
                                 End Sub)
        failures += AssertThrows("fractional counts rejected",
                                 Sub()
                                     Call CompressedBuilder.FromDense({{1.5}}).AssertIntegerCounts()
                                 End Sub)
        failures += AssertThrows("counts at 2^24 rejected",
                                 Sub()
                                     Call CompressedBuilder.FromDense({{16777216.0}}).AssertIntegerCounts()
                                 End Sub)

        Call DebugAudit.Mark($"{Section} audit completed", failures)

        Return failures
    End Function

    ''' <summary>
    ''' 断言一段代码抛出异常（结构与数值契约的负路径）
    ''' </summary>
    Private Function AssertThrows(name As String, code As Action) As Integer
        Try
            Call code()
        Catch ex As Exception
            Return 0
        End Try

        Call DebugAudit.Check(False, $"assertion failed: {name} should throw but did not")

        Return 1
    End Function

    ''' <summary>
    ''' 稠密矩阵 × 稠密向量的参考实现
    ''' </summary>
    Private Function DenseMultiplyVector(a As Double()(), x As Double()) As Double()
        Dim rows As Integer = a.Length
        Dim inner As Integer = If(rows = 0, 0, a(0).Length)
        Dim out As Double() = New Double(rows - 1) {}

        For i As Integer = 0 To rows - 1
            Dim sum As Double = 0.0

            For k As Integer = 0 To inner - 1
                sum += a(i)(k) * x(k)
            Next

            out(i) = sum
        Next

        Return out
    End Function

    Private Function DenseMultiply(a As Double()(), b As Double()()) As Double()()
        Dim rows As Integer = a.Length
        Dim inner As Integer = If(rows = 0, 0, a(0).Length)
        Dim columns As Integer = If(b.Length = 0, 0, b(0).Length)
        Dim out As Double()() = New Double(rows - 1)() {}

        For i As Integer = 0 To rows - 1
            out(i) = New Double(columns - 1) {}
        Next

        For i As Integer = 0 To rows - 1
            For k As Integer = 0 To inner - 1
                Dim av As Double = a(i)(k)

                If av = 0.0 Then Continue For

                For j As Integer = 0 To columns - 1
                    out(i)(j) += av * b(k)(j)
                Next
            Next
        Next

        Return out
    End Function

    Private Function DenseTranspose(a As Double()()) As Double()()
        Dim rows As Integer = a.Length
        Dim columns As Integer = If(rows = 0, 0, a(0).Length)
        Dim out As Double()() = New Double(columns - 1)() {}

        For j As Integer = 0 To columns - 1
            out(j) = New Double(rows - 1) {}
        Next

        For i As Integer = 0 To rows - 1
            For j As Integer = 0 To columns - 1
                out(j)(i) = a(i)(j)
            Next
        Next

        Return out
    End Function

End Module
