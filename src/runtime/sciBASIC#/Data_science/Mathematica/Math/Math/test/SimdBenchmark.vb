' ============================================================================
' SimdBenchmark.vb — Vector / NumericMatrix 的标量 vs SIMD 加速比基准
' ----------------------------------------------------------------------------
' 每一项都同时测量：
'   + 手写标量实现（baseline）
'   + 生产代码路径（经 SIMD 内核）
' 并输出两者的耗时与加速比，同时打印处理器能力描述，
' 便于在 CI / 不同机器上横向对比。
' ============================================================================

Imports System.Diagnostics
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SimdCapabilities = Microsoft.VisualBasic.Math.SIMD.SimdCapabilities
Imports SimdEngine = Microsoft.VisualBasic.Math.SIMD.SimdEngine
Imports SimdParallel = Microsoft.VisualBasic.Math.SIMD.SimdParallel
Imports SimdReduce = Microsoft.VisualBasic.Math.SIMD.SimdReduce
Imports SIMDEnvironment = Microsoft.VisualBasic.Math.SIMD.SIMDEnvironment
Imports SIMDIntrinsics = Microsoft.VisualBasic.Math.SIMD.SIMDIntrinsics
Imports std = System.Math

Public Module SimdBenchmark

    ''' <summary>
    ''' 累加器：把各基准的结果累加进来，防止 JIT 把整段计算当作无用代码消除。
    ''' </summary>
    Private sink As Double = 0

    Public Function RunAll() As Integer
        Console.WriteLine("==================================================================")
        Console.WriteLine(" SIMD 加速基准（标量 baseline vs 生产 SIMD 路径）")
        Console.WriteLine("==================================================================")
        Console.WriteLine($" 处理器: {SimdCapabilities.Description}")
        Console.WriteLine($" SIMD 启用: {SIMDEnvironment.IsEnabled}")
        Console.WriteLine($" 并行阈值: {SimdParallel.MinParallelLength}")
        Console.WriteLine()

        ' 故意让长度不是向量寄存器宽度的整数倍，把尾块处理也算进去
        Const n As Integer = 1 << 20
        Dim len As Integer = n + 3

        Dim a As Double() = Fill(len, 1.25)
        Dim b As Double() = Fill(len, 0.75)

        Console.WriteLine("--- 向量内核（长度 = 1,048,579） ---")
        Bench("逐元素加法", Function() ScalarVectorAdd(a, b), Function() Checksum(SimdEngine.Add(Of Double)(a, b)))
        Bench("点积", Function() ScalarDot(a, b), Function() SimdParallel.Dot(a, b))
        Bench("平方和", Function() ScalarDot(a, a), Function() SimdReduce.SumSquares(a))
        Bench("平方和（FMA 4 路）", Function() ScalarDot(a, a), Function() SIMDIntrinsics.SumSquaresFma(a))
        Bench("点积（FMA 4 路）", Function() ScalarDot(a, b), Function() SIMDIntrinsics.DotFma(a, b))
        Bench("L2 范数", Function() std.Sqrt(ScalarDot(a, a)), Function() SimdParallel.L2Norm(a))
        Console.WriteLine()

        Console.WriteLine("--- 上层 Vector 对象（含对象构造与物化开销） ---")
        Bench("Vector + Vector", Function() ScalarVectorAdd(a, b), Function() ChecksumFromVector(New Vector(a) + New Vector(b)))
        Bench("Vector.Mod", Function() ScalarDot(a, a), Function() New Vector(a).Mod)
        Bench("Vector.SumMagnitude", Function() std.Sqrt(ScalarDot(a, a)), Function() New Vector(a).SumMagnitude)
        Console.WriteLine()

        Const order As Integer = 1024
        Dim ma As New NumericMatrix(FillSquare(order, 1.5))
        Dim mb As New NumericMatrix(FillSquare(order, 0.5))

        Console.WriteLine($"--- 矩阵运算（{order} x {order}） ---")
        Bench("逐元素加法", Function() ScalarMatrixAdd(ma, mb), Function() ChecksumFromMatrix(ma + mb))
        Bench("数乘", Function() ScalarMatrixScale(ma, 2.5), Function() ChecksumFromMatrix(ma.Multiply(2.5)))
        Bench("转置", Function() ScalarTranspose(ma), Function() ChecksumFromMatrix(ma.Transpose()))
        Bench("范数 NormInf", Function() ScalarNormInf(ma), Function() ma.NormInf())
        Bench("范数 NormF", Function() ScalarNormF(ma), Function() ma.NormF())
        Console.WriteLine()

        Const mulDim As Integer = 384
        Dim ka As New NumericMatrix(FillSquare(mulDim, 1.25))
        Dim kb As New NumericMatrix(FillSquare(mulDim, 0.75))

        Console.WriteLine($"--- 矩阵乘法（{mulDim} x {mulDim} x {mulDim}） ---")
        Bench("DotProduct", Function() ScalarMatrixProduct(ka, kb), Function() ChecksumFromMatrix(ka.Multiply(kb)))
        Console.WriteLine()

        Console.WriteLine($" （校验和 sink = {sink:G6}，仅用于阻止 JIT 死代码消除）")

        Return 0
    End Function

#Region "harness"

    Private Sub Bench(name As String, scalar As Func(Of Double), simd As Func(Of Double))
        ' 预热，同时触发相关内核的静态初始化
        sink += scalar()
        sink += simd()

        Dim scalarMs As Double = BestOf(scalar)
        Dim simdMs As Double = BestOf(simd)
        Dim speedup As Double = If(simdMs > 0, scalarMs / simdMs, 0)

        Console.WriteLine($" {name,-16} 标量 {scalarMs,9:F2} ms | SIMD {simdMs,9:F2} ms | 加速 {speedup,5:F2}x")
    End Sub

    Private Function BestOf(action As Func(Of Double)) As Double
        Const rounds As Integer = 3

        Dim best As Double = Double.MaxValue

        For i As Integer = 0 To rounds - 1
            Dim sw As Stopwatch = Stopwatch.StartNew()
            Dim r As Double = action()
            sw.Stop()

            sink += r

            If sw.Elapsed.TotalMilliseconds < best Then
                best = sw.Elapsed.TotalMilliseconds
            End If
        Next

        Return best
    End Function

#End Region

#Region "data"

    Private Function Fill(length As Integer, value As Double) As Double()
        Dim data As Double() = New Double(length - 1) {}

        For i As Integer = 0 To length - 1
            data(i) = value + (i Mod 7) * 0.001
        Next

        Return data
    End Function

    Private Function FillSquare(order As Integer, value As Double) As Double()()
        Dim m As Double()() = New Double(order - 1)() {}

        For i As Integer = 0 To order - 1
            m(i) = New Double(order - 1) {}

            For j As Integer = 0 To order - 1
                m(i)(j) = value + ((i + j) Mod 11) * 0.01
            Next
        Next

        Return m
    End Function

    ''' <summary>
    ''' 抽样求和（每 97 个取一个），用于把结果折叠成一个标量而不引入额外开销。
    ''' </summary>
    Private Function Checksum(v As Double()) As Double
        Dim s As Double = 0

        For i As Integer = 0 To v.Length - 1 Step 97
            s += v(i)
        Next

        Return s
    End Function

    Private Function ChecksumFromVector(v As Vector) As Double
        Return Checksum(v.Array)
    End Function

    Private Function ChecksumFromMatrix(m As GeneralMatrix) As Double
        Dim data As Double()() = m.ArrayPack(deepcopy:=False)
        Dim s As Double = 0

        For i As Integer = 0 To data.Length - 1 Step 31
            s += Checksum(data(i))
        Next

        Return s
    End Function

