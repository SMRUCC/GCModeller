' ------------------------------------------------------------------------
' 双精度线性代数的审计
'
' 判据：
'   * GEMM：四种转置组合与非整数倍尺寸，与 CPU 三循环逐元素对拍（1e-12）
'   * GEMV：与 CPU 点积对拍（1e-12）
'   * QR  ：\|\|QᵀQ - I\|\| <= 1e-10，且 A ≈ Q·R 的重构相对误差 <= 1e-12；
'           |R| 与 CPU 的 Gram-Schmidt 结果一致（QR 在列符号上不唯一，
'           因此比较绝对值）
' ------------------------------------------------------------------------

Imports Microsoft.VisualBasic.Computing.ILCuda.Math

Namespace Audit

    ''' <summary>GpuLinalg 的验收用例</summary>
    Public Module LinalgAudit

        Public Function Run() As Integer
            Call Console.WriteLine()
            Call Console.WriteLine("== 线性代数（GEMM / GEMV / QR）==")

            GemmTransposeCombinations()
            GemmOddShapes()
            GemvAgainstDot()
            QrOrthogonality()
            QrLargeTall()
            QrAgainstGramSchmidt()

            Return AuditRunner.FailureCount
        End Function

        ' ------------------------------------------------------------------
        ' 用例
        ' ------------------------------------------------------------------

        Private Sub GemmTransposeCombinations()
            Dim m As Integer = 64
            Dim n As Integer = 48
            Dim k As Integer = 40

            Dim a As Double() = RandomMatrix(m * k, 101)
            Dim b As Double() = RandomMatrix(k * n, 103)

            ' A 的转置存放（k x m）与 B 的转置存放（n x k）
            Dim aT As Double() = Transpose(a, m, k)
            Dim bT As Double() = Transpose(b, k, n)

            Dim ok As Boolean = True
            Dim worst As Double = 0.0

            ok = CompareGemm(a, b, m, n, k, False, False, worst) AndAlso ok
            ok = CompareGemm(aT, b, m, n, k, True, False, worst) AndAlso ok
            ok = CompareGemm(a, bT, m, n, k, False, True, worst) AndAlso ok
            ok = CompareGemm(aT, bT, m, n, k, True, True, worst) AndAlso ok

            Call AuditRunner.Capture("gemm.transpose.maxRelError", worst)
            Call AuditRunner.Record("GEMM 四种转置组合与 CPU 一致", ok, $"maxRelError={worst:G6}")
        End Sub

        ''' <summary>单个转置组合的对比，返回是否通过，并把误差累计进 <paramref name="worst"/>。</summary>
        Private Function CompareGemm(a As Double(), b As Double(),
                                     m As Integer, n As Integer, k As Integer,
                                     transA As Boolean, transB As Boolean,
                                     ByRef worst As Double) As Boolean
            Dim actual As Double() = GpuLinalg.Gemm(AuditRunner.Engine, a, b, m, n, k, transA, transB)
            Dim expected As Double() = GpuLinalg.GemmCpu(a, b, m, n, k, transA, transB)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)

            If err > worst Then worst = err

            Return err <= 1.0E-12
        End Function

        Private Sub GemmOddShapes()
            Dim m As Integer = 37
            Dim n As Integer = 23
            Dim k As Integer = 51
            Dim a As Double() = RandomMatrix(m * k, 107)
            Dim b As Double() = RandomMatrix(k * n, 109)
            Dim actual As Double() = GpuLinalg.Gemm(AuditRunner.Engine, a, b, m, n, k)
            Dim expected As Double() = GpuLinalg.GemmCpu(a, b, m, n, k)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)

            Call AuditRunner.Record("GEMM 非整数倍尺寸（37x23x51）", err <= 1.0E-12, $"maxRelError={err:G6}")
        End Sub

        Private Sub GemvAgainstDot()
            Dim m As Integer = 300
            Dim k As Integer = 129
            Dim a As Double() = RandomMatrix(m * k, 113)
            Dim x As Double() = RandomMatrix(k, 127)
            Dim actual As Double() = GpuLinalg.Gemv(AuditRunner.Engine, a, x, m, k)
            Dim expected As Double() = GpuLinalg.GemvCpu(a, x, m, k)
            Dim err As Double = AuditRunner.MaxRelError(actual, expected)

            Call AuditRunner.Record("GEMV 与逐行点积一致", err <= 1.0E-12, $"maxRelError={err:G6}")
        End Sub

        Private Sub QrOrthogonality()
            Dim m As Integer = 600
            Dim n As Integer = 8
            Dim a As Double() = RandomMatrix(m * n, 131)
            Dim qr = GpuLinalg.QrEconomy(AuditRunner.Engine, a, m, n)
            Dim deviation As Double = OrthogonalityDeviation(qr.Q, m, n)
            Dim reconstruction As Double = ReconstructionError(qr.Q, qr.R, a, m, n)

            Call AuditRunner.Capture("qr.orthogonality", deviation)
            Call AuditRunner.Capture("qr.reconstruction", reconstruction)
            Call AuditRunner.Record("QR 的正交性 ‖QᵀQ - I‖ <= 1e-10", deviation <= 1.0E-10, $"deviation={deviation:G6}")
            Call AuditRunner.Record("QR 的重构误差 A ≈ Q·R <= 1e-12", reconstruction <= 1.0E-12, $"relError={reconstruction:G6}")
        End Sub

        Private Sub QrLargeTall()
            ' 随机化 SVD 的真实形状：基因数万级、分量百级
            Dim m As Integer = 18000
            Dim n As Integer = 16
            Dim a As Double() = RandomMatrix(m * n, 137)
            Dim qr = GpuLinalg.QrEconomy(AuditRunner.Engine, a, m, n)
            Dim deviation As Double = OrthogonalityDeviation(qr.Q, m, n)
            Dim reconstruction As Double = ReconstructionError(qr.Q, qr.R, a, m, n)

            Call AuditRunner.Capture("qr.large.orthogonality", deviation)
            Call AuditRunner.Capture("qr.large.reconstruction", reconstruction)
            Call AuditRunner.Record("大矩阵 QR（18000 x 16）正交性", deviation <= 1.0E-10, $"deviation={deviation:G6}")
            Call AuditRunner.Record("大矩阵 QR（18000 x 16）重构误差", reconstruction <= 1.0E-12, $"relError={reconstruction:G6}")
        End Sub

        Private Sub QrAgainstGramSchmidt()
            Dim m As Integer = 512
            Dim n As Integer = 12
            Dim a As Double() = RandomMatrix(m * n, 149)
            Dim actual = GpuLinalg.QrEconomy(AuditRunner.Engine, a, m, n)
            Dim expected = GpuLinalg.QrEconomyCpu(a, m, n)
            Dim worst As Double = 0.0

            For i As Integer = 0 To n * n - 1
                Dim d As Double = System.Math.Abs(System.Math.Abs(actual.R(i)) - System.Math.Abs(expected.R(i)))
                Dim scale As Double = System.Math.Max(System.Math.Abs(expected.R(i)), 1.0E-12)

                worst = System.Math.Max(worst, d / scale)
            Next

            Call AuditRunner.Capture("qr.rAbs.maxRelError", worst)
            Call AuditRunner.Record("|R| 与 Gram-Schmidt 参考一致", worst <= 1.0E-10, $"maxRelError={worst:G6}")
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

        ''' <summary>把 rows x cols 的行主序矩阵转置成 cols x rows。</summary>
        Private Function Transpose(a As Double(), rows As Integer, cols As Integer) As Double()
            Dim result(rows * cols - 1) As Double

            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To cols - 1
                    result(j * rows + i) = a(i * cols + j)
                Next
            Next

            Return result
        End Function

        ''' <summary>‖QᵀQ − I‖∞</summary>
        Private Function OrthogonalityDeviation(q As Double(), m As Integer, n As Integer) As Double
            Dim worst As Double = 0.0

            For a As Integer = 0 To n - 1
                For b As Integer = 0 To n - 1
                    Dim dot As Double = 0.0

                    For i As Integer = 0 To m - 1
                        dot += q(i * n + a) * q(i * n + b)
                    Next

                    Dim target As Double = If(a = b, 1.0, 0.0)

                    worst = System.Math.Max(worst, System.Math.Abs(dot - target))
                Next
            Next

            Return worst
        End Function

        ''' <summary>‖A − Q·R‖ / ‖A‖</summary>
        Private Function ReconstructionError(q As Double(), r As Double(), a As Double(),
                                            m As Integer, n As Integer) As Double
            Dim numerator As Double = 0.0
            Dim denominator As Double = 0.0

            For i As Integer = 0 To m - 1
                For c As Integer = 0 To n - 1
                    Dim acc As Double = 0.0

                    For j As Integer = 0 To n - 1
                        acc += q(i * n + j) * r(j * n + c)
                    Next

                    Dim d As Double = a(i * n + c) - acc

                    numerator += d * d
                    denominator += a(i * n + c) * a(i * n + c)
                Next
            Next

            Return System.Math.Sqrt(numerator / System.Math.Max(denominator, 1.0E-300))
        End Function
    End Module
End Namespace
