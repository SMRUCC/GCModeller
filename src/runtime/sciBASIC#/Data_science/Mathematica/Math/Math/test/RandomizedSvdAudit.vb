Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports stdf = System.Math

''' <summary>
''' 随机化截断 SVD（<see cref="RandomizedSVD"/>）与兼容入口 <see cref="TruncatedSVD"/> 的自检：
''' 奇异值精度、子空间精度、正交性、降维一致性、seed 可复现、三条输入入口一致、
''' 容差秩判定、Gram 路径对拍、尺寸契约与异常输入防护。
''' </summary>
''' <remarks>
''' 所有断言经由 <see cref="DebugAudit"/> 记录（<c>Test</c> / <c>AreEqual</c> /
''' <c>CheckThrows</c> / <c>Mark</c>，均为非 <c>&lt;Conditional("DEBUG")&gt;</c> 的入口，
''' 因此在 Release 下也真实执行），结束时用 <see cref="DebugAudit.SaveReport"/> 导出 JSON 报告。
''' </remarks>
Public Module RandomizedSvdAudit

    Private Const ReportName As String = "randomized-svd-test-report.json"

    ' 低秩矩阵的构造尺寸：取 m > n 以避开 LINPACK 参考实现在 m < n 时的 U 形状约定
    Private Const Rows As Integer = 80
    Private Const Columns As Integer = 60
    Private Const Rank As Integer = 5
    Private Const SeedValue As Integer = 20261126

    ''' <summary>运行全部随机化 SVD 审计用例并生成测试报告。</summary>
    ''' <returns>失败断言的数量（0 表示全部通过）。</returns>
    Public Function RunAll() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("randomized-svd audit started")

        Try
            Dim rnd As New Random(SeedValue)
            Dim A As Double()() = MakeLowRank(rnd, Rows, Columns, Rank)
            Dim reference As Double() = FullSpectrum(A, Rank + 3)

            Call CheckSpectrum(A, reference)
            Call CheckOrthogonality(A)
            Call CheckReducedMatrix(A)
            Call CheckDeterminism(A)
            Call CheckEntryPoints(A)
            Call CheckGramPath(A, reference)
            Call CheckToleranceRank()
            Call CheckTruncatedSvdCompatibility(A)
            Call CheckBoundary(A)
        Catch ex As Exception
            Call DebugAudit.Mark("randomized-svd.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"RandomizedSVD audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>奇异值精度：与稠密 SVD 的前 r 个奇异值逐个比对。</summary>
    Private Sub CheckSpectrum(A As Double()(), reference As Double())
        Dim svd As New RandomizedSVD(A, Rank, oversampling:=8, powerIterations:=2, seed:=7)

        Call DebugAudit.AreEqual(Rank, svd.RetainedRank, "retained rank should equal k on a rank-5 matrix")
        DebugAudit.Test(svd.Rank >= Rank, $"the numerical rank {svd.Rank} should cover the true rank {Rank}")

        Dim sigma As Double() = svd.SingularValues
        Dim worst As Double = 0.0

        For i As Integer = 0 To stdf.Min(sigma.Length, Rank) - 1
            Dim relative As Double = stdf.Abs(sigma(i) - reference(i)) / stdf.Max(reference(i), 1e-300)

            worst = stdf.Max(worst, relative)
        Next

        Call DebugAudit.Mark("rsvd.singular_relative_error", worst)
        DebugAudit.Test(worst < 1e-8, $"singular values should be accurate, worst relative error = {worst:E3}")
        DebugAudit.Test(svd.OperatorApplications = 78,
                        $"the operator should be applied 78 times (6 * ell, ell = 13), got {svd.OperatorApplications}")
    End Sub

    ''' <summary>正交性：U 与 V 的列必须单位正交。</summary>
    Private Sub CheckOrthogonality(A As Double()())
        Dim svd As New RandomizedSVD(A, Rank, 8, 2, 7)
        Dim errorU As Double = GramDeviation(svd.U)
        Dim errorV As Double = GramDeviation(svd.V)

        Call DebugAudit.Mark("rsvd.orthogonality_u", errorU)
        Call DebugAudit.Mark("rsvd.orthogonality_v", errorV)
        DebugAudit.Test(errorU < 1e-10, $"||U'U - I||_F = {errorU:E3} should be below 1e-10")
        DebugAudit.Test(errorV < 1e-10, $"||V'V - I||_F = {errorV:E3} should be below 1e-10")

        ' 子空间误差：U 与参考 U 的夹角，用 Uᵀ·U_true 的最小奇异值衡量（应接近 1）
        Dim reference As New SingularValueDecomposition(New NumericMatrix(A))
        Dim trueU As Double()() = reference.U.ArrayPack()
        Dim overlap As Double()() = Cross(svd.U, trueU, Rank, Rank)
        Dim overlapSvd As New SingularValueDecomposition(New NumericMatrix(overlap))
        Dim smallest As Double = overlapSvd.SingularValues.ToArray()(Rank - 1)

        Call DebugAudit.Mark("rsvd.subspace_overlap", smallest)
        DebugAudit.Test(smallest > 1.0 - 1e-6, $"the principal subspace overlap {smallest:F9} should be close to 1")
    End Sub

    ''' <summary>降维矩阵：ReducedMatrix 必须等于 A·V（截断子空间上的最优近似）。</summary>
    Private Sub CheckReducedMatrix(A As Double()())
        Dim svd As New RandomizedSVD(A, Rank, 8, 2, 7)
        Dim reduced As Double()() = svd.ReducedMatrix
        Dim expected As Double()() = Multiply(A, svd.V)
        Dim worst As Double = MaxAbsDiff(reduced, expected)

        Call DebugAudit.Mark("rsvd.reduced_matrix_error", worst)
        DebugAudit.Test(worst < 1e-10, $"ReducedMatrix should equal A*V, max |diff| = {worst:E3}")
        Call DebugAudit.AreEqual(Rows, reduced.Length, "ReducedMatrix should keep the row count")
        Call DebugAudit.AreEqual(Rank, reduced(0).Length, "ReducedMatrix should keep k columns")
    End Sub

    ''' <summary>可复现性：同一 seed 逐位一致，不同 seed 在容差内一致。</summary>
    Private Sub CheckDeterminism(A As Double()())
        Dim first As New RandomizedSVD(A, Rank, 8, 2, 123)
        Dim again As New RandomizedSVD(A, Rank, 8, 2, 123)
        Dim other As New RandomizedSVD(A, Rank, 8, 2, 456)

        DebugAudit.Test(ExactEqual(first.U, again.U), "the same seed must reproduce U bit by bit")
        DebugAudit.Test(ExactEqual(first.SingularValues, again.SingularValues),
                        "the same seed must reproduce the singular values bit by bit")

        Dim worst As Double = 0.0
        Dim left As Double() = first.SingularValues
        Dim right As Double() = other.SingularValues

        For i As Integer = 0 To stdf.Min(left.Length, right.Length) - 1
            worst = stdf.Max(worst, stdf.Abs(left(i) - right(i)) / stdf.Max(left(i), 1e-300))
        Next

        Call DebugAudit.Mark("rsvd.seed_stability", worst)
        DebugAudit.Test(worst < 1e-8, $"different seeds should still agree on the spectrum, worst = {worst:E3}")
    End Sub

    ''' <summary>三条输入入口（稠密 / SparseMatrix / CSR）在同一 seed 下必须一致。</summary>
    Private Sub CheckEntryPoints(A As Double()())
        Dim dense As New RandomizedSVD(A, Rank, 8, 2, 321)
        Dim sparse As New RandomizedSVD(ToSparseMatrix(A), Rank, 8, 2, 321)
        Dim csr As New RandomizedSVD(CompressedBuilder.FromDense(A), Rank, 8, 2, 321)

        Dim diffSparse As Double = MaxAbsDiff(dense.U, sparse.U)
        Dim diffCsr As Double = MaxAbsDiff(dense.U, csr.U)

        Call DebugAudit.Mark("rsvd.entry_dense_vs_sparse", diffSparse)
        Call DebugAudit.Mark("rsvd.entry_dense_vs_csr", diffCsr)
        DebugAudit.Test(diffSparse < 1e-10, $"SparseMatrix entry should agree with the dense entry, diff = {diffSparse:E3}")
        DebugAudit.Test(diffCsr < 1e-10, $"CSR entry should agree with the dense entry, diff = {diffCsr:E3}")
    End Sub

    ''' <summary>Gram 特征分解路径与 SmallSVD 路径对拍。</summary>
    Private Sub CheckGramPath(A As Double()(), reference As Double())
        Dim small As New RandomizedSVD(A, Rank, 8, 2, 55, RandomizedSvdSolver.SmallSVD)
        Dim gram As New RandomizedSVD(A, Rank, 8, 2, 55, RandomizedSvdSolver.GramEigen)

        Dim worst As Double = 0.0
        Dim left As Double() = small.SingularValues
        Dim right As Double() = gram.SingularValues

        For i As Integer = 0 To stdf.Min(left.Length, right.Length) - 1
            worst = stdf.Max(worst, stdf.Abs(left(i) - right(i)) / stdf.Max(reference(i), 1e-300))
        Next

        Call DebugAudit.Mark("rsvd.gram_path_gap", worst)
        DebugAudit.Test(worst < 1e-6, $"the Gram path should reproduce the spectrum within 1e-6, got {worst:E3}")
        DebugAudit.AreEqual(Rank, gram.RetainedRank, "the Gram path should also retain k components")
    End Sub

    ''' <summary>容差秩：谱中只有两个非零方向时，数值秩必须被截断到 2。</summary>
    Private Sub CheckToleranceRank()
        Dim size As Integer = 30
        Dim A As Double()() = RectangularArray.Matrix(Of Double)(size, size)

        For i As Integer = 0 To size - 1
            A(i)(i) = If(i = 0, 1.0, If(i = 1, 0.001, 0.0))
        Next

        Dim svd As New RandomizedSVD(A, 3, 8, 2, 11)

        Call DebugAudit.Mark("rsvd.tolerance", svd.SpectrumTolerance)
        Call DebugAudit.Mark("rsvd.numerical_rank", svd.Rank)
        Call DebugAudit.AreEqual(2, svd.Rank, "the numerical rank should be 2 for the spectrum {1, 1e-3}")
        Call DebugAudit.AreEqual(2, svd.RetainedRank, "only the two significant components should be retained")
        Call DebugAudit.AreEqual(2, svd.SingularValues.Length, "the singular value vector should be truncated")
    End Sub

    ''' <summary>兼容入口：TruncatedSVD 必须保持"恰好 k 列"的尺寸契约。</summary>
    Private Sub CheckTruncatedSvdCompatibility(A As Double()())
        Dim sparse As SparseMatrix = ToSparseMatrix(A)
        Dim svd As New TruncatedSVD(sparse, Rank)

        Call DebugAudit.AreEqual(Rows, svd.ReducedMatrix.Length, "TruncatedSVD should keep the row count")
        Call DebugAudit.AreEqual(Rank, svd.ReducedMatrix(0).Length, "TruncatedSVD should emit exactly k columns")
        Call DebugAudit.AreEqual(Rank, svd.Components.Length, "Components should be k rows")
        Call DebugAudit.AreEqual(Columns, svd.Components(0).Length, "Components should be n columns")

        Dim reduced As Double()() = TruncatedSVD.Reduce(sparse, 3)
        Call DebugAudit.AreEqual(Rows, reduced.Length, "Reduce should keep the row count")
        Call DebugAudit.AreEqual(3, reduced(0).Length, "Reduce should emit the requested dimension")

        ' 秩不足时补零列：rank-2 的矩阵请求 k=3
        Dim size As Integer = 30
        Dim low As Double()() = RectangularArray.Matrix(Of Double)(size, size)

        For i As Integer = 0 To size - 1
            low(i)(i) = If(i = 0, 1.0, If(i = 1, 0.001, 0.0))
        Next

        Dim padded As New TruncatedSVD(ToSparseMatrix(low), 3)
        Dim third As Double = padded.SingularValues(2)
        Dim columnZero As Boolean = True

        For i As Integer = 0 To size - 1
            If padded.ReducedMatrix(i)(2) <> 0.0 Then
                columnZero = False
                Exit For
            End If
        Next

        DebugAudit.Test(third = 0.0, $"the padded singular value should be exactly zero, got {third:E3}")
        DebugAudit.Test(columnZero, "the padded column of ReducedMatrix should be all zeros")
        Call DebugAudit.AreEqual(size, padded.ReducedMatrix.Length, "the padded matrix keeps the row count")
        Call DebugAudit.AreEqual(3, padded.ReducedMatrix(0).Length, "the padded matrix still has k columns")
    End Sub

    ''' <summary>异常输入防护。</summary>
    Private Sub CheckBoundary(A As Double()())
        Dim sparse As SparseMatrix = ToSparseMatrix(A)

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New RandomizedSVD(A, Rows + 1)
            End Sub, "k > min(m,n) should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New RandomizedSVD(A, Rank, oversampling:=-1)
            End Sub, "a negative oversampling should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New RandomizedSVD(A, Rank, powerIterations:=-1)
            End Sub, "a negative power iteration count should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New RandomizedSVD(A, -1)
            End Sub, "a negative k should be rejected")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Dim bad As New TruncatedSVD(sparse, 0)
            End Sub, "TruncatedSVD should keep rejecting k = 0 (historical behaviour)")

        ' k = 0 在引擎层是"只求秩、不保留分量"
        Dim probe As New RandomizedSVD(A, 0)
        Call DebugAudit.AreEqual(0, probe.RetainedRank, "k = 0 should retain no component")
        Call DebugAudit.AreEqual(0, probe.SingularValues.Length, "k = 0 should return an empty spectrum")

        ' 空矩阵返回空结果而不是抛异常
        Dim empty As New RandomizedSVD(New Double(-1)() {}, 3)
        Call DebugAudit.AreEqual(0, empty.RetainedRank, "an empty matrix should yield an empty decomposition")
    End Sub

