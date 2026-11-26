Imports CellularAI.Model
Imports std = System.Math

Namespace Synthesis

    ''' <summary>单个基因的"分位平移二分"参数。</summary>
    Public Class ShiftKernel

        ''' <summary>质量不足时接受补足的细胞数上限（<c>solve_ntop</c>）。</summary>
        Public Property NTop As Integer = 40

        ''' <summary>z 的容差（<c>solve_tol</c>）。</summary>
        Public Property Tolerance As Double = 0.05

        ''' <summary>二分步数上限（<c>solve_iters</c>）。</summary>
        Public Property Iterations As Integer = 40

        ''' <summary>平移区间下界（<c>shift_band[0]</c>）。</summary>
        Public Property Lower As Double = -0.6

        ''' <summary>平移区间上界（<c>shift_band[1]</c>）。</summary>
        Public Property Upper As Double = 1.6

        ''' <summary>稀疏化扫描步长（<c>sparsify_step</c>）。</summary>
        Public Property SparsifyStep As Double = 0.025

        ''' <summary>由模型参数构造。</summary>
        Public Shared Function FromModel(model As CellularAiModel) As ShiftKernel
            Dim options As SynthesisOptions = model.Synthesis

            Return New ShiftKernel With {
                .NTop = options.SolveNtop,
                .Tolerance = options.SolveTol,
                .Iterations = options.SolveIters,
                .Lower = options.ShiftBand(0),
                .Upper = options.ShiftBand(1),
                .SparsifyStep = options.SparsifyStep
            }
        End Function

    End Class

    ''' <summary>
    ''' 单基因的计数求解：在"分位平移量"上二分，使秩和 z 逼近目标；
    ''' 平移区间内达不到目标时改走"稀疏化扫描"（减少非零细胞数来压低 z）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么调平移量而不是调计数</b>：计数由"分位 → 对照 CPM → 按文库大小折算"决定，
    ''' 直接改计数会破坏边缘分布；而平移量只改变"用哪些分位"，
    ''' 等于在同一个对照分布上整体上下挪动，z 对它单调，二分才有意义。
    ''' </para>
    ''' <para>
    ''' <b>为什么还要稀疏化兜底</b>：平移只能把 z 抬到"全细胞都取最高分位"为止。
    ''' 若目标 z 更高（通常发生在"设计总量很大但细胞数很少"的基因上），
    ''' 唯一还能压低手段是<b>减少持有计数的细胞数</b>（把质量集中到少数细胞），
    ''' 因此按 <c>w</c> 从小到大扫描，找到第一个达标的稀疏度，再在相邻两个 <c>w</c> 之间二分。
    ''' </para>
    ''' <para>
    ''' <b>失败时保留最优解</b>：二分结束后返回的是"误差最小"的那次尝试，而不是最后一次，
    ''' 避免因为最后一步跨过头而丢掉一个几乎达标的解。
    ''' </para>
    ''' </remarks>
    Public Module BlockSolver

        ''' <summary>
        ''' 求某个基因的整数计数，使秩和 z 逼近 <paramref name="target"/>。
        ''' </summary>
        ''' <param name="q">每个细胞的名次分位</param>
        ''' <param name="ordering">按分位降序的细胞下标</param>
        ''' <param name="sortedCpm">对照侧已排序的 CPM</param>
        ''' <param name="controlTieSum">对照侧并列和</param>
        ''' <param name="librarySizes">每个细胞的目标文库大小</param>
        ''' <param name="total">设计总量</param>
        ''' <param name="target">目标 z</param>
        Public Function SolveShift(q As Double(), ordering As Integer(), sortedCpm As Double(),
                                   controlTieSum As Double, librarySizes As Double(),
                                   total As Double, target As Double, kernel As ShiftKernel) As (Counts As Double(), Z As Double)
            Dim counts As Double() = QuantileBuilder.Build(q, ordering, sortedCpm, librarySizes,
                                                          total, kernel.Lower, kernel.NTop)
            Dim zLow As Double = WilcoxonKernel.ZExact(counts, librarySizes, sortedCpm, controlTieSum)

            If target <= zLow Then
                Return (counts, zLow)
            End If

            counts = QuantileBuilder.Build(q, ordering, sortedCpm, librarySizes, total, kernel.Upper, kernel.NTop)

            Dim zHigh As Double = WilcoxonKernel.ZExact(counts, librarySizes, sortedCpm, controlTieSum)

            If target >= zHigh Then
                Return SparseFallback(q, ordering, sortedCpm, controlTieSum, librarySizes,
                                      total, target, kernel, counts, zHigh)
            End If

            Dim best As Double() = counts
            Dim bestError As Double = Double.MaxValue
            Dim bestZ As Double = 0.0
            Dim lo As Double = kernel.Lower
            Dim hi As Double = kernel.Upper

            For iteration As Integer = 1 To std.Max(kernel.Iterations, 1)
                Dim mid As Double = 0.5 * (lo + hi)

                counts = QuantileBuilder.Build(q, ordering, sortedCpm, librarySizes, total, mid, kernel.NTop)

                Dim z As Double = WilcoxonKernel.ZExact(counts, librarySizes, sortedCpm, controlTieSum)
                Dim error_ As Double = std.Abs(z - target)

                If error_ < bestError Then
                    bestError = error_
                    bestZ = z
                    best = counts
                End If
                If error_ < kernel.Tolerance Then
                    Exit For
                End If

                If z < target Then
                    lo = mid
                Else
                    hi = mid
                End If
            Next

            Return (best, bestZ)
        End Function

        ''' <summary>
        ''' 平移已到上限仍不达标：按稀疏度扫描 + 内层二分。
        ''' </summary>
        Private Function SparseFallback(q As Double(), ordering As Integer(), sortedCpm As Double(),
                                        controlTieSum As Double, librarySizes As Double(), total As Double,
                                        target As Double, kernel As ShiftKernel,
                                        denseCounts As Double(), denseZ As Double) As (Counts As Double(), Z As Double)
            Dim best As Double() = denseCounts
            Dim bestZ As Double = denseZ
            Dim previousW As Double = 0.0

            For iteration As Integer = 1 To 38
                Dim w As Double = kernel.SparsifyStep * iteration
                Dim counts As Double() = QuantileBuilder.BuildSparse(ordering, total, w)
                Dim z As Double = WilcoxonKernel.ZExact(counts, librarySizes, sortedCpm, controlTieSum)

                If z >= target Then
                    Dim lowW As Double = previousW
                    Dim highW As Double = w
                    Dim innerBest As Double() = counts
                    Dim innerZ As Double = z

                    For inner As Integer = 1 To 12
                        Dim mid As Double = 0.5 * (lowW + highW)
                        Dim probe As Double() = QuantileBuilder.BuildSparse(ordering, total, mid)

                        z = WilcoxonKernel.ZExact(probe, librarySizes, sortedCpm, controlTieSum)

                        If z >= target Then
                            highW = mid
                            innerBest = probe
                            innerZ = z
                        Else
                            lowW = mid
                        End If
                        If std.Abs(z - target) < kernel.Tolerance Then
                            Exit For
                        End If
                    Next

                    Return (innerBest, innerZ)
                End If

                If z > bestZ Then
                    bestZ = z
                    best = counts
                End If

                previousW = w
            Next

            Return (best, bestZ)
        End Function

    End Module
End Namespace
