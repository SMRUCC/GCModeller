' ------------------------------------------------------------------------
' 批量小线性求解的主机封装（ILCuda）
'
' 两类算子：
'   * CholeskySolve：对一批小方阵（对角加岭项）求多右端项的线性方程组，
'     小档（n <= 32）走共享内存内核，中档（n <= 256）走全局暂存内核
'   * BatchMaskedGram：带行掩码的 XᵀX + λI 与 XᵀY 融合计算
'     （逐基因岭回归的正规方程，避免为每个基因重复搬运设计矩阵）
'
' 退化行为与内核一致：主元 <= 0（数值上非正定）时该列因子置 0，
' 解退化为 0 而不是 NaN —— 上游的"保护性回退"依赖这一约定。
' ------------------------------------------------------------------------

Imports std = System.Math
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime

Namespace Math

    ''' <summary>批量 Cholesky 求解与掩码 Gram 计算</summary>
    Public Module GpuSolve

        ''' <summary>小档最大阶数（共享内存版内核的上界）</summary>
        Public Const SmallMaxOrder As Integer = 32
        ''' <summary>中档最大阶数（全局暂存版内核的上界）</summary>
        Public Const MediumMaxOrder As Integer = 256
        ''' <summary>低于该工作量直接走 CPU</summary>
        Public Const MinGpuElements As Integer = 1024

        ' ==================================================================
        ' 批量 Cholesky 求解
        ' ==================================================================

        ''' <summary>
        ''' 解 <c>(A + λI) X = B</c>（按 batch 逐个小方阵）。
        ''' </summary>
        ''' <param name="a">[batch, n, n] 对称正定矩阵</param>
        ''' <param name="b">[batch, n, nrhs] 右端项</param>
        ''' <param name="lambda">对角岭项（不修改输入的 A）</param>
        ''' <returns>[batch, n, nrhs] 的解</returns>
        Public Function CholeskySolve(engine As CudaEngine,
                                      a As Double(),
                                      b As Double(),
                                      batch As Integer,
                                      n As Integer,
                                      nrhs As Integer,
                                      Optional lambda As Double = 0.0) As Double()
            If a Is Nothing Then Throw New ArgumentNullException(NameOf(a))
            If b Is Nothing Then Throw New ArgumentNullException(NameOf(b))
            If batch <= 0 OrElse n <= 0 OrElse nrhs <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(batch))
            If n > MediumMaxOrder Then
                Throw New ArgumentOutOfRangeException(NameOf(n), n, $"批量 Cholesky 只支持 n <= {MediumMaxOrder}")
            End If
            If a.Length <> batch * n * n Then Throw New ArgumentException($"A 长度 {a.Length} 与 [{batch} x {n} x {n}] 不一致")
            If b.Length <> batch * n * nrhs Then Throw New ArgumentException($"B 长度 {b.Length} 与 [{batch} x {n} x {nrhs}] 不一致")

            Dim small As Boolean = n <= SmallMaxOrder
            Dim kernelName As String = If(small, KernelNames.CholSolveSmallF64, KernelNames.CholSolveMediumF64)

            If Not CanUseGpu(engine, CLng(batch) * n * n, kernelName) Then
                Return CholeskySolveCpu(a, b, batch, n, nrhs, lambda)
            End If

            Using deviceA As New DeviceBuffer(Of Double)(a.Length),
                  deviceB As New DeviceBuffer(Of Double)(b.Length),
                  deviceX As New DeviceBuffer(Of Double)(b.Length)

                deviceA.Write(a)
                deviceB.Write(b)

                If small Then
                    engine.GetKernel(KernelNames.CholSolveSmallF64).Launch(
                        batch, 1, 128, 1, n * n * 8,
                        deviceA, deviceB, deviceX, n, nrhs, lambda)

                    Return deviceX.Read()
                Else
                    Using scratch As New DeviceBuffer(Of Double)(batch * n * n)

                        engine.GetKernel(KernelNames.CholSolveMediumF64).Launch(
                            batch, 1, 256, 1, 0,
                            deviceA, deviceB, deviceX, scratch, n, nrhs, lambda)

                        Return deviceX.Read()
                    End Using
                End If
            End Using
        End Function

        ' ==================================================================
        ' 掩码 Gram 矩阵
        ' ==================================================================

        ''' <summary>
        ''' 一次算出 <c>XᵀX + λI</c> 与 <c>XᵀY</c>（按 batch，支持行掩码）。
        ''' </summary>
        ''' <param name="x">[batch, rows, f] 设计矩阵</param>
        ''' <param name="y">[batch, rows, k] 多响应</param>
        ''' <param name="mask">[batch, rows]，非 0 表示参与</param>
        Public Function BatchMaskedGram(engine As CudaEngine,
                                        x As Double(),
                                        y As Double(),
                                        mask As Integer(),
                                        batch As Integer,
                                        rows As Integer,
                                        f As Integer,
                                        k As Integer,
                                        Optional lambda As Double = 0.0) As (XX As Double(), XY As Double())
            If x Is Nothing Then Throw New ArgumentNullException(NameOf(x))
            If y Is Nothing Then Throw New ArgumentNullException(NameOf(y))
            If mask Is Nothing Then Throw New ArgumentNullException(NameOf(mask))

            Dim ok As Boolean = batch > 0 AndAlso rows > 0 AndAlso f > 0 AndAlso k > 0

            If Not ok Then Throw New ArgumentOutOfRangeException(NameOf(batch))

            If x.Length <> batch * rows * f Then Throw New ArgumentException($"X 长度 {x.Length} 与 [{batch} x {rows} x {f}] 不一致")
            If y.Length <> batch * rows * k Then Throw New ArgumentException($"Y 长度 {y.Length} 与 [{batch} x {rows} x {k}] 不一致")
            If mask.Length <> batch * rows Then Throw New ArgumentException($"mask 长度 {mask.Length} 与 [{batch} x {rows}] 不一致")

            If Not CanUseGpu(engine, CLng(batch) * rows * f * (f + k), KernelNames.GramMaskedF64) Then
                Return BatchMaskedGramCpu(x, y, mask, batch, rows, f, k, lambda)
            End If

            Using deviceX As New DeviceBuffer(Of Double)(x.Length),
                  deviceY As New DeviceBuffer(Of Double)(y.Length),
                  deviceMask As New DeviceBuffer(Of Integer)(mask.Length),
                  deviceXX As New DeviceBuffer(Of Double)(batch * f * f),
                  deviceXY As New DeviceBuffer(Of Double)(batch * f * k)

                deviceX.Write(x)
                deviceY.Write(y)
                deviceMask.Write(mask)

                engine.GetKernel(KernelNames.GramMaskedF64).Launch(
                    batch, 1, 256, 1, 0,
                    deviceX, deviceY, deviceMask, deviceXX, deviceXY, rows, f, k, lambda)

                Return (deviceXX.Read(), deviceXY.Read())
            End Using
        End Function

        ' ==================================================================
        ' CPU 参考实现
        ' ==================================================================

        ''' <summary>批量 Cholesky 求解的 CPU 参考实现（退化行为与内核一致）。</summary>
        Public Function CholeskySolveCpu(a As Double(), b As Double(),
                                         batch As Integer, n As Integer, nrhs As Integer,
                                         Optional lambda As Double = 0.0) As Double()
            Dim x(b.Length - 1) As Double
            Dim l(n * n - 1) As Double

            For item As Integer = 0 To batch - 1
                Dim aBase As Integer = item * n * n
                Dim bBase As Integer = item * n * nrhs

                Array.Clear(l, 0, l.Length)

                For i As Integer = 0 To n - 1
                    For j As Integer = 0 To i
                        l(i * n + j) = a(aBase + i * n + j) + If(i = j, lambda, 0.0)
                    Next
                Next

                FactorInPlace(l, n)

                For c As Integer = 0 To nrhs - 1
                    For i As Integer = 0 To n - 1
                        Dim s As Double = b(bBase + i * nrhs + c)

                        For j As Integer = 0 To i - 1
                            s -= l(i * n + j) * x(bBase + j * nrhs + c)
                        Next

                        x(bBase + i * nrhs + c) = If(l(i * n + i) > 0.0, s / l(i * n + i), 0.0)
                    Next

                    For i As Integer = n - 1 To 0 Step -1
                        Dim s As Double = x(bBase + i * nrhs + c)

                        For j As Integer = i + 1 To n - 1
                            s -= l(j * n + i) * x(bBase + j * nrhs + c)
                        Next

                        x(bBase + i * nrhs + c) = If(l(i * n + i) > 0.0, s / l(i * n + i), 0.0)
                    Next
                Next
            Next

            Return x
        End Function

        ''' <summary>掩码 Gram 的 CPU 参考实现。</summary>
        Public Function BatchMaskedGramCpu(x As Double(), y As Double(), mask As Integer(),
                                           batch As Integer, rows As Integer, f As Integer, k As Integer,
                                           Optional lambda As Double = 0.0) As (XX As Double(), XY As Double())
            Dim xx(batch * f * f - 1) As Double
            Dim xy(batch * f * k - 1) As Double

            For item As Integer = 0 To batch - 1
                Dim xBase As Integer = item * rows * f
                Dim yBase As Integer = item * rows * k
                Dim mBase As Integer = item * rows
                Dim xxBase As Integer = item * f * f
                Dim xyBase As Integer = item * f * k

                For r As Integer = 0 To rows - 1
                    If mask(mBase + r) = 0 Then Continue For

                    For a As Integer = 0 To f - 1
                        Dim xa As Double = x(xBase + r * f + a)

                        For b As Integer = 0 To f - 1
                            xx(xxBase + a * f + b) += xa * x(xBase + r * f + b)
                        Next

                        For c As Integer = 0 To k - 1
                            xy(xyBase + a * k + c) += xa * y(yBase + r * k + c)
                        Next
                    Next
                Next

                For a As Integer = 0 To f - 1
                    xx(xxBase + a * f + a) += lambda
                Next
            Next

            Return (xx, xy)
        End Function

        ' ==================================================================
        ' 内部工具
        ' ==================================================================

        ''' <summary>就地 Cholesky：下三角有效，主元非正时该列置 0。</summary>
        Private Sub FactorInPlace(l As Double(), n As Integer)
            For k As Integer = 0 To n - 1
                Dim s As Double = l(k * n + k)

                For j As Integer = 0 To k - 1
                    s -= l(k * n + j) * l(k * n + j)
                Next

                Dim pivot As Double = If(s > 0.0, std.Sqrt(s), 0.0)

                l(k * n + k) = pivot

                For i As Integer = k + 1 To n - 1
                    Dim t As Double = l(i * n + k)

                    For j As Integer = 0 To k - 1
                        t -= l(i * n + j) * l(k * n + j)
                    Next

                    l(i * n + k) = If(pivot > 0.0, t / pivot, 0.0)
                Next
            Next
        End Sub

        ''' <summary>工作量过小、引擎缺失或内核缺失时走 CPU。</summary>
        Public Function CanUseGpu(engine As CudaEngine, elements As Long, kernelName As String) As Boolean
            If engine Is Nothing Then Return False
            If elements < MinGpuElements Then Return False

            Dim kernel As CudaKernel = Nothing

            Return engine.TryGetKernel(kernelName, kernel)
        End Function
    End Module
End Namespace