#End Region

#Region "helpers"

    ''' <summary>构造一个精确秩为 r 的矩阵 A = U0·V0ᵀ。</summary>
    Private Function MakeLowRank(rnd As Random, rows As Integer, columns As Integer, r As Integer) As Double()()
        Dim factors(rows - 1)() As Double

        For i As Integer = 0 To rows - 1
            factors(i) = New Double(r - 1) {}

            For j As Integer = 0 To r - 1
                factors(i)(j) = rnd.NextDouble() * 2.0 - 1.0
            Next
        Next

        Dim other(columns - 1)() As Double

        For i As Integer = 0 To columns - 1
            other(i) = New Double(r - 1) {}

            For j As Integer = 0 To r - 1
                other(i)(j) = rnd.NextDouble() * 2.0 - 1.0
            Next
        Next

        Dim A As Double()() = RectangularArray.Matrix(Of Double)(rows, columns)

        For i As Integer = 0 To rows - 1
            For j As Integer = 0 To columns - 1
                Dim acc As Double = 0.0

                For t As Integer = 0 To r - 1
                    acc += factors(i)(t) * other(j)(t)
                Next

                A(i)(j) = acc
            Next
        Next

        Return A
    End Function

    ''' <summary>用稠密 SVD 取参考奇异值。</summary>
    Private Function FullSpectrum(A As Double()(), count As Integer) As Double()
        Dim svd As New SingularValueDecomposition(New NumericMatrix(A))
        Dim sigma As Double() = svd.SingularValues.ToArray()
        Dim top(count - 1) As Double

        For i As Integer = 0 To count - 1
            top(i) = If(i < sigma.Length, sigma(i), 0.0)
        Next

        Return top
    End Function

    ''' <summary>把稠密矩阵转成三元组稀疏矩阵（与 <c>TruncatedSVDTest</c> 的构造方式一致）。</summary>
    Private Function ToSparseMatrix(A As Double()()) As SparseMatrix
        Dim rowList As New List(Of Integer)
        Dim columnList As New List(Of Integer)
        Dim valueList As New List(Of Double)

        For i As Integer = 0 To A.Length - 1
            For j As Integer = 0 To A(i).Length - 1
                If A(i)(j) <> 0.0 Then
                    Call rowList.Add(i)
                    Call columnList.Add(j)
                    Call valueList.Add(A(i)(j))
                End If
            Next
        Next

        Return New SparseMatrix(rowList.ToArray, columnList.ToArray, valueList.ToArray,
                                A.Length, If(A.Length = 0, 0, A(0).Length))
    End Function

    ''' <summary>稠密矩阵乘法 <c>A(m×n)·V(n×k)</c>。</summary>
    Private Function Multiply(A As Double()(), V As Double()()) As Double()()
        Dim rows As Integer = A.Length
        Dim inner As Integer = V.Length
        Dim k As Integer = If(inner = 0, 0, V(0).Length)
        Dim result As Double()() = RectangularArray.Matrix(Of Double)(rows, k)

        For i As Integer = 0 To rows - 1
            For c As Integer = 0 To k - 1
                Dim acc As Double = 0.0

                For j As Integer = 0 To inner - 1
                    acc += A(i)(j) * V(j)(c)
                Next

                result(i)(c) = acc
            Next
        Next

        Return result
    End Function

    ''' <summary>叉积 <c>U(rows×a)ᵀ·V(rows×b)</c>。</summary>
    Private Function Cross(U As Double()(), V As Double()(), a As Integer, b As Integer) As Double()()
        Dim rows As Integer = U.Length
        Dim result As Double()() = RectangularArray.Matrix(Of Double)(a, b)

        For i As Integer = 0 To a - 1
            For j As Integer = 0 To b - 1
                Dim acc As Double = 0.0

                For r As Integer = 0 To rows - 1
                    acc += U(r)(i) * V(r)(j)
                Next

                result(i)(j) = acc
            Next
        Next

        Return result
    End Function

    ''' <summary><c>‖QᵀQ − I‖_F</c>。</summary>
    Private Function GramDeviation(Q As Double()()) As Double
        If Q.Length = 0 Then
            Return 0.0
        End If

        Dim k As Integer = Q(0).Length
        Dim sum As Double = 0.0

        For a As Integer = 0 To k - 1
            For b As Integer = 0 To k - 1
                Dim acc As Double = 0.0

                For i As Integer = 0 To Q.Length - 1
                    acc += Q(i)(a) * Q(i)(b)
                Next

                If a = b Then
                    acc -= 1.0
                End If

                sum += acc * acc
            Next
        Next

        Return stdf.Sqrt(sum)
    End Function

    Private Function MaxAbsDiff(a As Double()(), b As Double()()) As Double
        Dim diff As Double = 0.0
        Dim rows As Integer = stdf.Min(a.Length, b.Length)

        For i As Integer = 0 To rows - 1
            Dim columns As Integer = stdf.Min(a(i).Length, b(i).Length)

            For j As Integer = 0 To columns - 1
                diff = stdf.Max(diff, stdf.Abs(a(i)(j) - b(i)(j)))
            Next
        Next

        Return diff
    End Function

    Private Function ExactEqual(a As Double()(), b As Double()()) As Boolean
        If a.Length <> b.Length Then
            Return False
        End If

        For i As Integer = 0 To a.Length - 1
            If a(i).Length <> b(i).Length Then
                Return False
            End If

            For j As Integer = 0 To a(i).Length - 1
                If a(i)(j) <> b(i)(j) Then
                    Return False
                End If
            Next
        Next

        Return True
    End Function

    Private Function ExactEqual(a As Double(), b As Double()) As Boolean
        If a.Length <> b.Length Then
            Return False
        End If

        For i As Integer = 0 To a.Length - 1
            If a(i) <> b(i) Then
                Return False
            End If
        Next

        Return True
    End Function

#End Region

End Module
