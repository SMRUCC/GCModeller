' ------------------------------------------------------------------------
' 批量小线性求解的审计
'
' 判据：
'   * Cholesky 求解：与 CPU 参考解逐元素对拍（1e-10），
'     并且 (A + λI)·X ≈ B 的相对残差 <= 1e-10
'   * 近奇异 + 岭项：同上（岭项让正规方程保持正定）
'   * 掩码 Gram：与 CPU 逐元素对拍（1e-12）；
'     全空掩码时 XX = λI、XY = 0（退化语义）
' ------------------------------------------------------------------------

Imports Microsoft.VisualBasic.Computing.ILCuda.Math

Namespace Audit

    ''' <summary>GpuSolve 的验收用例</summary>
    Public Module SolveAudit

        Public Function Run() As Integer
            Call Console.WriteLine()
            Call Console.WriteLine("== 批量小线性求解 ==")

            SmallBatch()
            NearSingularWithRidge()
            MediumBatch()
            BoundaryOrder()
            MaskedGram()
            EmptyMask()

            Return AuditRunner.FailureCount
        End Function

        ' ------------------------------------------------------------------
        ' 用例
        ' ------------------------------------------------------------------

        Private Sub SmallBatch()
            Dim batch As Integer = 64
            Dim n As Integer = 6
            Dim nrhs As Integer = 3
            Dim lambda As Double = 0.1
            Dim a As Double() = SymmetricPositiveDefinite(batch, n, 211)
            Dim b As Double() = RandomMatrix(batch * n * nrhs, 223)
            Dim actual As Double() = GpuSolve.CholeskySolve(AuditRunner.Engine, a, b, batch, n, nrhs, lambda)
            Dim expected As Double() = GpuSolve.CholeskySolveCpu(a, b, batch, n, nrhs, lambda)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)
            Dim residual As Double = SolveResidual(a, b, actual, batch, n, nrhs, lambda)

            Call AuditRunner.Capture("solve.small.maxRelError", err)
            Call AuditRunner.Capture("solve.small.residual", residual)
            Call AuditRunner.Record("小档批量求解与 CPU 一致", err <= 1.0E-10, $"maxRelError={err:G6}")
            Call AuditRunner.Record("小档批量求解残差 (A+λI)X ≈ B", residual <= 1.0E-10, $"residual={residual:G6}")
        End Sub

        Private Sub NearSingularWithRidge()
            Dim batch As Integer = 8
            Dim n As Integer = 8
            Dim nrhs As Integer = 2
            Dim lambda As Double = 1.0E-06

            ' 让第一列的系数近乎退化：A 接近奇异，只能靠岭项撑住正定性
            Dim a As Double() = SymmetricPositiveDefinite(batch, n, 227)

            For item As Integer = 0 To batch - 1
                For i As Integer = 0 To n - 1
                    For j As Integer = 0 To n - 1
                        If i <> j Then
                            a(item * n * n + i * n + j) = a(item * n * n + i * n + j) * 0.999999
                        End If
                    Next
                Next
            Next

            Dim b As Double() = RandomMatrix(batch * n * nrhs, 229)
            Dim actual As Double() = GpuSolve.CholeskySolve(AuditRunner.Engine, a, b, batch, n, nrhs, lambda)
            Dim expected As Double() = GpuSolve.CholeskySolveCpu(a, b, batch, n, nrhs, lambda)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)
            Dim residual As Double = SolveResidual(a, b, actual, batch, n, nrhs, lambda)
            Dim finite As Boolean = True

            For i As Integer = 0 To actual.Length - 1
                If Double.IsNaN(actual(i)) OrElse Double.IsInfinity(actual(i)) Then finite = False
            Next

            Call AuditRunner.Record("近奇异 + 岭项仍与 CPU 一致", err <= 1.0E-10, $"maxRelError={err:G6}")
            Call AuditRunner.Record("近奇异 + 岭项的解是有限值", finite)
            Call AuditRunner.Record("近奇异 + 岭项残差可控", residual <= 1.0E-08, $"residual={residual:G6}")
        End Sub

        Private Sub MediumBatch()
            ' 投影 ridge 的真实形状：k = 160、右端项 8 列
            Dim batch As Integer = 4
            Dim n As Integer = 160
            Dim nrhs As Integer = 8
            Dim lambda As Double = 0.1
            Dim a As Double() = SymmetricPositiveDefinite(batch, n, 233)
            Dim b As Double() = RandomMatrix(batch * n * nrhs, 239)
            Dim actual As Double() = GpuSolve.CholeskySolve(AuditRunner.Engine, a, b, batch, n, nrhs, lambda)
            Dim residual As Double = SolveResidual(a, b, actual, batch, n, nrhs, lambda)
            Dim expected As Double() = GpuSolve.CholeskySolveCpu(a, b, batch, n, nrhs, lambda)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)

            Call AuditRunner.Capture("solve.medium.residual", residual)
            Call AuditRunner.Record("中档（160 阶）残差 (A+λI)X ≈ B", residual <= 1.0E-10, $"residual={residual:G6}")
            Call AuditRunner.Record("中档（160 阶）与 CPU 一致", err <= 1.0E-09, $"maxRelError={err:G6}")
        End Sub

        Private Sub BoundaryOrder()
            ' n = 32 是小档内核的上界，必须与 CPU 完全一致
            Dim batch As Integer = 8
            Dim n As Integer = GpuSolve.SmallMaxOrder
            Dim nrhs As Integer = 4
            Dim a As Double() = SymmetricPositiveDefinite(batch, n, 241)
            Dim b As Double() = RandomMatrix(batch * n * nrhs, 251)
            Dim actual As Double() = GpuSolve.CholeskySolve(AuditRunner.Engine, a, b, batch, n, nrhs, 0.05)
            Dim expected As Double() = GpuSolve.CholeskySolveCpu(a, b, batch, n, nrhs, 0.05)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)

            Call AuditRunner.Record($"小档上界（n = {n}）与 CPU 一致", err <= 1.0E-10, $"maxRelError={err:G6}")
        End Sub

        Private Sub MaskedGram()
            Dim batch As Integer = 16
            Dim rows As Integer = 40
            Dim f As Integer = 6
            Dim k As Integer = 8
            Dim lambda As Double = 2.5
            Dim x As Double() = RandomMatrix(batch * rows * f, 257)
            Dim y As Double() = RandomMatrix(batch * rows * k, 263)
            Dim mask As Integer() = RandomMask(batch, rows, 269)
            Dim actual = GpuSolve.BatchMaskedGram(AuditRunner.Engine, x, y, mask, batch, rows, f, k, lambda)
            Dim expected = GpuSolve.BatchMaskedGramCpu(x, y, mask, batch, rows, f, k, lambda)
            Dim xxErr As Double = AuditRunner.MaxRelError(actual.XX, expected.XX)
            Dim xyErr As Double = AuditRunner.MaxRelError(actual.XY, expected.XY)

            Call AuditRunner.Capture("gram.xx.maxRelError", xxErr)
            Call AuditRunner.Capture("gram.xy.maxRelError", xyErr)
            Call AuditRunner.Record("掩码 Gram 的 XᵀX + λI 与 CPU 一致", xxErr <= 1.0E-12, $"maxRelError={xxErr:G6}")
            Call AuditRunner.Record("掩码 Gram 的 XᵀY 与 CPU 一致", xyErr <= 1.0E-12, $"maxRelError={xyErr:G6}")
        End Sub

        Private Sub EmptyMask()
            Dim batch As Integer = 4
            Dim rows As Integer = 32
            Dim f As Integer = 5
            Dim k As Integer = 3
            Dim lambda As Double = 3.5
            Dim x As Double() = RandomMatrix(batch * rows * f, 271)
            Dim y As Double() = RandomMatrix(batch * rows * k, 277)
            Dim mask(batch * rows - 1) As Integer
            Dim actual = GpuSolve.BatchMaskedGram(AuditRunner.Engine, x, y, mask, batch, rows, f, k, lambda)
            Dim identity As Boolean = True
            Dim zero As Boolean = True

            For item As Integer = 0 To batch - 1
                For a As Integer = 0 To f - 1
                    For b As Integer = 0 To f - 1
                        Dim want As Double = If(a = b, lambda, 0.0)

                        If System.Math.Abs(actual.XX(item * f * f + a * f + b) - want) > 1.0E-12 Then identity = False
                    Next
                Next

                For a As Integer = 0 To f * k - 1
                    If System.Math.Abs(actual.XY(item * f * k + a)) > 1.0E-12 Then zero = False
                Next
            Next

            Call AuditRunner.Record("全空掩码时 XᵀX = λI", identity)
            Call AuditRunner.Record("全空掩码时 XᵀY = 0", zero)
        End Sub

        ' ------------------------------------------------------------------
        ' 工具
        ' ------------------------------------------------------------------

        Private Function RandomMatrix(count As Integer, seed As Integer) As Double()
            Dim rng As New Random(seed)
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = rng.NextDouble() * 2.0 - 1.0
            Next

            Return result
        End Function

        ''' <summary>生成一批对称正定矩阵：A = MᵀM / n + I。</summary>
        Private Function SymmetricPositiveDefinite(batch As Integer, n As Integer, seed As Integer) As Double()
            Dim rng As New Random(seed)
            Dim result(batch * n * n - 1) As Double
            Dim m(n * n - 1) As Double

            For item As Integer = 0 To batch - 1
                For i As Integer = 0 To m.Length - 1
                    m(i) = rng.NextDouble() * 2.0 - 1.0
                Next

                For i As Integer = 0 To n - 1
                    For j As Integer = 0 To n - 1
                        Dim acc As Double = 0.0

                        For r As Integer = 0 To n - 1
                            acc += m(r * n + i) * m(r * n + j)
                        Next

                        result(item * n * n + i * n + j) = acc / n + If(i = j, 1.0, 0.0)
                    Next
                Next
            Next

            Return result
        End Function

        Private Function RandomMask(batch As Integer, rows As Integer, seed As Integer) As Integer()
            Dim rng As New Random(seed)
            Dim result(batch * rows - 1) As Integer

            For i As Integer = 0 To result.Length - 1
                result(i) = If(rng.NextDouble() < 0.75, 1, 0)
            Next

            Return result
        End Function

        ''' <summary>‖(A + λI)·X − B‖_F / ‖B‖_F</summary>
        Private Function SolveResidual(a As Double(), b As Double(), x As Double(),
                                       batch As Integer, n As Integer, nrhs As Integer,
                                       lambda As Double) As Double
            Dim numerator As Double = 0.0
            Dim denominator As Double = 0.0

            For item As Integer = 0 To batch - 1
                Dim aBase As Integer = item * n * n
                Dim bBase As Integer = item * n * nrhs

                For i As Integer = 0 To n - 1
                    For c As Integer = 0 To nrhs - 1
                        Dim acc As Double = 0.0

                        For j As Integer = 0 To n - 1
                            Dim av As Double = a(aBase + i * n + j) + If(i = j, lambda, 0.0)

                            acc += av * x(bBase + j * nrhs + c)
                        Next

                        Dim d As Double = acc - b(bBase + i * nrhs + c)

                        numerator += d * d
                        denominator += b(bBase + i * nrhs + c) * b(bBase + i * nrhs + c)
                    Next
                Next
            Next

            Return System.Math.Sqrt(numerator / System.Math.Max(denominator, 1.0E-300))
        End Function
    End Module
End Namespace
