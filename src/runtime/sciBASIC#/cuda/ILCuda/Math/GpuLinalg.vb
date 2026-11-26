' ------------------------------------------------------------------------
' 双精度线性代数的主机封装（ILCuda）
'
' 覆盖复现 CellularAI 残差模型所需的两个原语：
'   * GEMM（带 transA / transB 标志）：随机化 SVD 的 A·Ω、AᵀQ、投影与重构
'   * economy QR：随机化范围查找里的正交化步骤
'
' l x l 的小规模对称特征分解刻意留在主机：单 block 的 Jacobi 扫掠在 GPU 上
' 反而比 CPU 慢一个量级，而 l <= 168 时 CPU 是微秒级。
'
' 所有入口都带 CPU 参考实现（回退 + 审计基准）。
' ------------------------------------------------------------------------

Imports std = System.Math
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime

Namespace Math

    ''' <summary>双精度矩阵乘、矩阵-向量乘与 economy QR</summary>
    Public Module GpuLinalg

        ''' <summary>GEMM 的分块尺寸（必须与 linalg.cu 的 ILCUDA_GEMM_TILE 一致）</summary>
        Public Const GemmTile As Integer = 16

        ''' <summary>QR 内核的 block 尺寸（必须与 linalg.cu 的 ILCUDA_QR_BLOCK 一致）</summary>
        Public Const QrBlockSize As Integer = 512

        ''' <summary>低于该规模直接走 CPU</summary>
        Public Const MinGpuElements As Integer = 4096

        ' ==================================================================
        ' GEMM
        ' ==================================================================

        ''' <summary>
        ''' C(m x n) = op(A) * op(B)。
        ''' <paramref name="transA"/> 为真时 A 以 k x m 存放，为假时以 m x k 存放；
        ''' <paramref name="transB"/> 同理（k x n / n x k）。
        ''' </summary>
        Public Sub Gemm(engine As CudaEngine,
                        a As DeviceBuffer(Of Double),
                        b As DeviceBuffer(Of Double),
                        c As DeviceBuffer(Of Double),
                        m As Integer,
                        n As Integer,
                        k As Integer,
                        transA As Boolean,
                        transB As Boolean,
                        Optional stream As CudaStream = Nothing)
            If engine Is Nothing Then Throw New ArgumentNullException(NameOf(engine))

            RequireSize("A", a, If(transA, k * m, m * k))
            RequireSize("B", b, If(transB, n * k, k * n))
            RequireSize("C", c, m * n)

            Dim config = LaunchPlanner.For2D(m, n, GemmTile, GemmTile, 0)

            engine.GetKernel(KernelNames.GemmF64).Launch(
                stream, config, a, b, c, m, n, k, If(transA, 1, 0), If(transB, 1, 0))
        End Sub

        ''' <summary>主机数组入口：C(m x n) = op(A) * op(B)。</summary>
        Public Function Gemm(engine As CudaEngine,
                             a As Double(),
                             b As Double(),
                             m As Integer,
                             n As Integer,
                             k As Integer,
                             Optional transA As Boolean = False,
                             Optional transB As Boolean = False) As Double()
            If a Is Nothing Then Throw New ArgumentNullException(NameOf(a))
            If b Is Nothing Then Throw New ArgumentNullException(NameOf(b))
            If m <= 0 OrElse n <= 0 OrElse k <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(m))

            If Not CanUseGpu(engine, CLng(m) * n * k, KernelNames.GemmF64) Then
                Return GemmCpu(a, b, m, n, k, transA, transB)
            End If

            Using deviceA As New DeviceBuffer(Of Double)(a.Length),
                  deviceB As New DeviceBuffer(Of Double)(b.Length),
                  deviceC As New DeviceBuffer(Of Double)(m * n)

                deviceA.Write(a)
                deviceB.Write(b)

                Gemm(engine, deviceA, deviceB, deviceC, m, n, k, transA, transB)

                Return deviceC.Read()
            End Using
        End Function

        ' ==================================================================
        ' GEMV
        ' ==================================================================

        ''' <summary>y(m) = A(m x k) * x(k)。</summary>
        Public Function Gemv(engine As CudaEngine, a As Double(), x As Double(),
                             m As Integer, k As Integer) As Double()
            If a Is Nothing Then Throw New ArgumentNullException(NameOf(a))
            If x Is Nothing Then Throw New ArgumentNullException(NameOf(x))
            If m <= 0 OrElse k <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(m))
            If a.Length <> m * k Then Throw New ArgumentException($"A 长度 {a.Length} 与 [{m} x {k}] 不一致")
            If x.Length <> k Then Throw new ArgumentException($"x 长度 {x.Length} 与 {k} 不一致")

            If Not CanUseGpu(engine, CLng(m) * k, KernelNames.GemvF64) Then
                Return GemvCpu(a, x, m, k)
            End If

            Using deviceA As New DeviceBuffer(Of Double)(a.Length),
                  deviceX As New DeviceBuffer(Of Double)(k),
                  deviceY As New DeviceBuffer(Of Double)(m)

                deviceA.Write(a)
                deviceX.Write(x)

                engine.GetKernel(KernelNames.GemvF64).Launch(
                    m, 1, GpuBlas.GemvBlockSize, 1, GpuBlas.GemvBlockSize * 8,
                    deviceA, deviceX, deviceY, m, k)

                Return deviceY.Read()
            End Using
        End Function

        ' ==================================================================
        ' economy QR
        ' ==================================================================

        ''' <summary>
        ''' economy QR：A(m x n) = Q(m x n) * R(n x n)，要求 m >= n。
        ''' </summary>
        Public Function QrEconomy(engine As CudaEngine, a As Double(), m As Integer, n As Integer) As (Q As Double(), R As Double())
            If a Is Nothing Then Throw New ArgumentNullException(NameOf(a))
            If m <= 0 OrElse n <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(m))
            If m < n Then Throw New ArgumentException($"economy QR 要求 m >= n，实际 [{m} x {n}]")
            If a.Length <> m * n Then Throw New ArgumentException($"A 长度 {a.Length} 与 [{m} x {n}] 不一致")

            If Not CanUseGpu(engine, CLng(m) * n, KernelNames.QrHouseholderF64) Then
                Return QrEconomyCpu(a, m, n)
            End If

            Using deviceA As New DeviceBuffer(Of Double)(m * n),
                  v0 As New DeviceBuffer(Of Double)(n),
                  betas As New DeviceBuffer(Of Double)(n),
                  deviceQ As New DeviceBuffer(Of Double)(m * n),
                  deviceR As New DeviceBuffer(Of Double)(n * n)

                deviceA.Write(a)

                engine.GetKernel(KernelNames.QrHouseholderF64).Launch(
                    1, 1, QrBlockSize, 1, 0,
                    deviceA, m, n, v0, betas, deviceQ, deviceR)

                Return (deviceQ.Read(), deviceR.Read())
            End Using
        End Function

        ' ==================================================================
        ' CPU 参考实现
        ' ==================================================================

        ''' <summary>GEMM 的 CPU 参考实现（逐元素三循环）。</summary>
        Public Function GemmCpu(a As Double(), b As Double(),
                                m As Integer, n As Integer, k As Integer,
                                Optional transA As Boolean = False,
                                Optional transB As Boolean = False) As Double()
            Dim c(m * n - 1) As Double

            For i As Integer = 0 To m - 1
                For p As Integer = 0 To k - 1
                    Dim av As Double = If(transA, a(p * m + i), a(i * k + p))

                    For j As Integer = 0 To n - 1
                        Dim bv As Double = If(transB, b(j * k + p), b(p * n + j))

                        c(i * n + j) += av * bv
                    Next
                Next
            Next

            Return c
        End Function

        ''' <summary>GEMV 的 CPU 参考实现。</summary>
        Public Function GemvCpu(a As Double(), x As Double(), m As Integer, k As Integer) As Double()
            Dim y(m - 1) As Double

            For i As Integer = 0 To m - 1
                Dim acc As Double = 0.0

                For p As Integer = 0 To k - 1
                    acc += a(i * k + p) * x(p)
                Next

                y(i) = acc
            Next

            Return y
        End Function

        ''' <summary>
        ''' economy QR 的 CPU 参考实现：改进的 Gram-Schmidt。
        ''' 与内核的 Householder 是两条完全不同的路径，因此可用于对拍
        ''' （Q 的符号约定可能相差一列符号，比较时看重构误差与 |R|）。
        ''' </summary>
        Public Function QrEconomyCpu(a As Double(), m As Integer, n As Integer) As (Q As Double(), R As Double())
            Dim q(m * n - 1) As Double
            Dim r(n * n - 1) As Double
            Dim v(m - 1) As Double

            For col As Integer = 0 To n - 1
                For i As Integer = 0 To m - 1
                    v(i) = a(i * n + col)
                Next

                For prev As Integer = 0 To col - 1
                    Dim dot As Double = 0.0

                    For i As Integer = 0 To m - 1
                        dot += q(i * n + prev) * v(i)
                    Next

                    r(prev * n + col) = dot

                    For i As Integer = 0 To m - 1
                        v(i) -= dot * q(i * n + prev)
                    Next
                Next

                Dim norm As Double = 0.0

                For i As Integer = 0 To m - 1
                    norm += v(i) * v(i)
                Next

                norm = std.Sqrt(norm)
                r(col * n + col) = norm

                If norm > 0.0 Then
                    For i As Integer = 0 To m - 1
                        q(i * n + col) = v(i) / norm
                    Next
                Else
                    For i As Integer = 0 To m - 1
                        q(i * n + col) = 0.0
                    Next
                End If
            Next

            Return (q, r)
        End Function

        ' ==================================================================
        ' 内部工具
        ' ==================================================================

        ''' <summary>规模过小、引擎缺失或内核缺失时走 CPU。</summary>
        Public Function CanUseGpu(engine As CudaEngine, elements As Long, kernelName As String) As Boolean
            If engine Is Nothing Then Return False
            If elements < MinGpuElements Then Return False

            Dim kernel As CudaKernel = Nothing

            Return engine.TryGetKernel(kernelName, kernel)
        End Function

        Private Sub RequireSize(name As String, buffer As DeviceBuffer(Of Double), expected As Integer)
            If buffer Is Nothing Then Throw New ArgumentNullException(name)
            If buffer.Count <> expected Then
                Throw New ArgumentException($"{name} 长度 {buffer.Count} 与预期 {expected} 不一致")
            End If
        End Sub
    End Module
End Namespace
