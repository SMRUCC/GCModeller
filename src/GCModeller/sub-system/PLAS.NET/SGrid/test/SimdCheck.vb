Imports System.Diagnostics
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SIMD
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports std = System.Math

Namespace SelfTest

    ''' <summary>
    ''' SIMD 内核的精度校验与微基准。
    ''' (accuracy check and micro benchmark of the SIMD kernels)
    ''' </summary>
    ''' <remarks>
    ''' 该入口不依赖任何外部数据，用于在改动数值内核之后快速判断：
    ''' 
    ''' 1. <see cref="SimdMath.ExpInPlace"/> / <see cref="SimdMath.LogInPlace"/>
    '''    与 <see cref="System.Math"/> 的相对误差；
    ''' 2. 向量化 exp / AXPY 相对标量实现的吞吐比；
    ''' 3. 重写之后的 <see cref="SlopeProblem"/> 目标函数与标量参考实现的结果一致性；
    ''' 4. 解析梯度与中心差分梯度的一致性。
    ''' </remarks>
    Public Module SimdCheck

        ''' <summary>
        ''' 执行全部检查，返回退出码（零表示通过）
        ''' </summary>
        ''' <param name="verbose"></param>
        ''' <returns></returns>
        Public Function Run(Optional verbose As Boolean = True) As Integer
            Dim report As New StringBuilder()
            Dim failures As New List(Of String)

            Call report.AppendLine("================ SIMD kernel check ================")

            Call checkExp(report, failures)
            Call checkLog(report, failures)
            Call checkHardware(report)
            Call checkThroughput(report)
            Call checkObjective(report, failures)
            Call checkGradient(report, failures)

            Call report.AppendLine()
            Call report.AppendLine(If(failures.Count = 0, "PASS: all of the assertions were satisfied.", $"FAIL: {failures.Count} assertion(s) were violated:"))

            For Each fail As String In failures
                Call report.AppendLine($"  {fail}")
            Next

            Call Console.WriteLine(report.ToString)

            Return If(failures.Count = 0, 0, 1)
        End Function

        ''' <summary>
        ''' 指数内核的精度：相对误差应当小于 1E-9
        ''' </summary>
        Private Sub checkExp(report As StringBuilder, failures As List(Of String))
            Dim rnd As New Random(1234567)
            Dim n As Integer = 100000
            Dim input As Double() = New Double(n - 1) {}
            Dim expect As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim x As Double = (rnd.NextDouble() * 2 - 1) * 30

                input(i) = x
                expect(i) = std.Exp(x)
            Next

            Dim actual As Double() = CType(input.Clone(), Double())

            Call SimdMath.ExpInPlace(actual)

            Dim maxError As Double = 0
            Dim worst As Double = 0

            For i As Integer = 0 To n - 1
                Dim rel As Double = std.Abs(actual(i) - expect(i)) / std.Max(std.Abs(expect(i)), 1.0E-300)

                If rel > maxError Then
                    maxError = rel
                    worst = input(i)
                End If
            Next

            Call report.AppendLine($"exp   : n={n}, max relative error={maxError.ToString("G3")} (worst x={worst.ToString("G4")})")

            If maxError > 1.0E-9 Then
                Call failures.Add($"SimdMath.ExpInPlace relative error {maxError.ToString("G4")} exceeds 1E-9")
            End If
        End Sub

        ''' <summary>
        ''' 对数内核的精度：相对误差应当小于 1E-9
        ''' </summary>
        Private Sub checkLog(report As StringBuilder, failures As List(Of String))
            Dim rnd As New Random(7654321)
            Dim n As Integer = 100000
            Dim input As Double() = New Double(n - 1) {}
            Dim expect As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim x As Double = std.Exp((rnd.NextDouble() * 2 - 1) * 20)

                input(i) = x
                expect(i) = std.Log(x)
            Next

            Dim actual As Double() = CType(input.Clone(), Double())

            Call SimdMath.LogInPlace(actual)

            Dim maxError As Double = 0
            Dim worst As Double = 0

            For i As Integer = 0 To n - 1
                Dim rel As Double = std.Abs(actual(i) - expect(i)) / std.Max(std.Abs(expect(i)), 1.0E-6)

                If rel > maxError Then
                    maxError = rel
                    worst = input(i)
                End If
            Next

            Call report.AppendLine($"log   : n={n}, max relative error={maxError.ToString("G3")} (worst x={worst.ToString("G4")})")

            If maxError > 1.0E-9 Then
                Call failures.Add($"SimdMath.LogInPlace relative error {maxError.ToString("G4")} exceeds 1E-9")
            End If
        End Sub

        ''' <summary>
        ''' 打印硬件加速能力与向量宽度的实测信息（排查 SIMD 退化的第一步）
        ''' </summary>
        Private Sub checkHardware(report As StringBuilder)
            Call report.AppendLine(
                $"hw    : Vector<Double>.Count={System.Numerics.Vector(Of Double).Count}, " &
                $"hwAccel={System.Numerics.Vector.IsHardwareAccelerated}, " &
                $"avx2={System.Runtime.Intrinsics.X86.Avx2.IsSupported}, " &
                $"fma={System.Runtime.Intrinsics.X86.Fma.IsSupported}, " &
                $"enabled={SIMDEnvironment.IsEnabled}")
        End Sub

        ''' <summary>
        ''' 微基准：向量化 exp 与标量 exp 的吞吐
        ''' </summary>
        Private Sub checkThroughput(report As StringBuilder)
            Dim m As Integer = 80
            Dim reps As Integer = 200000
            Dim buffer As Double() = New Double(m - 1) {}

            For i As Integer = 0 To m - 1
                buffer(i) = -6.0R + i * 0.15R
            Next

            Dim source As Double() = CType(buffer.Clone(), Double())

            ' 预热
            For k As Integer = 1 To 1000
                Call SimdMath.ExpInPlace(CType(source.Clone(), Double()))
            Next

            Dim watch As Stopwatch = Stopwatch.StartNew()

            For k As Integer = 1 To reps
                Dim work As Double() = CType(source.Clone(), Double())

                Call SimdMath.ExpInPlace(work)
            Next

            watch.Stop()

            Dim scalarWatch As Stopwatch = Stopwatch.StartNew()

            For k As Integer = 1 To reps
                Dim work As Double() = CType(source.Clone(), Double())

                For i As Integer = 0 To work.Length - 1
                    work(i) = std.Exp(work(i))
                Next
            Next

            scalarWatch.Stop()

            Dim simdMs As Double = watch.Elapsed.TotalMilliseconds
            Dim scalarMs As Double = scalarWatch.Elapsed.TotalMilliseconds

            Call report.AppendLine($"exp   : m={m}, {reps} rounds, simd={simdMs.ToString("F1")}ms, scalar={scalarMs.ToString("F1")}ms, speedup={((scalarMs / simdMs)).ToString("F2")}x")

            ' 原始 AVX2 循环：判断是“指令不可用”还是“库调用开销”
            Dim rawWatch As Stopwatch = Stopwatch.StartNew()

            For k As Integer = 1 To reps
                Dim work As Double() = CType(source.Clone(), Double())
                Dim acc As System.Runtime.Intrinsics.Vector256(Of Double) = System.Runtime.Intrinsics.Vector256(Of Double).Zero
                Dim scale As System.Runtime.Intrinsics.Vector256(Of Double) = System.Runtime.Intrinsics.Vector256.Create(1.0001R)

                For i As Integer = 0 To work.Length - 4 Step 4
                    Dim x As System.Runtime.Intrinsics.Vector256(Of Double) = System.Runtime.Intrinsics.Vector256.LoadUnsafe(Of Double)(work(i))

                    If System.Runtime.Intrinsics.X86.Fma.IsSupported Then
                        acc = System.Runtime.Intrinsics.X86.Fma.MultiplyAdd(x, scale, acc)
                    Else
                        acc = System.Runtime.Intrinsics.X86.Avx.Add(x, acc)
                    End If
                Next

                work(0) = System.Runtime.Intrinsics.Vector256.ToScalar(acc)
            Next

            rawWatch.Stop()

            Call report.AppendLine($"raw   : m={m}, {reps} rounds, avx2-fma loop={rawWatch.Elapsed.TotalMilliseconds.ToString("F1")}ms")

            ' 库内三个原语的开销（AXPY / 就地减 / 平方和）
            Dim acc2 As Double() = New Double(m - 1) {}
            Dim axpyWatch As Stopwatch = Stopwatch.StartNew()

            For k As Integer = 1 To reps
                Call SimdEngine.AddScaledInPlace(0.5R, source, acc2)
            Next

            axpyWatch.Stop()

            Dim subWatch As Stopwatch = Stopwatch.StartNew()

            For k As Integer = 1 To reps
                Call SimdEngine.SubtractInPlace(Of Double)(acc2, source)
            Next

            subWatch.Stop()

            Dim sumWatch As Stopwatch = Stopwatch.StartNew()

            For k As Integer = 1 To reps
                Dim v As Double = SimdReduce.SumSquares(acc2)

                acc2(0) += v * 1.0E-30
            Next

            sumWatch.Stop()

            Call report.AppendLine($"lib   : axpy={axpyWatch.Elapsed.TotalMilliseconds.ToString("F1")}ms, sub={subWatch.Elapsed.TotalMilliseconds.ToString("F1")}ms, sumsq={sumWatch.Elapsed.TotalMilliseconds.ToString("F1")}ms")
        End Sub

        ''' <summary>
        ''' 目标函数：与标量参考实现的结果一致性 + 吞吐
        ''' </summary>
        Private Sub checkObjective(report As StringBuilder, failures As List(Of String))
            Dim rnd As New Random(20261028)
            Dim k As Integer = 24
            Dim m As Integer = 80
            Dim lnX As Double()() = New Double(k - 1)() {}
            Dim observed As Double() = New Double(m - 1) {}

            For c As Integer = 0 To k - 1
                lnX(c) = New Double(m - 1) {}

                For t As Integer = 0 To m - 1
                    lnX(c)(t) = rnd.NextDouble() * 2 - 1
                Next
            Next
            For t As Integer = 0 To m - 1
                observed(t) = (rnd.NextDouble() * 2 - 1) * 0.5
            Next

            Dim p As Double() = New Double(2 + 2 * k - 1) {}

            p(0) = 0.1
            p(1) = 0.05

            For i As Integer = 2 To p.Length - 1
                p(i) = (rnd.NextDouble() * 2 - 1) * 1.5
            Next

            Dim problem As New SlopeProblem With {
                .LnX = lnX,
                .Observed = observed,
                .Scale = 1.0R,
                .L1 = 0
            }

            Dim simd As Double = problem.Objective(p)
            Dim scalar As Double = scalarObjective(lnX, observed, p)
            Dim rel As Double = std.Abs(simd - scalar) / std.Max(std.Abs(scalar), 1.0E-12)

            Call report.AppendLine($"objective: simd={simd.ToString("G10")}, scalar={scalar.ToString("G10")}, relative={rel.ToString("G3")}")

            If rel > 1.0E-6 Then
                Call failures.Add($"the SIMD objective differs from the scalar reference by {rel.ToString("G4")}")
            End If

            ' 吞吐
            Dim reps As Integer = 100000

            For i As Integer = 1 To 2000
                Call problem.Objective(p)
            Next

            Dim watch As Stopwatch = Stopwatch.StartNew()

            For i As Integer = 1 To reps
                Call problem.Objective(p)
            Next

            watch.Stop()

            Dim simdMs As Double = watch.Elapsed.TotalMilliseconds
            Dim scalarWatch As Stopwatch = Stopwatch.StartNew()

            For i As Integer = 1 To reps \ 10
                Call scalarObjective(lnX, observed, p)
            Next

            scalarWatch.Stop()

            Dim estScalarMs As Double = scalarWatch.Elapsed.TotalMilliseconds * 10

            Call report.AppendLine($"objective: {reps} evaluations, simd={simdMs.ToString("F1")}ms, scalar={estScalarMs.ToString("F1")}ms, speedup={(estScalarMs / simdMs).ToString("F2")}x")
        End Sub

        ''' <summary>
        ''' 解析梯度与中心差分梯度的一致性
        ''' </summary>
        Private Sub checkGradient(report As StringBuilder, failures As List(Of String))
            Dim rnd As New Random(424242)
            Dim k As Integer = 6
            Dim m As Integer = 40
            Dim lnX As Double()() = New Double(k - 1)() {}
            Dim observed As Double() = New Double(m - 1) {}

            For c As Integer = 0 To k - 1
                lnX(c) = New Double(m - 1) {}

                For t As Integer = 0 To m - 1
                    lnX(c)(t) = rnd.NextDouble() * 2 - 1
                Next
            Next
            For t As Integer = 0 To m - 1
                observed(t) = (rnd.NextDouble() * 2 - 1) * 0.5
            Next

            Dim p As Double() = New Double(2 + 2 * k - 1) {}
            Dim grad As Double() = New Double(p.Length - 1) {}

            For i As Integer = 0 To p.Length - 1
                p(i) = (rnd.NextDouble() * 2 - 1) * 1.2
            Next

            Dim problem As New SlopeProblem With {
                .LnX = lnX,
                .Observed = observed,
                .Scale = 1.0R,
                .L1 = 0.05R,
                .SelfIndex = 0,
                .StabilityWeight = 0.1R
            }

            Dim f As Double = problem.evaluate(p, grad)
            Dim eps As Double = 1.0E-6
            Dim maxRel As Double = 0
            Dim worst As Integer = -1

            For i As Integer = 0 To p.Length - 1
                Dim backup As Double = p(i)

                p(i) = backup + eps
                Dim f1 As Double = problem.Objective(p)
                p(i) = backup - eps
                Dim f2 As Double = problem.Objective(p)
                p(i) = backup

                Dim numeric As Double = (f1 - f2) / (2 * eps)
                Dim rel As Double = std.Abs(numeric - grad(i)) / std.Max(std.Abs(numeric) + std.Abs(grad(i)), 1.0E-8)

                If rel > maxRel Then
                    maxRel = rel
                    worst = i
                End If
            Next

            Call report.AppendLine($"gradient : f={f.ToString("G8")}, max relative deviation={maxRel.ToString("G3")} (index {worst})")

            If maxRel > 1.0E-4 Then
                Call failures.Add($"the analytic gradient deviates from the finite difference by {maxRel.ToString("G4")}")
            End If
        End Sub

        ''' <summary>
        ''' 标量参考实现（优化之前的目标函数）
        ''' </summary>
        Private Function scalarObjective(lnX As Double()(), observed As Double(), p As Double()) As Double
            Dim m As Integer = observed.Length
            Dim k As Integer = (p.Length - 2) \ 2
            Dim err As Double = 0

            For t As Integer = 0 To m - 1
                Dim la As Double = p(0)
                Dim lb As Double = p(1)

                For c As Integer = 0 To k - 1
                    Dim lx As Double = lnX(c)(t)

                    la += p(2 + c) * lx
                    lb += p(2 + k + c) * lx
                Next

                Dim a As Double = std.Exp(std.Max(std.Min(la, 700), -700))
                Dim b As Double = std.Exp(std.Max(std.Min(lb, 700), -700))
                Dim d As Double = (a - b) - observed(t)

                err += d * d
            Next

            Return std.Sqrt(err / m)
        End Function

    End Module
End Namespace
