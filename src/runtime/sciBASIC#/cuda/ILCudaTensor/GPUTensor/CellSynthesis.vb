' ------------------------------------------------------------------------
' 单细胞计数合成的主机封装（ILCudaTensor / CellularAI）
'
' 五个算子按合成流程的顺序排列：
'   1. CopulaQuantiles ：Gaussian copula 秩混合 → (i+0.5)/n 分位与降序名次
'   2. QuantileMass    ：分位锚点取值 + 整数质量守恒（每个基因的总量精确）
'   3. WilcoxonZ       ：精确秩和 z（float32 栅格、tie 项、合并方差）
'   4. ShiftBisect     ：逐基因平移二分，把 z 推到目标值
'   5. Burst           ：低表达基因按目标库大小随机散布
'
' 每个算子都有逐位对齐的 CPU 参考实现：
' 参考实现既用于 GPU 不可用时的回退，也是审计的判据来源
' （计数矩阵要求逐元素整数完全相等）。
' ------------------------------------------------------------------------

Imports ILCudaRuntime = Microsoft.VisualBasic.Computing.ILCuda.Runtime
Imports ILCudaMath = Microsoft.VisualBasic.Computing.ILCuda.Math

Namespace GPUTensor

    ''' <summary>copula 分位结果</summary>
    Public Class CopulaResult
        ''' <summary>[G, n] 每个细胞的 (i+0.5)/n 分位锚点</summary>
        Public Property Quantiles As Double()
        ''' <summary>[G, n] 降序名次（供质量守恒的补足顺序使用）</summary>
        Public Property Ordering As Integer()
    End Class

    ''' <summary>平移二分的结果</summary>
    Public Class BisectResult
        ''' <summary>[G, n] 整数计数</summary>
        Public Property Counts As Integer()
        ''' <summary>[G] 收敛到的平移量</summary>
        Public Property Shift As Double()
        ''' <summary>[G] 最终实现的 z</summary>
        Public Property Z As Double()
    End Class

    ''' <summary>count-based 单细胞合成算子</summary>
    Public Module CellSynthesis

        ''' <summary>内核每 block 的线程数</summary>
        Public Const DefaultBlockSize As Integer = 256

        ''' <summary>低于该规模直接走 CPU</summary>
        Public Const MinGpuElements As Long = 1024

        ' ==================================================================
        ' 1) copula 分位
        ' ==================================================================

        ''' <summary>
        ''' 把独立噪声与库大小的秩得分按相关系数混合，再转成分位锚点与降序名次。
        ''' </summary>
        ''' <param name="e0">[G, n] 独立标准正态噪声</param>
        ''' <param name="zl">[n] 目标秩得分（通常是库大小的正态分数）</param>
        Public Function CopulaQuantiles(engine As ILCudaRuntime.CudaEngine,
                                        e0 As Double(),
                                        zl As Double(),
                                        correlation As Double,
                                        g As Integer,
                                        n As Integer) As CopulaResult
            ValidateCopula(e0, zl, g, n)

            If Not CanUseGpu(engine, CLng(g) * n, TensorKernelNames.CellSynthCopulaQuantile) Then
                Return CopulaQuantilesCpu(e0, zl, correlation, g, n)
            End If

            Using deviceE0 As New ILCudaRuntime.DeviceBuffer(Of Double)(e0.Length),
                  deviceZl As New ILCudaRuntime.DeviceBuffer(Of Double)(n),
                  quantiles As New ILCudaRuntime.DeviceBuffer(Of Double)(g * n),
                  ordering As New ILCudaRuntime.DeviceBuffer(Of Integer)(g * n)

                deviceE0.Write(e0)
                deviceZl.Write(zl)

                engine.GetKernel(TensorKernelNames.CellSynthCopulaQuantile).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, DefaultBlockSize, 1, 0),
                    deviceE0, deviceZl, correlation, n, quantiles, ordering)

                Return New CopulaResult With {
                    .Quantiles = quantiles.Read(),
                    .Ordering = ordering.Read()
                }
            End Using
        End Function

        ' ==================================================================
        ' 2) 分位取值 + 质量守恒
        ' ==================================================================

        ''' <summary>
        ''' 按分位锚点生成整数计数，并把每个基因的总量精确调整到 <paramref name="totals"/>。
        ''' </summary>
        ''' <param name="ctrl">[G, m] 每个基因已升序排序的对照 CPM（float32 栅格）</param>
        Public Function QuantileMass(engine As ILCudaRuntime.CudaEngine,
                                     ctrl As Single(),
                                     q As Double(),
                                     ordq As Integer(),
                                     libs As Double(),
                                     totals As Long(),
                                     shift As Double(),
                                     g As Integer,
                                     n As Integer,
                                     m As Integer,
                                     ntop As Integer) As Integer()
            ValidateQuantileMass(ctrl, q, ordq, libs, totals, shift, g, n, m)

            If Not CanUseGpu(engine, CLng(g) * n, TensorKernelNames.CellSynthQuantileMass) Then
                Return QuantileMassCpu(ctrl, q, ordq, libs, totals, shift, g, n, m, ntop)
            End If

            Using deviceCtrl As New ILCudaRuntime.DeviceBuffer(Of Single)(ctrl.Length),
                  deviceQ As New ILCudaRuntime.DeviceBuffer(Of Double)(q.Length),
                  deviceOrdq As New ILCudaRuntime.DeviceBuffer(Of Integer)(ordq.Length),
                  deviceLibs As New ILCudaRuntime.DeviceBuffer(Of Double)(libs.Length),
                  deviceTotals As New ILCudaRuntime.DeviceBuffer(Of Long)(totals.Length),
                  deviceShift As New ILCudaRuntime.DeviceBuffer(Of Double)(shift.Length),
                  counts As New ILCudaRuntime.DeviceBuffer(Of Integer)(g * n)

                deviceCtrl.Write(ctrl)
                deviceQ.Write(q)
                deviceOrdq.Write(ordq)
                deviceLibs.Write(libs)
                deviceTotals.Write(totals)
                deviceShift.Write(shift)

                engine.GetKernel(TensorKernelNames.CellSynthQuantileMass).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, DefaultBlockSize, 1, 0),
                    deviceCtrl, deviceQ, deviceOrdq, deviceLibs, deviceTotals, deviceShift,
                    n, m, ntop, counts)

                Return counts.Read()
            End Using
        End Function

        ' ==================================================================
        ' 3) 精确秩和 z
        ' ==================================================================

        ''' <summary>对每个基因计算相对对照组的精确秩和 z。</summary>
        Public Function WilcoxonZ(engine As ILCudaRuntime.CudaEngine,
                                  counts As Integer(),
                                  libs As Double(),
                                  ctrl As Single(),
                                  tieSum As Double(),
                                  g As Integer,
                                  n As Integer,
                                  m As Integer) As Double()
            ValidateWilcoxon(counts, libs, ctrl, tieSum, g, n, m)

            If Not CanUseGpu(engine, CLng(g) * n, TensorKernelNames.CellSynthWilcoxonZ) Then
                Return WilcoxonZCpu(counts, libs, ctrl, tieSum, g, n, m)
            End If

            Using deviceCounts As New ILCudaRuntime.DeviceBuffer(Of Integer)(counts.Length),
                  deviceLibs As New ILCudaRuntime.DeviceBuffer(Of Double)(libs.Length),
                  deviceCtrl As New ILCudaRuntime.DeviceBuffer(Of Single)(ctrl.Length),
                  deviceTie As New ILCudaRuntime.DeviceBuffer(Of Double)(tieSum.Length),
                  output As New ILCudaRuntime.DeviceBuffer(Of Double)(g)

                deviceCounts.Write(counts)
                deviceLibs.Write(libs)
                deviceCtrl.Write(ctrl)
                deviceTie.Write(tieSum)

                engine.GetKernel(TensorKernelNames.CellSynthWilcoxonZ).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, DefaultBlockSize, 1, 0),
                    deviceCounts, deviceLibs, deviceCtrl, deviceTie, n, m, output)

                Return output.Read()
            End Using
        End Function

        ' ==================================================================
        ' 4) 平移二分
        ' ==================================================================

        ''' <summary>逐基因二分平移量，使秩和 z 命中 <paramref name="target"/>。</summary>
        Public Function ShiftBisect(engine As ILCudaRuntime.CudaEngine,
                                    ctrl As Single(),
                                    tieSum As Double(),
                                    q As Double(),
                                    ordq As Integer(),
                                    libs As Double(),
                                    totals As Long(),
                                    target As Double(),
                                    g As Integer,
                                    n As Integer,
                                    m As Integer,
                                    ntop As Integer,
                                    maxIterations As Integer,
                                    tolerance As Double,
                                    lower As Double,
                                    upper As Double) As BisectResult
            ValidateBisect(ctrl, tieSum, q, ordq, libs, totals, target, g, n, m)

            If Not CanUseGpu(engine, CLng(g) * n, TensorKernelNames.CellSynthShiftBisect) Then
                Return ShiftBisectCpu(ctrl, tieSum, q, ordq, libs, totals, target, g, n, m,
                                      ntop, maxIterations, tolerance, lower, upper)
            End If

            Using deviceCtrl As New ILCudaRuntime.DeviceBuffer(Of Single)(ctrl.Length),
                  deviceTie As New ILCudaRuntime.DeviceBuffer(Of Double)(tieSum.Length),
                  deviceQ As New ILCudaRuntime.DeviceBuffer(Of Double)(q.Length),
                  deviceOrdq As New ILCudaRuntime.DeviceBuffer(Of Integer)(ordq.Length),
                  deviceLibs As New ILCudaRuntime.DeviceBuffer(Of Double)(libs.Length),
                  deviceTotals As New ILCudaRuntime.DeviceBuffer(Of Long)(totals.Length),
                  deviceTarget As New ILCudaRuntime.DeviceBuffer(Of Double)(target.Length),
                  counts As New ILCudaRuntime.DeviceBuffer(Of Integer)(g * n),
                  shiftOut As New ILCudaRuntime.DeviceBuffer(Of Double)(g),
                  zOut As New ILCudaRuntime.DeviceBuffer(Of Double)(g)

                deviceCtrl.Write(ctrl)
                deviceTie.Write(tieSum)
                deviceQ.Write(q)
                deviceOrdq.Write(ordq)
                deviceLibs.Write(libs)
                deviceTotals.Write(totals)
                deviceTarget.Write(target)

                engine.GetKernel(TensorKernelNames.CellSynthShiftBisect).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, DefaultBlockSize, 1, 0),
                    deviceCtrl, deviceTie, deviceQ, deviceOrdq, deviceLibs, deviceTotals, deviceTarget,
                    n, m, ntop, maxIterations, tolerance, lower, upper,
                    counts, shiftOut, zOut)

                Return New BisectResult With {
                    .Counts = counts.Read(),
                    .Shift = shiftOut.Read(),
                    .Z = zOut.Read()
                }
            End Using
        End Function

        ' ==================================================================
        ' 5) 突发散布
        ' ==================================================================

        ''' <summary>把每个基因的整数总量按小突发散布到细胞上。</summary>
        Public Function Burst(engine As ILCudaRuntime.CudaEngine,
                              totals As Long(),
                              libs As Double(),
                              g As Integer,
                              n As Integer,
                              burstSize As Integer,
                              seed As ULong) As Integer()
            If totals Is Nothing Then Throw New ArgumentNullException(NameOf(totals))
            If libs Is Nothing Then Throw New ArgumentNullException(NameOf(libs))
            If g <= 0 OrElse n <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(g))

            If Not CanUseGpu(engine, CLng(g) * n, TensorKernelNames.CellSynthBurst) Then
                Return BurstCpu(totals, libs, g, n, burstSize, seed)
            End If

            Using deviceTotals As New ILCudaRuntime.DeviceBuffer(Of Long)(totals.Length),
                  deviceLibs As New ILCudaRuntime.DeviceBuffer(Of Double)(libs.Length),
                  counts As New ILCudaRuntime.DeviceBuffer(Of Integer)(g * n)

                deviceTotals.Write(totals)
                deviceLibs.Write(libs)

                engine.GetKernel(TensorKernelNames.CellSynthBurst).Launch(
                    New ILCudaRuntime.LaunchConfig(g, 1, 1, 1, 0),
                    deviceTotals, deviceLibs, n, burst,
                    ILCudaMath.GpuRandom.ToSigned(seed), counts)

                Return counts.Read()
            End Using
        End Function

        ' ==================================================================
        ' CPU 参考实现（与内核逐位对齐）
        ' ==================================================================

        ''' <summary>copula 分位的 CPU 参考实现。</summary>
        Public Function CopulaQuantilesCpu(e0 As Double(), zl As Double(),
                                           correlation As Double,
                                           g As Integer, n As Integer) As CopulaResult
            ValidateCopula(e0, zl, g, n)

            Dim quantiles(g * n - 1) As Double
            Dim ordering(g * n - 1) As Integer
            Dim complement As Double = System.Math.Sqrt(System.Math.Max(1.0 - correlation * correlation, 0.0))
            Dim keys(n - 1) As Double
            Dim order(n - 1) As Integer
            Dim compare As New KeyComparer(keys)

            For item As Integer = 0 To g - 1
                For i As Integer = 0 To n - 1
                    keys(i) = correlation * zl(i) + complement * e0(item * n + i)
                    order(i) = i
                Next

                Array.Sort(order, compare)

                For i As Integer = 0 To n - 1
                    quantiles(item * n + order(i)) = (i + 0.5) / n
                    ordering(item * n + i) = order(n - 1 - i)
                Next
            Next

            Return New CopulaResult With {.Quantiles = quantiles, .Ordering = ordering}
        End Function

        ''' <summary>分位取值 + 质量守恒的 CPU 参考实现。</summary>
        Public Function QuantileMassCpu(ctrl As Single(), q As Double(), ordq As Integer(),
                                        libs As Double(), totals As Long(), shift As Double(),
                                        g As Integer, n As Integer, m As Integer, ntop As Integer) As Integer()
            Dim counts(g * n - 1) As Integer

            For item As Integer = 0 To g - 1
                For c As Integer = 0 To n - 1
                    counts(item * n + c) = BuildCount(ctrl, item * m, m, q(item * n + c), shift(item), libs(c))
                Next

                FixMass(counts, item * n, ordq, item * n, n, totals(item), ntop)
            Next

            Return counts
        End Function

        ''' <summary>精确秩和 z 的 CPU 参考实现。</summary>
        Public Function WilcoxonZCpu(counts As Integer(), libs As Double(), ctrl As Single(),
                                     tieSum As Double(), g As Integer, n As Integer, m As Integer) As Double()
            Dim result(g - 1) As Double

            For item As Integer = 0 To g - 1
                Dim values(n - 1) As Double
                Dim order(n - 1) As Integer

                For c As Integer = 0 To n - 1
                    Dim library As Double = System.Math.Max(libs(c), 1.0)

                    values(c) = CDbl(CSng(1.0E+06 * counts(item * n + c) / library))
                    order(c) = c
                Next

                Dim compare As New KeyComparer(values)

                Array.Sort(order, compare)

                Dim sorted(n - 1) As Double

                For i As Integer = 0 To n - 1
                    sorted(i) = values(order(i))
                Next

                Dim u As Double = 0.0
                Dim tie As Double = 0.0

                RankSum(sorted, n, ctrl, item * m, m, u, tie)

                result(item) = ZValue(u, tie, tieSum(item), n, m)
            Next

            Return result
        End Function

        ''' <summary>平移二分的 CPU 参考实现（与内核同一迭代路径）。</summary>
        Public Function ShiftBisectCpu(ctrl As Single(), tieSum As Double(), q As Double(),
                                       ordq As Integer(), libs As Double(), totals As Long(),
                                       target As Double(), g As Integer, n As Integer, m As Integer,
                                       ntop As Integer, maxIterations As Integer,
                                       tolerance As Double, lower As Double, upper As Double) As BisectResult
            Dim counts(g * n - 1) As Integer
            Dim shiftOut(g - 1) As Double
            Dim zOut(g - 1) As Double
            Dim work(n - 1) As Integer

            For item As Integer = 0 To g - 1
                Dim lo As Double = lower
                Dim hi As Double = upper
                Dim bestZ As Double = 1.0E+300
                Dim bestShift As Double = lower
                Dim best(n - 1) As Integer

                For iteration As Integer = 0 To maxIterations - 1
                    Dim mid As Double = 0.5 * (lo + hi)

                    For c As Integer = 0 To n - 1
                        work(c) = BuildCount(ctrl, item * m, m, q(item * n + c), mid, libs(c))
                    Next

                    FixMass(work, 0, ordq, item * n, n, totals(item), ntop)

                    Dim values(n - 1) As Double
                    Dim order(n - 1) As Integer

                    For c As Integer = 0 To n - 1
                        values(c) = CDbl(CSng(1.0E+06 * work(c) / System.Math.Max(libs(c), 1.0)))
                        order(c) = c
                    Next

                    Dim compare As New KeyComparer(values)

                    Array.Sort(order, compare)

                    Dim sorted(n - 1) As Double

                    For i As Integer = 0 To n - 1
                        sorted(i) = values(order(i))
                    Next

                    Dim u As Double = 0.0
                    Dim tie As Double = 0.0

                    RankSum(sorted, n, ctrl, item * m, m, u, tie)

                    Dim z As Double = ZValue(u, tie, tieSum(item), n, m)

                    If System.Math.Abs(z - target(item)) < System.Math.Abs(bestZ - target(item)) Then
                        bestZ = z
                        bestShift = mid

                        For c As Integer = 0 To n - 1
                            best(c) = work(c)
                        Next
                    End If

                    If System.Math.Abs(z - target(item)) <= tolerance Then
                        lo = mid
                        hi = mid
                    ElseIf z < target(item) Then
                        lo = mid
                    Else
                        hi = mid
                    End If

                    If hi - lo <= 1.0E-12 Then Exit For
                Next

                shiftOut(item) = bestShift
                zOut(item) = bestZ

                For c As Integer = 0 To n - 1
                    counts(item * n + c) = best(c)
                Next
            Next

            Return New BisectResult With {.Counts = counts, .Shift = shiftOut, .Z = zOut}
        End Function

        ''' <summary>突发散布的 CPU 参考实现。</summary>
        Public Function BurstCpu(totals As Long(), libs As Double(),
                                 g As Integer, n As Integer, burst As Integer, seed As ULong) As Integer()
            Dim counts(g * n - 1) As Integer
            Dim sum As Double = 0.0

            For c As Integer = 0 To n - 1
                sum += System.Math.Max(libs(c), 0.0)
            Next

            For item As Integer = 0 To g - 1
                Dim remaining As Long = totals(item)
                Dim draws As Integer = 0

                If remaining <= 0 OrElse burst < 1 OrElse sum <= 0.0 Then Continue For

                While remaining > 0
                    Dim u As Double = ILCudaMath.Pcg64.Uniform(seed, 0UL, CULng(item * 7919 + draws))
                    Dim acc As Double = 0.0
                    Dim hit As Integer = n - 1

                    For c As Integer = 0 To n - 1
                        acc += System.Math.Max(libs(c), 0.0) / sum

                        If u < acc Then
                            hit = c
                            Exit For
                        End If
                    Next

                    Dim size As Integer = If(remaining < burst, CInt(remaining), burst)

                    counts(item * n + hit) += size
                    remaining -= size
                    draws += 1
                End While
            Next

            Return counts
        End Function

        ' ==================================================================
        ' 共享的算法细节
        ' ==================================================================

        ''' <summary>单个 (基因, 细胞) 的分位取值。</summary>
        Private Function BuildCount(ctrl As Single(), offset As Integer, m As Integer,
                                    quantile As Double, shift As Double, library As Double) As Integer
            Dim position As Double = shift + quantile - 0.5
            Dim v As Double = 0.0

            If position > 0.0 Then
                Dim at As Integer = CInt(System.Math.Floor(position * m))

                If at >= m Then at = m - 1

                v = ctrl(offset + at)
            End If

            Return CInt(System.Math.Floor(v * library * 1.0E-06 + 0.5))
        End Function

        ''' <summary>把一段计数调整到目标总量（与内核同一算法）。</summary>
        Private Sub FixMass(counts As Integer(), offset As Integer,
                            ordq As Integer(), ordOffset As Integer,
                            n As Integer, total As Long, ntop As Integer)
            Dim mass As Long = 0

            For c As Integer = 0 To n - 1
                mass += counts(offset + c)
            Next

            If mass < total Then
                Dim deficit As Long = total - mass
                Dim nd As Integer = CInt(If(deficit < ntop, deficit, CLng(ntop)))

                If nd > n Then nd = n
                If nd < 1 Then nd = 1

                Dim add As Long = deficit \ nd
                Dim rest As Long = deficit - add * nd

                For j As Integer = 0 To nd - 1
                    counts(offset + ordq(ordOffset + j)) += CInt(add + If(j < rest, 1, 0))
                Next

                Return
            End If

            If mass = total Then Return

            ' 质量超出：按降序水位削平
            Dim excess As Long = mass - total
            Dim order(n - 1) As Integer
            Dim keys(n - 1) As Double

            For c As Integer = 0 To n - 1
                keys(c) = counts(offset + c)
                order(c) = c
            Next

            Array.Sort(order, New KeyComparer(keys))

            Dim cumulative As Long = 0

            For i As Integer = n - 1 To 0 Step -1
                cumulative += CLng(keys(order(i)))

                Dim nextLevel As Long = If(i > 0, CLng(keys(order(i - 1))), 0L)
                Dim width As Integer = n - i

                If cumulative - width * nextLevel >= excess Then
                    Dim keep As Long = cumulative - excess
                    Dim level As Long = keep \ width
                    Dim rest As Long = keep - level * width

                    For j As Integer = 0 To width - 1
                        counts(offset + order(n - 1 - j)) = CInt(level + If(j < rest, 1L, 0L))
                    Next

                    Return
                End If
            Next

            For c As Integer = 0 To n - 1
                counts(offset + c) = 0
            Next
        End Sub

        ''' <summary>按游程累计 U 与合并 tie 校正量。</summary>
        Private Sub RankSum(sorted As Double(), n As Integer, ctrl As Single(), ctrlOffset As Integer,
                            m As Integer, ByRef u As Double, ByRef tie As Double)
            Dim c As Integer = 0

            While c < n
                Dim v As Single = CSng(sorted(c))
                Dim [end] As Integer = c + 1

                While [end] < n AndAlso CSng(sorted([end])) = v
                    [end] += 1
                End While

                Dim lo As Integer = LowerBound(ctrl, ctrlOffset, m, v)
                Dim up As Integer = UpperBound(ctrl, ctrlOffset, m, v)
                Dim run As Double = [end] - c
                Dim control As Double = up - lo
                Dim merged As Double = control + run

                u += run * (lo + 0.5 * control)
                tie += (merged * merged * merged - merged) - (control * control * control - control)

                c = [end]
            End While
        End Sub

        Private Function ZValue(u As Double, tieAdjust As Double, tieSum As Double,
                                n As Integer, m As Integer) As Double
            Dim nf As Double = n
            Dim mf As Double = m
            Dim total As Double = nf + mf
            Dim tc As Double = 1.0 - (tieSum + tieAdjust) / (total * total * total - total)
            Dim variance As Double = tc * nf * mf * (total + 1.0) / 12.0

            If variance <= 1.0E-24 Then Return 0.0

            Return (u - nf * mf / 2.0) / System.Math.Sqrt(variance)
        End Function

        Private Function LowerBound(ctrl As Single(), offset As Integer, m As Integer, value As Single) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = m

            While lo < hi
                Dim mid As Integer = (lo + hi) >> 1

                If ctrl(offset + mid) < value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        Private Function UpperBound(ctrl As Single(), offset As Integer, m As Integer, value As Single) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = m

            While lo < hi
                Dim mid As Integer = (lo + hi) >> 1

                If ctrl(offset + mid) <= value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        ' ==================================================================
        ' 校验与调度
        ' ==================================================================

        Public Function CanUseGpu(engine As ILCudaRuntime.CudaEngine, elements As Long, kernelName As String) As Boolean
            If engine Is Nothing Then Return False
            If elements < MinGpuElements Then Return False

            Dim kernel As ILCudaRuntime.CudaKernel = Nothing

            Return engine.TryGetKernel(kernelName, kernel)
        End Function

        Private Sub ValidateCopula(e0 As Double(), zl As Double(), g As Integer, n As Integer)
            If e0 Is Nothing Then Throw New ArgumentNullException(NameOf(e0))
            If zl Is Nothing Then Throw New ArgumentNullException(NameOf(zl))
            If g <= 0 OrElse n <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(g))
            If zl.Length <> n Then Throw New ArgumentException($"zl 长度 {zl.Length} 与 n={n} 不一致")
            If e0.Length <> g * n Then Throw New ArgumentException($"e0 长度 {e0.Length} 与 [{g} x {n}] 不一致")
        End Sub

        Private Sub ValidateQuantileMass(ctrl As Single(), q As Double(), ordq As Integer(),
                                         libs As Double(), totals As Long(), shift As Double(),
                                         g As Integer, n As Integer, m As Integer)
            If ctrl Is Nothing Then Throw New ArgumentNullException(NameOf(ctrl))
            If q Is Nothing Then Throw New ArgumentNullException(NameOf(q))
            If ordq Is Nothing Then Throw New ArgumentNullException(NameOf(ordq))
            If libs Is Nothing Then Throw New ArgumentNullException(NameOf(libs))
            If totals Is Nothing Then Throw New ArgumentNullException(NameOf(totals))
            If shift Is Nothing Then Throw New ArgumentNullException(NameOf(shift))

            If ctrl.Length <> g * m Then Throw New ArgumentException($"ctrl 长度 {ctrl.Length} 与 [{g} x {m}] 不一致")
            If q.Length <> g * n Then Throw New ArgumentException($"q 长度 {q.Length} 与 [{g} x {n}] 不一致")
            If ordq.Length <> g * n Then Throw New ArgumentException($"ordq 长度 {ordq.Length} 与 [{g} x {n}] 不一致")
            If libs.Length <> n Then Throw New ArgumentException($"libs 长度 {libs.Length} 与 n={n} 不一致")
            If totals.Length <> g Then Throw New ArgumentException($"totals 长度 {totals.Length} 与 g={g} 不一致")
            If shift.Length <> g Then Throw New ArgumentException($"shift 长度 {shift.Length} 与 g={g} 不一致")
        End Sub

        Private Sub ValidateWilcoxon(counts As Integer(), libs As Double(), ctrl As Single(),
                                     tieSum As Double(), g As Integer, n As Integer, m As Integer)
            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))
            If libs Is Nothing Then Throw New ArgumentNullException(NameOf(libs))
            If ctrl Is Nothing Then Throw New ArgumentNullException(NameOf(ctrl))
            If tieSum Is Nothing Then Throw New ArgumentNullException(NameOf(tieSum))

            If counts.Length <> g * n Then Throw New ArgumentException($"counts 长度 {counts.Length} 与 [{g} x {n}] 不一致")
            If ctrl.Length <> g * m Then Throw New ArgumentException($"ctrl 长度 {ctrl.Length} 与 [{g} x {m}] 不一致")
            If libs.Length <> n Then Throw New ArgumentException($"libs 长度 {libs.Length} 与 n={n} 不一致")
            If tieSum.Length <> g Then Throw New ArgumentException($"tieSum 长度 {tieSum.Length} 与 g={g} 不一致")
        End Sub

        Private Sub ValidateBisect(ctrl As Single(), tieSum As Double(), q As Double(), ordq As Integer(),
                                   libs As Double(), totals As Long(), target As Double(),
                                   g As Integer, n As Integer, m As Integer)
            ValidateQuantileMass(ctrl, q, ordq, libs, totals,
                                 New Double(g - 1) {}, g, n, m)

            If tieSum Is Nothing OrElse tieSum.Length <> g Then
                Throw New ArgumentException($"tieSum 长度应为 {g}")
            End If
            If target Is Nothing OrElse target.Length <> g Then
                Throw New ArgumentException($"target 长度应为 {g}")
            End If
        End Sub

        ''' <summary>按 (值, 下标) 升序比较，用于让排序结果完全确定。</summary>
        Private Class KeyComparer
            Implements IComparer(Of Integer)

            Private ReadOnly m_keys As Double()

            Public Sub New(keys As Double())
                m_keys = keys
            End Sub

            Public Function Compare(a As Integer, b As Integer) As Integer Implements IComparer(Of Integer).Compare
                Dim c As Integer = m_keys(a).CompareTo(m_keys(b))

                Return If(c <> 0, c, a.CompareTo(b))
            End Function
        End Class
    End Module
End Namespace
