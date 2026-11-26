' ------------------------------------------------------------------------
' 三维加权池化的主机封装（ILCudaTensor / CellularAI）
'
' 因子表达式（"sef*kd"、"inv:mad:dl" 这类算子串）在主机侧求值成稠密因子向量：
'   Row [T, G]、Col [T, P]、Mat [T]
' 内核只做 row * col * mat 的乘加，不做表达式解析。
'
' 权重语义（与上游一致）：
'   w = row[t,g] * col[t,p] * mat[t]
'   未测项（effect 为 NaN）按"值为 0、权重照算"计入分母
'
' 产出：DerSimonian-Laird 的 S0/S1/S2/SQ/K 充分统计量、
'       逆方差合并的 log2FC 加权均值、Stouffer 的 Σwz 与 Σw²。
' ------------------------------------------------------------------------

Imports ILCudaRuntime = Microsoft.VisualBasic.Computing.ILCuda.Runtime

Namespace GPUTensor

    ''' <summary>一条汇总记录（训练行）在基因轴与扰动轴上的权重因子</summary>
    Public Class PoolFactors

        ''' <summary>[T, G] 行方向（基因维度）的因子</summary>
        Public Property Row As Double()

        ''' <summary>[T, P] 列方向（扰动维度）的因子</summary>
        Public Property Col As Double()

        ''' <summary>[T] 整行因子</summary>
        Public Property Mat As Double()

        ''' <summary>全 1 因子（等价于不施加任何额外权重）</summary>
        Public Shared Function Identity(t As Integer, g As Integer, p As Integer) As PoolFactors
            Dim row(t * g - 1) As Double
            Dim col(t * p - 1) As Double
            Dim mat(t - 1) As Double

            For i As Integer = 0 To row.Length - 1
                row(i) = 1.0
            Next

            For i As Integer = 0 To col.Length - 1
                col(i) = 1.0
            Next

            For i As Integer = 0 To mat.Length - 1
                mat(i) = 1.0
            Next

            Return New PoolFactors With {.Row = row, .Col = col, .Mat = mat}
        End Function

        ''' <summary>校验形状是否与 (T, G, P) 一致。</summary>
        Friend Sub Validate(name As String, t As Integer, g As Integer, p As Integer)
            If Row Is Nothing OrElse Row.Length <> t * g Then
                Throw New ArgumentException($"{name}.Row 长度应为 [{t} x {g}]")
            End If
            If Col Is Nothing OrElse Col.Length <> t * p Then
                Throw New ArgumentException($"{name}.Col 长度应为 [{t} x {p}]")
            End If
            If Mat Is Nothing OrElse Mat.Length <> t Then
                Throw New ArgumentException($"{name}.Mat 长度应为 [{t}]")
            End If
        End Sub
    End Class

    ''' <summary>池化的完整结果（按 [G, P] 连续存放）</summary>
    Public Class PooledStats

        ''' <summary>Σw</summary>
        Public Property S0 As Double()
        ''' <summary>Σw·y</summary>
        Public Property S1 As Double()
        ''' <summary>Σw·y²</summary>
        Public Property S2 As Double()
        ''' <summary>Σw²</summary>
        Public Property SQ As Double()
        ''' <summary>参与的行数（实测项）</summary>
        Public Property Measured As Integer()
        ''' <summary>log2FC 的加权均值（S0 &lt;= 0 处为 NaN）</summary>
        Public Property L2 As Double()
        ''' <summary>合并后的 |z|（Stouffer）</summary>
        Public Property Z As Double()
        ''' <summary>Σw·z（只累加实测项）</summary>
        Public Property NumZ As Double()
        ''' <summary>Σw²（z 通道）</summary>
        Public Property SqwZ As Double()

        ''' <summary>
        ''' 有效行数 (Σw)² / Σw² —— 权重集中度的诊断量。
        ''' 与"参与行数"不同：权重若高度集中在少数行，
        ''' 有效行数会远小于参与行数，此时合并结果的可信度要打折。
        ''' </summary>
        Public Function EffectiveRows() As Double()
            Dim result(S0.Length - 1) As Double

            For i As Integer = 0 To result.Length - 1
                Dim q As Double = SQ(i)

                result(i) = If(q > 0.0, S0(i) * S0(i) / q, 0.0)
            Next

            Return result
        End Function
    End Class

    ''' <summary>三维加权池化：充分统计量、合并结果与剖面标准化</summary>
    Public Module CellPooling

        ''' <summary>内核每 block 的线程数（线程铺在扰动轴上）</summary>
        Public Const DefaultBlockSize As Integer = 256

        ''' <summary>低于该元素数直接走 CPU</summary>
        Public Const MinGpuElements As Long = 65536

        ' ==================================================================
        ' 池化
        ' ==================================================================

        ''' <summary>
        ''' 对 [T, G, P] 的效应与 z 做一次加权池化。
        ''' </summary>
        ''' <param name="effect">[T, G, P] 已中心化的 log2FC，NaN 表示未测</param>
        ''' <param name="z">[T, G, P] 带符号的正态分数，NaN 表示未测</param>
        ''' <param name="l2Factor">log2FC 通道的权重因子</param>
        ''' <param name="zFactor">z 通道的权重因子（可以与 l2 不同）</param>
        Public Function Pool(engine As ILCudaRuntime.CudaEngine,
                             effect As Double(),
                             z As Double(),
                             l2Factor As PoolFactors,
                             zFactor As PoolFactors,
                             t As Integer,
                             g As Integer,
                             p As Integer) As PooledStats
            ValidateArgs(effect, z, l2Factor, zFactor, t, g, p)

            If Not CanUseGpu(engine, CLng(t) * g * p, TensorKernelNames.CellPoolStats) Then
                Return PoolCpu(effect, z, l2Factor, zFactor, t, g, p)
            End If

            Dim size As Integer = g * p

            Using deviceEffect As New ILCudaRuntime.DeviceBuffer(Of Double)(effect.Length),
                  deviceZ As New ILCudaRuntime.DeviceBuffer(Of Double)(z.Length),
                  l2Row As New ILCudaRuntime.DeviceBuffer(Of Double)(l2Factor.Row.Length),
                  l2Col As New ILCudaRuntime.DeviceBuffer(Of Double)(l2Factor.Col.Length),
                  l2Mat As New ILCudaRuntime.DeviceBuffer(Of Double)(l2Factor.Mat.Length),
                  zRow As New ILCudaRuntime.DeviceBuffer(Of Double)(zFactor.Row.Length),
                  zCol As New ILCudaRuntime.DeviceBuffer(Of Double)(zFactor.Col.Length),
                  zMat As New ILCudaRuntime.DeviceBuffer(Of Double)(zFactor.Mat.Length),
                  s0 As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  s1 As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  s2 As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  sq As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  measured As New ILCudaRuntime.DeviceBuffer(Of Integer)(size),
                  l2 As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  zOut As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  numZ As New ILCudaRuntime.DeviceBuffer(Of Double)(size),
                  sqwZ As New ILCudaRuntime.DeviceBuffer(Of Double)(size)

                deviceEffect.Write(effect)
                deviceZ.Write(z)
                l2Row.Write(l2Factor.Row)
                l2Col.Write(l2Factor.Col)
                l2Mat.Write(l2Factor.Mat)

                engine.GetKernel(TensorKernelNames.CellPoolStats).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, DefaultBlockSize, 1, 0),
                    deviceEffect, l2Row, l2Col, l2Mat, t, g, p,
                    s0, s1, s2, sq, measured)

                zRow.Write(zFactor.Row)
                zCol.Write(zFactor.Col)
                zMat.Write(zFactor.Mat)

                engine.GetKernel(TensorKernelNames.CellPoolEffects).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, DefaultBlockSize, 1, 0),
                    deviceZ, s0, s1, zRow, zCol, zMat, t, g, p,
                    l2, zOut, numZ, sqwZ)

                Return New PooledStats With {
                    .S0 = s0.Read(),
                    .S1 = s1.Read(),
                    .S2 = s2.Read(),
                    .SQ = sq.Read(),
                    .Measured = measured.Read(),
                    .L2 = l2.Read(),
                    .Z = zOut.Read(),
                    .NumZ = numZ.Read(),
                    .SqwZ = sqwZ.Read()
                }
            End Using
        End Function

        ' ==================================================================
        ' 剖面标准化
        ' ==================================================================

        ''' <summary>逐元素标准化：out = (input - center) / scale（scale &lt;= 0 处取 0）。</summary>
        Public Function Profile(engine As ILCudaRuntime.CudaEngine,
                                input As Double(),
                                center As Double(),
                                scale As Double()) As Double()
            If input Is Nothing Then Throw New ArgumentNullException(NameOf(input))
            If center Is Nothing OrElse scale Is Nothing Then Throw New ArgumentNullException(NameOf(center))
            If center.Length <> input.Length OrElse scale.Length <> input.Length Then
                Throw New ArgumentException("input / center / scale 的长度必须一致")
            End If

            If Not CanUseGpu(engine, input.Length, TensorKernelNames.CellPoolProfile) Then
                Return ProfileCpu(input, center, scale)
            End If

            Using deviceInput As New ILCudaRuntime.DeviceBuffer(Of Double)(input.Length),
                  deviceCenter As New ILCudaRuntime.DeviceBuffer(Of Double)(center.Length),
                  deviceScale As New ILCudaRuntime.DeviceBuffer(Of Double)(scale.Length),
                  output As New ILCudaRuntime.DeviceBuffer(Of Double)(input.Length)

                deviceInput.Write(input)
                deviceCenter.Write(center)
                deviceScale.Write(scale)

                engine.GetKernel(TensorKernelNames.CellPoolProfile).Launch1D(
                    input.Length, DefaultBlockSize, 0,
                    deviceInput, deviceCenter, deviceScale, input.Length, output)

                Return output.Read()
            End Using
        End Function

        ' ==================================================================
        ' CPU 参考实现（回退 + 审计基准）
        ' ==================================================================

        ''' <summary>池化的 CPU 参考实现，语义与内核逐项对应。</summary>
        Public Function PoolCpu(effect As Double(), z As Double(),
                                l2Factor As PoolFactors, zFactor As PoolFactors,
                                t As Integer, g As Integer, p As Integer) As PooledStats
            ValidateArgs(effect, z, l2Factor, zFactor, t, g, p)

            Dim size As Integer = g * p
            Dim s0(size - 1) As Double
            Dim s1(size - 1) As Double
            Dim s2(size - 1) As Double
            Dim sq(size - 1) As Double
            Dim measured(size - 1) As Integer
            Dim l2(size - 1) As Double
            Dim zOut(size - 1) As Double
            Dim numZ(size - 1) As Double
            Dim sqwZ(size - 1) As Double

            For gi As Integer = 0 To g - 1
                For pi As Integer = 0 To p - 1
                    Dim index As Integer = gi * p + pi
                    Dim a0 As Double = 0.0
                    Dim a1 As Double = 0.0
                    Dim a2 As Double = 0.0
                    Dim aq As Double = 0.0
                    Dim count As Integer = 0
                    Dim nz As Double = 0.0
                    Dim qz As Double = 0.0

                    For ti As Integer = 0 To t - 1
                        Dim w As Double = l2Factor.Row(ti * g + gi) * l2Factor.Col(ti * p + pi) * l2Factor.Mat(ti)
                        Dim y As Double = effect((ti * g + gi) * p + pi)
                        Dim missing As Boolean = Double.IsNaN(y)
                        Dim value As Double = If(missing, 0.0, y)

                        a0 += w
                        a1 += w * value
                        a2 += w * value * value
                        aq += w * w

                        If Not missing Then count += 1

                        Dim zv As Double = z((ti * g + gi) * p + pi)

                        If Not Double.IsNaN(zv) Then
                            Dim wz As Double = zFactor.Row(ti * g + gi) * zFactor.Col(ti * p + pi) * zFactor.Mat(ti)

                            nz += wz * zv
                            qz += wz * wz
                        End If
                    Next

                    s0(index) = a0
                    s1(index) = a1
                    s2(index) = a2
                    sq(index) = aq
                    measured(index) = count
                    l2(index) = If(a0 > 0.0, a1 / a0, Double.NaN)
                    numZ(index) = nz
                    sqwZ(index) = qz
                    zOut(index) = If(qz > 0.0, System.Math.Abs(nz) / System.Math.Sqrt(qz), 0.0)
                Next
            Next

            Return New PooledStats With {
                .S0 = s0,
                .S1 = s1,
                .S2 = s2,
                .SQ = sq,
                .Measured = measured,
                .L2 = l2,
                .Z = zOut,
                .NumZ = numZ,
                .SqwZ = sqwZ
            }
        End Function

        ''' <summary>剖面标准化的 CPU 参考实现。</summary>
        Public Function ProfileCpu(input As Double(), center As Double(), scale As Double()) As Double()
            Dim result(input.Length - 1) As Double

            For i As Integer = 0 To result.Length - 1
                result(i) = If(scale(i) > 0.0, (input(i) - center(i)) / scale(i), 0.0)
            Next

            Return result
        End Function

        ' ==================================================================
        ' 内部工具
        ' ==================================================================

        Friend Function CanUseGpu(engine As ILCudaRuntime.CudaEngine, elements As Long, kernelName As String) As Boolean
            If engine Is Nothing Then Return False
            If elements < MinGpuElements Then Return False

            Dim kernel As ILCudaRuntime.CudaKernel = Nothing

            Return engine.TryGetKernel(kernelName, kernel)
        End Function

        Private Sub ValidateArgs(effect As Double(), z As Double(),
                                 l2Factor As PoolFactors, zFactor As PoolFactors,
                                 t As Integer, g As Integer, p As Integer)
            If effect Is Nothing Then Throw New ArgumentNullException(NameOf(effect))
            If z Is Nothing Then Throw New ArgumentNullException(NameOf(z))
            If l2Factor Is Nothing Then Throw New ArgumentNullException(NameOf(l2Factor))
            If zFactor Is Nothing Then Throw New ArgumentNullException(NameOf(zFactor))
            If t <= 0 OrElse g <= 0 OrElse p <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(t))
            If effect.Length <> t * g * p Then
                Throw New ArgumentException($"effect 长度 {effect.Length} 与 [{t} x {g} x {p}] 不一致")
            End If
            If z.Length <> effect.Length Then
                Throw New ArgumentException($"z 长度 {z.Length} 与 effect 的 {effect.Length} 不一致")
            End If

            l2Factor.Validate(NameOf(l2Factor), t, g, p)
            zFactor.Validate(NameOf(zFactor), t, g, p)
        End Sub
    End Module
End Namespace