#End Region

#Region "scalar baselines"

    Private Function ScalarVectorAdd(a As Double(), b As Double()) As Double
        Dim out As Double() = New Double(a.Length - 1) {}

        For i As Integer = 0 To a.Length - 1
            out(i) = a(i) + b(i)
        Next

        Return Checksum(out)
    End Function

    Private Function ScalarDot(a As Double(), b As Double()) As Double
        Dim s As Double = 0

        For i As Integer = 0 To a.Length - 1
            s += a(i) * b(i)
        Next

        Return s
    End Function

    Private Function ScalarMatrixAdd(a As GeneralMatrix, b As GeneralMatrix) As Double
        Dim s As Double = 0

        For i As Integer = 0 To a.RowDimension - 1
            For j As Integer = 0 To a.ColumnDimension - 1
                s += a(i, j) + b(i, j)
            Next
        Next

        Return s
    End Function

    Private Function ScalarMatrixScale(a As GeneralMatrix, scale As Double) As Double
        Dim s As Double = 0

        For i As Integer = 0 To a.RowDimension - 1
            For j As Integer = 0 To a.ColumnDimension - 1
                s += a(i, j) * scale
            Next
        Next

        Return s
    End Function

    Private Function ScalarTranspose(a As GeneralMatrix) As Double
        Dim rows As Integer = a.RowDimension
        Dim cols As Integer = a.ColumnDimension
        Dim out As Double()() = New Double(cols - 1)() {}

        For j As Integer = 0 To cols - 1
            out(j) = New Double(rows - 1) {}

            For i As Integer = 0 To rows - 1
                out(j)(i) = a(i, j)
            Next
        Next

        Return Checksum(out(cols \ 2))
    End Function

    Private Function ScalarNormInf(a As GeneralMatrix) As Double
        Dim f As Double = 0

        For i As Integer = 0 To a.RowDimension - 1
            Dim s As Double = 0

            For j As Integer = 0 To a.ColumnDimension - 1
                s += std.Abs(a(i, j))
            Next

            f = std.Max(f, s)
        Next

        Return f
    End Function

    Private Function ScalarNormF(a As GeneralMatrix) As Double
        Dim f As Double = 0

        For i As Integer = 0 To a.RowDimension - 1
            For j As Integer = 0 To a.ColumnDimension - 1
                f += a(i, j) * a(i, j)
            Next
        Next

        Return std.Sqrt(f)
    End Function

    Private Function ScalarMatrixProduct(a As GeneralMatrix, b As GeneralMatrix) As Double
        Dim n As Integer = a.ColumnDimension
        Dim s As Double = 0

        For i As Integer = 0 To a.RowDimension - 1
            For j As Integer = 0 To b.ColumnDimension - 1
                Dim acc As Double = 0

                For k As Integer = 0 To n - 1
                    acc += a(i, k) * b(k, j)
                Next

                s += acc
            Next
        Next

        Return s
    End Function

#End Region

End Module
