Imports System
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' 流式稀疏矩阵转换审计：稠密对拍、CSC↔CSR 互为转置、空行/全零行、列选择、形状校验。
''' </summary>
Public Module ConverterAudit

    Private Const ReportName As String = "virtualcellular-converter-report.json"

    ''' <summary>审计入口。</summary>
    Public Function Run() As Integer
        Dim reportPath As String = Path.Combine(AppContext.BaseDirectory, ReportName)

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 4096
        Call DebugAudit.Mark("converter audit started")

        Try
            Call CheckAgainstDense()
            Call CheckTranspose()
            Call CheckSparseEdgeCases()
            Call CheckColumnSelection()
            Call CheckInvalidInput()
        Catch ex As Exception
            Call DebugAudit.Mark("converter.audit.crash", ex.ToString())
            Call DebugAudit.Test(False, $"the converter audit should not throw unhandled exceptions: {ex.Message}")
        End Try

        Call DebugAudit.SaveReport(reportPath)
        Call Console.WriteLine($"   [audit] report -> {reportPath}")
        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

#Region "cases"

    ''' <summary>与稠密参考实现逐元素一致（两个方向）。</summary>
    Private Sub CheckAgainstDense()
        Dim samples As String() = {"c1", "c2", "c3", "c4"}
        Dim dense As Double()() = {
            New Double() {1.0, 0.0, 2.0, 0.0},
            New Double() {0.0, 0.0, 0.0, 0.0},
            New Double() {3.0, 4.0, 0.0, 5.0}
        }
        Dim rows As DataFrameRow() = MakeRows({"g1", "g2", "g3"}, dense)

        Dim csc As CscMatrix = rows.AsCscMatrix(samples, verbose:=False)
        Dim csr As CsrMatrix = rows.AsCsrMatrix(samples, verbose:=False)

        Call DebugAudit.AreEqual(3, csc.RowDimension, "CSC has one row per gene")
        Call DebugAudit.AreEqual(4, csc.ColumnDimension, "CSC has one column per sample")
        Call DebugAudit.AreEqual(4, csr.RowDimension, "CSR has one row per sample")
        Call DebugAudit.AreEqual(3, csr.ColumnDimension, "CSR has one column per gene")
        Call DebugAudit.AreEqual(5L, csc.NonZeros, "five non-zero entries survive")
        Call DebugAudit.AreEqual(5L, csr.NonZeros, "the transpose keeps the same nnz")

        Dim mismatch As Integer = 0

        For g As Integer = 0 To 2
            For c As Integer = 0 To 3
                Dim expected As Double = dense(g)(c)

                If expected <> csc.ValueOf(g, c) Then
                    mismatch += 1
                End If
                If expected <> csr.ValueOf(c, g) Then
                    mismatch += 1
                End If
            Next
        Next

        Call DebugAudit.Mark("converter.dense_mismatch", mismatch)
        Call DebugAudit.AreEqual(0, mismatch, "every entry matches the dense reference in both directions")
    End Sub

    ''' <summary>CSC 与 CSR 互为转置，且行内下标有序。</summary>
    Private Sub CheckTranspose()
        Dim samples As String() = {"a", "b", "c"}
        Dim dense As Double()() = {
            New Double() {0.5, 1.5, 0.0},
            New Double() {2.5, 0.0, 3.5}
        }
        Dim rows As DataFrameRow() = MakeRows({"x", "y"}, dense)

        Dim csc As CscMatrix = rows.AsCscMatrix(samples, verbose:=False)
        Dim csr As CsrMatrix = rows.AsCsrMatrix(samples, verbose:=False)

        Dim gap As Integer = 0

        For g As Integer = 0 To 1
            For c As Integer = 0 To 2
                If csc.ValueOf(g, c) <> csr.ValueOf(c, g) Then
                    gap += 1
                End If
            Next
        Next

        Call DebugAudit.AreEqual(0, gap, "CSC and CSR are exact transposes of each other")

        ' CSR 每行的列下标必须严格递增（CSR 的有序性约束）
        For r As Integer = 0 To csr.RowDimension - 1
            Dim previous As Integer = -1
            Dim ordered As Boolean = True

            For k As Integer = csr.Indptr(r) To csr.Indptr(r + 1) - 1
                If csr.Indices(k) <= previous Then
                    ordered = False
                End If

                previous = csr.Indices(k)
            Next

            DebugAudit.Test(ordered, $"row #{r} of the CSR keeps its column indices increasing")
        Next
    End Sub

    ''' <summary>空输入、全零行、全零矩阵。</summary>
    Private Sub CheckSparseEdgeCases()
        Dim samples As String() = {"s1", "s2"}

        ' 空输入
        Dim emptyCsc As CscMatrix = CType(New DataFrameRow() {}, IEnumerable(Of DataFrameRow)).AsCscMatrix(samples, verbose:=False)
        Dim emptyCsr As CsrMatrix = CType(New DataFrameRow() {}, IEnumerable(Of DataFrameRow)).AsCsrMatrix(samples, verbose:=False)

        Call DebugAudit.AreEqual(0, emptyCsc.RowDimension, "an empty stream gives an empty CSC")
        Call DebugAudit.AreEqual(0L, emptyCsr.NonZeros, "an empty stream gives an empty CSR")

        ' 全零行夹在中间
        Dim rows As DataFrameRow() = {
            New DataFrameRow With {.geneID = "g1", .experiments = {1.0, 2.0}},
            New DataFrameRow With {.geneID = "g2", .experiments = {0.0, 0.0}},
            New DataFrameRow With {.geneID = "g3", .experiments = {0.0, 3.0}}
        }
        Dim csr As CsrMatrix = CType(rows, IEnumerable(Of DataFrameRow)).AsCsrMatrix(samples, verbose:=False)

        Call DebugAudit.AreEqual(3L, csr.NonZeros, "the all-zero gene contributes no entry")
        Call DebugAudit.AreEqual(3, csr.ColumnDimension, "the zero row still occupies a gene column")
        Call DebugAudit.AreEqual(2.0, csr.ValueOf(1, 0), "the first sample keeps its value")
        Call DebugAudit.AreEqual(3.0, csr.ValueOf(1, 2), "the third gene lands on the same sample row")

        ' 全零矩阵：CSR 每行的 indptr 应完全相等
        Dim zeros As DataFrameRow() = {
            New DataFrameRow With {.geneID = "z1", .experiments = {0.0, 0.0}},
            New DataFrameRow With {.geneID = "z2", .experiments = {0.0, 0.0}}
        }
        Dim zeroMatrix As CsrMatrix = CType(zeros, IEnumerable(Of DataFrameRow)).AsCsrMatrix(samples, verbose:=False)

        Call DebugAudit.AreEqual(0L, zeroMatrix.NonZeros, "an all-zero matrix has no non-zero entries")
        Call DebugAudit.AreEqual(zeroMatrix.Indptr(0), zeroMatrix.Indptr(1), "every row of the zero matrix starts at 0")
    End Sub

    ''' <summary>列子集：只保留调用方指定的样本，行内值也要同步切片。</summary>
    Private Sub CheckColumnSelection()
        Dim samples As String() = {"c2", "c4"}
        Dim dense As Double()() = {
            New Double() {0.0, 7.0},
            New Double() {9.0, 0.0}
        }
        Dim rows As DataFrameRow() = MakeRows({"g1", "g2"}, dense)

        Dim csr As CsrMatrix = CType(rows, IEnumerable(Of DataFrameRow)).AsCsrMatrix(samples, verbose:=False)

        Call DebugAudit.AreEqual(2, csr.RowDimension, "the CSR keeps only the selected samples as rows")

        ' CSR 的行是样本、列是基因：7.0 落在 (样本 c4 = 行1, 基因 g1 = 列0)，9.0 落在 (行0, 列1)
        Call DebugAudit.AreEqual(7.0, csr.ValueOf(1, 0), "the first selected sample keeps its value")
        Call DebugAudit.AreEqual(9.0, csr.ValueOf(0, 1), "the second selected sample keeps its value")
        Call DebugAudit.AreEqual(2L, csr.NonZeros, "the selection does not introduce extra entries")
    End Sub

    ''' <summary>形状不匹配与空参数必须显式报错。</summary>
    Private Sub CheckInvalidInput()
        Dim rows As DataFrameRow() = {
            New DataFrameRow With {.geneID = "g1", .experiments = {1.0, 2.0, 3.0}}
        }

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call CType(rows, IEnumerable(Of DataFrameRow)).AsCsrMatrix({"only_one"}, verbose:=False),
            "a row wider than the sample axis must throw")

        Call DebugAudit.CheckThrows(Of ArgumentNullException)(
            Sub() Call CType(rows, IEnumerable(Of DataFrameRow)).AsCsrMatrix(Nothing, verbose:=False),
            "a missing sample axis must throw")
    End Sub

#End Region

#Region "helpers"

    Private Function MakeRows(genes As String(), dense As Double()()) As DataFrameRow()
        Dim rows(genes.Length - 1) As DataFrameRow

        For i As Integer = 0 To genes.Length - 1
            rows(i) = New DataFrameRow With {
                .geneID = genes(i),
                .experiments = DirectCast(dense(i).Clone(), Double())
            }
        Next

        Return rows
    End Function

#End Region

End Module
